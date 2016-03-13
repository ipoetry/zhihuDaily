using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace zhihuDaily.DataService
{
    public class CommonService<T> : ServiceBase, ICommonService<T> where T : new()
    {
        public string ExceptionsParameter { set; get; }
        public async Task<T> GetObjectAsync(params string[] parameter)
        {
            //先判断网络状态
            string rootUrl = "http://news-at.zhihu.com/api/4";
            List<string> paras = new List<string>(parameter);
            paras.ForEach(p=> rootUrl += "/"+p);
            string cacheName = EncryptUtils.GetMd5String(rootUrl);
            string cacheFilePath = $"Cache/{ cacheName}";
            try
            {
                if (await StorageHelper.Instance.StorageItemExistsAsync(cacheFilePath)) //判断是否有缓存await CacheManager.Instance.CacheIsAvailable(cacheName)
                {
                    if (rootUrl.Contains("latest")) //判断是否是今日消息请求
                    {
                        string jsonResult2 = string.Empty;
                        if (!await StorageHelper.Instance.CacheFileIsOutDate(cacheFilePath) || NetWorkHelper.NetWrokState==0)//判断是否过期
                        {
                            jsonResult2 = await StorageHelper.Instance.ReadTextAsync(cacheFilePath);//如果缓存未过期或者没有网络 
                            System.Diagnostics.Debug.WriteLine("lastnews---从缓存加载");
                        }
                        else
                        {
                            jsonResult2 = await GetJsonDataAsync(rootUrl);
                            if (!string.IsNullOrEmpty(jsonResult2))
                            {
                                await StorageHelper.Instance.WriteTextAsync(jsonResult2, cacheFilePath, true);
                            }
                        }
                        return string.IsNullOrEmpty(jsonResult2) ? new T() : JsonConvertHelper.JsonDeserialize<T>(jsonResult2);                    
                    }
                    else
                    {
                        var jsonResult3 = await StorageHelper.Instance.ReadTextAsync(cacheFilePath);
                        System.Diagnostics.Debug.WriteLine(rootUrl + "---从缓存加载----"+jsonResult3);
                        return string.IsNullOrEmpty(jsonResult3) ? new T() : JsonConvertHelper.JsonDeserialize<T>(jsonResult3);
                    }
                }
                else
                {
                    var jsonResult4 = await GetJsonDataAsync(rootUrl);
                    if (!string.IsNullOrEmpty(jsonResult4))
                    {
                        await StorageHelper.Instance.WriteTextAsync(jsonResult4, cacheFilePath, true);
                    }
                    return string.IsNullOrEmpty(jsonResult4) ? new T() : JsonConvertHelper.JsonDeserialize<T>(jsonResult4);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Error at GetObjectAsync");
                ExceptionsParameter = "未能获取到数据";
                return new T();
            }
        }


        public async Task<T> GetObjectAsync2(params string[] parameter)
        {
            //先判断网络状态
            string rootUrl = "http://news-at.zhihu.com/api/4";
            List<string> paras = new List<string>(parameter);
            paras.ForEach(p => rootUrl += "/" + p);
            string cacheName = EncryptUtils.GetMd5String(rootUrl);
            string cacheFilePath = $"Cache/{ cacheName}";
            try
            {
                if (await StorageHelper.Instance.StorageItemExistsAsync(cacheFilePath)) //判断是否有缓存await CacheManager.Instance.CacheIsAvailable(cacheName)
                {
                    string jsonResult2 = string.Empty;
                    if (!await StorageHelper.Instance.CacheFileIsOutDate(cacheFilePath) || !NetWorkHelper.IsConnectedToInternet)//判断是否过期
                    {
                        jsonResult2 = await StorageHelper.Instance.ReadTextAsync(cacheFilePath);//如果缓存未过期或者没有网络 
                        System.Diagnostics.Debug.WriteLine("lastnews-1-1-从缓存加载：："+ jsonResult2);
                    }
                    else
                    {
                        jsonResult2 = await GetJsonDataAsync(rootUrl);
                        if (!string.IsNullOrEmpty(jsonResult2))
                        {
                            await StorageHelper.Instance.WriteTextAsync(jsonResult2, cacheFilePath, true);
                        }
                    }
                    return string.IsNullOrEmpty(jsonResult2) ? new T() : JsonConvertHelper.JsonDeserialize<T>(jsonResult2);
                }
                else
                {
                    var jsonResult4 = await GetJsonDataAsync(rootUrl);
                    if (!string.IsNullOrEmpty(jsonResult4))
                    {
                        await StorageHelper.Instance.WriteTextAsync(jsonResult4, cacheFilePath, true);
                    }
                    return string.IsNullOrEmpty(jsonResult4) ? new T() : JsonConvertHelper.JsonDeserialize<T>(jsonResult4);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Error 2");
                ExceptionsParameter = "未能获取到数据";
                return new T();
            }
        }


        public async Task<T> GetNotAvailableCacheObjAsync(params string[] parameter)
        {
            string rootUrl = "http://news-at.zhihu.com/api/4";
            List<string> paras = new List<string>(parameter);
            paras.ForEach(p => rootUrl += "/" + p);
            string jsonResult = string.Empty;
            if (NetWorkHelper.NetWrokState != 0)
                jsonResult = await GetJsonDataAsync(rootUrl);

            return string.IsNullOrEmpty(jsonResult) ? new T() : JsonConvertHelper.JsonDeserialize<T>(jsonResult);

        }


        //public async Task<T> GetObjectAsync(params string[] parameter)
        //{
        //    string url = rootUrl;
        //    CacheManager cm = new CacheManager();
        //    List<string> paras = new List<string>(parameter);
        //    paras.ForEach(p => url += "/" + p);
        //    string cacheName = typeof(T).ToString().Replace(".", "_");
        //    System.Diagnostics.Debug.WriteLine("---------");
        //    string jsonResult = string.Empty;
        //    if (await cm.CacheIsAvailable(cacheName))
        //    {
        //        jsonResult = await cm.ReadCache(cacheName);
        //    }
        //    else
        //    {
        //        jsonResult = await GetJsonDataAsync(url);
        //        cm.CreateCache(cacheName, jsonResult);
        //    }

        //    if (result != null)
        //    {
        //        return result;
        //    }
        //    else
        //    {
        //        ExceptionsParameter = "未能获取到数据";
        //        return new T();
        //    }
        //}
    }
}
