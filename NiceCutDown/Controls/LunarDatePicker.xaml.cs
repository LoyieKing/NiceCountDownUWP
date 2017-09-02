using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YinYang;


namespace NiceCutDown.Controls
{
    public sealed partial class LunarDatePicker : UserControl
    {
        private List<string> year;
        private List<string> month;
        private List<string> day;



        ChineseCalendar chineseCalendar;

        public ChineseCalendar ChineseCalendar
        {
            get
            {
                return chineseCalendar;
            }
            set
            {
                setIndex(value);
            }
        }



        public LunarDatePicker()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            year = new List<string>();
            for(int i =(DateTime.Now.Year-100);i<=2099;i++)
            {
                year.Add(i.ToString()+"年");
            }
            yearComboBox.ItemsSource = year;


            yearComboBox.SelectionChanged += yearComboBox_SelectionChanged;
            monthComboBox.SelectionChanged += monthComboBox_SelectionChanged;
            dayComboBox.SelectionChanged += dayComboBox_SelectionChanged;
            yearComboBox.SelectedIndex = new ChineseCalendar(DateTime.Now).ChineseYear - (DateTime.Now.Year-100);

        }


        private async void setIndex(ChineseCalendar value)
        {
            while(yearComboBox.Items==null||yearComboBox.Items.Count==0||monthComboBox.Items==null||monthComboBox.Items.Count==0||dayComboBox.Items==null||dayComboBox.Items.Count==0)
            {
                await Task.Delay(5);
            }
            yearComboBox.SelectedIndex = value.ChineseYear-DateTime.Now.Year + 100;
            if(value.IsChineseLeapMonth)
            {
                monthComboBox.SelectedIndex = value.ChineseMonth + 1;
            }
            else
            {
                monthComboBox.SelectedIndex = value.ChineseMonth;
            }

            dayComboBox.SelectedIndex = value.ChineseDay-1;
        }

        private void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool fisrt = false;
            if(month==null||month.Count==0)
            {
                fisrt = true;
            }

            month = new List<string>();
            month.Add("正月");
            month.Add("二月");
            month.Add("三月");
            month.Add("四月");
            month.Add("五月");
            month.Add("六月");
            month.Add("七月");
            month.Add("八月");
            month.Add("九月");
            month.Add("十月");
            month.Add("冬月");
            month.Add("腊月");

            int leapMonth = ChineseCalendar.GetChineseLeapMonth((DateTime.Now.Year - 100) + yearComboBox.SelectedIndex);
            if(leapMonth!=0)
            {
                month.Insert(leapMonth, "闰" + ChineseNumber.ChineseNumberHelper.MonthConvert(leapMonth));
            }
            

            monthComboBox.ItemsSource = month;
            if(fisrt)
            {
                monthComboBox.SelectedIndex = new ChineseCalendar(DateTime.Now).ChineseMonth - 1;
            }
            else
            {
                monthComboBox.SelectedIndex = 0;
            }
        }

        private void monthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (monthComboBox.SelectedIndex == -1) return;

            bool fisrt = false;
            if (day == null || day.Count == 0)
            {
                fisrt = true;
            }
            day = new List<string>();
            int days;

            int year = (DateTime.Now.Year - 100) + yearComboBox.SelectedIndex;
            int leapMonth = ChineseCalendar.GetChineseLeapMonth(year);
            int selectMonth = monthComboBox.SelectedIndex + 1;
            if (selectMonth - 1 < leapMonth || leapMonth == 0)
            {
                days = ChineseCalendar.GetChineseMonthDays(year, selectMonth);
                chineseCalendar = new ChineseCalendar(year, selectMonth, 1, false);
            }
            else if (selectMonth - 1 == leapMonth)
            {
                days = ChineseCalendar.GetChineseLeapMonthDays(year);
                chineseCalendar = new ChineseCalendar(year, selectMonth - 1, 1, true);
            }
            else
            {
                days = ChineseCalendar.GetChineseMonthDays(year, selectMonth - 1);
                chineseCalendar = new ChineseCalendar(year, selectMonth - 1, 1, false);
            }



            for (int i = 1; i <= days; i++)
            {
                day.Add(ChineseNumber.ChineseNumberHelper.DayConvert(i));
            }
            dayComboBox.ItemsSource = day;

            if (fisrt)
            {
                dayComboBox.SelectedIndex = new ChineseCalendar(DateTime.Now).ChineseDay - 1;
            }
            else
            {
                dayComboBox.SelectedIndex = 0;
            }

            
        }

        private void dayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int cy = chineseCalendar.ChineseYear;
            cy = cy == 0 ? 1 : cy;
            int cm = chineseCalendar.ChineseMonth;
            cm = cm == 0 ? 1 : cm;
            int cd = dayComboBox.SelectedIndex + 1;
            cd = cd == 0 ? 1 : cd;
            chineseCalendar = new ChineseCalendar(cy, cm, cd, chineseCalendar.IsChineseLeapMonth);
        }
    }
}
