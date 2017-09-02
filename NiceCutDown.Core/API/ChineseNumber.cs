using System;

namespace ChineseNumber
{
    public class ChineseNumberHelper
    {
        private static string nStr1 = "日一二三四五六七八九";
        private static string nStr2 = "初十廿卅";
        private static string[] _monthString =
                {
                    "出错","正月","二月","三月","四月","五月","六月","七月","八月","九月","十月","十一月","腊月"
                };

        public static string PureConvert(int number)
        {
            string tmp = number.ToString();
            tmp = tmp.Replace("-", "负");
            tmp = tmp.Replace("0", "〇");
            tmp = tmp.Replace("1", "一");
            tmp = tmp.Replace("2", "二");
            tmp = tmp.Replace("3", "三");
            tmp = tmp.Replace("4", "四");
            tmp = tmp.Replace("5", "五");
            tmp = tmp.Replace("6", "六");
            tmp = tmp.Replace("7", "七");
            tmp = tmp.Replace("8", "八");
            tmp = tmp.Replace("9", "九");
            return tmp;
        }

        public static string PureConvert(string number)
        {
            number = number.Replace("-", "负");
            number = number.Replace("0", "〇");
            number = number.Replace("1", "一");
            number = number.Replace("2", "二");
            number = number.Replace("3", "三");
            number = number.Replace("4", "四");
            number = number.Replace("5", "五");
            number = number.Replace("6", "六");
            number = number.Replace("7", "七");
            number = number.Replace("8", "八");
            number = number.Replace("9", "九");
            return number;
        }

        public static string XXConvert(int number)
        {
            if(number.ToString().Length!=2)
            {
                return PureConvert(number);
            }
            else
            {
                int one = number % 10;
                int ten = (number - one) / 10;

                string tmp = PureConvert(ten) + "十" + PureConvert(one);

                tmp = tmp.Replace("一十", "十");
                tmp = tmp.Replace("十〇", "十");
                return tmp;
            }

        }

        public static string DateConvert(DateTime Time)
        {
            return YearConvert(Time.Year) + MonthConvert(Time.Month) + DayConvert(Time.Day);
        }

        public static string YearConvert(int year)
        {
            return YearConvert(year, true);
        }

        public static string YearConvert(int Year , bool tail)
        {
            string year = PureConvert(Year);
            if (tail) return year + "年";
            else return year;
        }

        public static string MonthConvert(int Month )
        {
            return _monthString[Month];

        }
        public static string DayConvert(int Day)
        {
            switch (Day)
            {
                case 0:
                    return "";
                case 10:
                    return "初十";
                case 20:
                    return "二十";
                case 30:
                    return "三十";
                default:
                    return nStr2[(int)(Day / 10)].ToString() + nStr1[Day % 10].ToString();

            }
        }
    }
}