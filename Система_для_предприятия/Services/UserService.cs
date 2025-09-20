using Система_для_предприятия.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Система_для_предприятия.Services
{
    class UserService
    {
        private static readonly string filePath = Path.Combine(Application.StartupPath, "Data", "users.txt");

        public static List<User> LoadUsers()
        {
            string path = Path.Combine(Application.StartupPath, "Data", "users.txt");
            if (!File.Exists(path)) return new List<User>();

            return File.ReadAllLines(path)
                .Select(User.FromString)
                .ToList();
        }

        public static void SaveUser(User user)
        {
            File.AppendAllText(filePath, user.ToString() + Environment.NewLine);
        }

        public static User FindUser(string login, string password)
        {
            return LoadUsers().FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public static bool IsLoginTaken(string login)
        {
            return LoadUsers().Any(u => u.Login == login);
        }
    }
}
