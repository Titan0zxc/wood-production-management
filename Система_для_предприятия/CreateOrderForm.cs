using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Система_для_предприятия.Models;
using Система_для_предприятия.Services;
using System.IO;

namespace Система_для_предприятия
{
    public partial class CreateOrderForm: Form
    {
        private string _clientLogin;

        private ComboBox cmbItemType;
        private ComboBox cmbModel;
        private TextBox txtMessage;
        private DateTimePicker dtpDelivery;
        private Button btnSubmit;

        private List<CatalogItem> catalog;
        public CreateOrderForm(string login)
        {
            InitializeComponent();
            _clientLogin = login;
            this.Text = "Оформление заказа";
            this.Width = 600;
            this.Height = 400;

            Label lblType = new Label { Text = "Тип изделия:", Top = 20, Left = 20 };
            cmbItemType = new ComboBox { Top = 40, Left = 20, Width = 250 };
            cmbItemType.SelectedIndexChanged += CmbItemType_SelectedIndexChanged;

            Label lblModel = new Label { Text = "Модель:", Top = 80, Left = 20 };
            cmbModel = new ComboBox { Top = 100, Left = 20, Width = 500 };

            Label lblDate = new Label { Text = "Желаемая дата доставки:", Top = 140, Left = 20 };
            dtpDelivery = new DateTimePicker { Top = 160, Left = 20, Width = 200 };

            Label lblMessage = new Label { Text = "Пожелания:", Top = 200, Left = 20 };
            txtMessage = new TextBox { Top = 220, Left = 20, Width = 500, Height = 60, Multiline = true };

            btnSubmit = new Button { Text = "Оформить заказ", Top = 300, Left = 20 };
            btnSubmit.Click += BtnSubmit_Click;

            this.Controls.AddRange(new Control[]
            {
                lblType, cmbItemType, lblModel, cmbModel,
                lblDate, dtpDelivery, lblMessage, txtMessage, btnSubmit
            });

            LoadCatalog();
        }

        private void LoadCatalog()
        {
            string path = Path.Combine(Application.StartupPath, "Data", "catalog.txt");
            if (!File.Exists(path))
            {
                MessageBox.Show("Файл каталога не найден.");
                return;
            }

            catalog = File.ReadAllLines(path)
                .Select(CatalogItem.FromString)
                .ToList();

            var types = catalog.Select(c => c.Type).Distinct().ToArray();
            cmbItemType.Items.AddRange(types);
        }
        private void CmbItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbModel.Items.Clear();
            string selectedType = cmbItemType.SelectedItem.ToString();
            var models = catalog.Where(c => c.Type == selectedType).ToList();

            foreach (var model in models)
                cmbModel.Items.Add(model);
        }
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (cmbModel.SelectedItem == null)
            {
                MessageBox.Show("Выберите модель изделия.");
                return;
            }

            var selectedModel = (CatalogItem)cmbModel.SelectedItem;

            Order newOrder = new Order
            {
                Name = selectedModel.Type,
                Category = selectedModel.Material,
                Requirements = selectedModel.Dimensions,
                Sum = selectedModel.Price,
                Deadline = dtpDelivery.Value,
                ClientLogin = _clientLogin,
                Status = "В процессе оформления" 
            };

            OrderService.SaveOrder(newOrder);

            string orderId = newOrder.Name + "_" + _clientLogin;

            MessageService.SaveMessage(new Система_для_предприятия.Models.Message
            {
                OrderId = orderId,
                FromRole = "Заказчик",
                Timestamp = DateTime.Now,
                Text = $"Модель: {selectedModel.ModelName}, Размеры: {selectedModel.Dimensions}, Материал: {selectedModel.Material}. Пожелание: {txtMessage.Text}"
            });

            MessageBox.Show("Заказ оформлен!");
            this.Close();
        }

    }
}

