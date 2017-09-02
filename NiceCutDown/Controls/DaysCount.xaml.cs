using NiceCutDown.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
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
    public sealed partial class DaysCount : UserControl
    {
        public CountDownTime CountDays
        {
            get { return (CountDownTime)GetValue(CountDaysProperty); }
            set { SetValue(CountDaysProperty, value); }
        }

        public static readonly DependencyProperty CountDaysProperty =
        DependencyProperty.Register("CountDays", typeof(CountDownTime), typeof(DaysCount), new PropertyMetadata(new CountDownTime(), DaysChanged));

        public DaysCount()
        {
            this.InitializeComponent();
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var stackPanel = ((StackPanel)sender);
            if (double.IsInfinity(stackPanel.ActualWidth) || double.IsNaN(stackPanel.ActualWidth) || stackPanel.ActualWidth == 0) return;//判断StackPanel宽度无限大、不存在、为零的情况，返回

            if(((Grid)((StackPanel)stackPanel.Parent).Parent).ActualWidth<stackPanel.ActualWidth)
            {
                stackPanel.Orientation = Orientation.Vertical;
            }
            else
            {
                stackPanel.Orientation = Orientation.Horizontal;
            }
        }

        private static void DaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetType() != typeof(DaysCount)) return;
            DaysCount daysCount = (DaysCount)d;
            if (e.NewValue == null) return;

            CountDownTime cdt = (CountDownTime)e.NewValue;




            (int Year, int Month, int Day) tuple;
            int totalDays = Math.Abs(TimeHelper.TotalDays(cdt));

            if (cdt.Repeat)
            {
                if(cdt.Lunar)
                {
                    int diff;

                    ChineseCalendar cc = new ChineseCalendar(cdt.Time);


                    int nextYear = (new ChineseCalendar(DateTime.Now)).ChineseYear - 1; ;
                    loop: nextYear++;
                    ChineseCalendar tmp;
                    try
                    {
                        tmp = new ChineseCalendar(nextYear, cc.ChineseMonth, cc.ChineseDay, cc.IsChineseLeapMonth);
                    }
                    catch
                    {
                        goto loop;
                    }
                    diff = new TimeSpan(tmp.Date.Ticks).Days - new TimeSpan(DateTime.Now.Ticks).Days;
                    if (diff < 0)
                    {
                        goto loop;
                    }

                    tuple = TimeDiff(DateTime.Now, tmp.Date);

                }
                else
                {
                    //判断是否为闰年二月29日，若是则需要详尽计算
                    if (DateTime.IsLeapYear(cdt.Time.Year) && cdt.Time.Month == 2 && cdt.Time.Day == 29)
                    {
                        //是闰年
                        int nextLeapYear = cdt.Time.Year;

                        //循环加4，知道今年开始的下个4整数倍年为止
                        while (nextLeapYear < DateTime.Now.Year)
                        {
                            nextLeapYear += 4;
                        }
                        //判断4整数年是否为闰年，若不是，继续加4，直到是为止
                        while (!DateTime.IsLeapYear(nextLeapYear))
                        {
                            nextLeapYear += 4;
                        }

                        DateTime time = new DateTime(nextLeapYear, 2, 29);
                        tuple = TimeDiff(DateTime.Now, time);

                    }
                    else
                    {
                        DateTime time = new DateTime(DateTime.Now.Year, cdt.Time.Month, cdt.Time.Day);
                        if (time >= DateTime.Now)
                        {
                            tuple = TimeDiff(DateTime.Now, time);
                        }
                        else
                        {
                            time = time.AddYears(1);
                            tuple = TimeDiff(DateTime.Now, time);
                        }

                    }
                }
            }
            else
            {
                if (cdt.Time < DateTime.Now)
                {
                    tuple = TimeDiff(cdt.Time, DateTime.Now);
                }
                else
                {
                    tuple = TimeDiff(DateTime.Now, cdt.Time);
                }
            }


            




            daysCount.years_year.Text = tuple.Year.ToString();
            daysCount.months_year.Text = tuple.Month.ToString();
            daysCount.days_year.Text = tuple.Day.ToString();

            daysCount.months_month.Text = ((tuple.Year * 12) + tuple.Month).ToString();
            daysCount.days_month.Text = tuple.Day.ToString();

            daysCount.days_day.Text = totalDays.ToString();


            //daysCount.mainFlipView.Items.Clear();
            if(totalDays==0)
            {
                daysCount.mainFlipView.Items.RemoveAt(1);
                daysCount.mainFlipView.Items.RemoveAt(1);
            }
            else
            {
                if (daysCount.years_year.Text == "0")
                {
                    daysCount.mainFlipView.Items.Remove(daysCount.year);
                }
                else
                {
                    daysCount.year.Visibility = Visibility.Visible;
                    if (daysCount.months_year.Text == "0")
                    {
                        daysCount.MONTHS_year_sp.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        daysCount.MONTHS_year_sp.Visibility = Visibility.Visible;
                    }


                    if (daysCount.days_year.Text == "0")
                    {
                        daysCount.DAYS_year_sp.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        daysCount.DAYS_year_sp.Visibility = Visibility.Visible;
                    }
                }

                if(daysCount.months_month.Text=="0")
                {
                    daysCount.mainFlipView.Items.Remove(daysCount.month);
                }
                else
                {
                    daysCount.month.Visibility = Visibility.Visible;
                    if(daysCount.days_month.Text=="0")
                    {
                        daysCount.DAYS_month_sp.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        daysCount.DAYS_month_sp.Visibility = Visibility.Visible;
                    }

                }

            



            }






        }

        

        private static (int Year, int Month , int Day) TimeDiff(DateTime start , DateTime end)
        {
            TimeSpan ts = end-start;
            int year = 0, month = 0, day = ts.Days;
            int temp = 0;
            //四年一周期
            temp = day / (365 * 3 + 366);
            year = temp * 4;
            day = day % (365 * 3 + 366);
            //一年一周期
            temp = day / 365;
            year += temp;
            day = day % 365;
            //一月一周期，由于每月天数不一样，这个只能逐月推算了
            DateTime mi = start.AddYears(year);
            while ((mi = mi.AddMonths(1)) <= end)
            {
                month++;
            }


            if(mi.Date==end.Date)
            {
                day = 0;
                month++;
                if(month==12)
                {
                    month = 0;
                    year++;
                }
            }
            else
            {
                //逐天推算，直接相减
                day = (end - mi.AddMonths(-1)).Days;
            }
            return (year, month, day);
        }

        private void FlipViewButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_LayoutUpdated(object sender, object e)
        {
        }
    }
}
