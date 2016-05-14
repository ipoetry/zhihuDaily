using MicroMsg.sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialShare.WeChat
{
    public class WeChatClient
    {
        const string APP_ID = "wx167b8355f065339c";//wxc6d5cff9be60265d //--读读日报
        public async void ShareLink(string url, string title,string description, byte[] thumb,int scene= SendMessageToWX.Req.WXSceneChooseByUser)
        {
            try
            {
                var message = new WXWebpageMessage
                {
                    WebpageUrl = url,
                    Title = title,
                    Description = description,
                    ThumbData = thumb
                };
                SendMessageToWX.Req req = new SendMessageToWX.Req(message, scene);
                IWXAPI api = WXAPIFactory.CreateWXAPI(APP_ID);
                var isValid = await api.SendReq(req);

            }
            catch (WXException)
            {
                
            }
        }
    }
}
