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

    public sealed partial class RemovePasswordPage : Page
    {
        byte[] password;

        public RemovePasswordPage()
        {
            this.InitializeComponent();
        }

        private async void PasswordControl_PasswordComplete(Controls.PasswordControl sender, Controls.PasswordCompleteEventArgs args)
        {
            var now = Tools.StringHeleper.MD5(args.Password);
            if(Tools.StringHeleper.BytesCompare(password,now))
            {
                await Tools.AppLocker.DeleteMD5();
                this.Frame.GoBack();
            }
            else
            {
                sender.Shake();
                sender.Message = "密码错误，请重新输入";
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
