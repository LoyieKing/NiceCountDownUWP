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


    public sealed partial class SetPasswordPage : Page
    {
        string firstPassword = "";


        public SetPasswordPage()
        {
            this.InitializeComponent();
        }

        private async void PasswordControl_PasswordComplete(Controls.PasswordControl sender, Controls.PasswordCompleteEventArgs args)
        {
            if(firstPassword=="")
            {
                firstPassword = args.Password;
                sender.RemoveAll();
                sender.Message = "再次输入密码进行确认";
            }
            else
            {
                if(args.Password==firstPassword)
                {
                    await Tools.AppLocker.WriteMD5(firstPassword);
                    this.Frame.GoBack();
                }
                else
                {
                    sender.Shake();
                    sender.Message = "两次输入密码不一致，请重新输入";
                    firstPassword = "";
                }
            }
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
    }
}
