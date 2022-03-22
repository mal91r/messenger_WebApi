using System;

namespace PeerGrade7.Models
{
    /// <summary>
    /// Модель сообщений.
    /// </summary>
    [Serializable]
    public class Messages
    {
        /// <summary>
        /// Тема сообщения.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Уникальный идентификатор отправителя.
        /// </summary>
        public string SenderId { get; set; }
        
        /// <summary>
        /// Уникальный идентификатор получателя.
        /// </summary>
        public string RecieverId { get; set; }
    }
}
