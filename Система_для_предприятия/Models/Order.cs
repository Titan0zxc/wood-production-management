using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Система_для_предприятия.Models
{
    public class Order
    {
        public string Name;
        public string Category;
        public string Requirements;
        public decimal Sum;
        public DateTime Deadline;
        public string ClientLogin;
        public string Status; 

        public override string ToString()
        {
            return $"{Name};{Category};{Requirements};{Sum};{Deadline};{ClientLogin};{Status}";
        }

        public static Order FromString(string line)
        {
            var parts = line.Split(';');
            return new Order
            {
                Name = parts[0],
                Category = parts[1],
                Requirements = parts[2],
                Sum = decimal.Parse(parts[3]),
                Deadline = DateTime.Parse(parts[4]),
                ClientLogin = parts[5],
                Status = parts[6]
            };
        }
    }

}
