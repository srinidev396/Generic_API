using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FusionWebApi.Models;
using Smead.RecordsManagement;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static FusionWebApi.Models.DatabaseSchema;
using System.Reflection;
using System.Text.RegularExpressions;
using Smead.Security;

namespace FusionWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DataController : ControllerBase
    {
        private IConfiguration _config;
        private ILogger<DataController> _logger;
        public DataController(IConfiguration config, ILogger<DataController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetUserViews")]
        public async Task<UserViews> GetUserViews()
        {
            var model = new UserViews();
            model.ErrorMessages.TimeStamp = DateTime.Now;
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                model.listOfviews = await Task.Run(() => Navigation.GetAllUserViews(passport));
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");

            }
            return model;
        }
        [HttpGet]
        [Route("GetDbSchema")]
        public async Task<SchemaModel> GetDbSchema()
        {
            var model = new SchemaModel();
            model.ErrorMessages.TimeStamp = DateTime.Now;
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                model = await Task.Run(() => DatabaseSchema.ReturnDbSchema(passport));
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            return model;
        }
        [HttpGet]
        [Route("GetTableSchema")]
        public async Task<TablesSchema> GetTableSchema(string TableName)
        {
            TablesSchema model = new TablesSchema();
            model.ErrorMessages.TimeStamp = DateTime.Now;
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                model = await Task.Run(() => DatabaseSchema.GetTableSchema(TableName, passport));
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            return model;
        }
        [HttpPost]
        [Route("NewRecord")]
        public async Task<Records> NewRecord(UIPostModel userdata)
        {

            var model = new Records();
            model.ErrorMessages.TimeStamp = DateTime.Now;
            if (userdata.PostRow.Count == 0)
            {
                model.ErrorMessages.FusionCode = (int)EventCode.NoColumn;
                model.ErrorMessages.FusionMessage = "No column to post";
                return model;
            }
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                var checkcolumns = await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                if (checkcolumns != "true")
                {
                    model.ErrorMessages.FusionMessage = checkcolumns;
                    model.ErrorMessages.FusionCode = (int)EventCode.ColumnIsnotExist;
                    return model;
                }
                var addrecord = new RecordsActions(passport);
                if (!addrecord.DataValidation<PostColumns>(userdata.PostRow))
                {
                    model.ErrorMessages.FusionCode = (int)EventCode.IllegalData;
                    model.ErrorMessages.FusionMessage = "Illegal Data";
                    return model;
                }
                if (await Task.Run(() => addrecord.AddNewRow(userdata)))
                {
                    model.ErrorMessages.FusionCode = (int)EventCode.NewRecordAdded;
                    model.ErrorMessages.FusionMessage = $"New Record Added!";
                    return model;
                }
                else
                {
                    model.FusionMessage = $"Insufficient permissions to Add";
                    return model;
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            //var rr = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            return model;
        }
        [HttpPost]
        [Route("NewRecordMulti")]
        //[RequestSizeLimit(100_000_000)]
        //[DisableRequestSizeLimit]
        [RequestSizeLimit(52428800)] //limit to 1mb
        public async Task<Records> NewRecordMulti(UIPostModel userdata)
        {
            var model = new Records();
            model.ErrorMessages.TimeStamp = DateTime.Now;
            if (userdata.PostMultiRows.Count == 0)
            {
                model.ErrorMessages.FusionCode = (int)EventCode.NoRow;
                model.ErrorMessages.FusionMessage = "No rows to post";
                return model;
            }
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            var start = DateTime.Now;
            try
            {
                var checkcolumns = await Task.Run(() => DatabaseSchema.GetColumntypeMulti(passport, userdata));
                if (checkcolumns != "true")
                {
                    model.ErrorMessages.FusionMessage = checkcolumns;
                    model.ErrorMessages.FusionCode = (int)EventCode.ColumnIsnotExist;
                    return model;
                }
                var addrecord = new RecordsActions(passport);

                for (int i = 0; i < userdata.PostMultiRows.Count; i++)
                {
                    if (!addrecord.DataValidation<PostColumns>(userdata.PostMultiRows[i]))
                    {
                        model.ErrorMessages.FusionCode = (int)EventCode.IllegalData;
                        model.ErrorMessages.FusionMessage = "Illegal Data";
                        return model;
                    }
                }

                model.ErrorMessages.FusionMessage = await Task.Run(() => addrecord.AddNewRowMulti(userdata));
                model.ErrorMessages.FusionCode = (int)EventCode.NewRecordsAdded;
                if (!model.ErrorMessages.FusionMessage.Contains("Added"))
                {
                    model.ErrorMessages.Code = 0;
                    model.ErrorMessages.Message = model.ErrorMessages.FusionMessage;
                    model.ErrorMessages.FusionMessage = "";
                    model.ErrorMessages.FusionCode = 0;
                }
                var end = DateTime.Now;
                var total = end - start;
                model.ErrorMessages.FusionMessage += $"/ Insertion process time: {total}";
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            //var rr = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            return model;
        }
        [HttpPost]
        [Route("EditRecord")]
        public async Task<Records> EditRecord(UIPostModel userdata)
        {
            var model = new Records();
            model.ErrorMessages.TimeStamp = DateTime.Now;
            if (userdata.PostRow.Count == 0)
            {
                model.ErrorMessages.FusionCode = (int)EventCode.NoColumn;
                model.ErrorMessages.FusionMessage = "No column to post";
                return model;
            }
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                var checkcolumns = await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                if (checkcolumns != "true")
                {
                    model.ErrorMessages.FusionMessage = checkcolumns;
                    model.ErrorMessages.FusionCode = (int)EventCode.ColumnIsnotExist;
                    return model;
                }
                var editrecord = new RecordsActions(passport);

                if (!editrecord.DataValidation<PostColumns>(userdata.PostRow))
                {
                    model.ErrorMessages.FusionCode = (int)EventCode.IllegalData;
                    model.ErrorMessages.FusionMessage = "Illegal Data";
                    return model;
                }

                if (await Task.Run(() => editrecord.EditRow(userdata)))
                {
                    model.ErrorMessages.FusionCode = (int)(EventCode.RecordUpdated);
                    model.ErrorMessages.FusionMessage = $"Record Updated!";
                }
                else
                {
                    model.ErrorMessages.FusionCode = (int)EventCode.insufficientpermissions;
                    model.ErrorMessages.FusionMessage = $"insufficient permissions to Edit";
                }

            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");

            }

            return model;
        }
        [HttpPost]
        [Route("EditRecordByColumn")]
        public async Task<Records> EditRecordByColumn(UIPostModel userdata)
        {
            var model = new Records();
            model.ErrorMessages.TimeStamp = DateTime.Now;
            if (userdata.PostRow.Count == 0)
            {
                model.ErrorMessages.FusionCode = (int)EventCode.NoColumn;
                model.ErrorMessages.FusionMessage = "No column to post";
                return model;
            }

            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                var checkcolumns = await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                if (checkcolumns != "true")
                {
                    model.ErrorMessages.FusionMessage = checkcolumns;
                    model.ErrorMessages.FusionCode = (int)EventCode.ColumnIsnotExist;
                    return model;
                }
                var editrecord = new RecordsActions(passport);
                if (!editrecord.DataValidation<PostColumns>(userdata.PostRow))
                {
                    model.ErrorMessages.FusionCode = (int)EventCode.IllegalData;
                    model.ErrorMessages.FusionMessage = "Illegal Data";
                    return model;
                }

                model.ErrorMessages.FusionMessage = await Task.Run(() => editrecord.EditRecordByColumn(userdata));
                if (model.ErrorMessages.FusionMessage == "nopermission")
                {
                    model.ErrorMessages.FusionMessage = "insufficient permissions to Edit";
                    model.ErrorMessages.FusionCode = (int)EventCode.insufficientpermissions;
                    return model;
                }
                else
                {
                    model.ErrorMessages.FusionCode = (int)EventCode.RecordUpdated;
                }
                if (!model.ErrorMessages.FusionMessage.Contains("Updated"))
                {
                    model.ErrorMessages.FusionCode = 0;
                    model.ErrorMessages.Message = model.ErrorMessages.FusionMessage;
                    model.ErrorMessages.FusionMessage = "";
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            return model;
        }
        [HttpPost]
        [Route("EditIfNotExistAdd")]
        public async Task<Records> EditIfNotExistAdd(UIPostModel userdata)
        {
            var model = new Records();
            model.ErrorMessages.TimeStamp = DateTime.Now;
            //to use this method the developer must have an account with add\edit permission
            //so first we check for permissions
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            if (!passport.CheckPermission(userdata.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Edit) ||
                !passport.CheckPermission(userdata.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Add))
            {
                model.ErrorMessages.FusionCode = (int)EventCode.insufficientpermissions;
                model.ErrorMessages.FusionMessage = "Insufficient permissions to modify or add data.";
                return model;
            }
            if (userdata.PostRow.Count == 0)
            {
                model.ErrorMessages.FusionCode = (int)EventCode.NoColumn;
                model.ErrorMessages.FusionMessage = "No column to post";
                return model;
            }
            try
            {
                var checkcolumns = await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                if (checkcolumns != "true")
                {
                    model.ErrorMessages.FusionMessage = checkcolumns;
                    model.ErrorMessages.FusionCode = (int)EventCode.ColumnIsnotExist;
                    return model;
                }
                var record = new RecordsActions(passport);
                if (!record.DataValidation<PostColumns>(userdata.PostRow))
                {
                    model.ErrorMessages.FusionCode = (int)EventCode.IllegalData;
                    model.ErrorMessages.FusionMessage = "Illegal Data";
                    return model;
                }
                model.ErrorMessages.FusionMessage = await Task.Run(() => record.EditRecordByColumn(userdata));
                if (model.ErrorMessages.FusionMessage == "nopermission")
                {
                    model.ErrorMessages.FusionMessage = "insufficient permissions to Edit";
                    model.ErrorMessages.FusionCode = (int)EventCode.insufficientpermissions;
                    return model;
                }
                else
                {
                    model.ErrorMessages.FusionCode = (int)EventCode.RecordUpdated;
                }

                if (!model.ErrorMessages.FusionMessage.Contains("Updated"))
                {
                    model.ErrorMessages.FusionCode = 0;
                    model.ErrorMessages.Message = model.ErrorMessages.FusionMessage;
                    model.ErrorMessages.FusionMessage = "";
                }

                if (model.ErrorMessages.Message.Contains("0"))
                {
                    if (await Task.Run(() => record.AddNewRow(userdata)))
                    {
                        model.ErrorMessages.FusionCode = (int)EventCode.NewRecordAdded;
                        model.ErrorMessages.FusionMessage = $"New Record Added!";
                    }
                    else
                    {
                        model.ErrorMessages.FusionCode = (int)EventCode.insufficientpermissions;
                        model.ErrorMessages.FusionMessage = $"Insufficient permissions to Add";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            return model;
        }
        [HttpGet]
        [Route("TestExceptionMethod")]
        public void TestExceptionMethod()
        {

            var model = new ErrorMessages();
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {

                throw new ArgumentException("This is an intentional test exception by FusionRMS");
            }
            catch (Exception ex)
            {
                model.Code = ex.HResult;
                model.Message = ex.Message;
                model.TimeStamp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
        }
        [HttpGet]
        [Route("GetViewData")]
        public async Task<Viewmodel> GetViewData(int viewid, int pageNumber)
        {
            var getview = new Viewmodel();
            getview.ErrorMessages.TimeStamp = DateTime.Now;
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            var v = new RecordsActions(passport);
            try
            {
                if (viewid == 0 || pageNumber == 0)
                {
                    getview.ErrorMessages.FusionCode = (int)EventCode.WrongValue;
                    getview.ErrorMessages.FusionMessage = $"Viewid or pageNumber cannot be 0";
                }
                else
                {
                    getview = await Task.Run(() => v.GetviewData(viewid, pageNumber));
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("position 0"))
                {
                    getview.ErrorMessages.Code = 0;
                    getview.ErrorMessages.Message = "";
                    getview.ErrorMessages.FusionCode = (int)EventCode.WrongValue;
                    getview.ErrorMessages.FusionMessage = $"View {viewid} is not found";
                }
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }

            return getview;
        }

    }

}