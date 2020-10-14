using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace BoaDataEngine.Library
{
    public static class HelperMethods
    {
        /// <summary>
        /// Shared method used to send a notification email to the Sku Team public folder and/or the OMDC Operator.
        /// </summary>
        public static void SendEmailWithLog(string message, Exception ex, bool notifyOperator, string hostType)
        {
            using (MailMessage mail = new MailMessage())
            {
                StringBuilder body = new StringBuilder();

                mail.From = new MailAddress("Sku_data_engine@menards.net");

                if (hostType == "Live")
                {
                    mail.ReplyToList.Add(new MailAddress(Constants.SkuTeamEmailAddress));
                    mail.To.Add(Constants.SkuTeamEmailAddress);
                }
                else if (Debugger.IsAttached)
                {
                    mail.To.Add(Environment.UserName + "@menards.net");
                }
                else
                {
                    mail.ReplyToList.Add(new MailAddress(Constants.TestingEmailAddress));
                    mail.To.Add("isboappsteam@menards.net");
                }

                if (hostType == "Live")
                {
                    mail.To.Add(Constants.SkuTeamPublicFolder);
                }

                if (notifyOperator && hostType == "Live")
                {
                    mail.Priority = MailPriority.High;

                    if (hostType == "Live")
                    {
                        mail.To.Add(Constants.OperatorEmailAddress);
                    }
                }

                mail.IsBodyHtml = false;

                mail.Subject = hostType == "Live" ? 
                    "Sku Data Engine Service Error" : "TESTING! Sku Data Engine Service Error";

                if (hostType != "Live")
                {
                    mail.Subject += " (" + hostType + ")";
                }

                body.AppendLine("An error has occurred in the Sku Data Engine service on " + Environment.MachineName + ".");
                body.AppendLine();

                // Only send to the operator when working on LIVE
                if (notifyOperator && hostType == "Live")
                {
                    body.AppendLine("PLEASE CALL THE SKUMAN TEAM ON-CALL PERSON NOW.");
                    body.AppendLine();
                }

                body.AppendLine(message);
                body.AppendLine();

                if (ex != null)
                {
                    body.AppendLine("----------------------------------- Exception Details -----------------------------------");
                    body.AppendLine(ex.ToString());
                    body.AppendLine();
                }

                mail.Body = body.ToString();

                using (SmtpClient smtp = new SmtpClient(Constants.SmtpHost))
                {
                    smtp.Send(mail);
                }
            }
        }

        /// <summary>
        /// Shared method used to send an email to the users letting them know a new order has arrived.
        /// </summary>
        public static void SendEmail(string toAddress, string subject, string message, string hostType)
        {
            using (MailMessage mail = new MailMessage())
            {
                StringBuilder body = new StringBuilder();

                mail.From = new MailAddress(toAddress);

                if (hostType == "Live")
                {
                    mail.ReplyToList.Add(new MailAddress(Constants.SkuTeamEmailAddress));
                    mail.To.Add(toAddress);
                }
                else if (Debugger.IsAttached)
                {
                    mail.To.Add(Environment.UserName + "@menards.net");
                }
                else
                {
                    mail.ReplyToList.Add(new MailAddress(Constants.TestingEmailAddress));
                    mail.To.Add(Constants.TestingEmailAddress);
                }

                mail.IsBodyHtml = false;

                mail.Subject = subject;

                if (hostType != "Live")
                {
                    mail.Subject += " (" + hostType + ")";
                }

                body.AppendLine(message);
                body.AppendLine();

                mail.Body = body.ToString();

                using (SmtpClient smtp = new SmtpClient(Constants.SmtpHost))
                {
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// Shared method used to send an email to the users letting them know a new order has arrived.
        /// </summary>
        public static void SendEmailWithAttachment(string toAddress, string subject, string message, string fileAttachment, string hostType)
        {
            using (var mail = new MailMessage())
            {
                StringBuilder body = new StringBuilder();

                mail.From = new MailAddress(toAddress);

                if (hostType == "Live")
                {
                    mail.ReplyToList.Add(new MailAddress(Constants.SkuTeamEmailAddress));
                    mail.To.Add(toAddress);
                }
                else if (Debugger.IsAttached)
                {
                    mail.To.Add(Environment.UserName + "@menards.net");
                }
                else
                {
                    mail.ReplyToList.Add(new MailAddress(Constants.TestingEmailAddress));
                    mail.To.Add(Constants.TestingEmailAddress);
                }

                mail.IsBodyHtml = false;

                mail.Subject = subject;

                if (hostType != "Live")
                {
                    mail.Subject += " (" + hostType + ")";
                }

                mail.Attachments.Add(new Attachment(fileAttachment));

                body.AppendLine(message);
                body.AppendLine();

                mail.Body = body.ToString();

                using (SmtpClient smtp = new SmtpClient(Constants.SmtpHost))
                {
                    smtp.Send(mail);
                }
            }
        }

        /// <summary>
        /// Shared method used to purge archived xml files older than the specified number of days.
        /// </summary>
        public static void PurgeArchives(string archivePath, int archiveDays)
        {
            PurgeArchives(archivePath, archiveDays, "xml");
        }

        public static void PurgeArchives(string archivePath, int archiveDays, string extension)
        {
            //Archive the request folder if it exists; otherwise create it.
            if (Directory.Exists(archivePath))
            {
                var di = new DirectoryInfo(archivePath);
                var files = extension != null ? di.GetFiles("*." + extension) : di.GetFiles();

                foreach (var fi in files)
                {
                    if (DateTime.Compare(fi.CreationTime.AddDays(archiveDays), DateTime.Now) < 0)
                    {
                        fi.Delete();
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(archivePath);
            }
        }
    }
}
