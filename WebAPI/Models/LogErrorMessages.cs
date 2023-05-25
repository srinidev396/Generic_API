using System.Diagnostics;
using System;
using Smead.Security;

namespace FusionWebApi.Models
{
    public class LogErrorMessages
    {
        public static void LogErrorMessage(Exception ex, Passport passport)
        {
            EventLog log = new EventLog();
            log.Source = "FusionWebAPI";
            log.WriteEntry($"DataBaseName: {passport.DatabaseName}; UserName: {passport.UserName};  Error:{ex.Message};", EventLogEntryType.Error, 1);
        }
    }

    
}
