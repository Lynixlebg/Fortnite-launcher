using DiscordRPC;
using System;
using System.Windows;
using System.Windows.Navigation;
using ModernWpf.Controls;
using System.Net;
using WpfApp6.Pages;

namespace WpfApp6
{
    public partial class MainWindow : Window
    {
        Home home = new Home();
        Settings settings = new Settings();
        download downloud = new download();

        DiscordRpcClient client;

        public MainWindow()
        {
            InitializeComponent();

            // Works
            client = new DiscordRpcClient("1206183757434847283");
            client.Initialize();

            /* api just change google.com to your api
            WebClient omg = new WebClient();
            try
            {
                string fds = omg.DownloadString("http://google.com/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed Connecting To Servers");
                Application.Current.Shutdown();
            } */
        } 


        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {

            HomeItem.IsSelected = true;
        }

        private void NavView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(settings);
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                if (item.Tag.ToString() == "Home")
                {
                    ContentFrame.Navigate(home);
                    UpdatePresence("Home");
                }
                else if (item.Tag.ToString() == "download")
                {
                    ContentFrame.Navigate(downloud);
                    UpdatePresence("Download");
                }
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page.");
        }

        private void UpdatePresence(string Page)
        {
            var presence = new RichPresence()
            {
                Details = $"OG launcher Made by Marvelco",
                State = "https://github.com/MarvelcoDev",
                Assets = new Assets()
                {
                    LargeImageKey = "48595",
                    LargeImageText = "Fortnite OG launcher",
                    SmallImageText = "48595"
                }
            };
            client.SetPresence(presence);
        }
    }
}