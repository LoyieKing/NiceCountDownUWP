using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;

namespace NiceCutDown.Tools
{
    public class SettingsManager
    {
        static ApplicationDataContainer adc = ApplicationData.Current.RoamingSettings;

        public static int Reminder
        {
            get
            {
                if (adc.Values.ContainsKey("Reminder"))
                {
                    return (int)adc.Values["Reminder"];
                }
                else
                {
                    return 2;
                }
            }
            set
            {
                adc.Values["Reminder"] = value;
            }
        }


        public static bool Ganzhi
        {
            get
            {
                if (adc.Values.ContainsKey("Ganzhi"))
                {
                    return (bool)adc.Values["Ganzhi"];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                adc.Values["Ganzhi"] = value;
            }
        }

        public static string Version
        {
            get
            {
                return Package.Current.Id.Version.Major.ToString() + "." + Package.Current.Id.Version.Minor.ToString() + "." + Package.Current.Id.Version.Build.ToString();
            }
        }

        public static bool FisrtRunThisVersion
        {
            get
            {
                if (adc.Values.ContainsKey("frtv"))
                {
                    if((string)adc.Values["frtv"]==Version)
                    {
                        return false;
                    }
                    else
                    {
                        adc.Values["frtv"] = Version;
                        return true;
                    }
                }
                else
                {
                    adc.Values["frtv"] = Version;
                    return true;
                }
            }
        }

        public static bool FisrtRun
        {
            get
            {
                if(adc.Values.ContainsKey("fr"))
                {
                    if ((string)adc.Values["fr"] == "LoyieKing")
                    {
                        return false;
                    }
                    else
                    {
                        adc.Values["fr"] = "LoyieKing";
                        return true; ;
                    }
                }
                else
                {
                    adc.Values["fr"] = "LoyieKing";
                    return true;
                }

                //在这里一样采用和上面类似的记录措施是为了将来有可能的重置第一次运行状况。
                //到时候只要将fr的字符串改成别的就行了。
            }
        }
    }
}