using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;




namespace NiceCutDown
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VideoPage : Page
    {
        DisplayOrientations ori;

        public VideoPage()
        {
            this.InitializeComponent();
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mediaElement.Stop();
            Navigate();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }
            ori = DisplayInformation.AutoRotationPreferences;
            DisplayInformation.AutoRotationPreferences = (DisplayOrientations)5;
        }

        private void Navigate()
        {
            DisplayInformation.AutoRotationPreferences = ori;
            this.Frame.Navigate(typeof(MainPage));
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Navigate();
        }

        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Navigate();
        }
    }
}
