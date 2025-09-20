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
    public partial class DeveloperForm: Form
    {
        private ListBox lstOrders;
        private TextBox txtDetails;
        private ListBox lstMessages;
        private TextBox txtNewMessage;
        private Button btnSendMessage;
        private Button btnApprove;
        private Button btnReject;
        private Button btnBeer;
        private Button btnLogout;

        private List<Order> allOrders;
        private Order selectedOrder;

        public DeveloperForm()
        {
            InitializeComponent();
            this.Text = "Кабинет разработчика";
            this.Width = 1200;
            this.Height = 600;

            lstOrders = new ListBox { Top = 20, Left = 20, Width = 400, Height = 200 };
            lstOrders.SelectedIndexChanged += LstOrders_SelectedIndexChanged;

            txtDetails = new TextBox { Top = 230, Left = 20, Width = 400, Height = 100, Multiline = true, ReadOnly = true };

            lstMessages = new ListBox { Top = 20, Left = 440, Width = 700, Height = 250 };
            txtNewMessage = new TextBox { Top = 280, Left = 440, Width = 700, Height = 60, Multiline = true };

            btnSendMessage = new Button { Text = "Отправить сообщение", Top = 350, Left = 440, Width = 200 };
            btnSendMessage.Click += BtnSendMessage_Click;

            btnApprove = new Button { Text = "Сможем реализовать", Top = 400, Left = 440, Width = 200 };
            btnApprove.Click += (s, e) => RespondToRequest("Разработка возможна.", "Ожидает подтверждения");

            btnReject = new Button { Text = "Не сможем реализовать", Top = 400, Left = 660, Width = 200 };
            btnReject.Click += (s, e) => RespondToRequest("Разработка невозможна по текущим условиям.", "Отклонено разработчиком");

            btnBeer = new Button { Text = "Пошли пить пиво", Top = 450, Left = 660, Width = 200 };
            btnBeer.Click += (s, e) => MessageBox.Show("\uD83C\uDF7B Время пива! Беру бокал и иду!");

            btnLogout = new Button { Text = "На главную", Top = 450, Left = 440, Width = 200 };
            btnLogout.Click += (s, e) => { this.Hide(); new LoginForm().Show(); this.Close(); };

            this.Controls.AddRange(new Control[]
            {
                lstOrders, txtDetails, lstMessages, txtNewMessage,
                btnSendMessage, btnApprove, btnReject, btnBeer, btnLogout
            });

            LoadOrders();
        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
        private void LoadOrders()
        {
            lstOrders.Items.Clear();
            allOrders = OrderService.LoadOrders();

            var devOrders = allOrders.Where(o => o.Status == "Ожидает подтверждения от разработчика").ToList();

            foreach (var order in devOrders)
            {
                lstOrders.Items.Add($"{order.Name} | {order.ClientLogin} | {order.Status}");
            }
        }

        private void LstOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOrders.SelectedIndex == -1) return;

            var devOrders = allOrders.Where(o => o.Status == "Ожидает подтверждения от разработчика").ToList();
            if (lstOrders.SelectedIndex < devOrders.Count)
            {
                selectedOrder = devOrders[lstOrders.SelectedIndex];

                txtDetails.Text = $"Модель: {selectedOrder.Name}\r\n" +
                                  $"Категория: {selectedOrder.Category}\r\n" +
                                  $"Размеры: {selectedOrder.Requirements}\r\n" +
                                  $"Сумма: {selectedOrder.Sum} руб\r\n" +
                                  $"Срок: {selectedOrder.Deadline:dd.MM.yyyy}\r\n" +
                                  $"Заказчик: {selectedOrder.ClientLogin}\r\n" +
                                  $"Статус: {selectedOrder.Status}";

                LoadMessages();
            }
        }

        private void LoadMessages()
        {
            lstMessages.Items.Clear();
            if (selectedOrder == null) return;

            string orderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin;
            var messages = MessageService.LoadMessages(orderId)
                .Where(m => m.FromRole == "Менеджер" || m.FromRole == "Разработчик")
                .ToList();

            foreach (var msg in messages)
            {
                lstMessages.Items.Add($"[{msg.Timestamp:dd.MM HH:mm}] {msg.FromRole}: {msg.Text}");
            }
        }

        private void BtnSendMessage_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null || string.IsNullOrWhiteSpace(txtNewMessage.Text)) return;

            var msg = new Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Разработчик",
                Text = txtNewMessage.Text.Trim(),
                Timestamp = DateTime.Now
            };

            MessageService.SaveMessage(msg);
            txtNewMessage.Clear();
            LoadMessages();
        }

        private void RespondToRequest(string messageText, string newStatus)
        {
            if (selectedOrder == null) return;

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Разработчик",
                Text = messageText,
                Timestamp = DateTime.Now
            });

            selectedOrder.Status = newStatus;
            OrderService.UpdateOrder(selectedOrder);

            LoadOrders();
            LoadMessages();
        }
    }
}
