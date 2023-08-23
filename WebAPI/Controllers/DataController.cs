using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Smead.Security;
using System.Data.SqlClient;
using System.Net.Http;
using FusionWebApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Smead.RecordsManagement;
using System.Data;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
                model.ErrorMessages.TimeStemp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
                
            }
            return model;
        }
        [HttpGet]
        [Route("GetDbSchema")]
        public async Task<SchemaModel> GetDbSchema()
        {
            var model = new SchemaModel();
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                model.getDbSchemas = await Task.Run(() => DatabaseSchema.ReturnDbSchema(passport));
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                model.ErrorMessages.TimeStemp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            return model;
        }
        [HttpGet]
        [Route("GetTableSchema")]
        public async Task<TablesSchema> GetTableSchema(string TableName)
        {
            TablesSchema model = new TablesSchema();
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
                model.ErrorMessages.TimeStemp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            return model;
        }
        [HttpPost]
        [Route("NewRecord")]
        public async Task<Records> NewRecord(UIPostModel userdata)
        {
            var model = new Records();
            if (userdata.PostRow.Count == 0)
            {
                model.FusionMessage = "No column to post";
                return model;
            } 
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                var addrecord = new RecordsActions(passport);
                if (await Task.Run(() => addrecord.AddNewRow(userdata)))
                {
                    model.FusionMessage = $"New Record Added!";
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
                model.ErrorMessages.TimeStemp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            //var rr = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            return model;
        }
        [HttpPost]
        [Route("NewRecordMulti")]
        //[RequestSizeLimit(100_000_000)]
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(1048576)] limit to 1mb
        public async Task<Records> NewRecordMulti(UIPostModel userdata)
        {
            var model = new Records();
            if (userdata.PostMultiRows.Count == 0)
            {
                model.FusionMessage = "No rows to post";
                return model;
            }
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntypeMulti(passport, userdata));
                var addrecord = new RecordsActions(passport);
                model.FusionMessage = await Task.Run(() => addrecord.AddNewRowMulti(userdata));
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                model.ErrorMessages.TimeStemp = DateTime.Now;
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
            if (userdata.PostRow.Count == 0)
            {
                model.FusionMessage = "No column to post";
                return model;
            } 
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                var editrecord = new RecordsActions(passport);
                if (await Task.Run(() => editrecord.EditRow(userdata)))
                {
                    model.FusionMessage = $"Record Updated!";
                }
                else
                {
                    model.FusionMessage = $"insufficient permissions to Edit";
                }

            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                model.ErrorMessages.TimeStemp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");

            }

            return model;
        }
        [HttpPost]
        [Route("EditRecordByColumn")]
        public async Task<Records> EditRecordByColumn(UIPostModel userdata)
        {
            var model = new Records();
            if (userdata.PostRow.Count == 0)
            {
                model.FusionMessage  = "No column to post";
                return model;
            }
                
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                var editrecord = new RecordsActions(passport);
                model.FusionMessage = await Task.Run(() => editrecord.EditRecordByColumn(userdata));
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                model.ErrorMessages.TimeStemp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
            return model;
        }
        [HttpPost]
        [Route("EditIfNotExistAdd")]
        public async Task<Records> EditIfNotExistAdd(UIPostModel userdata)
        {
            var model = new Records();
            if (userdata.PostRow.Count == 0)
            {
                model.FusionMessage = "No column to post";
                return model;
            }
                
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                var record = new RecordsActions(passport);
                model.FusionMessage = await Task.Run(() => record.EditRecordByColumn(userdata));
                if (model.FusionMessage.Contains("0"))
                {
                    if (await Task.Run(() => record.AddNewRow(userdata)))
                    {
                        model.FusionMessage = $"New Record Added!";
                    }
                    else
                    {
                        model.FusionMessage = $"Insufficient permissions to Add";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Code = ex.HResult;
                model.ErrorMessages.Message = ex.Message;
                model.ErrorMessages.TimeStemp = DateTime.Now;
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
                model.TimeStemp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }
        }
        [HttpGet]
        [Route("GetViewData")]
        public async Task<Viewmodel> GetViewData(int viewid, int pageNumber)
        {   
            var getview = new Viewmodel();
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
                    if(pageNumber > getview.TotalPages)
                    {
                        getview.ErrorMessages.FusionCode = (int)EventCode.WrongValue;
                        getview.ErrorMessages.FusionMessage = $"My friend Jerald Total page is {getview.TotalPages} and you entered {pageNumber} so please stop breaking my code :) :) :)";
                    }
                }
            }
         
            catch (Exception ex)
            {
                if(ex.Message.Contains("position 0"))
                {
                    getview.ErrorMessages.Code = 0;
                    getview.ErrorMessages.Message = "";
                    getview.ErrorMessages.FusionCode = (int)EventCode.WrongValue;
                    getview.ErrorMessages.FusionMessage = $"View {viewid} is not found";
                }
                
                getview.ErrorMessages.TimeStemp = DateTime.Now;
                _logger.LogError($"{ex.Message} DataBaseName: {passport.DatabaseName} UserName: {passport.UserName}");
            }

            return getview;
        }

    }

}