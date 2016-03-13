using BackgroundTaskLibrary;
using System;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace zhihuDaily.DataService
{
    class LiveTileUtils
    {
        private const string LIVETILETASK = "LIVETILETAKS";

        /// <summary>
        /// 注册后台任务
        /// </summary>
        public static async void RegisterLiveTileTask()
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            //if (status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.Denied)
            //{
            //    return;
            //}  //PC

            foreach (var t in BackgroundTaskRegistration.AllTasks)
            {
                if (t.Value.Name == LIVETILETASK)
                {
                    t.Value.Unregister(true);
                }
            }

            var taskBuilder = new BackgroundTaskBuilder
            {
                Name = LIVETILETASK,
                TaskEntryPoint = typeof(LiveTileTask).FullName
            };
            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Clear();
            taskBuilder.SetTrigger(new TimeTrigger(60, false));
            var b = taskBuilder.Register();

            await LiveTileTask.GetLatestNews();
            System.Diagnostics.Debug.WriteLine("Reg BackgroundTask" + b.Name);
        }
        /// <summary>
        /// 卸载后台任务
        /// </summary>
        public static async void UnRegisterLiveTileTask()
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();

            foreach (var t in BackgroundTaskRegistration.AllTasks)
            {
                if (t.Value.Name == LIVETILETASK)
                {
                    t.Value.Unregister(true);
                    System.Diagnostics.Debug.WriteLine("UnReg BackgroundTask:"+ LIVETILETASK);
                }
            }
            
        }
    }
}
