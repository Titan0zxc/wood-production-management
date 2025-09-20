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
    public partial class ManagerForm: Form
    {
        private ComboBox cmbChatTarget;
        private ListBox lstOrders, lstMessages;
        private TextBox txtNewMessage, txtDetails;
        private Button btnSendMessage, btnRequestDev, btnRequestPrepay, btnRequestSupplier, btnToProduction, btnToTesting, btnPayPenalty, btnSendOrder, btnLogout;
        private List<Order> allOrders;
        private Order selectedOrder;


        public ManagerForm()
        {
            InitializeComponent();
            this.Text = "Кабинет менеджера";
            this.Width = 1250;
            this.Height = 750;

            cmbChatTarget = new ComboBox { Top = 20, Left = 20, Width = 200 };
            cmbChatTarget.Items.AddRange(new[] { "Заказчик", "Разработчик", "Поставщик", "Производитель", "Тестировщик" });
            cmbChatTarget.SelectedIndex = 0;
            cmbChatTarget.SelectedIndexChanged += (s, e) => LoadMessages();

            lstOrders = new ListBox { Top = 60, Left = 20, Width = 400, Height = 200 };
            lstOrders.SelectedIndexChanged += LstOrders_SelectedIndexChanged;

            txtDetails = new TextBox { Top = 270, Left = 20, Width = 400, Height = 120, Multiline = true, ReadOnly = true };

            lstMessages = new ListBox { Top = 20, Left = 440, Width = 760, Height = 250 };
            txtNewMessage = new TextBox { Top = 280, Left = 440, Width = 500, Height = 60, Multiline = true };

            btnSendMessage = new Button { Text = "Отправить сообщение", Top = 350, Left = 440, Width = 200 };
            btnSendMessage.Click += BtnSendMessage_Click;

            btnRequestDev = new Button { Text = "Запрос разработчику", Top = 400, Left = 440, Width = 200 };
            btnRequestDev.Click += (s, e) => UpdateStatusAndNotify("Ожидает подтверждения от разработчика", "Пожалуйста, подтвердите возможность реализации проекта.", "Разработчик");

            btnRequestPrepay = new Button { Text = "Запрос предоплаты", Top = 400, Left = 660, Width = 200 };
            btnRequestPrepay.Click += (s, e) => UpdateStatusAndNotify("Ожидает предоплаты", "Пожалуйста, внесите предоплату.", "Заказчик");

            btnRequestSupplier = new Button { Text = "Запрос поставщику", Top = 450, Left = 440, Width = 200 };
            btnRequestSupplier.Click += (s, e) => UpdateStatusAndNotify("Сверка необходимых материалов", "Пожалуйста, проверьте наличие необходимых материалов.", "Поставщик");

            btnToProduction = new Button { Text = "Отправить в производство", Top = 450, Left = 660, Width = 200 };
            btnToProduction.Click += (s, e) => UpdateStatusAndNotify("Скоро начнем производство", "Производство будет запущено скоро.", "Производитель");

            btnToTesting = new Button { Text = "Отправить на тестирование", Top = 450, Left = 880, Width = 200 };
            btnToTesting.Click += (s, e) => UpdateStatusAndNotify("Передано тестировщику", "Проект передан на тестирование.", "Тестировщик");

            btnPayPenalty = new Button { Text = "Оплата неустойки", Top = 500, Left = 440, Width = 200 };
            btnPayPenalty.Click += (s, e) =>
            {
                if (selectedOrder == null || selectedOrder.Status != "Готовый заказ")
                {
                    MessageBox.Show("Оплата неустойки возможна только для статуса 'Готовый заказ'.");
                    return;
                }

                UpdateStatusAndNotify("Ожидание оплаты неустойки","Пожалуйста, оплатите неустойку, чтобы мы могли подготовить заказ к отправке.", "Заказчик");
            };

            btnSendOrder = new Button { Text = "Отправить заказ", Top = 500, Left = 660, Width = 200 };
            btnSendOrder.Click += (s, e) =>
            {
                if (selectedOrder == null || selectedOrder.Status != "Подготовка к отправке")
                {
                    MessageBox.Show("Заказ не готов к отправке. Ожидается завершение всех предыдущих этапов.");
                    return;
                }

                UpdateStatusAndNotify("Заказ в пути", "Заказ отправлен заказчику.", "Заказчик");
            };

            btnLogout = new Button { Text = "На главную", Top = 550, Left = 440, Width = 200 };
            btnLogout.Click += (s, e) => { this.Hide(); new LoginForm().Show(); this.Close(); };

            this.Controls.AddRange(new Control[] {
                cmbChatTarget, lstOrders, txtDetails,
                lstMessages, txtNewMessage, btnSendMessage,
                btnRequestDev, btnRequestPrepay, btnRequestSupplier,
                btnToProduction, btnToTesting, btnPayPenalty, btnSendOrder, btnLogout
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
            allOrders = OrderService.LoadOrders();
            lstOrders.Items.Clear();
            foreach (var order in allOrders)
            {
                lstOrders.Items.Add($"{order.Name} | {order.ClientLogin} | {order.Status}");
            }
        }

        private void LstOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOrders.SelectedIndex == -1) return;

            selectedOrder = allOrders[lstOrders.SelectedIndex];

            txtDetails.Text = $"Модель: {selectedOrder.Name}\r\n" +
                              $"Категория: {selectedOrder.Category}\r\n" +
                              $"Размеры: {selectedOrder.Requirements}\r\n" +
                              $"Сумма: {selectedOrder.Sum} руб\r\n" +
                              $"Срок: {selectedOrder.Deadline:dd.MM.yyyy}\r\n" +
                              $"Заказчик: {selectedOrder.ClientLogin}\r\n" +
                              $"Статус: {selectedOrder.Status}";

            LoadMessages();
        }

        private void LoadMessages()
        {
            lstMessages.Items.Clear();
            if (selectedOrder == null || cmbChatTarget.SelectedItem == null) return;

            string orderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin;
            string chatRole = cmbChatTarget.SelectedItem.ToString();

            var messages = MessageService.LoadMessages(orderId)
                .Where(m => m.FromRole == "Менеджер" || m.FromRole == chatRole)
                .ToList();

            foreach (var msg in messages)
            {
                lstMessages.Items.Add($"[{msg.Timestamp:dd.MM HH:mm}] {msg.FromRole}: {msg.Text}");
            }
        }

        private void BtnSendMessage_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null || string.IsNullOrWhiteSpace(txtNewMessage.Text) || cmbChatTarget.SelectedItem == null) return;

            string orderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin;

            var msg = new Система_для_предприятия.Models.Message
            {
                OrderId = orderId,
                FromRole = "Менеджер",
                Text = txtNewMessage.Text.Trim(),
                Timestamp = DateTime.Now
            };

            MessageService.SaveMessage(msg);
            txtNewMessage.Clear();
            LoadMessages();
        }

        private void UpdateStatusAndNotify(string newStatus, string messageText, string toRole)
        {
            if (selectedOrder == null) return;

            selectedOrder.Status = newStatus;
            OrderService.UpdateOrder(selectedOrder);
            LoadOrders();

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Менеджер",
                Text = messageText,
                Timestamp = DateTime.Now
            });

            LoadMessages();
        }

        private void SendPreset(string text)
        {
            if (selectedOrder == null) return;

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Менеджер",
                Text = text,    
                Timestamp = DateTime.Now
            });

            LoadMessages();
        }
    }
}
