using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для UserPageWindow.xaml
    /// </summary>
    public partial class UserPageWindow : Window
    {
        //public static string nickServer;
        //public static int ratingServer;
        //public static string nickClient;
        //public static int ratingClient;
        public UserPageWindow()
        {
            var db = new DB();
            
            InitializeComponent();
            
            List<Users> users = db.Users.ToList();

            listOfUsers.ItemsSource = users;
        }

        private void Button_New_Game_Click(object sender, RoutedEventArgs e)
        {
            //nickServer = Login.authUser.Login;
            //ratingServer = (int)Login.authUser.Rating;
            //GameWindow gameWindow = new GameWindow();
            //gameWindow.Show();
            //Close();
            IPAddressWindow window = new IPAddressWindow();
            window.Show();
            Close();
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            //nickClient = Login.authUser.Login;
            //ratingClient = (int)Login.authUser.Rating;
            ClientConnection window = new ClientConnection();
            window.Show();
            Close();
        }

        private void Button_Connect_Back(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            Close();
        }
    }
}
