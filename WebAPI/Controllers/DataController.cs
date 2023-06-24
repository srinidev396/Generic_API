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

namespace FusionWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DataController : ControllerBase
    {
        private IConfiguration _config;
        public DataController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("GetUserViews")]
        public async Task<List<Navigation.ListOfviews>> GetUserViews()
        {
            var lst = new List<Navigation.ListOfviews>();
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                lst = await Task.Run(() => Navigation.GetAllUserViews(passport));
            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
            }
            return lst;
        }
        [HttpGet]
        [Route("GetDbSchema")]
        public async Task<List<GetDbSchema>> GetDbSchema()
        {
            var skl = new List<GetDbSchema>();
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                skl = await Task.Run(() => DatabaseSchema.ReturnDbSchema(passport));
            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
            }
            return skl;
        }
        [HttpGet]
        [Route("GetTableSchema")]
        public async Task<TablesSchema> GetTableSchema(string TableName)
        {
            TablesSchema tc = new TablesSchema();
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                tc = await Task.Run(() => DatabaseSchema.GetTableSchema(TableName, passport));
            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
            }
            return tc;
        }
        [HttpPost]
        [Route("NewRecord")]
        public async Task<string> NewRecord(UIPostModel userdata)
        {
            var msg = string.Empty;
            if (userdata.PostColomn.Count == 0) return "No column to post";
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                var addrecord = new RecordsActions(passport);
                if (await Task.Run(() => addrecord.AddNewRow(userdata)))
                {
                    msg = $"New Record Added!";
                }
                else
                {
                    msg = $"Insufficient permissions to Add";
                }
            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
                msg = ex.Message;
            }
            //var rr = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            return msg;
        }
        [HttpPost]
        [Route("NewRecordMulti")]
        //[RequestSizeLimit(100_000_000)]
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(1048576)] limit to 1mb
        public async Task<string> NewRecordMulti(UIPostModel userdata)
        {
            var msg = string.Empty;
            if (userdata.PostColumnsMulti.Count == 0) return "No rows to post";
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntypeMulti(passport, userdata));
                var addrecord = new RecordsActions(passport);
                msg = await Task.Run(() => addrecord.AddNewRowMulti(userdata));
            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
                msg = ex.Message;
            }
            //var rr = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            return msg;
        }
        [HttpPost]
        [Route("EditRecord")]
        public async Task<string> EditRecord(UIPostModel userdata)
        {
            var msg = string.Empty;
            if (userdata.PostColomn.Count == 0) return "No column to post";
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                var editrecord = new RecordsActions(passport);
                if (await Task.Run(() => editrecord.EditRow(userdata)))
                {
                    msg = $"Record Updated!";
                }
                else
                {
                    msg = $"insufficient permissions to Edit";
                }

            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
                msg = ex.Message;
            }

            return msg;
        }
        [HttpPost]
        [Route("EditRecordByColumn")]
        public async Task<string> EditRecordByColumn(UIPostModel userdata)
        {
            var msg = string.Empty;
            if (userdata.PostColomn.Count == 0) return "No column to post";
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                var editrecord = new RecordsActions(passport);
                msg = await Task.Run(() => editrecord.EditRecordByColumn(userdata));
            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
                msg = ex.Message;
            }
            return msg;
        }
        [HttpPost]
        [Route("EditIfNotExistAdd")]
        public async Task<string> EditIfNotExistAdd(UIPostModel userdata)
        {
            var msg = string.Empty;
            if (userdata.PostColomn.Count == 0) return "No column to post";
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                await Task.Run(() => DatabaseSchema.GetColumntype(passport, userdata));
                var record = new RecordsActions(passport);
                msg = await Task.Run(() => record.EditRecordByColumn(userdata));
                if (msg.Contains("0"))
                {
                    if (await Task.Run(() => record.AddNewRow(userdata)))
                    {
                        msg = $"New Record Added!";
                    }
                    else
                    {
                        msg = $"Insufficient permissions to Add";
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
                msg = ex.Message;
            }
            return msg;
        }
        [HttpGet]
        [Route("TestExceptionMethod")]
        public void TestExceptionMethod()
        {
            var m = new SecurityAccess(_config);
            var passport = m.GetPassport(User.Identity.Name);
            try
            {
                throw new ArgumentException("This is an intentional test exception by Moti Mashiah");
            }
            catch (Exception ex)
            {
                LogErrorMessages.LogErrorMessage(ex, passport);
            }
        }

    }

}