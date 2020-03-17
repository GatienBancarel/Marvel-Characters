using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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

using System.Net;

namespace WpfApp1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string url = "https://gateway.marvel.com/v1/public/characters?ts=";
        const string publicKey = "3a12e0bedc5ccb31e93535f4f661f5d2";
        const string privateKey = "9ff0d7b4c3c6223fc5374ab41624fbee915ce0ef";
        const string format = "&format=json";



        public MainWindow()
        {
            InitializeComponent();
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        private void btnEnter(object sender, RoutedEventArgs e)
        {
            resultat();
        }

        private void resultat()
        {
            String timeStamp = GetTimestamp(new DateTime());
            string hash = CreateMD5(timeStamp+privateKey+publicKey);
            
            var json = new WebClient().DownloadString(url + timeStamp + "&name=" + textBoxSearch.Text + "&apikey=" + publicKey + "&hash=" + hash);
            content.Text = json;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


    }
}
