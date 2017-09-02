using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using NiceCutDown.Resources;
using NiceCutDown.Tools;
using NiceCutDown;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace BackgroundTask
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            LocateCutDownHelper lcdh = new LocateCutDownHelper();
            await lcdh.Read();
            if(lcdh.CountDowns.Count==0)
            {
                deferral.Complete();
                return;
            }
            foreach(CountDown cd in lcdh.CountDowns)
            {
                int timeDiff = TimeHelper.TotalDays(cd.Time);
                if (timeDiff <= SettingsManager.Reminder && timeDiff >= 0)
                {
                    if (cd.InRemind != timeDiff)
                    {
                        string time;
                        switch (timeDiff)
                        {
                            case 0: time = "今天"; break;
                            case 1: time = "明天"; break;
                            case 2: time = "后天"; break;
                            default: time = "要到了"; break;
                        }

                        NotificationHelper.ShowToastNotification(time + "," + cd.Title, lcdh.CountDowns.IndexOf(cd).ToString());
                        cd.InRemind = timeDiff;
                    }



                }
                else
                {
                    cd.InRemind = -1;
                }
            }
            await lcdh.Save();

            NotificationHelper.UpdateTitleNotification(lcdh.CountDowns);

            deferral.Complete();
        }
    }
}
