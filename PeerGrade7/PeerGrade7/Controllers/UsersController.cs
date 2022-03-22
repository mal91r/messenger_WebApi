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
    /// Контроллер пользователей.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Random random;

        /// <summary>
        /// Список пользователей. Служит временным хранилищем, а json-файл - постоянным.
        /// </summary>
        public static List<Users> users;

        /// <summary>
        /// Конструктор без параметров. Производит инициализацию списка пользователей в зависимости от содержимого json-файла.
        /// </summary>
        public UsersController()
        {
            random = new Random();
            string path = @"wwwroot\users.json";
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string jsonString = Encoding.Default.GetString(buffer);
                try
                {
                    users = JsonSerializer.Deserialize<List<Users>>(jsonString);
                }
                catch (Exception)
                {
                    users = new List<Users>();
                }
            }
            path = @"wwwroot\messages.json";
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                string jsonString = Encoding.Default.GetString(buffer);
                try
                {
                    MessagesController.messages = JsonSerializer.Deserialize<List<Messages>>(jsonString);
                }
                catch (Exception)
                {
                    MessagesController.messages = new List<Messages>();
                }
            }
        }

        /// <summary>
        /// Метод получает список всех пользователей.
        /// </summary>
        /// <returns>Ок и список пользователей в качестве параметра</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(users);
        }

        /// <summary>
        /// Метод получает список пользователей, ограниченный параметрами.
        /// </summary>
        /// <param name="limit">Сколько пользователей нужно взять</param>
        /// <param name="offset">Сколько пользователей нужно пропустить</param>
        /// <returns>Ок, если параматры корректны, иначе BadRequest</returns>
        [HttpGet("Limit")]
        public IActionResult Get(int limit, int offset)
        {
            if (offset < 0 || limit <= 0)
            {
                return BadRequest();
            }
            return Ok(users.Skip(offset).Take(limit));
        }

        /// <summary>
        /// Метод производит первичную инициализацию списка пользоватлей.
        /// </summary>
        /// <returns>ОК без параметров</returns>
        [HttpPost]
        public IActionResult Post()
        {
            string path = @"wwwroot\users.json";
            string jsonString = System.IO.File.ReadAllText(path);
            try
            {
                users = JsonSerializer.Deserialize<List<Users>>(jsonString);
            }
            catch (Exception)
            {
                users = GenerateUsers().OrderBy(person => person.Email).ToList();
                jsonString = JsonSerializer.Serialize(users);
                System.IO.File.WriteAllText(path, jsonString);
            }
            return Ok();
        }

        private List<Users> GenerateUsers()
        {
            int numberOfUsers = random.Next(0, 20);
            List<Users> users = new List<Users>();

            try
            {
                for (int i = 0; i < numberOfUsers; i++)
                {
                    string name;
                    string secondname;
                    string domain;
                    string email;
                    char[] delimiterChars = { '\t', ' ', '\n', '\r' };
                    using (StreamReader sr = new StreamReader(@"wwwroot\data\firstNames.txt"))
                    {
                        name = sr.ReadToEnd().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries)[random.Next(67)];
                    }
                    using (StreamReader sr = new StreamReader(@"wwwroot\data\secondNames.txt"))
                    {
                        secondname = sr.ReadToEnd().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries)[random.Next(100)];
                    }
                    using (StreamReader sr = new StreamReader(@"wwwroot\data\domains.txt"))
                    {
                        domain = sr.ReadToEnd().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries)[random.Next(5)];
                    }
                    email = name + secondname + "@" + domain;
                    if (!users.Select(person => person.Email).Contains(email))
                    {
                        users.Add(new Users() { UserName = name, Email = email });
                    }
                    else
                    {
                        i--;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Нарушена целостность файлов из папки wwwroot.");
            }

            return users;
        }

        /// <summary>
        /// Метод находит пользователя по уникальному идентификатору.
        /// </summary>
        /// <param name="email">Уникальный идентификатор</param>
        /// <returns>ОК, если пользователь найдет, иначе NotFound</returns>
        [HttpGet("{email}")]
        public IActionResult Get(string email)
        {
            if (!users.Any(person => person.Email == email))
            {
                return NotFound(email);
            }
            return Ok(users.First(person => person.Email == email));
        }

        /// <summary>
        /// Метод добавляет пользователя в список.
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Ок, если пользователь новый и корректный, иначе BadRequest</returns>
        [HttpPost("AddUser")]
        public IActionResult PostBody([FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!users.Select(person => person.Email).Contains(user.Email))
            {
                users.Add(user);
                users.Sort((x, y) => x.Email.CompareTo(y.Email));
                string jsonString = JsonSerializer.Serialize(users);
                string path = @"wwwroot\users.json";
                System.IO.File.WriteAllText(path, jsonString);
                return Ok(user);
            }
            return BadRequest(user);
        }

    }
}
