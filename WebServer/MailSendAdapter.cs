using System;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System.Linq;

namespace WebServer
{
    public class MailSendAdapter
    {
        SmtpClient _smtpClient = null;
        /// <summary>Электронная почта отправителя</summary>
        public string Login { get; }

        public MailSendAdapter(string SmtpServer, int SmtpPort, string Login, string Password)
        {
            this.Login = Login;
            _smtpClient = new SmtpClient();
            _smtpClient.Connect(SmtpServer, SmtpPort);
            _smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
            _smtpClient.Authenticate(Login, Password);
        }

        #region IBaseSendAdapter

        /// <summary>
        /// Отправка сообщения используя Email
        /// </summary>
        /// <param name="message">объект класса BaseMessage, содержащий в качестве
        /// содержимого (свойство MessageData) объект типа MessageParams</param>
        /// <returns>true в случае успешной отправки, иначе false</returns>
        public bool Send(MessageParams message)
        {
            #region Пример использования бодибилдера
            //Если нужно можно забабахать бодибилдера
            //Примерчик, чтоб не забыть:
            //            var builder = new BodyBuilder();
            //            builder.TextBody = @"Hey Alice,Will you be my +1?-- Joey";
            //            var image = builder.LinkedResources.Add(@"C:\Users\Joey\Documents\Selfies\selfie.jpg");
            //            image.ContentId = MimeUtils.GenerateMessageId();
            //            builder.HtmlBody = string.Format(@"<p>Hey Alice,<br><p>What are you up to this weekend? Monica is throwing one of her parties on Saturday<center><img src=""cid:{0}""></center>", image.ContentId);
            //            builder.Attachments.Add(@"C:\Users\Joey\Documents\party.ics");
            //            message.Body = builder.ToMessageBody();
            #endregion
            MimeMessage Mail = new MimeMessage();
            Mail.From.Add(new MailboxAddress(MessageParams.SenderName, Login));
            
            //Добавляе получателей в скрытую копию, чтобы они не знали о других адресатах
            Mail.Bcc.AddRange(MessageParams.MailTo.Trim()
                .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(addr => new MailboxAddress(addr))
                );
            Mail.Subject = message.Caption?.Trim();

            Mail.Body = new TextPart(MessageParams.TextFormat) {
                Text = message.Body
            };
            if (!string.IsNullOrEmpty(MessageParams.ReplyTo))
            {
                Mail.ReplyTo.Add(new MailboxAddress(MessageParams.ReplyTo.Trim()));
            }

            try
            {
                lock (_smtpClient.SyncRoot) {
                    _smtpClient.Send(Mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public Task<bool> SendAsync(MessageParams message)
        {
            return Task.Run(() => Send(message));
        }

        #region IDisposable
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _smtpClient.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #endregion
    }
}
