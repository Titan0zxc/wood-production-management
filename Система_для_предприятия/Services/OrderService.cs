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
    public static class OrderService
    {
        private static readonly string filePath = Path.Combine(Application.StartupPath, "Data", "orders.txt");

        public static List<Order> LoadOrders()
        {
            if (!File.Exists(filePath))
                return new List<Order>();

            var lines = File.ReadAllLines(filePath);
            return lines.Select(Order.FromString).ToList();
        }

        public static void SaveOrder(Order order)
        {
            File.AppendAllText(filePath, order.ToString() + Environment.NewLine);
        }

        public static void SaveAll(List<Order> orders)
        {
            var lines = orders.Select(o => o.ToString());
            File.WriteAllLines(filePath, lines);
        }

        public static void UpdateOrder(Order updatedOrder)
        {
            var orders = LoadOrders();

            // Ищем заказ по ключу (Name + ClientLogin)
            var index = orders.FindIndex(o => o.Name == updatedOrder.Name && o.ClientLogin == updatedOrder.ClientLogin);

            if (index >= 0)
            {
                orders[index] = updatedOrder;
            }
            else
            {
                orders.Add(updatedOrder);
            }

            SaveAll(orders);
        }
    }

}
