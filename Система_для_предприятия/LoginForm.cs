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
    public partial class LoginForm: Form
    {
        TextBox txtLogin;
        TextBox txtPassword;
        ComboBox cmbRole;

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "Вход в систему";

            Label lblLogin = new Label { Text = "Логин", Top = 20, Left = 20 };
            txtLogin = new TextBox { Name = "txtLogin", Top = 40, Left = 20, Width = 200 };

            Label lblPassword = new Label { Text = "Пароль", Top = 70, Left = 20 };
            txtPassword = new TextBox { Name = "txtPassword", Top = 90, Left = 20, Width = 200, UseSystemPasswordChar = true };

            Label lblRole = new Label { Text = "Роль", Top = 120, Left = 20 };
            cmbRole = new ComboBox { Name = "cmbRole", Top = 140, Left = 20, Width = 200 };
            cmbRole.Items.AddRange(new[] { "Заказчик", "Менеджер", "Разработчик", "Поставщик", "Производитель", "Тестировщик" });

            Button btnLogin = new Button { Text = "Войти", Top = 180, Left = 20 };
            btnLogin.Click += BtnLogin_Click;

            Button btnRegister = new Button { Text = "Регистрация", Top = 180, Left = 120 };
            btnRegister.Click += (s, e) => new RegisterForm().Show();

            this.Controls.AddRange(new Control[] { lblLogin, txtLogin, lblPassword, txtPassword, lblRole, cmbRole, btnLogin, btnRegister });


        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = cmbRole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            var user = UserService.LoadUsers()
                .FirstOrDefault(u => u.Login == login && u.Password == password && u.Role == role);

            if (user == null)
            {
                MessageBox.Show("Неверный логин, пароль или роль.");
                return;
            }

            OpenRoleForm(user);
        }
        private void OpenRoleForm(User user)
        {
            Form form = null;

            if (user.Role == "Заказчик")
                form = new ClientForm(user.Login);
            else if (user.Role == "Менеджер")
                form = new ManagerForm();
            else if (user.Role == "Разработчик")
                form = new DeveloperForm();
            else if (user.Role == "Производитель")
                form = new ManufacturerForm(); // если у тебя есть такая форма
            else if (user.Role == "Тестировщик")
                form = new TesterForm(); // если есть
            else if (user.Role == "Поставщик")
                form = new SupplierForm(); // если есть

            if (form != null)
            {
                form.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Форма для указанной роли ещё не реализована.");
            }
        }
    }
}
