using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using zhihuDaily.Model;

namespace zhihuDaily.DataService
{
    public static class Functions
    {
        static Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        /// <summary>
        ///获取设备唯一ID 
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueDeviceId()
        {
            HardwareToken ht = HardwareIdentification.GetPackageSpecificToken(null);
            var id = ht.Id;
            var dataReader = DataReader.FromBuffer(id);
            byte[] bytes = new byte[id.Length];
            dataReader.ReadBytes(bytes);
            string s = BitConverter.ToString(bytes);
            return s.Replace("-", "");
        }

        /// <summary>
        /// 读取资源文件
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string LoadResourceString(string resourceName)
        {
            string str = loader.GetString(resourceName);
            return str;
        }

        /// <summary>
        /// 获取当前程序版本
        /// </summary>
        /// <returns></returns>
        public static string GetVersionString()
        {
            string appVersion = "Version：" + string.Format("{0}.{1}.{2}.{3}",
                Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor,
                Package.Current.Id.Version.Build,
                Package.Current.Id.Version.Revision);
            return appVersion;
        }


        public static void btn_NightMode_Click(Grid page)
        {
            //AppSettings setting = AppSettings.Instance;
            //if (setting.NightModeTheme)    // true = night mode
            //{
            //    page.RequestedTheme = ElementTheme.Light;
            //    setting.NightModeTheme = false;
            //}
            //else // false = day mode
            //{
            //    page.RequestedTheme = ElementTheme.Dark;
            //    setting.NightModeTheme = true;
            //}
            SwitchTheme();
            page.RequestedTheme = AppSettings.Instance.CurrentTheme;
        }

        public static void SetTheme(Grid page)
        {
            page.RequestedTheme = AppSettings.Instance.CurrentTheme;
        }
        /// <summary>
        /// 主题切换
        /// </summary>
        public static void SwitchTheme()
        {
            var temp = AppSettings.Instance.CurrentTheme;
            if (temp == ElementTheme.Dark || temp == ElementTheme.Default)
            {
                AppSettings.Instance.CurrentTheme = ElementTheme.Light;
            }
            else
            {
                AppSettings.Instance.CurrentTheme = ElementTheme.Dark;
            }
        }

        /// <summary>
        /// 替换html字符串中的Img标签src,来实现无图模式
        /// </summary>
        /// <param name="strHtml">需要处理的字符串</param>
        /// <param name="isSaveImgName">是否保留图片名</param>
        /// <returns></returns>
        public static string ReplaceImgUrl(string strHtml, bool isSaveImgName = false)
        {
            string pattern = isSaveImgName ? "(<img.+src=\").+/([^>]+>)" : @"(?i)(?<=<img\b[^>]*?src=\s*(['""]?))([^'""]*/)+(?=[^'""/]+\1)";
            string replacement = isSaveImgName ? "<img src=''>" : "";
            return Regex.Replace(strHtml, pattern, replacement);
        }

        public static string ReplaceImgSrc(string strHtml,string newUrl,string patternPara="src")
        {
            string pattern = $@"(?i)(?<=<img[^>]*?\s{patternPara}=(['""]?))[^'""\s>]+(?=\1[^>]*>)";
            return Regex.Replace(strHtml, pattern, newUrl);
        }


        /// <summary> 
        /// 正则表达式获取HTML中所有图片的 URL。 
        /// </summary> 
        /// <param name="sHtmlText">HTML代码</param> 
        /// <returns>图片的URL列表</returns> 
        public static IList<string> GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签 
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(sHtmlText);
            //int i = 0;
            // string[] sUrlList = new string[matches.Count];
            List<string> urlList = new List<string>();
            // 取得匹配项列表 
            foreach (Match match in matches)
                // sUrlList[i++] = match.Groups["imgUrl"].Value;
                urlList.Add(match.Groups["imgUrl"].Value);
            return urlList;
        }

        public static string GetPhoneInfo()
        {
            Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation deviceInfo =
                new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();

                Package package = Package.Current;
                var SystemArchitecture = package.Id.Architecture.ToString();

            return "设备型号:" + deviceInfo.SystemManufacturer + " " + deviceInfo.SystemProductName+
                       "\n\r系统版本:" + GetSystemVersion()
                        +"\n\r处理器:" + SystemArchitecture+"\n\r";

            // get the system family name
            //AnalyticsVersionInfo ai = AnalyticsInfo.VersionInfo;
            //SystemFamily = ai.DeviceFamily;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetSystemVersion()
        {
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            return $"{v1}.{v2}.{v3}.{v4}";
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailAddress">邮箱地址</param>
        /// <param name="emailSubject">主题</param>
        /// <param name="messageBody">邮件内容</param>
        private static async void SendEmail(string emailAddress, string emailSubject, string messageBody)
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            emailMessage.Body = messageBody;
            emailMessage.Subject = emailSubject;
            emailMessage.To.Add(new Windows.ApplicationModel.Email.EmailRecipient(emailAddress));

            //if (attachmentFile != null)
            //{
            //    var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);

            //    var attachment = new Windows.ApplicationModel.Email.EmailAttachment(
            //        attachmentFile.Name,
            //        stream);

            //    emailMessage.Attachments.Add(attachment);
            //}

            //var email = recipient.Emails.FirstOrDefault<Windows.ApplicationModel.Contacts.ContactEmail>();
            //if (email != null)
            //{
            //    var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient(email.Address);
            //    emailMessage.To.Add(emailRecipient);
            //}

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }
        /// <summary>
        /// 发送反馈邮件
        /// </summary>
        public static void SendFeedBackByEmail()
        {
            SendEmail("wrox1226@live.com", "Suggestion and Feedback ", GetPhoneInfo()+GetVersionString()+"\r\n");
        }

        #region Unix Time Hepler
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        #endregion
    }
}
