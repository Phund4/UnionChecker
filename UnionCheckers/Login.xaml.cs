using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UnionCheckers
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Auth_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = passBox.Password.Trim();

            if (login.Length < 5)
            {
                MessageBox.Show("Логин должен быть не менее 5 символов!");

            }
            else if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов!");
            }
            else
            {
                Users authUser = null;
                using (var db = new DB())
                {
                    authUser = db.Users.Where(user => user.Login == login && user.Password == password).FirstOrDefault();
                }

                if (authUser != null)
                {
                    MessageBox.Show("Вы успешно авторизовались!");
                    UserPageWindow userPageWindow = new UserPageWindow();
                    userPageWindow.Show();
                    Close();
                }
                else
                    MessageBox.Show("Одно из полей заполнено некорректно!");
            }
        }
        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}