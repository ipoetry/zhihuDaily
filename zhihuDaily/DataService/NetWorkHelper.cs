using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;

namespace zhihuDaily.DataService
{
    class NetWorkHelper
    {
        /// <summary>
        /// 获取网络状态 0:无网络 1:Wlan 2：Wwan 3:未知网络状态
        /// </summary>
        public static int NetWrokState {
            get {
                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                if (InternetConnectionProfile == null)
                    return 0;
                else
                {
                    if (InternetConnectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.InternetAccess)
                        return 0; 
                    else if (InternetConnectionProfile.IsWlanConnectionProfile)
                        return 1;
                    else if (InternetConnectionProfile.IsWwanConnectionProfile)
                        return 2;
                    else
                        return 0;
                }
            }
        }

        public static bool IsConnectedToInternet {
            get {return NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;}
        }
       

    }
}
