using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.Devices.Notification;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NiceCutDown.Controls
{
    public class PasswordCompleteEventArgs : EventArgs
    {
        string password;
        public string Password { get { return password; } }

        public PasswordCompleteEventArgs(string Password)
        {
            password = Password;
        }
    }

    public sealed partial class PasswordControl : UserControl
    {
        public event TypedEventHandler<PasswordControl, PasswordCompleteEventArgs> PasswordComplete;

        List<string> pins = new List<string>();

        bool inShake = false;


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register("Message", typeof(string), typeof(PasswordControl), new PropertyMetadata("", MessageChanged));

        private static void MessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            PasswordControl passwordControl = (PasswordControl)d;
            passwordControl.dokf.Value = (string)e.NewValue;
            passwordControl.messageStoryBoard.Begin();
        }

        public PasswordControl()
        {
            this.InitializeComponent();
        }


        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {

            /*    
             *    if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification.VibrationDevice"))
                        {
                            VibrationDevice vibrationDevice = VibrationDevice.GetDefault();
                            vibrationDevice.Vibrate(new TimeSpan(0, 0, 0, 0, 120));
                        }*/

            if (inShake)
            {
                ShakeStoryBoard.Stop();
                RemoveAll();
                deleteButton.Click += DeleteButton_Click;
                inShake = false;
            }


            Button button = (Button)sender;
            AddNumber(button.Content as string);
        }


        private void AddNumber(string pin)
        {
            if (pins.Count < 5)
            {
                pins.Add(pin);

                switch (pins.Count)
                {
                    case 1: ell1StoryBoard.Begin(); break;
                    case 2: ell2StoryBoard.Begin(); break;
                    case 3: ell3StoryBoard.Begin(); break;
                    case 4: ell4StoryBoard.Begin(); break;
                }

            }
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Ellipse backgroundEllipse = NiceCutDown.Controls.Helper.FindVisualChild<Ellipse>(button, "backgroundEllipse");
            DoubleAnimationUsingKeyFrames daukf = new DoubleAnimationUsingKeyFrames();
            daukf.KeyFrames.Add(new LinearDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 0)), Value = 1 });
            daukf.KeyFrames.Add(new LinearDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 400)), Value = 1 });
            daukf.KeyFrames.Add(new LinearDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 700)), Value = 0 });
            Storyboard.SetTarget(daukf, backgroundEllipse);
            Storyboard.SetTargetProperty(daukf, "Opacity");


            DoubleAnimation da1 = new DoubleAnimation();
            da1.From = 0;
            da1.To = 1;
            da1.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
            da1.EasingFunction = new BackEase() { Amplitude = 0.3, EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(da1, backgroundEllipse);
            Storyboard.SetTargetProperty(da1, "(Ellipse.RenderTransform).(CompositeTransform.ScaleX)");

            DoubleAnimation da2 = new DoubleAnimation();
            da2.From = 0;
            da2.To = 1;
            da2.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
            da2.EasingFunction = new BackEase() { Amplitude = 0.3, EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(da2, backgroundEllipse);
            Storyboard.SetTargetProperty(da2, "(Ellipse.RenderTransform).(CompositeTransform.ScaleY)");

            Storyboard sb = new Storyboard();
            sb.Children.Add(daukf);
            sb.Children.Add(da1);
            sb.Children.Add(da2);
            button.Click += (s, rea) =>
            {
                sb.Begin();
            };

        }

        private void UpdateEllipses()
        {
            ell1StoryBoard.Stop();
            ell2StoryBoard.Stop();
            ell3StoryBoard.Stop();
            ell4StoryBoard.Stop();
            switch (pins.Count)
            {
                case 0:
                    ((CompositeTransform)ell1.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell1.RenderTransform).ScaleY = 1;
                    ((CompositeTransform)ell2.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell2.RenderTransform).ScaleY = 1;
                    ((CompositeTransform)ell3.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell3.RenderTransform).ScaleY = 1;
                    ((CompositeTransform)ell4.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell4.RenderTransform).ScaleY = 1; break;
                case 1:
                    ((CompositeTransform)ell1.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell1.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell2.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell2.RenderTransform).ScaleY = 1;
                    ((CompositeTransform)ell3.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell3.RenderTransform).ScaleY = 1;
                    ((CompositeTransform)ell4.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell4.RenderTransform).ScaleY = 1; break;
                case 2:
                    ((CompositeTransform)ell1.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell1.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell2.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell2.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell3.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell3.RenderTransform).ScaleY = 1;
                    ((CompositeTransform)ell4.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell4.RenderTransform).ScaleY = 1; break;
                case 3:
                    ((CompositeTransform)ell1.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell1.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell2.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell2.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell3.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell3.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell4.RenderTransform).ScaleX = 1;
                    ((CompositeTransform)ell4.RenderTransform).ScaleY = 1; break;
                case 4:
                    ((CompositeTransform)ell1.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell1.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell2.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell2.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell3.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell3.RenderTransform).ScaleY = 2;
                    ((CompositeTransform)ell4.RenderTransform).ScaleX = 2;
                    ((CompositeTransform)ell4.RenderTransform).ScaleY = 2; break;
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (pins.Count > 0)
            {
                pins.RemoveAt(pins.Count - 1);
                UpdateEllipses();
            }
        }

        public void RemoveAll()
        {
            pins = new List<string>();
            UpdateEllipses();
        }

        public void Shake()
        {
            inShake = true;
            ShakeStoryBoard.Begin();
            deleteButton.Click -= DeleteButton_Click;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            messageTextBlock.Text = Message;
        }

        private void ShakeStoryBoard_Completed(object sender, object e)
        {
            RemoveAll();
            deleteButton.Click += DeleteButton_Click;
            inShake = false;
        }

        private void ell4StoryBoard_Completed(object sender, object e)
        {
            string password = pins[0] + pins[1] + pins[2] + pins[3];

            if (PasswordComplete != null)
            {
                PasswordComplete.Invoke(this, new PasswordCompleteEventArgs(password));
            }
        }
    }
}
