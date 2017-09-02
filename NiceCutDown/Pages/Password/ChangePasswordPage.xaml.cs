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

    public sealed partial class ChangePasswordPage : Page
    {
        byte[] password;
        bool confirm = false;
        string firstPassword = "";


        public ChangePasswordPage()
        {
            this.InitializeComponent();
        }

        private async void PasswordControl_PasswordComplete(Controls.PasswordControl sender, Controls.PasswordCompleteEventArgs args)
        {
            if(confirm)
            {
                if (firstPassword == "")
                {
                    firstPassword = args.Password;
                    sender.RemoveAll();
                    sender.Message = "再次输入密码进行确认";
                }
                else
                {
                    if (args.Password == firstPassword)
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
            else
            {

                var now = Tools.StringHeleper.MD5(args.Password);
                if (Tools.StringHeleper.BytesCompare(password, now))
                {
                    confirm = true;
                    sender.RemoveAll();
                    sender.Message = "输入新密码";
                }
                else
                {
                    sender.Shake();
                    sender.Message = "密码错误，请重新输入";
                }
            }


        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            password = await Tools.AppLocker.ReadMD5();
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
