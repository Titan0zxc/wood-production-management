using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Система_для_предприятия.Models
{
    public class User
    {
        public string Login;
        public string Password;
        public string Role;

        public override string ToString()
        {
            return $"{Login};{Password};{Role}";
        }

        public static User FromString(string line)
        {
            var parts = line.Split(';');
            return new User
            {
                Login = parts[0],
                Password = parts[1],
                Role = parts[2]
            };
        }
    }

}
