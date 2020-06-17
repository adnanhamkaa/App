using App.DataAccess;
using App.Web.Models.Email;
using App.Web.Utilities;
using App.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace App.Web.Services.Repositories {
    public class EmailServices : ServiceBase, IEmailServices {
        private IMasterDataServices _setupSvc;
        public EmailServices(ApplicationDbContext context, IMasterDataServices setupSvc) {
            this.context = context;
            this._setupSvc = setupSvc;
        }

        public bool SendEmail(string[] emailDestination, string subject, string body) {
            try {
                //Client Configuration for Send Email
                var client = new SmtpClient(WebAppSettings.SMTPClientHost, WebAppSettings.SMTPClientPort) {
                    Credentials = new NetworkCredential(WebAppSettings.SMTPUsername, WebAppSettings.SMTPPassword),
                    EnableSsl = false,
                    Timeout = 100000
                };
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("adminspop@idx.com");
                foreach (var email in emailDestination) {
                    msg.To.Add(new MailAddress(email));
                }
                msg.Subject = subject;
                msg.Body = body;
                //Send Email
                client.Send(msg);

            } catch (Exception excp) {
                return false;
            }
            return true;
        }
        
        public bool SendEmailICS(string[] emailDestination, string subject, string body, ICSFileModel icsFileModel) {
            try {
                icsFileModel.IcsGuid = icsFileModel?.IcsGuid ?? Guid.NewGuid().ToString();

                //Client Configuration for Send Email
                var client = new SmtpClient(WebAppSettings.SMTPClientHost, WebAppSettings.SMTPClientPort) {
                    Credentials = new NetworkCredential(WebAppSettings.SMTPUsername, WebAppSettings.SMTPPassword),
                    EnableSsl = WebAppSettings.UseSSL,
                    Timeout = 100000,
                    
                };

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("spopplus@idx.co.id");
                msg.IsBodyHtml = true;
                foreach (var email in emailDestination) {
                    try
                    {
                        msg.To.Add(new MailAddress(email?.Trim()));
                    }
                    catch (Exception exc)
                    {
                        ErrorLog(exc);
                    }
                }
                msg.Subject = subject;
                //msg.Body = body;

                var htmlContentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Html);
                var avHtmlBody = AlternateView.CreateAlternateViewFromString(body, htmlContentType);
                msg.AlternateViews.Add(avHtmlBody);

                
                // Now Contruct the ICS file using string builder
                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine($"PRODID:-//{icsFileModel.Subject}");
                str.AppendLine("VERSION:2.0");
                str.AppendLine($"METHOD:{icsFileModel.Method??"REQUEST"}");
                str.AppendLine("BEGIN:VEVENT");
                str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", icsFileModel.StartDate));
                str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", icsFileModel.EndDate));
                str.AppendLine("LOCATION: " + icsFileModel.Location);
                
                str.AppendLine(string.Format("UID:{0}", icsFileModel.IcsGuid));
                str.AppendLine(string.Format("DESCRIPTION:{0}", icsFileModel.Description));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", icsFileModel.Description));
                str.AppendLine(string.Format("SUMMARY:{0}", msg.Subject));
                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", msg.From.Address));

                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", msg.To[0].DisplayName, msg.To[0].Address));

                str.AppendLine(string.Format("SEQUENCE:{0}", icsFileModel.Sequence));
                str.AppendLine(string.Format("X-MICROSOFT-CDO-APPT-SEQUENCE:{0}", icsFileModel.Sequence));
                var stst = icsFileModel.Method == "CANCEL" ? "CANCELLED" : "TENTATIVE";
                str.AppendLine($"X-MICROSOFT-CDO-BUSYSTATUS:{stst}");
                str.AppendLine($"STATUS:{stst}");

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");

                System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
                contype.Parameters.Add("method", icsFileModel.Method??"REQUEST");
                contype.Parameters.Add("name", "Meeting.ics");
                AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
                msg.AlternateViews.Add(avCal);
                msg.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                

                msg.IsBodyHtml = true;
                //Send Email

                client.Send(msg);

            } catch (Exception excp) {
                ErrorLog(excp);
                return false;
            }
            return true;
        }

        public bool SendEmailOnly(string[] emailDestination, string subject, string body) {
            try {
                
                //Client Configuration for Send Email
                var client = new SmtpClient(WebAppSettings.SMTPClientHost, WebAppSettings.SMTPClientPort) {
                    Credentials = new NetworkCredential(WebAppSettings.SMTPUsername, WebAppSettings.SMTPPassword),
                    EnableSsl = WebAppSettings.UseSSL,
                    Timeout = 100000,
                };

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("spopplus@idx.co.id");
                msg.IsBodyHtml = true;
                foreach (var email in emailDestination) {
                    try {
                        msg.To.Add(new MailAddress(email?.Trim()));
                    } catch (Exception exc) {
                        ErrorLog(exc);
                    }
                }
                msg.Subject = subject;
                //msg.Body = body;

                var htmlContentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Html);
                var avHtmlBody = AlternateView.CreateAlternateViewFromString(body, htmlContentType);
                msg.AlternateViews.Add(avHtmlBody);


                // Now Contruct the ICS file using string builder
                StringBuilder str = new StringBuilder();
                //str.AppendLine("BEGIN:VCALENDAR");
                //str.AppendLine($"PRODID:-//{icsFileModel.Subject}");
                //str.AppendLine("VERSION:2.0");
                //str.AppendLine($"METHOD:{icsFileModel.Method ?? "REQUEST"}");
                //str.AppendLine("BEGIN:VEVENT");
                //str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", icsFileModel.StartDate));
                //str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
                //str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", icsFileModel.EndDate));
                //str.AppendLine("LOCATION: " + icsFileModel.Location);

                //str.AppendLine(string.Format("UID:{0}", icsFileModel.IcsGuid));
                //str.AppendLine(string.Format("DESCRIPTION:{0}", icsFileModel.Description));
                //str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", icsFileModel.Description));
                //str.AppendLine(string.Format("SUMMARY:{0}", msg.Subject));
                //str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", msg.From.Address));

                //str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", msg.To[0].DisplayName, msg.To[0].Address));

                //str.AppendLine(string.Format("SEQUENCE:{0}", icsFileModel.Sequence));
                //str.AppendLine(string.Format("X-MICROSOFT-CDO-APPT-SEQUENCE:{0}", icsFileModel.Sequence));
                //var stst = icsFileModel.Method == "CANCEL" ? "CANCELLED" : "TENTATIVE";
                //str.AppendLine($"X-MICROSOFT-CDO-BUSYSTATUS:{stst}");
                //str.AppendLine($"STATUS:{stst}");

                //str.AppendLine("BEGIN:VALARM");
                //str.AppendLine("TRIGGER:-PT15M");
                //str.AppendLine("ACTION:DISPLAY");
                //str.AppendLine("DESCRIPTION:Reminder");
                //str.AppendLine("END:VALARM");
                //str.AppendLine("END:VEVENT");
                //str.AppendLine("END:VCALENDAR");

                //System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
                //contype.Parameters.Add("method", icsFileModel.Method ?? "REQUEST");
               // contype.Parameters.Add("name", "Meeting.ics");
                //AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
                //msg.AlternateViews.Add(avCal);
                //msg.Headers.Add("Content-class", "urn:content-classes:calendarmessage");


                msg.IsBodyHtml = true;
                //Send Email

                client.Send(msg);

            } catch (Exception excp) {
                ErrorLog(excp);
                return false;
            }
            return true;
        }

    }
}