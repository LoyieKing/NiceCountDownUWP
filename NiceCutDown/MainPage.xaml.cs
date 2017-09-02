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
using System.Collections.ObjectModel;
using Windows.UI;
using NiceCutDown.Controls;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using NiceCutDown;
using Windows.Phone.UI.Input;
using NiceCutDown.Tools;

namespace NiceCutDown
{



    public sealed partial class MainPage : Page
    {
        ObservableCollection<CountDown> occd = new ObservableCollection<CountDown>();
        Tools.LocateCutDownHelper lcdh;
        bool inEditMode = false;

        public MainPage()
        {
            this.InitializeComponent();
            
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            
            LoadData();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            ExitEditMode();
        }

        private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inEditMode)
            {
                if (listview.SelectedIndex != -1)
                {
                    this.Frame.Navigate(typeof(CountDownPage), listview.SelectedIndex);
                    listview.SelectedIndex = -1;
                }

            }
            else if(listview.SelectedItems.Count>0)
            {
                BottomAppBar.Visibility = Visibility.Visible;
            }
            else
            {
                BottomAppBar.Visibility = Visibility.Collapsed;
            }


        }

        private void AddCutDownButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditPage));
        }

        private async void LoadData()
        {
            lcdh = new Tools.LocateCutDownHelper();
            await lcdh.Read();
            occd = new ObservableCollection<CountDown>(lcdh.CountDowns);
            listview.ItemsSource = new ObservableCollection<CountDown>();
            listview.ItemsSource = occd;
            if (occd.Count == 0)
            {
                mainGrid.Visibility = Visibility.Collapsed;
                emptyGrid.Visibility = Visibility.Visible;
                editButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                mainGrid.Visibility = Visibility.Visible;
                emptyGrid.Visibility = Visibility.Collapsed;
                editButton.Visibility = Visibility.Visible;
            }
        }


        private void EnterEditMode()
        {
            editButton.Visibility = Visibility.Collapsed;
            addButton.Visibility = Visibility.Collapsed;
            listview.SelectionMode = ListViewSelectionMode.Multiple;
            TopAppBar.Visibility = Visibility.Visible;
            settingButton.Visibility = Visibility.Collapsed;
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            inEditMode = true; 
        }


        private void ExitEditMode()
        {
            editButton.Visibility = Visibility.Visible;
            addButton.Visibility = Visibility.Visible;
            listview.SelectionMode = ListViewSelectionMode.Single;
            TopAppBar.Visibility = Visibility.Collapsed;
            settingButton.Visibility = Visibility.Visible;
            if (occd.Count == 0)
            {
                mainGrid.Visibility = Visibility.Collapsed;
                emptyGrid.Visibility = Visibility.Visible;
                editButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                mainGrid.Visibility = Visibility.Visible;
                emptyGrid.Visibility = Visibility.Collapsed;
                editButton.Visibility = Visibility.Visible;
            }
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
            inEditMode = false;
        }


        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            EnterEditMode();
        }

        

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ExitEditMode();
        }

        private async void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            lcdh.ReplaceAll(occd);
            await lcdh.Save();
            await Task.Delay(50);
            LoadData();
            ExitEditMode();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            List<CountDown> lcd = new List<CountDown>();
            foreach(CountDown cd in listview.SelectedItems)
            {
                lcd.Add(cd);
            }

            foreach (CountDown cd in lcd)
            {
                occd.Remove(cd);
            }
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            List<CountDown> lcd = new List<CountDown>();
            foreach (CountDown cd in listview.SelectedItems)
            {
                lcd.Add(cd);
            }

            foreach (CountDown cd in lcd)
            {
                int index = occd.IndexOf(cd);
                if (index == 0) continue;
                occd.Move(index, index - 1);
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            List<CountDown> lcd = new List<CountDown>();
            foreach (CountDown cd in listview.SelectedItems)
            {
                lcd.Add(cd);
            }

            for (int i = lcd.Count-1;i>-1;i--)
            {
                int index = occd.IndexOf(lcd[i] as CountDown);
                if (index == occd.Count-1) continue;
                occd.Move(index, index + 1);
            }
        }

        private void settingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingPage));
        }
    }





}
