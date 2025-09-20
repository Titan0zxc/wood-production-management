using Система_для_предприятия.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Система_для_предприятия.Services
{
    class TaskService
    {
        private static readonly string filePath = Path.Combine(Application.StartupPath, "Data", "tasks.txt");

        public static List<Task> LoadTasks()
        {
            if (!File.Exists(filePath)) return new List<Task>();
            var lines = File.ReadAllLines(filePath);
            return lines.Select(Task.FromString).ToList();
        }

        public static void SaveTask(Task task)
        {
            File.AppendAllText(filePath, task.ToString() + Environment.NewLine);
        }

        public static void SaveAll(List<Task> tasks)
        {
            var lines = tasks.Select(t => t.ToString());
            File.WriteAllLines(filePath, lines);
        }
    }
}
