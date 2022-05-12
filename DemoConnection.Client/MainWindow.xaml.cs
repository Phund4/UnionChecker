using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DemoConnection.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Random _rand;
        
        private static TcpClient _client; // объект для подключения к серверу
        
        private const int Port = 9050; // порт подkлючения

        private static NetworkStream? _stream; // объект для обмена данными с сервером
        
        public MainWindow()
        {
            _rand = new Random();

            StartClient();
            
            InitializeComponent();
            
            const string clientData = "Client, 1";
            PlayerData.Text = clientData;
            EnemyData.Text = DataChanging(clientData);
            
            ReceivedMsg.Text = ReceiveMessage();
        }

        private void StartClient()
        {
            _client = new TcpClient("192.168.0.112", Port); //TODO Ввод ip, полученного от сервера, вручную 
            _stream = _client.GetStream();
        }
        
        /// <summary>
        /// Приём сообщения
        /// </summary>
        /// <returns>Возвращает строку с координатами необходимых перемещений на поле сервера</returns>
        private static string ReceiveMessage()
        {
            var bytes = new byte[20];
            _stream!.Read(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="message"> Координаты для клиента, для изменений на его поле</param>
        private static void SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            _stream!.Write(bytes, 0, bytes.Length); // запись байтов в поток
            _stream.Flush();
        }

        /// <summary>
        /// Обмен данными об игроках
        /// </summary>
        /// <param name="playerData"> Сведенья об игроке: никнэйм и рейтинг игрока</param>
        /// <returns> Данные о противнике: никнэйм и рейтинг противника</returns>
        private static string DataChanging(string playerData)
        {
            SendMessage(playerData);

            return ReceiveMessage();
        }

        /// <summary>
        /// Обработка нажатия на клавишу отправки сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMsg_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = _rand.Next(1000, 10000).ToString();
            SendMessage(msg);
            var RMsg = ReceiveMessage();
            ReceivedMsg.Text = RMsg;
        }
        
    }
}