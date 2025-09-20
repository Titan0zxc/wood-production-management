using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Система_для_предприятия.Models
{
    public class Message
    {
        public string OrderId;
        public string FromRole;
        public string Text;
        public DateTime Timestamp;

        public override string ToString()
        {
            return $"{Timestamp:G} [{FromRole}]: {Text}";
        }
    }
}
