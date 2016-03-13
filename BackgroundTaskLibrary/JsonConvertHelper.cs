using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace BackgroundTaskLibrary
{
    public sealed class JsonConvertHelper
    {
        public static LatestNews JsonDeserialize(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式    
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LatestNews));
            // 字符串转流  
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            // 通过使用 DataContractJsonSerializer 的 ReadObject 方法，将 JSON 编码数据反序列化为T   
            LatestNews obj = (LatestNews)jsonSerializer.ReadObject(ms);
            return obj;
        }

        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }


    }

}
