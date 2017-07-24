using Microsoft.Win32;
using System;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ScreenQ
{
    /// <summary>
    /// Lógica de interacción para Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();
            PopupsToggle.IsChecked = Convert.ToBoolean(Properties.Settings.Default["PopupIsChecked"]);
            SaveAndClose.IsChecked = Convert.ToBoolean(Properties.Settings.Default["SaveAndClose"]);
            TypoPathText.Text = Properties.Settings.Default["XMLTypoPath"].ToString();
            SavePathText.Text = Properties.Settings.Default["ScreenShotSavePath"].ToString();
        }

        private void PopupsToggle_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["PopupIsChecked"] = true;
            Properties.Settings.Default.Save();
        }

        private void PopupsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["PopupIsChecked"] = false;
            Properties.Settings.Default.Save();
        }

        private void SaveAndClose_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["SaveAndClose"] = true;
            Properties.Settings.Default.Save();
        }

        private void SaveAndClose_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["SaveAndClose"] = false;
            Properties.Settings.Default.Save();
        }

        private void TypoPathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = "xml",
                Filter = "XML Files (*.xml)|*.xml"
            };
            bool? result = dlg.ShowDialog();

            if(result != false)
            {
                Properties.Settings.Default["XMLTypoPath"] = dlg.FileName.ToString();
                Properties.Settings.Default.Save();

                TypoPathText.Text = dlg.FileName.ToString();
            }

        }

        private void SavePathButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            Properties.Settings.Default["ScreenShotSavePath"] = dialog.FileName.ToString();
            Properties.Settings.Default.Save();

            SavePathText.Text = dialog.FileName.ToString();
        }

    }
}
