using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Система_для_предприятия.Models;
using System.IO;
using System.Windows.Forms;

namespace Система_для_предприятия.Services
{
    public static class MessageService
    {
        private static readonly string filePath = Path.Combine(Application.StartupPath, "Data", "messages.txt");

        public static void SaveMessage(Система_для_предприятия.Models.Message message) => File.AppendAllText(filePath, ToString(message) + Environment.NewLine);

        private static string ToString(System.Windows.Forms.Message message)
        {
            throw new NotImplementedException();
        }

        public static List<Система_для_предприятия.Models.Message> LoadMessages(string orderId)
        {
            if (!File.Exists(filePath)) return new List<Система_для_предприятия.Models.Message>();

            return File.ReadAllLines(filePath)
                .Select(FromString)
                .Where(m => m.OrderId == orderId)
                .ToList();
        }

        private static string ToString(Система_для_предприятия.Models.Message msg)
        {
            return $"{msg.OrderId};{msg.FromRole};{msg.Timestamp};{msg.Text.Replace(";", "|")}";
        }

        private static Система_для_предприятия.Models.Message FromString(string line)
        {
            var parts = line.Split(';');
            return new Система_для_предприятия.Models.Message
            {
                OrderId = parts[0],
                FromRole = parts[1],
                Timestamp = DateTime.Parse(parts[2]),
                Text = parts[3].Replace("|", ";")
            };
        }
    }
}
