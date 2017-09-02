using NiceCutDown.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using YinYang;


namespace NiceCutDown
{

    public sealed partial class CountDownPage : Page
    {
        List<CountDown> countDowns;
        int index;
        bool savePicInBinding = false;

        public CountDownPage()
        {
            this.InitializeComponent();
            hideAcitons.Completed += Acitons_Completed;
            showAcitons.Completed += Acitons_Completed;
        }

        private void Acitons_Completed(object sender, object e)
        {
            rootGrid.Tapped += Grid_Tapped;
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            if (e.Parameter != null && e.Parameter.GetType() == typeof(int))
            {
                Tools.LocateCutDownHelper lcdh = new Tools.LocateCutDownHelper();
                await lcdh.Read();
                countDowns = lcdh.CountDowns;
                index = (int)e.Parameter;

                var cd = countDowns[index];


                


                daysCount.CountDays = cd.Time;

                BitmapImage bi;
                if (cd.Picture=="")
                {
                    bi = new BitmapImage();
                }
                else
                {
                    bi = new BitmapImage(new Uri(cd.Picture));
                }
                
                background.Image = bi;


                if(cd.Time.Lunar)
                {
                    var date = (new ChineseCalendar(cd.Time.Time));
                    if (SettingsManager.Ganzhi)
                    {
                        dateTextBlock.Text = date.GanZhiYearString + date.ChineseMonthString + date.ChineseDayString;
                    }
                    else
                    {
                        dateTextBlock.Text = date.ChineseDateString;
                    }
                    
                }
                else
                {
                    dateTextBlock.Text = cd.Time.Time.Year.ToString() + "-" + cd.Time.Time.Month.ToString() + "-" + cd.Time.Time.Day.ToString();
                }
                
                titleTextBlock.Text = cd.Title;
            }

        }



        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        private void PageBack()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            PageBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            PageBack();
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            rootGrid.Tapped -= Grid_Tapped;
            if (ActionGrid.Visibility == Visibility.Visible)
            {
                hideAcitons.Begin();
            }
            else
            {
                showAcitons.Begin();
            }
        }

        private async void action_delete_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog message_dialog = new MessageDialog("你确定要删除吗？");
            message_dialog.Commands.Add(new UICommand("确定", cmd => { }, "delete"));
            message_dialog.Commands.Add(new UICommand("取消", cmd => { }));
            message_dialog.DefaultCommandIndex = 0;
            message_dialog.CancelCommandIndex = 1;
            IUICommand result = await message_dialog.ShowAsync();
            if (result.Id as string == "delete")
            {
                Tools.LocateCutDownHelper lcdh = new Tools.LocateCutDownHelper();
                await lcdh.Read();
                lcdh.Remove(index);
                await lcdh.Save();
                this.Frame.GoBack();
            }

        }

        private void Handled_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void action_download_Click(object sender, RoutedEventArgs e)
        {
            hideAcitons.Begin();
            if (savePicInBinding == true) return;
            hideAcitons.Completed +=async (s, o) =>
             {
                 try
                 {
                     RectangleGeometry rg = new RectangleGeometry();
                     rg.Rect = new Rect(0, 0, background.ActualWidth, background.ActualHeight);
                     background.Clip = rg;


                     RenderTargetBitmap bitmap = new RenderTargetBitmap();
                     await bitmap.RenderAsync(rootGrid);

                     background.Clip = null;

                     var buffer = await bitmap.GetPixelsAsync();




                     StorageFolder folder = await KnownFolders.PicturesLibrary.GetFolderAsync("Saved Pictures");
                     var file = await folder.CreateFileAsync(DateTime.Now.Ticks.ToString() + ".png", CreationCollisionOption.OpenIfExists);
                     using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                     {
                         var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                         encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                              BitmapAlphaMode.Ignore,
                              (uint)bitmap.PixelWidth,
                              (uint)bitmap.PixelHeight,
                              DisplayInformation.GetForCurrentView().LogicalDpi,
                              DisplayInformation.GetForCurrentView().LogicalDpi,
                              buffer.ToArray());
                         await encoder.FlushAsync();
                     }

                     MessageDialog md = new MessageDialog("保存成功");
                     await md.ShowAsync();
                 }
                 catch(Exception error)
                 {
                     MessageDialog md = new MessageDialog("保存失败：" + error.Message);
                     await md.ShowAsync();
                 }
             };
            savePicInBinding = true;

        }

        private void action_new_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
            this.Frame.Navigate(typeof(EditPage));
        }

        private void action_edit_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditPage), index);
        }
    }

}
