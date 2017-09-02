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



    public sealed partial class PasswordManagerPage : Page
    {
        public PasswordManagerPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            bool inLock = await AppLocker.HasMD5();
            if (inLock)
            {
                lockerComboBox.SelectedIndex = 0;
                reLockGrid.Visibility = Visibility.Visible;
            }
            else
            {
                lockerComboBox.SelectedIndex = 1;
                reLockGrid.Visibility = Visibility.Collapsed;
            }
            lockerComboBox.SelectionChanged += LockerComboBox_SelectionChanged;
        }

        private void LockerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(lockerComboBox.SelectedIndex)
            {
                case -1:return;
                case 0:this.Frame.Navigate(typeof(SetPasswordPage));break;
                case 1:this.Frame.Navigate(typeof(RemovePasswordPage));break;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
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

        private void reLockGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChangePasswordPage));
        }
    }
}
