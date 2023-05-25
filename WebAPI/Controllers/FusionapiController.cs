using Microsoft.AspNetCore.Mvc;
using System;
using Smead.Security;
using System.Configuration;


namespace FusionWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class FusionapiController : ControllerBase
    {
        //    [HttpGet]
        //    [Route("api/Getmyname")]
        //    public string Getmyname(string name)
        //    {
        //        return "your name is: " + name;
        //    }
        //    [HttpPost]
        //    [Route("api/getauth")]
        //    public auth GetAutmodel(auth person)
        //    {
        //        var x = new auth();
        //        x.UserName = "moti";
        //        x.Password = "aldskjfl";
        //        x.ExpirationSeconds = 50;
        //        x.DatabaseName = "database";
        //        return x;
        //    }
        //    [HttpGet]
        //    [Route("api/whatisup")]
        //    public string whatisup()
        //    {
        //        return "Hello";
        //    }
        //    [HttpGet]
        //    [Route("api/getviewdata")]
        //    public int getdatafromview(int viewid)
        //    {
        //        return viewid;
        //    }
        //    [HttpPost]
        //    [Route("api/GenerateToken")]
        //    public string SetAuthentication(auth param)
        //    {
        //        var db = Attachments.GetDatabase(param.DatabaseName, string.Format("TAB FusionRMS {0}", "Document Service"));
        //        if (db == null)
        //            throw new Exception("DatabaseNotFound");
        //        Passport pass = new Passport();
        //        pass.SignIn(param.UserName, param.Password, string.Empty, db.Server, db.Database, db.UserName, db.Password);
        //        if (!pass.SignedIn)
        //            throw new Exception("NotAuthorized");

        //        if (param.ExpirationSeconds < 0 || param.ExpirationSeconds > 600)
        //            param.ExpirationSeconds = 90;

        //        return Encrypt.AesEncrypt(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", ReverseExpiry(DateTime.Now.ToUniversalTime().AddSeconds(param.ExpirationSeconds), false), string.Format(@"{0}\{1}", db.Server, db.Database), pass.UserId, param.UserName, param.Password, db.UserName, db.Password));

        //    }
        //    private static string ReverseExpiry(DateTime expiryDate, bool forNow)
        //    {
        //        if (forNow)
        //            return string.Format("{0}/{1}/{2} {3}:{4}:{5}", expiryDate.Month.ToString("00"), expiryDate.Day.ToString("00"), expiryDate.Year.ToString("0000"), expiryDate.Hour.ToString("00"), expiryDate.Minute.ToString("00"), expiryDate.Second.ToString("00"));
        //        return string.Format("{0}:{1}:{2}:{3}:{4}:{5}", expiryDate.Second.ToString("00"), expiryDate.Minute.ToString("00"), expiryDate.Hour.ToString("00"), expiryDate.Day.ToString("00"), expiryDate.Month.ToString("00"), expiryDate.Year.ToString("0000"));
        //    }
        //}
        //public class auth
        //{
        //    public string UserName { get; set; }
        //    public string Password { get; set; }
        //    public int ExpirationSeconds { get; set; }
        //    public string DatabaseName { get; set; }
        //}
    }
}
