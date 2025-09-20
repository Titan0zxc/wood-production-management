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
    public partial class ClientForm: Form
    {
        private string _clientLogin;
        private ListBox lstOrders, lstMessages;
        private TextBox txtNewMessage;
        private Button btnCreateOrder, btnSendMessage, btnPayPrepay, btnReject, btnPayPenalty, btnConfirmReceive, btnLogout;

        private List<Order> myOrders;
        private Order selectedOrder;

        public ClientForm(string login)
        {
            InitializeComponent();
            _clientLogin = login;
            this.Text = $"Кабинет заказчика: {_clientLogin}";
            this.Width = 1000;
            this.Height = 750;

            lstOrders = new ListBox { Top = 20, Left = 20, Width = 950, Height = 200 };
            lstOrders.SelectedIndexChanged += LstOrders_SelectedIndexChanged;

            btnCreateOrder = new Button { Text = "Оформить заказ", Top = 230, Left = 20, Width = 150 };
            btnCreateOrder.Click += BtnCreateOrder_Click;

            lstMessages = new ListBox { Top = 270, Left = 20, Width = 950, Height = 200 };

            txtNewMessage = new TextBox { Top = 480, Left = 20, Width = 700, Height = 60, Multiline = true };

            btnSendMessage = new Button { Text = "Ответить", Top = 550, Left = 20, Width = 150 };
            btnSendMessage.Click += BtnSendMessage_Click;

            btnPayPrepay = new Button { Text = "Оплатить предоплату", Top = 550, Left = 200, Width = 180 };
            btnPayPrepay.Click += BtnPayPrepay_Click;

            btnReject = new Button { Text = "Отказаться от проекта", Top = 550, Left = 400, Width = 180 };
            btnReject.Click += BtnReject_Click;

            btnPayPenalty = new Button { Text = "Оплатить неустойку", Top = 600, Left = 200, Width = 180 };
            btnPayPenalty.Click += BtnPayPenalty_Click;

            btnConfirmReceive = new Button { Text = "Получил заказ", Top = 600, Left = 400, Width = 180 };
            btnConfirmReceive.Click += BtnConfirmReceive_Click;

            btnLogout = new Button { Text = "На главную", Top = 650, Left = 400, Width = 180 };
            btnLogout.Click += BtnLogout_Click;

            this.Controls.AddRange(new Control[] {
                lstOrders, btnCreateOrder, lstMessages, txtNewMessage,
                btnSendMessage, btnPayPrepay, btnReject, btnPayPenalty,
                btnConfirmReceive, btnLogout
            });

            LoadOrders();
        }
        private void BtnCreateOrder_Click(object sender, EventArgs e)
        {
            new CreateOrderForm(_clientLogin).ShowDialog();
            LoadOrders();
        }

        private void LstOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOrders.SelectedIndex == -1) return;

            selectedOrder = myOrders[lstOrders.SelectedIndex];
            LoadMessages();
        }

        private void LoadOrders()
        {
            lstOrders.Items.Clear();
            myOrders = OrderService.LoadOrders().Where(o => o.ClientLogin == _clientLogin).ToList();

            foreach (var order in myOrders)
            {
                string display = $"Изделие: {order.Name}, Сумма: {order.Sum} руб, " +
                                 $"Дата: {order.Deadline:dd.MM.yyyy}, " +
                                 $"Размеры: {order.Requirements}, Материал: {order.Category}, " +
                                 $"Статус: {order.Status}";
                lstOrders.Items.Add(display);
            }
        }

        private void LoadMessages()
        {
            lstMessages.Items.Clear();
            if (selectedOrder == null) return;

            string orderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin;
            var messages = MessageService.LoadMessages(orderId);

            foreach (var msg in messages)
            {
                lstMessages.Items.Add($"[{msg.Timestamp:dd.MM HH:mm}] {msg.FromRole}: {msg.Text}");
            }
        }

        private void BtnSendMessage_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null || string.IsNullOrWhiteSpace(txtNewMessage.Text)) return;

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Заказчик",
                Text = txtNewMessage.Text.Trim(),
                Timestamp = DateTime.Now
            });

            txtNewMessage.Clear();
            LoadMessages();
        }

        private void BtnPayPrepay_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null || selectedOrder.Status != "Ожидает предоплаты")
            {
                MessageBox.Show("Нет заказа, ожидающего предоплаты.");
                return;
            }

            selectedOrder.Status = "Предоплата внесена";
            OrderService.UpdateOrder(selectedOrder);

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Заказчик",
                Text = "Предоплата внесена.",
                Timestamp = DateTime.Now
            });

            MessageBox.Show("Предоплата принята.");
            LoadOrders();
            LoadMessages();
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null) return;

            selectedOrder.Status = "Проект отклонён";
            OrderService.UpdateOrder(selectedOrder);

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Заказчик",
                Text = "Отказ от проекта.",
                Timestamp = DateTime.Now
            });

            MessageBox.Show("Вы отказались от проекта.");
            LoadOrders();
            LoadMessages();
        }

        private void BtnPayPenalty_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null || selectedOrder.Status != "Ожидание оплаты неустойки")
            {
                MessageBox.Show("Необходимо дождаться запроса на оплату неустойки.");
                return;
            }

            selectedOrder.Status = "Подготовка к отправке";
            OrderService.UpdateOrder(selectedOrder);

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Заказчик",
                Text = "Неустойка оплачена. Готов к получению заказа.",
                Timestamp = DateTime.Now
            });

            MessageBox.Show("Неустойка успешно оплачена.");
            LoadOrders();
            LoadMessages();
        }

        private void BtnConfirmReceive_Click(object sender, EventArgs e)
        {
            if (selectedOrder == null || selectedOrder.Status != "Заказ в пути")
            {
                MessageBox.Show("Заказ пока не отправлен или уже завершён.");
                return;
            }

            selectedOrder.Status = "Заказ закрыт";
            OrderService.UpdateOrder(selectedOrder);

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = selectedOrder.Name + "_" + selectedOrder.ClientLogin,
                FromRole = "Заказчик",
                Text = "Заказ получен. Спасибо за сотрудничество!",
                Timestamp = DateTime.Now
            });

            MessageBox.Show("Заказ успешно закрыт.");
            LoadOrders();
            LoadMessages();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Hide();
            new LoginForm().Show();
            Close();
        }

    }
}
