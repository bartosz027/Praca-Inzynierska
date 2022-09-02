using System;

using System.Net;
using System.Net.Mail;

namespace Network.Server.Core {

    internal static class SMTP {
        public static void Init(string email, string password) {
            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password)) {
                _Email = email;
                _Password = password;
            }
        }

        public static void SendMail(string receiver, string subject, string body) {
            if (_SMTP == null) {
                _SMTP = new SmtpClient {
                    Host = "smtp-mail.outlook.com",
                    Port = 587,

                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,

                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_Email, _Password)
                };
            }

            if (_SMTP != null) {
                var message = new MailMessage(_Email, receiver) {
                    Subject = subject,
                    Body = body
                };

                _SMTP.Send(message);
            }
            else {
                throw new Exception("[SMTP]: Not initialized!");
            }
        }

        // SMTP
        private static SmtpClient _SMTP;

        // Credentials
        private static string _Email;
        private static string _Password;
    }

}