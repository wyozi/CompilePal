﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace CompilePalX
{
    /// <summary>
    /// Interaction logic for LaunchWindow.xaml
    /// </summary>
    public partial class LaunchWindow
    {
        public LaunchWindow()
        {
            string gameConfigurationFolder = "GameConfiguration";
            string gameConfigurationsPath = Path.Combine(gameConfigurationFolder, "gameConfigs.json");

            InitializeComponent();

            if (!Directory.Exists(gameConfigurationFolder))
                Directory.CreateDirectory(gameConfigurationFolder);

            //Loading the last used configurations for hammer
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Hammer\General");

            var configs = new List<GameConfiguration>();

            //try loading json
            if (File.Exists(gameConfigurationsPath))
            {
                string jsonLoadText = File.ReadAllText(gameConfigurationsPath);
                configs.AddRange(JsonConvert.DeserializeObject<List<GameConfiguration>>(jsonLoadText));
            }

            //try loading from registry
            if (rk != null)
            {
                string BinFolder = (string)rk.GetValue("Directory");


                string gameData = Path.Combine(BinFolder, "GameConfig.txt");

                configs.AddRange(GameConfigurationParser.Parse(gameData));
            }

            //finalise config loading
            if (configs.Any())
            {
                //remove duplicates
                configs = configs.GroupBy(g => g.Name).Select(grp => grp.First()).ToList();

                //save
                string jsonSaveText = JsonConvert.SerializeObject(configs, Formatting.Indented);
                File.WriteAllText(gameConfigurationsPath, jsonSaveText);

                if (configs.Count == 1)
                    Launch(configs.First());


                GameGrid.ItemsSource = configs;
            }
            else//oh noes
            {
                LaunchButton.IsEnabled = false;
                WarningLabel.Content = "No Hammer configurations found. Cannot launch.";
            }
        }

        private void Launch(GameConfiguration config)
        {
            GameConfigurationManager.GameConfiguration = config;
            var c = new MainWindow();
            c.Show();

            Close();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (GameConfiguration)GameGrid.SelectedItem;

            if (selectedItem != null)
                Launch(selectedItem);
        }
    }
}
