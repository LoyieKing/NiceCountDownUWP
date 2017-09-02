using System;
using System.Collections.Generic;
using Windows.UI;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace NiceCutDown
{
    namespace Resources
    {
        public class ColorManager
        {
            public async static Task<List<SolidColorBrush>> GetColorList()
            {
                // 读取文件的文本信息
                string text;
                // 通过Uri获取本地文件
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/colors/colors"));
                // 打开文件获取文件的数据流
                IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
                // 使用StreamReader读取文件的内容，需要将IRandomAccessStream对象转化为Stream对象来初始化StreamReader对象
                using (StreamReader streamReader = new StreamReader(accessStream.AsStreamForRead((int)accessStream.Size)))
                {
                    text = streamReader.ReadToEnd();
                }

                string[] hexs = text.Split('#');

                List<SolidColorBrush> colors = new List<SolidColorBrush>();

                foreach (string hex in hexs)
                {
                    byte[] colorall = Tools.StringHeleper.strToToHexByte(hex);
                    colors.Add(new SolidColorBrush(Color.FromArgb(255, colorall[0], colorall[1], colorall[2])));
                }

                return colors;
            }
        }

        public class PicturesManager
        {
            public static async Task<string> Save(StorageFile File)
            {
                if (File != null)
                {
                    IStorageFolder folder = ApplicationData.Current.LocalFolder;
                    string name = DateTime.Now.Ticks.ToString();

                    await File.CopyAsync(folder,name, NameCollisionOption.ReplaceExisting);

                    return "ms-appdata:///local/" + name;
                }
                return "";
            }

            public static async Task<BitmapImage> Get(string path)
            {
                try
                {
                    var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
                    // 打开文件获取文件的数据流
                    IRandomAccessStream accessStream = await storageFile.OpenReadAsync();
                    BitmapImage bi = new BitmapImage();
                    bi.SetSource(accessStream);
                    accessStream.Dispose();
                    return bi;
                }
                catch
                {
                    return new BitmapImage();
                }

            }
        }
    }

}