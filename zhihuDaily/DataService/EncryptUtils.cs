using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace zhihuDaily.DataService
{
    public class EncryptUtils
    {
        /// <summary>
        /// Md5加密处理
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns>返回加密后的串</returns>
        public static string GetMd5String(string strSource)
        {
            //可以选择MD5 Sha1 Sha256 Sha384 Sha512
            string strAlgName = HashAlgorithmNames.Md5;

            // 创建一个 HashAlgorithmProvider 对象
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);

            // 创建一个 CryptographicHash 对象        
            CryptographicHash objHash = objAlgProv.CreateHash();

            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(strSource, BinaryStringEncoding.Utf8);
            objHash.Append(buffMsg);
            IBuffer buffHash = objHash.GetValueAndReset();
            string strResult = CryptographicBuffer.EncodeToHexString(buffHash);

            return strResult;
        }
    }
}
