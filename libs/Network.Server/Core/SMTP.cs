using System;
using System.Net;
using System.Net.Mail;

namespace Network.Server.Core {

    internal static class SMTP {
        public static void Init(string email, string password) {
            if(!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password)) {
                _Email = email;
                _Password = password;
            }
            else {
                throw new ArgumentException("[SMTP]: Invalid \'email\' or \'password\'!");
            }

            _Initialized = true;
        }

        public static void SendMail(string receiver, string subject, string body) {
            if(_Initialized) {
                var smtp = new SmtpClient {
                    Host = "smtp.gmail.com",
                    Port = 587,

                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,

                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_Email, _Password)
                };

                var message = new MailMessage(_Email, receiver) {
                    Subject = subject,
                    Body = body
                };

                smtp.Send(message);
            }
            else {
                throw new Exception("[SMTP]: Not initialized!");
            }
        }

        // Flags
        private static bool _Initialized;

        // Login data
        private static string _Email;
        private static string _Password;
    }

}