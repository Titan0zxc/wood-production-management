using Система_для_предприятия.Models;
using Система_для_предприятия.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Система_для_предприятия
{
    public partial class TesterForm: Form
    {
        private ListBox lstOrders, lstMessages;
        private TextBox txtDetails, txtNewMessage;
        private Button btnSendMessage, btnPass, btnFail, btnLogout;
        private List<Order> allOrders;
        private Order selectedOrder;
        public TesterForm()
        {
            InitializeComponent();
            this.Text = "Кабинет тестировщика";
            this.Width = 1200;
            this.Height = 600;

            lstOrders = new ListBox { Top = 20, Left = 20, Width = 400, Height = 200 };
            lstOrders.SelectedIndexChanged += LstOrders_SelectedIndexChanged;

            txtDetails = new TextBox { Top = 230, Left = 20, Width = 400, Height = 120, Multiline = true, ReadOnly = true };

            lstMessages = new ListBox { Top = 20, Left = 440, Width = 720, Height = 250 };
            txtNewMessage = new TextBox { Top = 280, Left = 440, Width = 500, Height = 60, Multiline = true };

            btnSendMessage = new Button { Text = "Отправить сообщение", Top = 350, Left = 440, Width = 200 };
            btnSendMessage.Click += BtnSendMessage_Click;

            btnFail = new Button { Text = "Тест не пройден", Top = 400, Left = 440, Width = 200 };
            btnFail.Click += (s, e) => UpdateStatus("Повторное производство", "Тест не пройден. Требуется повторное производство.");

            btnPass = new Button { Text = "Тест пройден", Top = 400, Left = 660, Width = 200 };
            btnPass.Click += (s, e) => UpdateStatus("Готовый заказ", "Тестирование пройдено. Заказ готов к следующему этапу.");

            btnLogout = new Button { Text = "На главную", Top = 450, Left = 440, Width = 200 };
            btnLogout.Click += (s, e) => { this.Hide(); new LoginForm().Show(); this.Close(); };

            this.Controls.AddRange(new Control[] {
                lstOrders, txtDetails, lstMessages, txtNewMessage,
                btnSendMessage, btnPass, btnFail, btnLogout
            });

            LoadOrders();
        }
        private void LoadOrders()
        {
            lstOrders.Items.Clear();
            allOrders = OrderService.LoadOrders();

            var testOrders = allOrders.Where(o =>
                o.Status == "Передано тестировщику" ||
                o.Status == "Повторное тестирование" ||
                o.Status == "Отправка на тестирование" // 👈 ДОБАВЛЕН
            ).ToList();

            foreach (var order in testOrders)
            {
                lstOrders.Items.Add($"{order.Name} | {order.ClientLogin} | {order.Status}");
            }
        }
        private void LstOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOrders.SelectedIndex == -1) return;

            var testOrders = allOrders.Where(o =>
                o.Status == "Передано тестировщику" ||
                o.Status == "Повторное тестирование" ||
                o.Status == "Отправка на тестирование" // 👈 ДОБАВЛЕН
            ).ToList();

            selectedOrder = testOrders[lstOrders.SelectedIndex];

            txtDetails.Text = $"Модель: {selectedOrder.Name}\r\n" +
                              $"Категория: {selectedOrder.Category}\r\n" +
                              $"Требования: {selectedOrder.Requirements}\r\n" +
                              $"Сумма: {selectedOrder.Sum} руб\r\n" +
                              $"Срок: {selectedOrder.Deadline.ToShortDateString()}\r\n" +
                              $"Заказчик: {selectedOrder.ClientLogin}\r\n" +
                              $"Статус: {selectedOrder.Status}";

            LoadMessages();

        }

        private void LoadMessages()
        {
            lstMessages.Items.Clear();
            if (selectedOrder == null) return;

            string orderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin;
            var messages = MessageService.LoadMessages(orderId)
                .Where(m => m.FromRole == "Тестировщик" || m.FromRole == "Менеджер").ToList();

            foreach (var msg in messages)
            {
                lstMessages.Items.Add($"[{msg.Timestamp:dd.MM HH:mm}] {msg.FromRole}: {msg.Text}");
            }
        }

        private void BtnSendMessage_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null || string.IsNullOrWhiteSpace(txtNewMessage.Text)) return;

            var msg = new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Тестировщик",
                Text = txtNewMessage.Text.Trim(),
                Timestamp = DateTime.Now
            };

            MessageService.SaveMessage(msg);
            txtNewMessage.Clear();
            LoadMessages();
        }

        private void UpdateStatus(string newStatus, string messageText)
        {
            if (selectedOrder == null) return;

            selectedOrder.Status = newStatus;
            OrderService.UpdateOrder(selectedOrder);

            var msg = new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Тестировщик",
                Text = messageText,
                Timestamp = DateTime.Now
            };

            MessageService.SaveMessage(msg);
            MessageBox.Show("Статус обновлён: " + newStatus);
            LoadOrders();
            LoadMessages();
        }
    }
}
