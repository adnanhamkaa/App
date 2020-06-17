using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Utilities {
    public static class FlashMessageExtensions {
        public static ActionResult Error(this ActionResult result, string message) {
            message = System.Web.HttpUtility.HtmlEncode(message);
            CreateCookieWithFlashMessage(Notification.Danger, message);
            return result;
        }

        public static ActionResult Warning(this ActionResult result, string message) {
            message = System.Web.HttpUtility.HtmlEncode(message);
            CreateCookieWithFlashMessage(Notification.Warning, message);
            return result;
        }

        public static ActionResult Success(this ActionResult result, string message) {
            message = System.Web.HttpUtility.HtmlEncode(message);
            CreateCookieWithFlashMessage(Notification.Success, message);
            return result;
        }

        public static ActionResult Information(this ActionResult result, string message) {
            message = System.Web.HttpUtility.HtmlEncode(message);
            CreateCookieWithFlashMessage(Notification.Info, message);
            return result;
        }

        public static ActionResult ShowReminder(this ActionResult result) {
            
            CreateCookieWithFlashMessage(Notification.Reminder, "Reminder");
            return result;
        }

        public static ActionResult File(this ActionResult result, string filePath) {
            CreateCookieWithFlashMessage(Notification.File, "File tersedia di " + filePath);
            return result;
        }

        public static ActionResult File(this RedirectResult result, string filePath) {
            CreateCookieWithFlashMessage(Notification.File, "File tersedia di " + filePath);
            return result;
        }

        private static void CreateCookieWithFlashMessage(Notification notification, string message) {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(string.Format("Flash.{0}", notification), message) { Path = "/", HttpOnly = false });
        }

        private enum Notification {
            Danger,
            Warning,
            Success,
            Info,
            File,
            Reminder
        }
    }
}