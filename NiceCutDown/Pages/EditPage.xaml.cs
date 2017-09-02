using NiceCutDown.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Media.Capture;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace NiceCutDown
{

    public sealed partial class EditPage : Page
    {
        List<Uri> pics = new List<Uri>();
        List<SolidColorBrush> colors = new List<SolidColorBrush>();
        string path;

        LocateCutDownHelper lcdh = new LocateCutDownHelper();
        int index=-1;

        public EditPage()
        {
            this.InitializeComponent();

            pics.Add(new Uri("ms-appx:///Assets/button/edit_addpic_normal.png"));
            pics.Add(new Uri("ms-appx:///Assets/button/camera.png"));
            pics.Add(new Uri("ms-appx:///Assets/pictures/wp_autumn1.jpg"));
            pics.Add(new Uri("ms-appx:///Assets/pictures/wp_autumn2.jpg"));
            pics.Add(new Uri("ms-appx:///Assets/pictures/edit_default.jpg"));
            pics.Add(new Uri("ms-appx:///Assets/pictures/love.jpg"));
            pics.Add(new Uri("ms-appx:///Assets/pictures/birthday.jpg"));
            pics.Add(new Uri("ms-appx:///Assets/pictures/school.jpg"));
            pics.Add(new Uri("ms-appx:///Assets/pictures/baby.jpg"));
            pics.Add(new Uri("ms-appx:///Assets/pictures/sport.jpg"));
            picsListView.ItemsSource = pics;

            datePicker.MaxYear = new DateTimeOffset(2099, 12, 31, 0, 0, 0, 0, new TimeSpan(8, 0, 0));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            if (colors == null || colors.Count == 0)
            {
                colors = await NiceCutDown.Resources.ColorManager.GetColorList();
                colorSelectorListView.ItemsSource = colors;

            }
            if (e.Parameter == null)
            {
                background.Image = new BitmapImage(pics[4]);
                path = pics[4].AbsoluteUri;
                colorGrid.Background = colors[(new Random()).Next(0, colors.Count - 1)];
            }
            else
            {
                await lcdh.Read();
                index = (int)e.Parameter;
                CountDown cd = lcdh.CountDowns[index];
                path = cd.Picture;
                if(path=="")
                {
                    background.Image = new BitmapImage();
                }
                else
                {
                    background.Image = new BitmapImage(new Uri(cd.Picture));
                }

                repeatComboBox.SelectedIndex = cd.Time.Repeat ? 1 : 0;

                titleTextBox.Text = cd.Title;

                colorGrid.Background = new SolidColorBrush(cd.Color);

                datePicker.Date = cd.Time.Time;

                lunarDatePicker.ChineseCalendar = new YinYang.ChineseCalendar(cd.Time.Time);

                lunarComboBox.SelectedIndex = cd.Time.Lunar ? 1 : 0;
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
            if (colorSelectorGrid.Visibility == Visibility.Visible)
            {
                ExitColorSelector();
            }
            else
            {
                this.Frame.GoBack();
            }
            
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(80);
            if (datePicker == null) return;
            switch(((ComboBox)sender).SelectedIndex)
            {
                case 0:
                    datePicker.Visibility = Visibility.Visible;
                    lunarDatePicker.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    datePicker.Visibility = Visibility.Collapsed;
                    lunarDatePicker.Visibility = Visibility.Visible;
                    break;
                default:
                    datePicker.Visibility = Visibility.Visible;
                    lunarDatePicker.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void picsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(picsListView.SelectedIndex!=-1)
            {
                if(picsListView.SelectedIndex==0)
                {
                    LoadPictures();
                    picsListView.SelectedIndex = -1;
                }
                else if (picsListView.SelectedIndex == 1)
                {
                    LoadCamera();
                    picsListView.SelectedIndex = -1;
                }
                else
                {
                    Uri pic = pics[picsListView.SelectedIndex];
                    path = pic.AbsoluteUri;
                    BitmapImage bi = new BitmapImage(pic);
                    background.Image = bi;
                }
            }
        }

        private async void LoadPictures()
        {
            try
            {
                var picker = new FileOpenPicker()
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp" },
                };

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    path = await NiceCutDown.Resources.PicturesManager.Save(file);
                    background.Image = new BitmapImage(new Uri(path));
                }
            }
            catch
            {
            }
        }


        private async void LoadCamera()
        {
            CameraCaptureUI ccui = new CameraCaptureUI();
            ccui.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            StorageFile file = await ccui.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file != null)
            {
                path = await NiceCutDown.Resources.PicturesManager.Save(file);
                background.Image = new BitmapImage(new Uri(path));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(titleTextBox.Visibility == Visibility.Collapsed)
            {
                BeginTitleEdit();
            }
            else
            {
                EndTitleEdit();
            }
        }

        private async void BeginTitleEdit()
        {
            titleTextBox.Visibility = Visibility.Visible;
            Storyboard sb = new Storyboard();

            DoubleAnimation da = new DoubleAnimation();
            da.From = 30;
            da.To = ActualWidth - titleTitleGrid.ActualWidth;
            da.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            da.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
            da.EnableDependentAnimation = true;
            Storyboard.SetTarget(da, titleTextBox);
            Storyboard.SetTargetProperty(da, "Width");
            sb.Children.Add(da);

            DoubleAnimation da2 = new DoubleAnimation();
            da2.From = 1;
            da2.To = 0;
            da2.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            da2.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
            da.EnableDependentAnimation = true;
            Storyboard.SetTarget(da2, titleTextBlock);
            Storyboard.SetTargetProperty(da2, "Opacity");
            sb.Children.Add(da2);

            sb.Begin();

            await Task.Delay(300);
            titleTextBlock.Visibility = Visibility.Collapsed;
            await Task.Delay(200);
            titleTextBox.Focus(FocusState.Programmatic);
        }


        private async void EndTitleEdit()
        {
            titleTextBlock.Visibility = Visibility.Visible;
            titleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            Storyboard sb = new Storyboard();

            DoubleAnimation da = new DoubleAnimation();
            da.From = ActualWidth - titleTitleGrid.ActualWidth;
            da.To = titleTextBlock.ActualWidth;
            da.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            da.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
            da.EnableDependentAnimation = true;
            Storyboard.SetTarget(da, titleTextBox);
            Storyboard.SetTargetProperty(da, "Width");
            sb.Children.Add(da);


            DoubleAnimation da2 = new DoubleAnimation();
            da2.From = 0;
            da2.To = 1;
            da2.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            da2.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
            da.EnableDependentAnimation = true;
            Storyboard.SetTarget(da2, titleTextBlock);
            Storyboard.SetTargetProperty(da2, "Opacity");
            sb.Children.Add(da2);

            sb.Begin();

            await Task.Delay(300);
            titleTextBox.Visibility = Visibility.Collapsed;
            await Task.Delay(200);
            titleTextBlock.Focus(FocusState.Programmatic);
            titleTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

        }

        private async void ShowColorSelector()
        {
            colorSelectorGrid.Visibility = Visibility.Visible;
            colorSelectorStoryboard_show.Begin();
            await Task.Delay(450);
            maskGrid.Visibility = Visibility.Collapsed; 
        }

        private async void ExitColorSelector()
        {
            maskGrid.Visibility = Visibility.Visible;
            colorSelectorStoryboard_exit.Begin();
            await Task.Delay(500);
            colorSelectorGrid.Visibility = Visibility.Collapsed;
        }


        private void titleTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                EndTitleEdit();
            }
        }

        private void titleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EndTitleEdit();
        }

        private void ColorSelectorBackButton_Click(object sender, RoutedEventArgs e)
        {
            ExitColorSelector();
        }

        private void colorSelectorListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(colorSelectorListView.SelectedIndex!=-1)
            {
                colorGrid.Background = colors[colorSelectorListView.SelectedIndex];
                colorSelectorListView.SelectedIndex = -1;
                ExitColorSelector();
            }
        }

        private void colorGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ShowColorSelector();
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            CountDown cd = new CountDown();
            CountDownTime cdt= new CountDownTime();
            if(lunarComboBox.SelectedIndex == 1)
            {
                cdt.Lunar = true;
                cdt.Time = lunarDatePicker.ChineseCalendar.Date;
            }
            else
            {
                cdt.Lunar = false;
                cdt.Time = datePicker.Date.Date;
            }

            cdt.Repeat = repeatComboBox.SelectedIndex == 0 ? false : true;

            cd.Time = cdt;
            cd.Color = ((SolidColorBrush)colorGrid.Background).Color;

            cd.Title = titleTextBlock.Text;

            cd.Picture = path == null ? "" : path;


            await lcdh.Read();
            if (index==-1)
            {
                lcdh.Add(cd);
            }
            else
            {
                lcdh.Replace(index, cd);
            }


            int i=0;
            loop:
            try
            {
                await lcdh.Save();
            }
            catch(Exception error)
            {
                i++;
                if(i>5)
                {
                    MessageDialog md = new MessageDialog("错误提示:" + error.Message, "保存失败");
                    await md.ShowAsync();
                    return;
                }
                else
                {
                    goto loop;
                }
            }


            this.Frame.GoBack();
        }
    }
}
