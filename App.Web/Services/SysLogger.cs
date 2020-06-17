using System;
using Elmah;
using App.Web.Services.Contracts;

namespace App.Web.Services {
    public class SysLogger : ISysLogger {
        /// <inheritdoc />
        public void LogError(Exception ex) {
            ErrorSignal.FromCurrentContext().Raise(ex);
        }

        /// <inheritdoc />
        public void LogMessage(string message) {
            ErrorSignal.FromCurrentContext().Raise(new Exception(message));
        }
    }
}