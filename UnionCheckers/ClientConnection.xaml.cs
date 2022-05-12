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
    /// Логика взаимодействия для ClientConnection.xaml
    /// </summary>
    public partial class ClientConnection : Window
    {
        public static string ip; 
        public ClientConnection()
        {
            InitializeComponent();
        }

        private void SendIP_Click(object sender, RoutedEventArgs e)
        {
            ip = TextBoxIP.Text;
        }
    }
}
