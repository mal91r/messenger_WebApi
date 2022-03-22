using Microsoft.AspNetCore.Mvc;
using PeerGrade7.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PeerGrade7.Controllers
{
    /// <summary>
    /// Контроллер сообщений.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly Random random;

        /// <summary>
        /// Список сообщений. Является временным хранилищем, а json-файл - основым.
        /// </summary>
        public static List<Messages> messages;

        /// <summary>
        /// Конструктор без параментров.
        /// Инициализирует список пользователей в зависимости от содержимого json-файла.
        /// </summary>
        public MessagesController()
        {
            random = new Random();
            string path = @"wwwroot\messages.json";
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string jsonString = Encoding.Default.GetString(buffer);
                try
                {
                    messages = JsonSerializer.Deserialize<List<Messages>>(jsonString);
                }
                catch (Exception)
                {
                    messages = new List<Messages>();
                }
            }
            path = @"wwwroot\users.json";
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string jsonString = Encoding.Default.GetString(buffer);
                try
                {
                    UsersController.users = JsonSerializer.Deserialize<List<Users>>(jsonString);
                }
                catch (Exception)
                {
                    UsersController.users = new List<Users>();
                }
            }
        }

        /// <summary>
        /// Метод возвращает список сообщений(может быть пустым).
        /// </summary>
        /// <returns>Ок и список сообщений</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(messages);
        }

        /// <summary>
        /// Метод инициализирует список сообщений(в случае, если он пуст или не существует).
        /// </summary>
        /// <returns>Ок без параметров</returns>
        [HttpPost]
        public IActionResult Post()
        {
            string path = @"wwwroot\messages.json";
            string jsonString = System.IO.File.ReadAllText(path);
            try
            {
                messages = JsonSerializer.Deserialize<List<Messages>>(jsonString);
                if (messages.Count == 0)
                {
                    messages = GenerateMessages();
                    jsonString = JsonSerializer.Serialize(messages);
                    System.IO.File.WriteAllText(path, jsonString);
                }
            }
            catch (Exception)
            {
                messages = GenerateMessages();
                jsonString = JsonSerializer.Serialize(messages);
                System.IO.File.WriteAllText(path, jsonString);
            }
            return Ok();
        }

        private List<Messages> GenerateMessages()
        {
            int numberOfUsers, numberOfMessages = random.Next(1, 40);
            string reciever, sender, message = "", subject = "";
            List<Messages> messages = new List<Messages>();
            if (UsersController.users != null && UsersController.users.Count != 0)
            {
                numberOfUsers = UsersController.users.Count;
                string path = @"wwwroot\data\Morgenshtern_12.txt";
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string text = reader.ReadToEnd();
                        string[] wordsInText = text.Split(new char[] { '\t', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < numberOfMessages; i++)
                        {
                            reciever = UsersController.users[random.Next(numberOfUsers)].Email;
                            sender = UsersController.users[random.Next(numberOfUsers)].Email;
                            int subjectLength = random.Next(1, 5);
                            for (int j = 0; j < subjectLength; j++)
                            {
                                subject += wordsInText[random.Next(wordsInText.Length)] + " ";
                            }
                            int messageLength = random.Next(5, 40);
                            for (int j = 0; j < messageLength; j++)
                            {
                                message += wordsInText[random.Next(wordsInText.Length)] + " ";
                            }
                            messages.Add(new Messages() { RecieverId = reciever, SenderId = sender, Message = message.Trim(), Subject = subject.Trim() });  
                            subject = "";
                            message = "";
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Нарушена целостность файлов из папки wwwroot.");
                }
            }
            return messages;
        }

        /// <summary>
        /// Метод получается список сообщений, отправленных данным пользователем. 
        /// </summary>
        /// <param name="email">Идентификатор отправителя</param>
        /// <returns>Ок, если сообщения есть, NotFound, если их нет</returns>
        [HttpGet("From")]
        public IActionResult GetFrom(string email)
        {
            var mess = messages.Where(message => message.SenderId == email).ToList();
            if(mess.Count == 0)
            {
                return NotFound(email);
            }
            return Ok(mess);
        }

        /// <summary>
        /// Метод получает список сообщений, полученных данным пользователем.
        /// </summary>
        /// <param name="email">Идентификатор получателя</param>
        /// <returns>Ок, если сообщения есть, NotFound, если их нет</returns>
        [HttpGet("To")]
        public IActionResult GetTo(string email)
        {
            var mess = messages.Where(message => message.RecieverId == email).ToList();
            if (mess.Count == 0)
            {
                return NotFound(email);
            }
            return Ok(mess);
        }

        /// <summary>
        /// Метод получает список всех писем по отправителю и получателю.
        /// </summary>
        /// <param name="senderEmail">Идентификатор отправителя</param>
        /// <param name="recieverEmail">Идентификатор получателя</param>
        /// <returns>Ок, если сообщения есть, NotFound, если их нет</returns>
        [HttpGet("FromTo")]
        public IActionResult GetFromTo(string senderEmail, string recieverEmail)
        {
            var mess = messages.Where(message => message.RecieverId == recieverEmail && message.SenderId == senderEmail).ToList();
            if (mess.Count == 0)
            {
                return NotFound();
            }
            return Ok(mess);
        }

        /// <summary>
        /// Метод добавляет сообщение в базу.
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns>Ок, если все хорошо, NotFound, если одного из идентификаторов нет в базе, и BadRequest в случае некорректного запроса</returns>
        [HttpPost("AddMessage")]
        public IActionResult PostBody([FromBody] Messages message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (UsersController.users.Select(person => person.Email).Contains(message.SenderId) && UsersController.users.Select(person => person.Email).Contains(message.RecieverId))
            {
                messages.Add(message);
                string jsonString = JsonSerializer.Serialize(messages);
                string path = @"wwwroot\messages.json";
                System.IO.File.WriteAllText(path, jsonString);
                return Ok(message);
            }
            return NotFound(message);
        }
    }
}
