using System.Threading.Tasks;

namespace zhihuDaily.DataService
{
    public interface ICommonService<T>
    {
        string ExceptionsParameter { set; get; }
        Task<T> GetObjectAsync(params string[] parameter);
        Task<T> GetObjectAsync2(params string[] parameter);
        Task<T> GetNotAvailableCacheObjAsync(params string[] parameter);
    }
}
