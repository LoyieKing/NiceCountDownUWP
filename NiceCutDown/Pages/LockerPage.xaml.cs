using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace NiceCutDown
{

    public sealed partial class LockerPage : Page
    {
        byte[] password;

        public LockerPage()
        {
            this.InitializeComponent();
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            password = await Tools.AppLocker.ReadMD5();
        }

        private void PasswordControl_PasswordComplete(Controls.PasswordControl sender, Controls.PasswordCompleteEventArgs args)
        {
            var now = Tools.StringHeleper.MD5(args.Password);
            if (Tools.StringHeleper.BytesCompare(password, now))
            {
                this.Frame.GoBack();
            }
            else
            {
                sender.Shake();
            }
        }
    }
}
