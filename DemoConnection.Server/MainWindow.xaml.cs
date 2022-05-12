using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace DemoConnection.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Random _rand;
        
        private const int Port = 9050;
        
        private static TcpListener? _serverSocket; // объект для прослушки подключения клиента

        private static NetworkStream? _stream; // объект для обмена данными с клиентом
        
        public MainWindow()
        {
            _rand = new Random();

            ServerStarting();
            
            InitializeComponent();
            
            var clientSocket = _serverSocket?.AcceptTcpClient(); // объект для взаимодействия с клиентом
            _stream = clientSocket?.GetStream(); // запуск сервера
            
            const string serverData = "Server, 1";
            PlayerData.Text = serverData;
            EnemyData.Text = DataChanging(serverData);


            //clientSocket?.Close(); // прекращение связи с клиентом
            //_serverSocket?.Stop();  // остановка  сервера
        }

        private static void ServerStarting()
        {
            _serverSocket = new TcpListener(IPAddress.Any, Port); //TODO активное получение ip для передачи противнику
            _serverSocket.Start(); // запуск сервера
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

        private void StartGame_OnClick(object sender, RoutedEventArgs e)
        {
            SendMessage("start");
        }
    }
}