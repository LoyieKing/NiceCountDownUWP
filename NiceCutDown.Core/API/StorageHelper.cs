using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NiceCutDown
{
    namespace Tools
    {
        public class LocateCutDownHelper
        {
            List<CountDown> llw;

            public List<CountDown> CountDowns { get { return llw; } }

            public async Task Read()
            {
                try
                {
                    llw = await StorageFileHelper.ReadAsync<List<CountDown>>("countdown.dat");
                    if (llw == null)
                    {
                        llw = new List<CountDown>();
                        await Save();
                    }
                }
                catch
                {
                    llw = new List<CountDown>();
                    await Save();
                }
            }

            public async Task Save()
            {
                await StorageFileHelper.WriteAsync<List<CountDown>>(llw, "countdown.dat");

            }


            public void Add(CountDown locateWiFi)
            {
                llw.Add(locateWiFi);
            }


            public void Remove(int index)
            {
                llw.RemoveAt(index);


            }

            public void Replace(int index, CountDown Item)
            {
                if (llw.Count > index && index > -1)
                {
                    llw[index] = Item;
                }
            }

            public void ReplaceAll(IList<CountDown> Items)
            {
                llw = new List<CountDown>(Items);
            }

        }



        public class StorageFileHelper
        {
            private const string FolderName = "Data";
            private static IStorageFolder DataFolder = null;
            private static async Task<IStorageFolder> GetDataFolder()
            {
                if (DataFolder == null)
                {
                    DataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
                }
                Debug.Write("DataFolderPath:" + DataFolder.Path + "\n");
                return DataFolder;
            }

            public static async Task WriteAsync<T>(T data, string filename)
            {
                IStorageFolder applicationFolder = await GetDataFolder();
                StorageFile file = await applicationFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream raStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(outStream.AsStreamForWrite(), data);
                        await outStream.FlushAsync();
                    }
                }

            }

            public static async Task<T> ReadAsync<T>(string filename)
            {
                T sessionState_ = default(T);
                IStorageFolder applicationFolder = await GetDataFolder();
                StorageFile file = await applicationFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                if (file == null)
                    return sessionState_;
                try
                {
                    using (IInputStream inStream = await file.OpenSequentialReadAsync())
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        sessionState_ = (T)serializer.ReadObject(inStream.AsStreamForRead());
                    }
                }
                catch (Exception error)
                {
                    Debug.Write("ReadError:" + error.Message + "\n");
                }

                return sessionState_;
            }





        }

        public class AppLocker
        {
            private const string FolderName = "Data";
            private static IStorageFolder DataFolder = null;
            private static async Task<IStorageFolder> GetDataFolder()
            {
                if (DataFolder == null)
                {
                    DataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
                }
                Debug.Write("DataFolderPath:" + DataFolder.Path + "\n");
                return DataFolder;
            }

            public static async Task<bool> HasMD5()
            {
                try
                {
                    IStorageFolder applicationFolder = await GetDataFolder();
                    var file = await applicationFolder.GetFileAsync("enkeydata");
                    Stream s = await file.OpenStreamForReadAsync();
                    if(s.Length==0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            public static async Task DeleteMD5()
            {
                IStorageFolder applicationFolder = await GetDataFolder();
                StorageFile file = await applicationFolder.CreateFileAsync("enkeydata", CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync();
            }

            public static async Task<byte[]> ReadMD5()
            {
                IStorageFolder applicationFolder = await GetDataFolder();
                StorageFile file = await applicationFolder.CreateFileAsync("enkeydata", CreationCollisionOption.OpenIfExists);
                if (file == null)
                    return null;

                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    using (Stream s = inStream.AsStreamForRead())
                    {
                        byte[] b = new byte[s.Length];
                        await s.ReadAsync(b, 0, (int)s.Length);
                        return b;
                    }
                }

            }

            public static async Task WriteMD5(string newKey)
            {
                byte[] newByte = StringHeleper.MD5(newKey);

                IStorageFolder applicationFolder = await GetDataFolder();
                StorageFile file = await applicationFolder.CreateFileAsync("enkeydata", CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream raStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
                    {
                        using (Stream s = outStream.AsStreamForWrite())
                        {
                            await s.WriteAsync(newByte, 0, newByte.Length);
                        }
                    }
                }

            }
        }

    }
}

