using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WpfApp6.Services;
using WpfApp6.Services.Launch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Threading;

namespace WpfApp6.Pages
{
    public partial class Home : System.Windows.Controls.UserControl
    {
        private DispatcherTimer clockTimer;
        public ObservableCollection<VersionItem> VersionItems { get; set; }
        private int versionNumber = 1;

        public Home()
        {
            InitializeComponent();


            InitializeClockTimer();


            VersionItems = new ObservableCollection<VersionItem>();
            versionList.ItemsSource = VersionItems;


            LoadConfigFile();
        }

        private void InitializeClockTimer()
        {
            clockTimer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();


            UpdateDigitalClock();
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateDigitalClock();
        }

        private void UpdateDigitalClock()
        {

            digitalClock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void LoadConfigFile()
        {
            try
            {

                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");


                if (File.Exists(configFilePath))
                {

                    string jsonText = File.ReadAllText(configFilePath);


                    JObject json = JObject.Parse(jsonText);


                    JArray folderPathsArray = (JArray)json["FolderPaths"];


                    VersionItems.Clear();
                    versionNumber = 1;


                    foreach (var folderPathToken in folderPathsArray)
                    {
                        string folderPath = folderPathToken.ToString();


                        DisplayBuilds(folderPath, versionNumber);
                        versionNumber++;
                    }
                }
                else
                {

                    System.Windows.MessageBox.Show("Configuration file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show($"Error loading configuration file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayBuilds(string selectedFolderPath, int versionNumber)
        {
            try
            {

                string[] buildFiles = Directory.GetFiles(selectedFolderPath, "Splash.bmp", SearchOption.AllDirectories);

                foreach (string buildFile in buildFiles)
                {

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(buildFile, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();


                    VersionItem versionItem = new VersionItem
                    {
                        FullImagePath = buildFile,
                        ImageSource = bitmap,
                        CornerRadius = new CornerRadius(10),
                        VersionNumber = versionNumber
                    };


                    VersionItems.Add(versionItem);
                }

                versionList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show($"Error loading builds: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                versionList.Visibility = Visibility.Collapsed;
            }
        }

        private void LaunchButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button launchButton && launchButton.Tag is VersionItem versionItem)
                {
                    string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                    if (File.Exists(configFilePath))
                    {
                        string jsonText = File.ReadAllText(configFilePath);
                        JObject json = JObject.Parse(jsonText);
                        JArray folderPathsArray = (JArray)json["FolderPaths"];

                        // Retrieve the index of the version item
                        int index = VersionItems.IndexOf(versionItem);

                        if (index >= 0 && index < folderPathsArray.Count)
                        {
                            string selectedFolderPath = folderPathsArray[index].ToString();
                            string executablePath = Path.Combine(selectedFolderPath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe");

                            if (File.Exists(executablePath))
                            {
                                string credentialsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");

                                if (File.Exists(credentialsFilePath))
                                {
                                    string credentialsText = File.ReadAllText(credentialsFilePath);
                                    JObject credentialsJson = JObject.Parse(credentialsText);
                                    string email = credentialsJson["Email"].ToString();
                                    string password = credentialsJson["Password"].ToString();

                                    if (email == "NONE" || password == "NONE")
                                    {
                                        System.Windows.MessageBox.Show("Error code 1", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }

                                    WebClient OMG = new WebClient();
                                    string dllPath = Path.Combine(selectedFolderPath, "Engine\\Binaries\\ThirdParty\\NVIDIA\\NVaftermath\\Win64", "GFSDK_Aftermath_Lib.x64.dll");
                                    OMG.DownloadFile("Your Dll", dllPath);

                                    PSBasics.Start(selectedFolderPath, "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", email, password);
                                    FakeAC.Start(selectedFolderPath, "FortniteClient-Win64-Shipping_BE.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", "r");
                                    FakeAC.Start(selectedFolderPath, "FortniteLauncher.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", "dsf");

                                    PSBasics._FortniteProcess.WaitForExit();
                                    try
                                    {
                                        FakeAC._FNLauncherProcess.Close();
                                        FakeAC._FNAntiCheatProcess.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Windows.MessageBox.Show("There was an error closing: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Fortnite executable not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Invalid version item index.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Configuration file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error launching Fortnite: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void FindBuild(object sender, RoutedEventArgs e)
        {
            try
            {
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");


                if (File.Exists(configFilePath))
                {

                    string jsonText = File.ReadAllText(configFilePath);


                    JObject json = JObject.Parse(jsonText);


                    JArray folderPathsArray = (JArray)json["FolderPaths"];


                    if (folderPathsArray.Count >= 4)
                    {
                        System.Windows.MessageBox.Show("You cannot add more than 4 versions please delete one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var dialog = new FolderBrowserDialog();

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string selectedFolderPath = dialog.SelectedPath;


                    SaveSelectedFolderPath(selectedFolderPath);

                    LoadConfigFile();
                }

            }
            catch (Exception ex) { 

                System.Windows.MessageBox.Show($"Error finding build: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveSelectedFolderPath(string selectedFolderPath)
        {
            try
            {

                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");


                JObject json = new JObject();


                if (File.Exists(configFilePath))
                {

                    string jsonText = File.ReadAllText(configFilePath);


                    json = JObject.Parse(jsonText);
                }


                JArray folderPathsArray = json.ContainsKey("FolderPaths") ? (JArray)json["FolderPaths"] : new JArray();


                folderPathsArray.Add(selectedFolderPath);


                json["FolderPaths"] = folderPathsArray;


                string output = json.ToString();


                File.WriteAllText(configFilePath, output);

                Console.WriteLine($"Selected Folder Path saved to config file: {configFilePath}");
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show($"Error saving selected folder path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteVersion(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is VersionItem version)
            {
                VersionItems.Remove(version);
                SaveConfigFile();
            }
        }
        private void SaveConfigFile()
        {
            try
            {
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                JObject json;
                JArray folderPathsArray;

                if (File.Exists(configFilePath))
                {

                    string existingJson = File.ReadAllText(configFilePath);
                    json = JObject.Parse(existingJson);

                    if (json["FolderPaths"] == null)
                    {

                        folderPathsArray = new JArray();
                        json["FolderPaths"] = folderPathsArray;
                    }
                    else
                    {

                        folderPathsArray = (JArray)json["FolderPaths"];
                    }
                }
                else
                {

                    json = new JObject();
                    folderPathsArray = new JArray();
                    json["FolderPaths"] = folderPathsArray;
                }

                if (folderPathsArray.Count > 0)
                {
                    folderPathsArray.RemoveAt(0);
                }


                string output = json.ToString();
                File.WriteAllText(configFilePath, output);
                Console.WriteLine($"Configuration file updated: {configFilePath}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving configuration file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //class
        public class VersionItem
        {
            public string FullImagePath { get; set; }
            public BitmapImage ImageSource { get; set; }  
            public CornerRadius CornerRadius { get; set; } = new CornerRadius(10);
            public int VersionNumber { get; set; } 
        }
    }
}
