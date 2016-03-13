using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace SocialShare.Weibo
{
    public class JsonConvertHelper
    {
        // 序列化  
        public static string JsonSerializer<T>(T t)
        {
            // 使用 DataContractJsonSerializer 将 T 对象序列化为内存流。
            string jsonString = string.Empty;
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                // 使用 WriteObject 方法将 JSON 数据写入到流中。   
                jsonSerializer.WriteObject(ms, t);
                // 流转字符串  
                jsonString = Encoding.UTF8.GetString(ms.ToArray());
            }
            //替换Json的Date字符串    
            string p = @"\\/Date(\d+)\+\d+\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }
        public static T JsonDeserialize<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式    
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            // 字符串转流  
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            // 通过使用 DataContractJsonSerializer 的 ReadObject 方法，将 JSON 编码数据反序列化为T   
            T obj = (T)jsonSerializer.ReadObject(ms);
            return obj;
        }
        public static string ConvertJsonDateToDateString(Match match)
        {
            string result = string.Empty;
            DateTime dateTime = new DateTime(1970, 1, 1);
            dateTime = dateTime.AddMilliseconds(long.Parse(match.Groups[1].Value));
            dateTime = dateTime.ToLocalTime();
            result = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
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
