using NiceCutDown.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;




namespace NiceCutDown
{

    public sealed partial class SettingPage : Page
    {

        public SettingPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            versionTextBlock.Text = SettingsManager.Version;

            switch(SettingsManager.Reminder)
            {
                case 0:reminderComboBox.SelectedIndex = 2;break;
                case 1: reminderComboBox.SelectedIndex = 0; break;
                case 2: reminderComboBox.SelectedIndex = 1; break;
            }

            if(SettingsManager.Ganzhi)
            {
                ganzhiComboBox.SelectedIndex = 0; 
            }
            else
            {
                ganzhiComboBox.SelectedIndex = 1;
            }
            reminderComboBox.SelectionChanged += ReminderComboBox_SelectionChanged;
            ganzhiComboBox.SelectionChanged += GanzhiComboBox_SelectionChanged;
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }


        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            this.Frame.GoBack();

        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void ReminderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (reminderComboBox.SelectedIndex)
            {
                case 0: SettingsManager.Reminder = 1; break;
                case 1: SettingsManager.Reminder = 2; break;
                case 2: SettingsManager.Reminder = 0; break;
            }
        }

        private void GanzhiComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(ganzhiComboBox.SelectedIndex)
            {
                case 0:SettingsManager.Ganzhi = true;break;
                case 1:SettingsManager.Ganzhi = false;break;
            }

        }

        private void LockerGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PasswordManagerPage));
        }
    }
}
