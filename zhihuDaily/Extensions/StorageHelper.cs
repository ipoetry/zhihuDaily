using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace zhihuDaily
{
    public class StorageHelper : IStorageHelper
    {
        private static readonly AsyncLock asyncLock = new AsyncLock();

        #region 提供单例的支持

        public static IStorageHelper Instance { get; private set; }

        public static object LockObject;

        static StorageHelper()
        {
            Instance = new StorageHelper();
            LockObject = new object();
        }

        private StorageHelper()
        {
        }

        #endregion


        public async Task<Stream> ReadFileAsync(string filePath)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        filePath = filePath.Trim('/').Replace("/", "\\");

                        var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);

                        using (Stream stream = await file.OpenStreamForReadAsync())
                        {
                            return CopyStream(stream);
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        throw new FileNotFoundException(string.Format("没有找到文件:{0}", filePath));
                    }
                });
            }
        }

        public async Task<string> ReadTextAsync(string filePath)
        {
            System.Diagnostics.Debug.WriteLine("Read Begin");
            var text = string.Empty;
            using (var stream = await ReadFileAsync(filePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    text = await reader.ReadToEndAsyncThread();
                    System.Diagnostics.Debug.WriteLine(filePath+"  Readed:::" +text);
                }
            }
            System.Diagnostics.Debug.WriteLine("Read Complete");

            return text;
        }

        public async Task WriteFileAsync(Stream stream, string filePath, bool replace = false)
        {
            await WriteFileAsync(ToBytes(stream), filePath, replace);
        }

        public async Task WriteFileAsync(byte[] data, string filePath, bool replace = false)
        {
            System.Diagnostics.Debug.WriteLine("Write Begin!");

            using (await asyncLock.LockAsync())
            {
                await await Task.Factory.StartNew(async () =>
                {
                    filePath = filePath.Trim('/').Replace("/", "\\");
                    if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(filePath)!=null)
                    {
                        System.Diagnostics.Debug.WriteLine("Write Complete!-2");
                        if (replace)
                        {
                            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
                            await file.DeleteAsync();
                            System.Diagnostics.Debug.WriteLine("Write Complete!0");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Write Complete2!");
                            return;
                        }
                    }

                    //创建文件
                    var fileName = filePath.Trim('/').Replace("/", "\\");
                    var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName);
                    using (var stream = await storageFile.OpenStreamForWriteAsync())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                });
            }

            System.Diagnostics.Debug.WriteLine("Write Complete!");
        }

        public async Task WriteTextAsync(string text, string filePath, bool replace = false)
        {
            await WriteFileAsync(Encoding.UTF8.GetBytes(text), filePath, replace);
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        filePath = filePath.Trim('/').Replace("/", "\\");
                        IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(filePath);
                        
                        await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
                        return true;
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine(filePath);
                        return false;
                    }
                });
            }
        }

        public async Task<bool> DirectoryExistsAsync(string directory)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        directory = directory.Trim('/').Replace("/", "\\");
                        await ApplicationData.Current.LocalFolder.GetFolderAsync(directory);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
            }
        }

        public async Task<bool> StorageItemExistsAsync(string itemPath)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {

                    itemPath = itemPath.Trim('/').Replace("/", "\\");
                    IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(itemPath);

                    return storageItem!=null;

                });
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(string.Format("ms-appdata:///local/{0}", filePath), UriKind.Absolute));
                        await file.DeleteAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
            }
        }

        public async Task<bool> DeleteDirectoryAsync(string directory, bool isDeleteAll = false)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    StorageFolder folder = null;
                    try
                    {
                        directory = directory.Trim('/').Replace("/", "\\");
                        folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(directory);
                        await folder.DeleteAsync();
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                        foreach (var file in files)
                        {
                            await file.DeleteAsync();
                        }
                        return true;
                    }

                    catch (Exception)
                    {
                        return false;
                    }
                });
            }

        }

        public async Task<bool> CreateDirectoryAsync(string directory)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        directory = directory.Trim('/').Replace("/", "\\");
                        await ApplicationData.Current.LocalFolder.CreateFolderAsync(directory, CreationCollisionOption.OpenIfExists);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
            }
        }

        public async Task<ulong> GetFileLengthAsync(string filePath)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    var file = await
                        StorageFile.GetFileFromApplicationUriAsync(new Uri(string.Format("ms-appdata:///local/{0}", filePath),
                            UriKind.Absolute));

                    return (await file.OpenReadAsync()).Size;
                });
            }
        }

        public async Task<string[]> GetFilesAsync(string directory)
        {
            directory = directory.Trim('/').Replace("/", "\\");
            var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(directory);
            var files = await folder.GetFilesAsync();
            return files.ToList()
                .Select(f => f.Path.Replace(ApplicationData.Current.LocalFolder.Path, string.Empty).Trim('\\').Replace("\\", "/"))
                .ToArray();
        }

        public async Task<string[]> GetDirectoriesAsync(string directory)
        {
            directory = directory.Trim('/').Replace("/", "\\");
            var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(directory);
            var files = await folder.GetFoldersAsync();
            return files.ToList()
                .Select(f => f.Path.Replace(ApplicationData.Current.LocalFolder.Path, string.Empty).Trim('\\').Replace("\\", "/"))
                .ToArray();
        }

        public async Task SerializeAsync<T>(string filePath, T obj)
        {
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            await WriteFileAsync(stream, filePath, true);
        }

        public async Task<T> DeSerializeAsync<T>(string filePath)
        {
            using (var stream = await ReadFileAsync(filePath))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream);
            }
        }

        public async Task CopyPackageFileToLocalAsync(string source, string target = null, bool replace = false)
        {
            using (var stream = await GetResourceStreamAsync(source))
            {
                target = target ?? source;
                await WriteFileAsync(stream, target, replace);
            }
        }

        public async Task CopyPackageFolderToLocalAsync(string source, string target = null, bool replace = false)
        {

            source = source.Trim('/').Replace("/", "\\");
            target = target != null ? target.Trim('/').Replace("/", "\\") : source;

            var sourseFolder = await Package.Current.InstalledLocation.GetFolderAsync(source);

            //创建目标文件夹
            var targetFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(target, CreationCollisionOption.OpenIfExists);

            await CopyPackageFolderToLocalAsync(sourseFolder, targetFolder, replace);
        }

        public async Task CopyPackageFolderToLocalAsync(StorageFolder source, StorageFolder target, bool replace = false)
        {
            var folders = await source.GetFoldersAsync();
            foreach (var storageFolder in folders)
            {
                var targetFolder = await target.CreateFolderAsync(storageFolder.Name, CreationCollisionOption.OpenIfExists);
                await CopyPackageFolderToLocalAsync(storageFolder, targetFolder, replace);
            }

            var files = await source.GetFilesAsync();
            foreach (var storageFile in files)
            {
                try
                {
                    await storageFile.CopyAsync(target, storageFile.Name, replace
                        ? NameCollisionOption.ReplaceExisting
                        : NameCollisionOption.FailIfExists);
                }
                catch (Exception)
                {
                    //文件已存在（不替换），抛出异常
                }
            }
        }

        public async Task<Stream> GetResourceStreamAsync(string filePath)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    filePath = filePath.Trim('/').Replace("/", "\\");

                    //发现通过ms-appx:///访问的方会出现问题，现改成通过下面方式访问文件
                    var f = await Package.Current.InstalledLocation.GetFileAsync(filePath);
                    using (Stream stream = await f.OpenStreamForReadAsync())
                    {
                        return CopyStream(stream);
                    }
                });
            }
        }


        #region 辅助函数

        private static byte[] ToBytes(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            int length = Convert.ToInt32(stream.Length);
            var data = new byte[length];
            stream.Read(data, 0, length);
            return data;
        }

        public Stream CopyStream(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var tempStream = new MemoryStream();
            stream.CopyTo(tempStream);
            tempStream.Seek(0, SeekOrigin.Begin);
            return tempStream;
        }


        #endregion

        public async Task<bool> CacheFileIsOutDate(string filePath)
        {
            using (await asyncLock.LockAsync())
            {
                return await await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        filePath = filePath.Trim('/').Replace("/", "\\");

                        var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath);

                        return (DateTime.Now - file.DateCreated).TotalMinutes > 5;
                    }
                    catch (FileNotFoundException)
                    {
                        return false;
                       // throw new FileNotFoundException(string.Format("没有找到文件:{0}", filePath));
                    }
                });
            }

        }

    }
    public static class StreamReaderExtension
    {
        public static async Task<String> ReadToEndAsyncThread(this StreamReader reader)
        {
            return await Task.Factory.StartNew<String>(reader.ReadToEnd);
        }
    }

    public static class IStorageFileExtension
    {
        /// <summary>
        /// 扩展方法，获取文件大小
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>存在则返回StorageFile对象，否则返回NULL</returns>
        public async static Task<long> FileSize(this IStorageFile storageFile)
        {
            try
            {
                var stream = await storageFile.OpenStreamForReadAsync();
                return stream.Length;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
