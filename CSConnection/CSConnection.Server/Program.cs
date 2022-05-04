using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

    
namespace CSConnection.Server
{
    /// <summary>
    /// Приложение-сервер для игры в шашки в локальной сети
    /// </summary>
    internal static class Program
    {
        private static TcpListener _serverSocket; // объект для прослушки подключения клиента

        private static NetworkStream _stream; // объект для обмена данными с клиентом

        private static void Main()
        {
            //несработавшие методы получения ip 
            
            //var ip = Dns.GetHostAddresses(Dns.GetHostName()).First(address => address.AddressFamily == AddressFamily.InterNetwork);
            //var ip = Dns.GetHostEntry(host).AddressList[1].ToString();
            
            // ip необходимо получать внутри программы и через стороннее приложение передавать клиенту для его последующего подключения

            _serverSocket = new TcpListener(IPAddress.Any, 9050); //TODO активное получение ip для передачи противнику
            _serverSocket.Start(); // запуск сервера
            
            var clientSocket = _serverSocket.AcceptTcpClient(); // объект для взаимодействия с клиентом
            _stream = clientSocket.GetStream(); // запуск сервера

            var playerData = "Player_Server, 251"; //TODO активное получение данных о самом себе для пересылки и отображения
            var enemyData = DataChanging(playerData); //TODO отобразить данные на игровом поле
            
            Console.WriteLine(enemyData);
            
            while (true)    // Главный процесс всей игры. Бесконечное прослушивание клиента и отправка ему сообщений
            {
                var message = Console.ReadLine();
                SendMessage(message);
                if (message == "stop") break;
                
                var receivedMessage = ReceiveMessage();
                if (receivedMessage == "stop") break;
            }
            
            clientSocket.Close(); // прекращение связи с клиентом
            _serverSocket.Stop();  // остановка  сервера
        }

        /// <summary>
        /// Приём сообщения
        /// </summary>
        /// <returns>Возвращает строку с координатами необходимых перемещений на поле сервера</returns>
        private static string ReceiveMessage()
        {
            var bytes = new byte[20];
            _stream.Read(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="message"> Координаты для клиента, для изменений на его поле</param>
        private static void SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            _stream.Write(bytes, 0, bytes.Length); // запись байтов в поток
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
    }
}