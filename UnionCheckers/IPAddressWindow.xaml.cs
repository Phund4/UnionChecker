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
    /// Логика взаимодействия для IPAddressWindow.xaml
    /// </summary>
    public partial class IPAddressWindow : Window
    {
        public IPAddressWindow()
        {
            InitializeComponent();
            //foreach (var el in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            //{
            //    TextIP.Text += el.ToString();
            //}
            TextIP.Text += Dns.GetHostEntry(Dns.GetHostName()).AddressList[6].ToString();
        }

        private void Ready_Click(object sender, RoutedEventArgs e)
        {
            GameWindow window = new GameWindow();
            window.Show();
            Close();
        }
    }
}
