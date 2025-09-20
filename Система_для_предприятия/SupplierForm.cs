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
    public partial class SupplierForm: Form
    {
        private ListBox lstOrders, lstMessages;
        private TextBox txtDetails, txtNewMessage;
        private Button btnSendMessage, btnMaterialsAvailable, btnNeedToBuy, btnLogout;
        private List<Order> allOrders;
        private Order selectedOrder;
        private TextBox txtMaterials;

        public SupplierForm()
        {
            InitializeComponent();
            this.Text = "Поставщик";
            this.Width = 1200;
            this.Height = 600;

            lstOrders = new ListBox { Top = 20, Left = 20, Width = 400, Height = 200 };
            lstOrders.SelectedIndexChanged += LstOrders_SelectedIndexChanged;

            txtDetails = new TextBox { Top = 230, Left = 20, Width = 400, Height = 100, Multiline = true, ReadOnly = true };

            lstMessages = new ListBox { Top = 20, Left = 440, Width = 720, Height = 250 };
            txtNewMessage = new TextBox { Top = 280, Left = 440, Width = 400, Height = 60, Multiline = true };

            btnSendMessage = new Button { Text = "Отправить сообщение", Top = 350, Left = 440, Width = 200 };
            btnSendMessage.Click += BtnSendMessage_Click;

            btnMaterialsAvailable = new Button { Text = "Материалы есть", Top = 400, Left = 440, Width = 200 };
            btnMaterialsAvailable.Click += (s, e) => UpdateStatus("Подготовка к производству");

            btnNeedToBuy = new Button { Text = "Закупка материалов", Top = 400, Left = 660, Width = 200 };
            btnNeedToBuy.Click += (s, e) => UpdateStatus("Продление дедлайна на 3 дня для закупки материалов");

            btnLogout = new Button { Text = "На главную", Top = 450, Left = 440, Width = 200 };
            btnLogout.Click += (s, e) => { new LoginForm().Show(); this.Close(); };

            txtMaterials = new TextBox { Top = 340, Left = 20, Width = 400, Height = 60, Multiline = true, ReadOnly = true };
            this.Controls.Add(txtMaterials);

            this.Controls.AddRange(new Control[] {
                lstOrders, txtDetails, lstMessages, txtNewMessage,
                btnSendMessage, btnMaterialsAvailable, btnNeedToBuy, btnLogout, txtMaterials
            });

            LoadOrders();
        }

        private void LoadOrders()
        {
            lstOrders.Items.Clear();
            allOrders = OrderService.LoadOrders();
            var relevantOrders = allOrders.Where(o => o.Status.Contains("Поставщик") || o.Status.Contains("материал")).ToList();
            foreach (var order in relevantOrders)
            {
                lstOrders.Items.Add($"{order.Name} | {order.ClientLogin} | {order.Status}");
            }
        }

        private void LstOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOrders.SelectedIndex == -1) return;

            // Восстановим список только с подходящими заказами
            var relevantOrders = allOrders
                .Where(o => o.Status.Contains("Поставщик") || o.Status.Contains("материал"))
                .ToList();

            if (lstOrders.SelectedIndex >= relevantOrders.Count) return;

            selectedOrder = relevantOrders[lstOrders.SelectedIndex];

            txtDetails.Text = $"Модель: {selectedOrder.Name}\r\n" +
                              $"Материал: {selectedOrder.Category}\r\n" +
                              $"Размеры: {selectedOrder.Requirements}\r\n" +
                              $"Срок: {selectedOrder.Deadline:dd.MM.yyyy}\r\n" +
                              $"Статус: {selectedOrder.Status}";

            txtMaterials.Text = "Информация о материалах: ожидается от менеджера/уточняется.";

            LoadMessages(); // ← важно, чтобы сообщения тоже обновлялись
        }

        private void LoadMessages()
        {
            lstMessages.Items.Clear();
            if (selectedOrder == null) return;

            string orderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin;
            var messages = MessageService.LoadMessages(orderId)
                .Where(m => m.FromRole == "Поставщик" || m.FromRole == "Менеджер").ToList();

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
                FromRole = "Поставщик",
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
            LoadOrders();
            MessageBox.Show("Статус обновлён: " + newStatus);
        }
    }
}
