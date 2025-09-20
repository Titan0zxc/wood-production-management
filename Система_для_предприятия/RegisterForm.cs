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
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            this.Text = "Регистрация";
            comboBox1.Items.AddRange(new[] { "Заказчик", "Менеджер", "Разработчик", "Тестировщик", "Поставщик", "Производитель" });

            //Label lblLogin = new Label { Text = "Логин", Top = 20, Left = 20 };
            //TextBox textBox1 = new TextBox { Top = 40, Left = 20, Width = 200 };

            //Label lblPassword = new Label { Text = "Пароль", Top = 70, Left = 20 };
            //TextBox textBox2 = new TextBox { Top = 90, Left = 20, Width = 200, UseSystemPasswordChar = true };

            //Label lblRole = new Label { Text = "Роль", Top = 120, Left = 20 };
            //ComboBox comboBox1 = new ComboBox { Top = 140, Left = 20, Width = 200 };
            //comboBox1.Items.AddRange(new[] { "Заказчик", "Менеджер", "Разработчик", "Тестировщик", "Поставщик", "Производитель" });

            //Button btnRegister = new Button { Text = "Создать", Top = 180, Left = 20 };
            //btnRegister.Click += (s, e) => MessageBox.Show("Заглушка регистрации");

            //this.Controls.AddRange(new Control[] { textBox1, lblLogin, lblPassword, textBox2, lblRole, comboBox1, btnRegister });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string password = textBox2.Text;
            string role = comboBox1.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            if (UserService.IsLoginTaken(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует.");
                return;
            }

            var newUser = new User
            {
                Login = login,
                Password = password,
                Role = role
            };

            UserService.SaveUser(newUser);
            MessageBox.Show("Регистрация успешна!");

            OpenRoleForm(role, login);

        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрыть текущую форму
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Close(); // Закрыть текущую форму, чтобы не висела в памяти
        }
        private void OpenRoleForm(string userRole, string login)
        {
            Form form = null;

            if (userRole == "Заказчик")
                form = new ClientForm(login);
            else if (userRole == "Менеджер")
                form = new ManagerForm();
            else if (userRole == "Разработчик")
                form = new DeveloperForm();
            else if (userRole == "Тестировщик")
                form = new TesterForm(); // если есть
            else if (userRole == "Производитель")
                form = new ManufacturerForm(); // если есть
            else if (userRole == "Поставщик")
                form = new SupplierForm(); // если есть

            if (form != null)
            {
                form.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Форма для указанной роли ещё не реализована.\nВход доступен с главного окна.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрыть текущую форму
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Close(); // Закрыть текущую форму, чтобы не висела в памяти
        }
    }
}
