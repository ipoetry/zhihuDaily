using System.IO;
using System.Threading.Tasks;

namespace zhihuDaily
{
    public interface IStorageHelper
    {

        Task<Stream> ReadFileAsync(string filePath);

        Task<string> ReadTextAsync(string filePath);

        Task WriteFileAsync(Stream stream, string filePath, bool replace = false);

        Task WriteFileAsync(byte[] data, string filePath, bool replace = false);

        Task WriteTextAsync(string text, string filePath, bool replace = false);

        Task<bool> FileExistsAsync(string filePath);

        Task<bool> DirectoryExistsAsync(string directory);

        Task<bool> StorageItemExistsAsync(string itemPath);

        Task<bool> DeleteFileAsync(string filePath);

        Task<bool> DeleteDirectoryAsync(string directory, bool isDeleteAll = false);

        Task<bool> CreateDirectoryAsync(string directory);

        Task<ulong> GetFileLengthAsync(string filePath);

        Task<string[]> GetFilesAsync(string directory);

        Task<string[]> GetDirectoriesAsync(string directory);

        Task SerializeAsync<T>(string filePath, T obj);

        Task<T> DeSerializeAsync<T>(string filePath);

        /// <summary>
        /// 拷贝安装目录的文件到本地
        /// </summary>
        Task CopyPackageFileToLocalAsync(string source, string target = null, bool replace = false);

        /// <summary>
        /// 拷贝安装目录的文件夹（包括子文件夹和子文件）到本地
        /// </summary>
        Task CopyPackageFolderToLocalAsync(string source, string target = null, bool replace = false);

        Task<Stream> GetResourceStreamAsync(string filePath);

        Task<bool> CacheFileIsOutDate(string filePath);
    }
}
