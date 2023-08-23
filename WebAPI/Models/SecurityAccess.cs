using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Smead.Security;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace FusionWebApi.Models
{

    public class SecurityAccess
    {
        public SecurityAccess(IConfiguration config)
        {
            sqlUsername = config.GetSection("Sql").GetSection("userName").Value;
            sqlPassword = config.GetSection("Sql").GetSection("password").Value;
            sqlServername = config.GetSection("Sql").GetSection("serverName").Value;
            Secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            ExpDate = config.GetSection("JwtConfig").GetSection("expirationInMinutes").Value;
            ErrorMessages = new ErrorMessages();
        }
   
        internal string sqlServername { get; set; }
        internal string sqlUsername { get; set; }
        internal string sqlPassword { get; set; }
        internal string Secret { get; set; }
        public string ExpDate { get; set; }
        public ErrorMessages ErrorMessages { get; set; }
        public string Token { get; set; }
        public Passport GetPassport(string userdata)
        {
            var passport = new Passport();
            try
            {
                var data = Encrypt.DecryptParameters(userdata);
                var ud = JsonConvert.DeserializeObject<UserData>(data);
                var start = DateTime.Now;
                var end = DateTime.Now;
                Debug.WriteLine($"Total Millisecond: { (end - start).TotalMilliseconds }");
                passport.SignIn(ud.UserName, "3kszs932ksdjjdjwqp00qkksj", string.Empty, sqlServername, ud.Database, sqlUsername, sqlPassword);
              
            }
            catch (System.Exception)
            {

                throw;
            }


            return passport;
        }


    }

    public class UserData
    {
        IConfiguration _config;
        public UserData() { }
        public UserData(IConfiguration config)
        {
            _config = config;
        }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string Database { get; set; }

    }

    
    

}
