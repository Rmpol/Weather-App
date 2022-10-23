using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using itProgerWPF.Models;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Linq;
using Microsoft.Win32;
using System.Windows.Media;

namespace itProgerWPF
{
    public partial class MainWindow : Window
    {

        private const string API_KEY = "3d9de74844d28377e81415151cbe6a66";

        public MainWindow()
        {
            InitializeComponent();
            MainScreen.IsChecked = true;
            SetDefaultSize.IsSelected = true;

            if (!File.Exists("user.xml"))
                ShowAuthWindow();

            XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
            using (FileStream file = new FileStream("user.xml", FileMode.Open))
            {
                AuthUser auth = (AuthUser)xml.Deserialize(file);
                UserNameLabel.Content = auth.Login;
            }
        }

        private async void GetWeatherBtn_Click(object sender, RoutedEventArgs e)
        {
            string city = UserCityTextBox.Text.Trim();
            if(city.Length < 2)
            {
                MessageBox.Show("Укажите верный город");
                return;
            }

            try
            {
                string data = await GetWeather(city);
                var json = JObject.Parse(data);
                string temp = json["main"]["temp"].ToString();
                WeatherResults.Content = $"В городе {city} {temp} градусов";
            } catch (HttpRequestException ex)
            {
                MessageBox.Show("Укажите верный город");
                WeatherResults.Content = "";
            }

        }

        private async Task<string> GetWeather(string city)
        {
            HttpClient client = new HttpClient();
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units=metric";
            return await client.GetStringAsync(url);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string objName = ((RadioButton)sender).Name;

            StackPanel[] panels = { MainScreenPanel, CabinetScreenPanel, NotesScreenPanel };
            foreach (var panel in panels)
                panel.Visibility = Visibility.Hidden;

            switch(objName)
            {
                case "MainScreen": MainScreenPanel.Visibility = Visibility.Visible; break;
                case "CabinetScreen": CabinetScreenPanel.Visibility = Visibility.Visible; break;
                case "NotesScreen": NotesScreenPanel.Visibility = Visibility.Visible; break;
            }

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("user.xml");
            ShowAuthWindow();
        }

        private void ShowAuthWindow()
        {
            Hide();
            AuthWindow window = new AuthWindow();
            window.Show();
            Close();
        }

        private void UserChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = UserLogin.Text.Trim();
            string email = UserEmail.Text.Trim();
            if (login.Equals("") || !email.Contains("@"))
            {
                MessageBox.Show("Вы что-то ввели неверно");
                return;
            }

            AppDbContext db = new AppDbContext();
            int countUsers = Convert.ToInt32(db.Users.Count(el => el.Login == login));
            if(countUsers != 0 && !login.Equals(UserNameLabel.Content))
            {
                MessageBox.Show("Такой логин уже занят");
                return;
            }

            User user = db.Users.FirstOrDefault(el => el.Login == UserNameLabel.Content.ToString());
            user.Email = email;
            user.Login = login;
            db.SaveChanges();
            UserNameLabel.Content = login;
            UserChangeBtn.Content = "Готово";

            AuthUser auth = new AuthUser(login, email);
            XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
            using (FileStream file = new FileStream("user.xml", FileMode.Create))
            {
                xml.Serialize(file, auth);
            }
        }

        private void MenuOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool isFolder = (bool)openFileDialog.ShowDialog();

            if(isFolder)
            {
                using(Stream stream = File.Open(openFileDialog.FileName, FileMode.Open))
                {
                    using (StreamReader writer = new StreamReader(stream))
                    {
                        UserNotesTextBox.Text = writer.ReadToEnd();
                    }
                }
            }
        }

        private void MenuSaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveTextToFile();
        }

        private void TimesNewRomanSetText_Click(object sender, RoutedEventArgs e)
        {
            UserNotesTextBox.FontFamily = new FontFamily("Times New Roman");
            VerdanaSetText.IsChecked = false;
        }

        private void VerdanaSetText_Click(object sender, RoutedEventArgs e)
        {
            UserNotesTextBox.FontFamily = new FontFamily("Verdana");
            TimesNewRomanSetText.IsChecked = false;
        }

        private void SelectFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = (ComboBoxItem)SelectFontSize.SelectedItem;
            int fontSize = Convert.ToInt32(comboBoxItem.Tag);
            UserNotesTextBox.FontSize = fontSize;
        }

        private void MenuNewFile_Click(object sender, RoutedEventArgs e)
        {
            if (UserNotesTextBox.Text.Trim().Equals(""))
                return;

            SaveTextToFile();
            UserNotesTextBox.Text = "";
        }

        private void SaveTextToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            bool isFolder = (bool)saveFileDialog.ShowDialog();

            if (isFolder)
            {
                using (Stream file = File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        writer.Write(UserNotesTextBox.Text);
                    }
                }
            }
        }
    }
}
