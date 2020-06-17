using System;

namespace App.Web.Services.Contracts
{
    public interface ISysLogger
    {
        void LogError(Exception ex);
        void LogMessage(string message);
    }
}
