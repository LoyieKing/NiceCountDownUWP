using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace NiceCutDown.Controls
{
    public sealed partial class PictureBackground : UserControl
    {
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register("Image", typeof(ImageSource), typeof(PictureBackground), new PropertyMetadata(new BitmapImage(), ImageChanged));



        Accelerometer accelerometer;
        AccelerometerReading accelerometerReading;
        DispatcherTimer timer;

        bool canLoad = false;
        bool inChange = false;
        bool inLoad = false;

        public PictureBackground()
        {
            this.InitializeComponent();
        }

        private static void ImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PictureBackground pictureBackground = (PictureBackground)d;
            pictureBackground.BackgroundImage.Opacity = 0;
            pictureBackground.BackgroundImage.Source = (ImageSource)e.NewValue;
        }



        private void LoadAccelerometer()
        {
            if (!canLoad) return;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += Timer_Tick;
            accelerometer.ReportInterval = accelerometer.MinimumReportInterval * 2;
            accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            accelerometerReading = accelerometer.GetCurrentReading();
            timer.Start();
        }

        private void DisposeAccelerometer()
        {
            try
            {
                timer.Stop();
                timer = new DispatcherTimer();
                accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            }
            catch { }
        }


        private double ft(double number)
        {
            double t = number;
            if (t > 1) t = 1;
            if (t < -1) t = -1;
            double f = 1 - Math.Sqrt((1 - (t * t)));

            if (t >= 0)
                return f;
            else
                return 0 - f;
        }

        private void Timer_Tick(object sender, object e)
        {
            double left = Canvas.GetLeft(BackgroundImage) + ((accelerometerReading.AccelerationX) * 2.5);
            if (left > 0) left = 0;
            if (left < 0 - BackgroundImage.ActualWidth + ActualWidth) left = 0 - BackgroundImage.ActualWidth + ActualWidth;
            Canvas.SetLeft(BackgroundImage,left );

        }

        private void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            accelerometerReading = args.Reading;
        }


        private void PictureBackground_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BackgroundImage.Height = e.NewSize.Height;
            Canvas.SetLeft(BackgroundImage, 0 - (BackgroundImage.ActualWidth - e.NewSize.Width) / 2);
            if (accelerometer != null && BackgroundImage.ActualWidth > e.NewSize.Width && canLoad == false)
            {
                canLoad = true;
                LoadAccelerometer();
            }
            if (BackgroundImage.ActualWidth <= e.NewSize.Width)
            {
                canLoad = false;
                DisposeAccelerometer();
            }
        }


        private async void BackgroundImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            BackgroundImage.Height = ActualHeight;
            await Task.Delay(30);
            while(double.IsInfinity(BackgroundImage.ActualWidth)||double.IsNaN(BackgroundImage.ActualWidth)||BackgroundImage.ActualWidth==0||double.IsInfinity(ActualWidth)||double.IsNaN(ActualWidth)||ActualWidth==0)
            {
                await Task.Delay(5);
            }
            Canvas.SetLeft(BackgroundImage, 0 - (BackgroundImage.ActualWidth - ActualWidth) / 2);
            await Task.Delay(10);
            try
            {
                showImage.Begin();
            }
            catch
            {
                BackgroundImage.Opacity = 1;
            }

            accelerometer = Accelerometer.GetDefault();
            if (accelerometer != null && BackgroundImage.ActualWidth > ActualWidth)
            {
                canLoad = true;
                if(!inLoad)
                {
                    LoadAccelerometer();
                    inLoad = true;
                }
                
            }
            if(!inChange)
            {
                SizeChanged += PictureBackground_SizeChanged;
                inChange = true;
            }
            
        }

    }
}
