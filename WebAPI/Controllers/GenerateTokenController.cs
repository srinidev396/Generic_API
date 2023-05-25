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


namespace FusionWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GenerateTokenController : ControllerBase
    {
        private Passport passport;
        IConfiguration _config;
        public GenerateTokenController(IConfiguration config)
        {
            passport = new Passport();
            _config = config;
        }
        [HttpGet]
        public string AuthenticateUser(string userName, string passWord, string database)
        {
            var m = new SecurityAccess(_config);

            var token = string.Empty;
            try
            {
                passport.SignIn(userName, passWord, String.Empty, m.sqlServername, database, m.sqlUsername, m.sqlPassword);
            }
            catch (Exception ex)
            {
                token = ex.Message;
                return token;
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
                token = jw.GenerateSecurityToken(Userprops);
            }
            else
            {
                token = "Faild to authenticate!";
            }

            return token;
        }
    }
}
