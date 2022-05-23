using System.Net;
using System.Net.Mail;

namespace Network.Server.Core {

    internal static class SMTP {
        public static void SendMail(string receiver, string subject, string body) {
            var smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,

                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,

                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Email, Password)
            };

            var message = new MailMessage(Email, receiver) {
                Subject = subject,
                Body = body
            };

            smtp.Send(message);
        }

        public static string Email { get; set; }
        public static string Password { get; set; }
    }

}