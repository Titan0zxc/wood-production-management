using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Система_для_предприятия.Models
{
    public class Task
    {
        public string Name;
        public string Description;
        public DateTime Deadline;
        public string Person;
        public bool Done;

        public override string ToString()
        {
            return $"{Name};{Description};{Deadline};{Person};{Done}";
        }

        public static Task FromString(string line)
        {
            var parts = line.Split(';');
            return new Task
            {
                Name = parts[0],
                Description = parts[1],
                Deadline = DateTime.Parse(parts[2]),
                Person = parts[3],
                Done = bool.Parse(parts[4])
            };
        }
    }

}
