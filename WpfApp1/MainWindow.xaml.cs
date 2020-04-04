using System;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Net;
using System.Diagnostics;
using System.Web;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        const string urlCharacters = "https://gateway.marvel.com/v1/public/characters?ts=";
        const string urlComics = "https://gateway.marvel.com/v1/public/comics?ts=";
        const string publicKey = "3a12e0bedc5ccb31e93535f4f661f5d2";
        const string privateKey = "9ff0d7b4c3c6223fc5374ab41624fbee915ce0ef";
        const string format = "&format=json";

        List<Comic> comics = new List<Comic>();


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
            string charId = getCharacterId();
            if (charId != "error"){
                getComics(charId);
            }
        }

        private void getComics(String charId)
        {
            String timeStamp = GetTimestamp(new DateTime());
            string hash = CreateMD5(timeStamp + privateKey + publicKey);
            var jsonRep = "";

            using (WebClient client = new WebClient())
            {
                try
                {
                    jsonRep = client.DownloadString(urlComics + timeStamp + "&characters=" + charId + "&apikey=" + publicKey + "&hash=" + hash);
                    //Trace.WriteLine(jsonRep);

                    dynamic JObj = JsonConvert.DeserializeObject(jsonRep);
                    int comicsCount = JObj.data.count;
                    Trace.WriteLine(comicsCount);

                    comics = new List<Comic>();

                    for (int i = 0; i < comicsCount; i++)
                    {
                        Trace.WriteLine(i);

                        int crtId = JObj.data.results[i].id;
                        String crtTitle = JObj.data.results[i].title;
                        int crtIssue = JObj.data.results[i].issueNumber;
                        String crtDesc = JObj.data.results[i].description;
                        String crtThumbnail = JObj.data.results[i].thumbnail.path + ""+ JObj.data.results[i].thumbnail.extension;

                        comics.Add(new Comic() {Title = crtTitle, Thumbnail = crtThumbnail, Description = crtDesc });
                    }

                    listDonne.ItemsSource = comics;
                }
                catch (WebException e)
                {
                    textName.Text = "La requête à échoué";
                    textName.Visibility= Visibility.Visible;
                    textDescription.Visibility= Visibility.Collapsed;
                    imageCharacter.Visibility= Visibility.Collapsed;
                    textListComics.Visibility= Visibility.Collapsed;
                    listDonne.Visibility= Visibility.Collapsed;
                    throw e;
                }
            }
        }



        private String getCharacterId()
        {
            String timeStamp = GetTimestamp(new DateTime());
            string hash = CreateMD5(timeStamp+privateKey+publicKey);
            var jsonRep = "";

            using (WebClient client = new WebClient())
            {
                try
                {
                    jsonRep = client.DownloadString(urlCharacters + timeStamp + "&name=" + textBoxSearch.Text + "&apikey=" + publicKey + "&hash=" + hash);

                    dynamic JObj = JsonConvert.DeserializeObject(jsonRep);

                    if (JObj.data.count != 0){
                        string charactersId = JObj.data.results[0].id;
                        string charactersIdStr = charactersId.ToString();
                        string characterThumbnail = JObj.data.results[0].thumbnail.path+ "/standard_large.jpg";
                        string characterDescription = JObj.data.results[0].description;
                        string characterName = JObj.data.results[0].name;
                        textName.Text = "Name : " + characterName;
                        textName.Visibility= Visibility.Visible;
                        textDescription.Text = "Description : " + characterDescription;
                        textDescription.Visibility= Visibility.Visible;
                        var uriSource = new Uri(characterThumbnail);
                        imageCharacter.Source = new BitmapImage(uriSource);
                        imageCharacter.Visibility= Visibility.Visible;
                        textListComics.Visibility= Visibility.Visible;
                        listDonne.Visibility= Visibility.Visible;

                        //content.Text = json;
                        return charactersId;

                    } else {
                        textName.Text = "Error, votre super héros ne fais pas partie de l'univers marvel ou est mal orthographié";
                        textName.Visibility= Visibility.Visible;
                        textDescription.Visibility= Visibility.Collapsed;
                        imageCharacter.Visibility= Visibility.Collapsed;
                        textListComics.Visibility= Visibility.Collapsed;
                        listDonne.Visibility= Visibility.Collapsed;
                        return "error";
                    }

                }
                catch (WebException e)
                {
                    textName.Text = "La requête à échoué";
                    textName.Visibility= Visibility.Visible;
                    textDescription.Visibility= Visibility.Collapsed;
                    imageCharacter.Visibility= Visibility.Collapsed;
                    textListComics.Visibility= Visibility.Collapsed;
                    listDonne.Visibility= Visibility.Collapsed;
                    throw e;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}

