using MimeKit.Text;

namespace HMLib
{
    public class MessageParams
    {
        /// <summary>
        /// Псевдоним отправителя
        /// </summary>
        public static string SenderName { get; set; }
        /// <summary>
        /// Определяет кому посылать ответ на письмо
        /// </summary>
        public static string ReplyTo { get; set; }
        /// <summary>
        /// Электронный адрес получателя
        /// </summary>
        public static string MailTo { get; set; }
        /// <summary>
        /// Заголовок письма
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// Тело письма
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Определяет формат тела заголовка (HTML, Text)
        /// </summary>
        public static TextFormat TextFormat { get; set; }
    }
}
