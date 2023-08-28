using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smead.Security;
using FusionWebApi.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using FusionWebApi.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Linq.Expressions;

namespace FusionWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GenerateTokenController : ControllerBase
    {
        private Passport passport;
        IConfiguration _config;
        private ILogger<GenerateTokenController> _logger;
        public GenerateTokenController(IConfiguration config, ILogger<GenerateTokenController> logger)
        {
            passport = new Passport();
            _config = config;
            _logger = logger;
        }
        [HttpGet]
        public SecurityAccess AuthenticateUser(string userName, string passWord, string database)
        {
            userName = userName == null ? string.Empty : userName;
            passWord = passWord == null ? string.Empty : passWord;
            database = database == null ? string.Empty : database;
            var m = new SecurityAccess(_config);
            m.ErrorMessages.TimeStamp = DateTime.Now;
            var token = string.Empty;
            try
            {
                passport.SignIn(userName, passWord, String.Empty, m.sqlServername, database, m.sqlUsername, m.sqlPassword);
            }
            catch (Exception ex)
            {
                CatchExceptions(m, ex);
                _logger.LogError($"{ex.Message} Username: {userName} DatabaseName {database}");
            }

            var jw = new JwtService(m);
            if (passport.SignedIn)
            {
                var userdata = new UserData()
                {
                    UserName = passport.UserName,
                    UserId = passport.UserId,
                    Database = passport.DatabaseName
                };

                string Userprops = Encrypt.EncryptParameters(JsonConvert.SerializeObject(userdata));
                m.Token = jw.GenerateSecurityToken(Userprops);

                m.ErrorMessages.FusionCode = (int)EventCode.LoginSuccess;
                m.ErrorMessages.FusionMessage = $"User {userName} successfully logged in to database: {database}!";
                _logger.LogInformation($"User {userName} successfully logged in to database: {database}!");
            }
            else
            {
                m.ErrorMessages.FusionCode = (int)EventCode.LoginFail;
                m.ErrorMessages.FusionMessage = "Faild to authenticate, username or password are incorrect!";
                _logger.LogError($"Username: {userName} DatabaseName {database}");
            }

            return m;
        }
        //written to prevent sensitive data exposure to the swagger UI Moti Mashiah.
        private void CatchExceptions(SecurityAccess m, Exception ex)
        {
            if (ex.Message.Contains("nLogin failed for user 'sa'"))
            {
                m.ErrorMessages.Message = "";
            }
            else if (ex.Message.Contains("SLAuditLogins"))
            {
                m.ErrorMessages.Message = "";
            }
            m.ErrorMessages.Code = ex.HResult;
            m.ErrorMessages.TimeStamp = DateTime.Now;
            
        }
    }
}
