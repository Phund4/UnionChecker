using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для GameWindowClient.xaml
    /// </summary>
    public partial class GameWindowClient : Window
    {
        private Button[] arrGreenButtons;
        private Button ButtonReserve;
        private int index;
        private int CountMoveWhite;
        private int CountMoveBlack;
        public SoundPlayer sp;
        private static TcpClient _client; // объект для подключения к серверу
        private const int Port = 9050; // порт подkлючения
        private static NetworkStream _stream; // объект для обмена данными с сервером

        public GameWindowClient()
        {
            StartClient();
            InitializeComponent();
            index = 0;
            CountMoveWhite = 1;
            CountMoveBlack = 0;
            InitGreenButtons();
            BlockDraughts("Black", ClickOnBlack);
            sp = new SoundPlayer();
            sp.SoundLocation = "step.wav";
            sp.Load();

            string clientData = $"{Login.authUser.Login}, {Login.authUser.Rating}";
            ClientBox.Text = clientData;
            ServerBox.Text = DataChanging(clientData);
        }

        #region Connection

        private void StartClient()
        {
            _client = new TcpClient(ClientConnection.ip, Port); //TODO Ввод ip, полученного от сервера, вручную 
            _stream = _client.GetStream();
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

        #endregion Connection

        #region GameModul

        #region Additional

        private bool IsWin(string firstname, string secondname)
        {
            foreach (Button element in Shashki.Children)
                if (element.Content.ToString() == firstname || element.Content.ToString() == secondname)
                    return false;
            return true;
        }

        private void IsWin()
        {
            if (IsWin("White", "WhiteQueen"))
            {
                Login.authUser.Rating++;
                MessageBox.Show("Черные победили!");
                Close();
                var window = new UserPageWindow();
                window.Show();
            }
            else if (IsWin("Black", "BlackQueen"))
            {
                Login.authUser.Rating--;
                MessageBox.Show("Белые победили!");
                Close();
                var window = new UserPageWindow();
                window.Show();
            }
        }

        private async Task SoundAsync()
        {
            await Task.Run(() =>
            {
                sp.PlayLooping();
                Thread.Sleep(400);
                sp.Stop();
            });
        }

        private void InitGreenButtons()
        {
            arrGreenButtons = new Button[30];

            Style style = new Style();
            style.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush(Colors.LightGreen) });
            style.Setters.Add(new Setter { Property = OpacityProperty, Value = 0.5 });
            style.BasedOn = (Style)FindResource("ForDraught");
            style.Setters.Add(new EventSetter { Event = Button.ClickEvent, Handler = new RoutedEventHandler(ClickOnGreen) });
            style.TargetType = typeof(Button);

            for (int i = 0; i < 30; i++)
            {
                arrGreenButtons[i] = new Button();
                arrGreenButtons[i].Style = style;
            }
        } // Инициализировать зеленые кнопки

        private UIElement GetElementFromGrid(int x, int y)
        {
            foreach (UIElement element in Shashki.Children)
                if (x == Grid.GetRow(element) && y == Grid.GetColumn(element))
                    return element;
            return null;
        } // Получить элемент из Grid

        private void AddNewButtonToGrid(int x, int y, Button button)
        {
            Grid.SetRow(button, x);
            Grid.SetColumn(button, y);
            Shashki.Children.Add(button);
        } // Добавить элемент в Grid

        private void Changes(string coordinates)
        {
            int c = int.Parse(coordinates);
            int xCut = c / 1000;
            int yCut = c / 100 % 10;
            int x = c / 10 % 10;
            int y = c % 10;
            CutDraught(x, y, xCut, yCut);
        }

        private void DraughtMove(int x, int y)
        {
            if (GetElementFromGrid(x, y) == null)
            {
                AddNewButtonToGrid(x, y, arrGreenButtons[index]);
                index++;
            }
        } // Добавить зеленую кнопку на поле

        private void BlockDraughts(string content, RoutedEventHandler click)
        {
            foreach (Button button in Shashki.Children)
                if (button.Content.ToString() == content)
                    button.Click -= click;
        } // Заблокировать все шашки по цвету

        private void GetActiveDraughts(string content, RoutedEventHandler click)
        {
            foreach (Button button in Shashki.Children)
                if (button.Content.ToString() == content)
                    button.Click += click;
        } // Разблокировать все шашки по цвету

        private void CutDraught(int x, int y, int xCut, int yCut)
        {
            if ((x - xCut) >= 2 && (y - yCut) <= -2)
            {
                xCut++;
                yCut--;
                while (x != xCut && y != yCut)
                {
                    Shashki.Children.Remove(GetElementFromGrid(xCut, yCut));
                    xCut++;
                    yCut--;
                }
            }
            else if ((x - xCut) >= 2 && (y - yCut) >= 2)
            {
                xCut++;
                yCut++;
                while (x != xCut && y != yCut)
                {
                    Shashki.Children.Remove(GetElementFromGrid(xCut, yCut));
                    xCut++;
                    yCut++;
                }
            }
            else if ((x - xCut) <= -2 && (y - yCut) <= -2)
            {
                xCut--;
                yCut--;
                while (x != xCut && y != yCut)
                {
                    Shashki.Children.Remove(GetElementFromGrid(xCut, yCut));
                    xCut--;
                    yCut--;
                }
            }
            else if ((x - xCut) <= -2 && (y - yCut) >= 2)
            {
                xCut--;
                yCut++;
                while (x != xCut && y != yCut)
                {
                    Shashki.Children.Remove(GetElementFromGrid(xCut, yCut));
                    xCut--;
                    yCut++;
                }
            }
            if (ButtonReserve.Content.ToString() == "WhiteQueen")
                ButtonReserve.Click -= ClickWhiteQueenToCut;
            else if (ButtonReserve.Content.ToString() == "BlackQueen")
                ButtonReserve.Click -= ClickBlackQueenToCut;
            else if (ButtonReserve.Content.ToString() == "Black")
                ButtonReserve.Click -= ClickBlackToCut;
            else if (ButtonReserve.Content.ToString() == "White")
                ButtonReserve.Click -= ClickWhiteToCut;
        } // Срубить шашку

        private void ClearAllGreens()
        {
            for (int i = 0; i < index; i++)
                Shashki.Children.Remove(arrGreenButtons[i]);
            index = 0;
        } // Убрать все зеленые кнопки с поля

        #endregion Additional

        #region Conditions

        private bool RTLD(int x, int y, string content)
        {
            if (x < 6 && y > 1)
                if (GetElementFromGrid(x + 1, y - 1) != null)
                    if ((GetElementFromGrid(x + 1, y - 1) as Button).Content.ToString() ==
                        content && GetElementFromGrid(x + 2, y - 2) == null)
                        return true;
            return false;
        } // RightToLeftDown

        private bool LTRD(int x, int y, string content)
        {
            if (x < 6 && y < 6)
                if (GetElementFromGrid(x + 1, y + 1) != null)
                    if ((GetElementFromGrid(x + 1, y + 1) as Button).Content.ToString() ==
                        content && GetElementFromGrid(x + 2, y + 2) == null)
                        return true;
            return false;
        } // LeftToRightDown-

        private bool LTRU(int x, int y, string content)
        {
            if (x > 1 && y < 6)
                if (GetElementFromGrid(x - 1, y + 1) != null)
                    if ((GetElementFromGrid(x - 1, y + 1) as Button).Content.ToString() ==
                        content && GetElementFromGrid(x - 2, y + 2) == null)
                        return true;
            return false;
        } // LeftToRightDown

        private bool RTLU(int x, int y, string content)
        {
            if (x > 1 && y > 1)
                if (GetElementFromGrid(x - 1, y - 1) != null)
                    if ((GetElementFromGrid(x - 1, y - 1) as Button).Content.ToString() ==
                        content && GetElementFromGrid(x - 2, y - 2) == null)
                        return true;
            return false;
        } // RightToLeftUp

        private bool CheckDraughts(string first, string second, string third, RoutedEventHandler click)
        {
            try
            {
                bool b = false;
                foreach (Button button in Shashki.Children)
                {
                    if (button.Content.ToString() == first)
                    {
                        int x = Grid.GetRow(button);
                        int y = Grid.GetColumn(button);
                        if (RTLD(x, y, second) || LTRD(x, y, second) || LTRU(x, y, second) || RTLU(x, y, second)
                            || RTLD(x, y, third) || LTRD(x, y, third) || LTRU(x, y, third) || RTLU(x, y, third))
                        {
                            b = true;
                            button.Click += click;
                        }
                    }
                }
                return b;
            }
            catch (Exception) { }
            return false;
        } // Проанализировать все обычные шашки. Возвращает true, если шашка может рубить, иначе false

        private bool CheckQueens(string first, string second, string third, RoutedEventHandler click)
        {
            try
            {
                bool b = false;
                foreach (Button button in Shashki.Children)
                {
                    if (button.Content.ToString() == first)
                    {
                        int x = Grid.GetRow(button);
                        int y = Grid.GetColumn(button);
                        int i = 0;
                        bool isCut = false;
                        while (x - i >= 0 && y - i >= 0)
                        {
                            if (GetElementFromGrid(x - i - 1, y - i - 1) != null)
                            {
                                if (RTLU(x - i, y - i, second) || RTLU(x - i, y - i, third))
                                {
                                    b = true;
                                    isCut = true;
                                    button.Click += click;
                                }
                                break;
                            }
                            i++;
                        }
                        i = 0;
                        if (!isCut)
                        {
                            while (x - i >= 0 && y + i <= 7)
                            {
                                if (GetElementFromGrid(x - i - 1, y + i + 1) != null)
                                {
                                    if (LTRU(x - i, y + i, second) || LTRU(x - i, y + i, third))
                                    {
                                        b = true;
                                        isCut = true;
                                        button.Click += click;
                                    }
                                    break;
                                }
                                i++;
                            }
                        }
                        else
                            continue;
                        i = 0;
                        if (!isCut)
                        {
                            while (x + i <= 7 && y - i >= 0)
                            {
                                if (GetElementFromGrid(x + i + 1, y - i - 1) != null)
                                {
                                    if (RTLD(x + i, y - i, second) || RTLD(x + i, y - i, third))
                                    {
                                        b = true;
                                        isCut = true;
                                        button.Click += click;
                                    }
                                    break;
                                }
                                i++;
                            }
                        }
                        else
                            continue;
                        i = 0;
                        if (!isCut)
                        {
                            while (x + i <= 7 && y + i <= 7)
                            {
                                if (GetElementFromGrid(x + i + 1, y + i + 1) != null)
                                {
                                    if (LTRD(x + i, y + i, second) || LTRD(x + i, y + i, third))
                                    {
                                        b = true;
                                        isCut = true;
                                        button.Click += click;
                                    }
                                    break;
                                }
                                i++;
                            }
                        }
                        else
                            continue;
                    }
                }
                return b;
            }
            catch (Exception) { }
            return false;
        } // Проанализировать всех королев. Возвращает true, если королева может рубить, иначе false

        private void ConditionToCut(int x, int y, string content)
        {
            if (RTLD(x, y, content))
                DraughtMove(x + 2, y - 2);
            if (LTRD(x, y, content))
                DraughtMove(x + 2, y + 2);
            if (LTRU(x, y, content))
                DraughtMove(x - 2, y + 2);
            if (RTLU(x, y, content))
                DraughtMove(x - 2, y - 2);
        } // Подготовка к рубке для обычных шашек

        private void ConditionToCutQueen(int x, int y, string first, string second)
        {
            int i = 0;

            while (x - i >= 0 && y - i >= 0)
            {
                bool isCut = false;
                if (RTLU(x - i, y - i, first) || RTLU(x - i, y - i, second))
                {
                    isCut = true;
                    i += 2;
                    while (x - i >= 0 && y - i >= 0 && GetElementFromGrid(x - i, y - i) == null)
                    {
                        DraughtMove(x - i, y - i);
                        i++;
                    }
                }
                if (isCut)
                    break;
                i++;
            }
            i = 0;

            while (x - i >= 0 && y + i <= 7)
            {
                bool isCut = false;
                if (LTRU(x - i, y + i, first) || LTRU(x - i, y + i, second))
                {
                    isCut = true;
                    i += 2;
                    while (x - i >= 0 && y + i <= 7 && GetElementFromGrid(x - i, y + i) == null)
                    {
                        DraughtMove(x - i, y + i);
                        i++;
                    }
                }
                if (isCut)
                    break;
                i++;
            }
            i = 0;

            while (x + i <= 7 && y - i >= 0)
            {
                bool isCut = false;
                if (RTLD(x + i, y - i, first) || RTLD(x + i, y - i, second))
                {
                    isCut = true;
                    i += 2;
                    while (x + i <= 7 && y - i >= 0 && GetElementFromGrid(x + i, y - i) == null)
                    {
                        DraughtMove(x + i, y - i);
                        i++;
                    }
                }
                if (isCut)
                    break;
                i++;
            }
            i = 0;

            while (x + i <= 7 && y + i <= 7)
            {
                bool isCut = false;
                if (LTRD(x + i, y + i, first) || LTRD(x + i, y + i, second))
                {
                    isCut = true;
                    i += 2;
                    while (x + i <= 7 && y + i <= 7 && GetElementFromGrid(x + i, y + i) == null)
                    {
                        DraughtMove(x + i, y + i);
                        i++;
                    }
                }
                if (isCut)
                    break;
                i++;
            }
        } // Подготовка к рубке для королев

        private void ConditionToMoveQueen(int x, int y)
        {
            int i = 1;
            while (x - i >= 0 && y + i <= 7 && GetElementFromGrid(x - i, y + i) == null)
            {
                DraughtMove(x - i, y + i);
                i++;
            }
            i = 1;
            while (x - i >= 0 && y - i >= 0 && GetElementFromGrid(x - i, y - i) == null)
            {
                DraughtMove(x - i, y - i);
                i++;
            }
            i = 1;
            while (x + i <= 7 && y + i <= 7 && GetElementFromGrid(x + i, y + i) == null)
            {
                DraughtMove(x + i, y + i);
                i++;
            }
            i = 1;
            while (x + i <= 7 && y - i >= 0 && GetElementFromGrid(x + i, y - i) == null)
            {
                DraughtMove(x + i, y - i);
                i++;
            }
        } // Подготовка к ходу для королев

        #endregion Conditions

        #region Clicks

        private async void ClickOnGreen(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Style style = new Style();
            style.TargetType = typeof(Button);
            int x = Grid.GetRow(button);
            int y = Grid.GetColumn(button);
            int xCut = Grid.GetRow(ButtonReserve);
            int yCut = Grid.GetColumn(ButtonReserve);
            string coordinates = $"{xCut}{yCut}{x}{y}";

            ClearAllGreens();
            Shashki.Children.Remove(ButtonReserve);
            AddNewButtonToGrid(x, y, ButtonReserve);

            if (ButtonReserve.Content.ToString() == "White" || ButtonReserve.Content.ToString() == "WhiteQueen")
            {
                if (CountMoveWhite == 2)
                {
                    CutDraught(x, y, xCut, yCut);
                    if (CheckQueens("WhiteQueen", "Black", "BlackQueen", ClickWhiteQueenToCut)) { }
                }
                if (CountMoveWhite == 1 || (!CheckDraughts("White", "Black", "BlackQueen", ClickWhiteToCut) && !CheckQueens("WhiteQueen", "Black", "BlackQueen", ClickWhiteQueenToCut)))
                {
                    if (ButtonReserve.Content.ToString() == "White")
                    {
                        ButtonReserve.Click -= ClickWhiteToCut;
                        ButtonReserve.Click -= ClickWhiteToCut;
                    }
                    if (!CheckDraughts("Black", "White", "WhiteQueen", ClickBlackToCut) && !CheckQueens("BlackQueen", "White", "WhiteQueen", ClickBlackQueenToCut))
                    {
                        CountMoveBlack = 1;
                        GetActiveDraughts("Black", ClickOnBlack);
                        GetActiveDraughts("BlackQueen", ClickOnQueen);
                    }
                    else if (CheckQueens("BlackQueen", "White", "WhiteQueen", ClickBlackQueenToCut) || CheckDraughts("Black", "White", "WhiteQueen", ClickBlackToCut))
                    {
                        CountMoveBlack = 2;
                    }
                    CountMoveWhite = 0;
                    BlockDraughts("White", ClickOnWhite);
                    BlockDraughts("WhiteQueen", ClickOnQueen);
                }
            }

            else if (ButtonReserve.Content.ToString() == "Black" || ButtonReserve.Content.ToString() ==  "BlackQueen")
            {
                if (CountMoveBlack == 2)
                {
                    CutDraught(x, y, xCut, yCut);
                    if (CheckQueens("BlackQueen", "White", "WhiteQueen", ClickBlackQueenToCut)) { }
                }
                if (CountMoveBlack == 1 || (!CheckDraughts("Black", "White", "WhiteQueen", ClickBlackToCut) && !CheckQueens("BlackQueen", "White", "WhiteQueen", ClickBlackQueenToCut)))
                {
                    if (ButtonReserve.Content.ToString() == "Black")
                    {
                        ButtonReserve.Click -= ClickBlackToCut;
                        ButtonReserve.Click -= ClickBlackToCut;
                    }
                    if (!CheckDraughts("White", "Black", "BlackQueen", ClickWhiteToCut) && !CheckQueens("WhiteQueen", "Black", "BlackQueen", ClickWhiteQueenToCut))
                    {
                        CountMoveWhite = 1;
                        GetActiveDraughts("White", ClickOnWhite);
                        GetActiveDraughts("WhiteQueen", ClickOnQueen);
                    }
                    else if (CheckQueens("WhiteQueen", "Black", "BlackQueen", ClickWhiteQueenToCut) || CheckDraughts("White", "Black", "BlackQueen", ClickWhiteToCut))
                    {
                        CountMoveWhite = 2;
                    }

                    CountMoveBlack = 0;
                    BlockDraughts("Black", ClickOnBlack);
                    BlockDraughts("BlackQueen", ClickOnQueen);
                }
            }

            if (ButtonReserve.Content.ToString() == "White" && x == 7)
            {
                style.BasedOn = (Style)FindResource("ForWhiteQueen");
                ButtonReserve.Style = style;
                ButtonReserve.Click -= ClickOnWhite;
                ButtonReserve.Click -= ClickWhiteToCut;
                if (CheckQueens("WhiteQueen", "Black", "BlackQueen", ClickWhiteQueenToCut))
                {

                }
            }

            else if (ButtonReserve.Content.ToString() == "Black" && x == 0)
            {
                style.BasedOn = (Style)FindResource("ForBlackQueen");
                ButtonReserve.Style = style;
                ButtonReserve.Click -= ClickOnBlack;
                ButtonReserve.Click -= ClickBlackToCut;
                if (CheckQueens("BlackQueen", "White", "WhiteQueen", ClickBlackQueenToCut))
                {

                }
            }

            var sound = SoundAsync();
            await sound;

            SendMessage(coordinates);

            IsWin();

            coordinates = ReceiveMessage();
            Changes(coordinates);
        } // Нажать на зеленую кнопку

        private void ClickOnBlack(object sender, RoutedEventArgs e)
        {
            ClearAllGreens();
            ButtonReserve = (Button)sender;
            int x = Grid.GetRow(ButtonReserve);
            int y = Grid.GetColumn(ButtonReserve);
            if (x > 0)
            {
                if (y > 0)
                    DraughtMove(x - 1, y - 1);
                if (y < 7)
                    DraughtMove(x - 1, y + 1);
            }
        } // Нажать на черную шашку

        private void ClickOnWhite(object sender, RoutedEventArgs e)
        {
            ClearAllGreens();
            ButtonReserve = (Button)sender;
            int x = Grid.GetRow(ButtonReserve);
            int y = Grid.GetColumn(ButtonReserve);

            if (x < 7)
            {
                if (y > 0)
                    DraughtMove(x + 1, y - 1);
                if (y < 7)
                    DraughtMove(x + 1, y + 1);
            }
        } // Нажать на белую шашку

        private void ClickWhiteToCut(object sender, RoutedEventArgs e)
        {
            ClearAllGreens();
            ButtonReserve = (Button)sender;
            int x = Grid.GetRow(ButtonReserve);
            int y = Grid.GetColumn(ButtonReserve);
            ConditionToCut(x, y, "Black");
            ConditionToCut(x, y, "BlackQueen");
        } // Нажать на белую шашку, чтобы срубить

        private void ClickBlackToCut(object sender, RoutedEventArgs e)
        {
            ClearAllGreens();
            ButtonReserve = (Button)sender;
            int x = Grid.GetRow(ButtonReserve);
            int y = Grid.GetColumn(ButtonReserve);
            ConditionToCut(x, y, "White");
            ConditionToCut(x, y, "WhiteQueen");
        } // Нажать на черную шашку, чтобы срубить

        private void ClickOnQueen(object sender, RoutedEventArgs e)
        {
            ClearAllGreens();
            ButtonReserve = (Button)sender;
            int x = Grid.GetRow(ButtonReserve);
            int y = Grid.GetColumn(ButtonReserve);
            ConditionToMoveQueen(x, y);
        } // Нажать на белую королеву

        private void ClickWhiteQueenToCut(object sender, RoutedEventArgs e)
        {
            ClearAllGreens();
            ButtonReserve = (Button)sender;
            int x = Grid.GetRow(ButtonReserve);
            int y = Grid.GetColumn(ButtonReserve);
            ConditionToCutQueen(x, y, "Black", "BlackQueen");
        } // Нажать на белую королеву, чтобы срубить

        private void ClickBlackQueenToCut(object sender, RoutedEventArgs e)
        {
            ClearAllGreens();
            ButtonReserve = (Button)sender;
            int x = Grid.GetRow(ButtonReserve);
            int y = Grid.GetColumn(ButtonReserve);
            ConditionToCutQueen(x, y, "White", "WhiteQueen");
        } // Нажать на черную королеву, чтобы срубить

        #endregion Clicks

        #endregion GameModul

        private void GetIP_Click(object sender, RoutedEventArgs e)
        {

        } // Получить IP

        private void ToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            UserPageWindow window = new UserPageWindow();
            window.Show();
            Close();
        } // Вернуться в главное меню
    }
}