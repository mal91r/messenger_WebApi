using System;

namespace PeerGrade7.Models
{
    /// <summary>
    /// Модель пользователя.
    /// </summary>
    [Serializable]
    public class Users
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string UserName { get; set; }   

        /// <summary>
        /// Адрес электронной почты, уникальный идетификатор.
        /// </summary>
        public string Email { get; set; }
    }
}
