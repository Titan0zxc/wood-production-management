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
    public partial class ManufacturerForm: Form
    {
        private ListBox lstOrders;
        private TextBox txtDetails;
        private ListBox lstMessages;
        private TextBox txtNewMessage;
        private Button btnSendMessage;
        private Button btnStartProduction;
        private Button btnFinishProduction;
        private Button btnBack;

        private List<Order> allOrders;
        private Order selectedOrder;
        public ManufacturerForm()
        {
            InitializeComponent();
            this.Text = "Кабинет производителя";
            this.Width = 1100;
            this.Height = 600;

            lstOrders = new ListBox { Top = 20, Left = 20, Width = 400, Height = 200 };
            lstOrders.SelectedIndexChanged += LstOrders_SelectedIndexChanged;

            txtDetails = new TextBox { Top = 230, Left = 20, Width = 400, Height = 100, Multiline = true, ReadOnly = true };

            lstMessages = new ListBox { Top = 20, Left = 440, Width = 600, Height = 250 };
            txtNewMessage = new TextBox { Top = 280, Left = 440, Width = 400, Height = 60, Multiline = true };

            btnSendMessage = new Button { Text = "Отправить сообщение", Top = 350, Left = 440, Width = 200 };
            btnSendMessage.Click += BtnSendMessage_Click;

            btnStartProduction = new Button { Text = "Начали производство", Top = 400, Left = 440, Width = 200 };
            btnStartProduction.Click += (s, e) => UpdateStatus("Начали производство");

            btnFinishProduction = new Button { Text = "Закончили производство", Top = 400, Left = 660, Width = 200 };
            btnFinishProduction.Click += (s, e) => UpdateStatus("Отправка на тестирование");

            btnBack = new Button { Text = "На главную", Top = 500, Left = 440, Width = 200 };
            btnBack.Click += (s, e) =>
            {
                this.Hide();
                new LoginForm().Show();
                this.Close();
            };

            this.Controls.AddRange(new Control[] {
                lstOrders, txtDetails, lstMessages, txtNewMessage,
                btnSendMessage, btnStartProduction, btnFinishProduction, btnBack
            });

            LoadOrders();
        }
        private void LoadOrders()
        {
            lstOrders.Items.Clear();
            allOrders = OrderService.LoadOrders();

            var prodOrders = allOrders.Where(o =>
                o.Status == "Скоро начнем производство" ||
                o.Status == "Начали производство" ||
                o.Status == "Повторное производство" // ✅ ДОБАВЛЕН
            ).ToList();

            foreach (var order in prodOrders)
            {
                lstOrders.Items.Add($"{order.Name} | {order.ClientLogin} | {order.Status}");
            }
        }
        private void LstOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOrders.SelectedIndex == -1) return;

            var prodOrders = allOrders.Where(o =>
            o.Status == "Скоро начнем производство" ||
            o.Status == "Начали производство" ||
            o.Status == "Повторное производство" // ✅ ОБЯЗАТЕЛЬНО
                ).ToList();

            if (lstOrders.SelectedIndex >= prodOrders.Count) return;

            selectedOrder = prodOrders[lstOrders.SelectedIndex];

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
                .Where(m => m.FromRole == "Менеджер" || m.FromRole == "Производитель")
                .ToList();

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
                FromRole = "Производитель",
                Text = txtNewMessage.Text.Trim(),
                Timestamp = DateTime.Now
            };

            MessageService.SaveMessage(msg);
            txtNewMessage.Clear();
            LoadMessages();
        }

        private void UpdateStatus(string newStatus)
        {
            if (selectedOrder == null) return;

            selectedOrder.Status = newStatus;
            OrderService.UpdateOrder(selectedOrder);

            MessageBox.Show($"Статус обновлен: {newStatus}", "Успех");
            LoadOrders();
            LoadMessages();
        }
    }
}
