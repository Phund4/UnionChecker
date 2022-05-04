using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UnionCheckers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public ObservableCollection<Users> Users { get; set; }
        public static DB db = new DB();
        public MainWindow()
        {
            /*var db = new DB();*/
            InitializeComponent();
            Users = new ObservableCollection<Users>(db.Users);
        }

        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = passBox.Password.Trim();
            string cnfPwd = passBox_2.Password.Trim();
            int rating = 0;

            if (login.Length < 5)
            {
                MessageBox.Show("Логин должен быть не менее 5 символов!");

            }
            else if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов!");
            }
            else if (cnfPwd != password)
            {
                MessageBox.Show("Пароли не совпадают!");
            }
            else
            {
                MessageBox.Show("Вы успешно зарегистрировались!");
                Users user = new Users(login, password, rating);
                db.Users.Add(user);
                db.SaveChanges();

                Login authWindow = new Login();
                authWindow.Show();
                Close();
            }
        }

        private void Button_Window_Auth_Click(object sender, RoutedEventArgs e)
        {
            Login authWindow = new Login();
            authWindow.Show();
            Close();
        }
    }


}

        

