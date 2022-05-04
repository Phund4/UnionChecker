using System;
using System.Net.Sockets;
using System.Text;

namespace CSConnection.Client
{
    /// <summary>
    /// Приложение-клиент для игры в шашки в локальной сети
    /// </summary>
    internal static class Program
    {
        private static TcpClient _client; // объект для подключения к серверу

        private static NetworkStream _stream; // объект для обмена данными с сервером

        private static void Main()
        {
            try
            {
                _client = new TcpClient("192.168.0.112", 9050); //TODO Ввод ip, полученного от сервера, вручную 

                _stream = _client.GetStream();

                var playerData = "Player_Client, 111"; //TODO активное получение данных о самом себе для пересылки и отображения
                var enemyData = DataChanging(playerData); //TODO отобразить данные на игровом поле

                while (true)
                {
                    var receivedMessage = ReceiveMessage();
                    if (receivedMessage == "stop") break;

                    var message = Console.ReadLine();
                    SendMessage(message);
                    if (message == "stop") break;
                }

                _client.Close(); // разрыв подключения
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Приём сообщения
        /// </summary>
        /// <returns>Возвращает строку с координатами необходимых перемещений на поле клиента</returns>
        private static string ReceiveMessage()
        {
            var bytes = new byte[20];
            _stream.Read(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="message"> Координаты для сервера, для изменений на его поле</param>

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