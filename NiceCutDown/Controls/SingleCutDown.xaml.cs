using NiceCutDown.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;




namespace NiceCutDown.Controls
{
    public sealed partial class SingleCutDown : UserControl
    {
        public CountDownTime CountDays
        {
            get { return (CountDownTime)GetValue(CountDaysProperty); }
            set { SetValue(CountDaysProperty, value); }
        }

        public static readonly DependencyProperty CountDaysProperty =
        DependencyProperty.Register("CountDays", typeof(CountDownTime), typeof(SingleCutDown), new PropertyMetadata(null, DaysChanged));

        public Color CountBackground
        {
            get { return (Color)GetValue(CountBackgroundProperty); }
            set { SetValue(CountBackgroundProperty, value); }
        }
        public static readonly DependencyProperty CountBackgroundProperty =
        DependencyProperty.Register("CountBackground", typeof(Color), typeof(SingleCutDown), new PropertyMetadata(Colors.Red, BackgroundChanged));

        public string CountTitle
        {
            get { return (string)GetValue(CountTitleProperty); }
            set { SetValue(CountTitleProperty, value); }
        }
        public static readonly DependencyProperty CountTitleProperty =
        DependencyProperty.Register("CountTitle", typeof(string), typeof(SingleCutDown), new PropertyMetadata("",TitleChanged));

        

        public SingleCutDown()
        {
            this.InitializeComponent();
        }




        private static void DaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            SingleCutDown singleCutDown = d as SingleCutDown;

            if (singleCutDown.ActualWidth == 0) return;


            ChangingAnimation((CountDownTime)d.GetValue(CountDaysProperty), singleCutDown.countTitle.Text, singleCutDown);
            

        }

        private static void BackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SingleCutDown).backgroundGrid.Background = new SolidColorBrush( (Color)d.GetValue(CountBackgroundProperty));
        }

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SingleCutDown singleCutDown = d as SingleCutDown;

            if (singleCutDown.ActualWidth == 0) return;


            int time;
            if(singleCutDown.countDays.Text.Contains("+"))
            {
                time = Convert.ToInt32(singleCutDown.countDays.Text.Substring(0, singleCutDown.countDays.Text.Length - 1));
            }
            else
            {
                time = Convert.ToInt32(singleCutDown.countDays.Text);
            }
            
            ChangingAnimation(time, d.GetValue(CountTitleProperty) as string, singleCutDown);
        }

        private async static Task ChangingAnimation(int Time, string Title, SingleCutDown singleCutDown)
        {
            if (double.IsInfinity(singleCutDown.countDaysColumn.ActualWidth) || singleCutDown.ActualWidth == 0) return;


            if (Time <= 0)
            {
                Time = 0 - Time;
                singleCutDown.countDays.Text = Time.ToString();
            }
            else
            {
                singleCutDown.countDays.Text = Time.ToString() + "+";
            }
            singleCutDown.countTitle.Text = Title;

            Random r = new Random((int)DateTime.Now.Ticks);
            await Task.Delay(r.Next(20, 50));

            double Old = ((CompositeTransform)singleCutDown.backgroundGrid.RenderTransform).ScaleX;
            double New = ((double)(Time > 365 ? 365 : Time)) / 365;


            double leftWidth = New * singleCutDown.ActualWidth - singleCutDown.countDaysColumn.ActualWidth;
            if (leftWidth < 0) leftWidth = 0;

            double rightWidth = (1 - New) * singleCutDown.ActualWidth;

            singleCutDown.countProgress.Width = new GridLength(leftWidth);


            if (((singleCutDown.countTitle.ActualWidth + 8) < rightWidth) || rightWidth > leftWidth)
            {
                Grid.SetColumn(singleCutDown.countTitle, 2);
                singleCutDown.countTitle.MaxWidth = rightWidth;
                singleCutDown.countTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
                singleCutDown.countTitle.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                Grid.SetColumn(singleCutDown.countTitle, 1);
                singleCutDown.countTitle.MaxWidth = leftWidth;
                singleCutDown.countTitle.Foreground = new SolidColorBrush(Colors.White);
                singleCutDown.countTitle.HorizontalAlignment = HorizontalAlignment.Right;
            }


            double to = New;
            if (to < (singleCutDown.countDaysColumn.ActualWidth / singleCutDown.ActualWidth)) to = (singleCutDown.countDaysColumn.ActualWidth / singleCutDown.ActualWidth);
            if (double.IsInfinity(to) || double.IsNaN(to)) to = 0;

            //double to = New + singleCutDown.countDaysColumn.ActualWidth / singleCutDown.ActualWidth;

                singleCutDown.doub.From = Old;
                singleCutDown.doub.To = to;
                singleCutDown.ani.Begin();
        }
        private async static Task ChangingAnimation(CountDownTime cdt, string Title ,SingleCutDown singleCutDown)
        {
            if (double.IsInfinity(singleCutDown.countDaysColumn.ActualWidth)||singleCutDown.ActualWidth==0) return;
            if (cdt == null) return;

            int Time = TimeHelper.TotalDays(cdt);

            await ChangingAnimation(Time, Title, singleCutDown);


        }




        private void Changing()
        {
            var singleCutDown = this;
            if (double.IsInfinity(singleCutDown.countDaysColumn.ActualWidth) || singleCutDown.ActualWidth == 0) return;

            var cdt = CountDays;
            int Time = TimeHelper.TotalDays(cdt);
            if (Time <= 0)
            {
                Time = 0 - Time;
                singleCutDown.countDays.Text = Time.ToString();
            }
            else
            {
                singleCutDown.countDays.Text = Time.ToString() + "+";
            }
            singleCutDown.countTitle.Text = CountTitle;


            double New = ((double)(Time > 365 ? 365 : Time)) / 365;


            double leftWidth = New * singleCutDown.ActualWidth - singleCutDown.countDaysColumn.ActualWidth;
            if (leftWidth < 0) leftWidth = 0;

            double rightWidth = (1 - New) * singleCutDown.ActualWidth;

            singleCutDown.countProgress.Width = new GridLength(leftWidth);


            if (((singleCutDown.countTitle.ActualWidth + 10) < rightWidth) || rightWidth > leftWidth)
            {
                Grid.SetColumn(singleCutDown.countTitle, 2);
                singleCutDown.countTitle.MaxWidth = rightWidth;
                singleCutDown.countTitle.Foreground = new SolidColorBrush(Colors.LightGray);
                singleCutDown.countTitle.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                Grid.SetColumn(singleCutDown.countTitle, 1);
                singleCutDown.countTitle.MaxWidth = leftWidth;
                singleCutDown.countTitle.Foreground = new SolidColorBrush(Colors.White);
                singleCutDown.countTitle.HorizontalAlignment = HorizontalAlignment.Right;
            }


            double to = New;
            if (to < (singleCutDown.countDaysColumn.ActualWidth / singleCutDown.ActualWidth)) to = (singleCutDown.countDaysColumn.ActualWidth / singleCutDown.ActualWidth);
            if (double.IsInfinity(to) || double.IsNaN(to)) to = 0;

            ((CompositeTransform)singleCutDown.backgroundGrid.RenderTransform).ScaleX = to;


        }



        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(50);
            backgroundGrid.Background = new SolidColorBrush(CountBackground);
            await ChangingAnimation(CountDays, CountTitle, sender as SingleCutDown);
            ((SingleCutDown)sender).SizeChanged += SingleCutDown_SizeChanged;
        }

        private void SingleCutDown_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Changing();

        }

    }
}
