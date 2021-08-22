using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cassandra.Explorer.ViewModels;
using ControlzEx.Theming;
using MahApps.Metro.Controls;

namespace Cassandra.Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {
        bool _isFirstLoad = true;
        public List<ThemeColorViewModel> ThemeColors { get; set; }
        ThemeColorViewModel _currentTheme;

        public Shell()
        {
            InitializeComponent();
            Loaded += Shell_Loaded;
            LoadThemes();
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            _isFirstLoad = false;
        }


        private void LoadThemes()
        {
            var colors = ThemeManager.Current.ColorSchemes.ToList();

            var removeAccents = "Indigo,Yellow,Brown";
            colors.RemoveAll(a => removeAccents.Contains(a));

            ThemeColors = colors.Select(c => new ThemeColorViewModel
            {
                Name = c,
                ColorBrush = ThemeManager.Current.Themes.FirstOrDefault(t => t.Name == $"Light.{c}").ShowcaseBrush
            })
            .ToList();

            ThemeColorsList.ItemsSource = ThemeColors;

            var currentTheme = ThemeManager.Current.DetectTheme();
            if (currentTheme != null)
                _currentTheme = new ThemeColorViewModel
                {
                    Name = currentTheme.Name.Replace("Dark.", string.Empty).Replace("Light.", string.Empty),
                    ColorBrush = currentTheme.ShowcaseBrush
                };
        }

        void AboutClicked(object sender, RoutedEventArgs e)
        {
            AboutFlyout.IsOpen = !AboutFlyout.IsOpen;
        }

        private void SettingsClicked(object sender, RoutedEventArgs e)
        {
            ThemeSettingsFlyout.IsOpen = !ThemeSettingsFlyout.IsOpen;
        }

        private void ThemeColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isFirstLoad || (sender as ListBox) == null)
                return;

            var selectedColor = ((ListBox)sender).SelectedItem as ThemeColorViewModel;
            if (selectedColor == null || _currentTheme == null) return;

            var theme = ThemeManager.Current.ChangeTheme(Application.Current, DarkTheme.IsOn ? "Dark" : "Light", selectedColor.Name);
            if (theme != null && _currentTheme != null)
            {
                _currentTheme.Name = selectedColor.Name;
                _currentTheme.ColorBrush = theme.ShowcaseBrush;
            }

            //Commit Accent theme to user profile
            //Settings.Default.ThemeAccent = selectedColor.Name;
            //Settings.Default.Save();
        }

        private void DarkTheme_Toggled(object sender, RoutedEventArgs e)
        {
            if (_isFirstLoad) return;

            ThemeManager.Current.ChangeTheme(Application.Current, DarkTheme.IsOn ? "Dark" : "Light", _currentTheme.Name);
        }

    }
}