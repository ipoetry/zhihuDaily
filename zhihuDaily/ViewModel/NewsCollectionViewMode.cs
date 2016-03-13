using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class NewsCollectionViewMode: ViewModelBase
    {
        public NewsCollectionViewMode()
        {
            this.ItemClickCommand = new RelayCommand<object>((e) =>
            {
                Messenger.Default.Send(new NotificationMessage(e, "OnItemClick"));
            });
        }
        public RelayCommand<object> ItemClickCommand { set; get; }
    }

    public abstract class CollectionNewsDataSource : DataSourceBase<Story>
    {
        protected override void AddItems(IList<Story> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (!this.Any(it => it.Id == item.Id))
                    {
                        this.Add(item);
                    }
                }
            }
        }

    }
    public class CollectionDS : CollectionNewsDataSource
    {
        protected async override Task<IList<Story>> LoadItemsAsync()
        {

            var favFolder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync("fav", CreationCollisionOption.OpenIfExists);

            var files = await favFolder.GetFilesAsync();

            var result = new List<Story>();

            var num = 0;

            foreach (var file in files.Where(f => f.FileType == ".json").OrderByDescending(f => f.DateCreated))
            {
                if (!_loadedFiles.ContainsKey(file.Name))
                {
                    //var properties = await file.GetBasicPropertiesAsync();
                    //System.Diagnostics.Debug.WriteLine("file {0}:{1}", file.DisplayName, properties.Size);

                    _loadedFiles.Add(file.Name, true);

                    var jsonData = await FileIO.ReadTextAsync(file);
                    var newItem = DataService.JsonConvertHelper.JsonDeserialize<Story>(jsonData);

                    result.Add(newItem);

                    num++;

                    //TODO: change to a member later
                    if (num >= 20)
                    {
                        break;
                    }
                }
            }

            return result;
        }

     //   private StorageFolder favFolder;
        private  CollectionDS()
        {
          //  favFolder= await ApplicationData.Current.RoamingFolder.CreateFolderAsync("fav", CreationCollisionOption.OpenIfExists);
        }

        private readonly static CollectionDS _instance = new CollectionDS();

        public static CollectionDS Instance { get { return _instance; } }

        public async Task AddFavStory(Story story)
        {
            if (!this.Any(p => p.Id == story.Id))
            {
                // await post.AsFavorite();
                var favFolder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync("fav", CreationCollisionOption.OpenIfExists);
                var postFile = await favFolder.CreateFileAsync(story.Id+".json", CreationCollisionOption.ReplaceExisting);

                var jsonData = DataService.JsonConvertHelper.JsonSerializer(story);

                await FileIO.WriteTextAsync(postFile, jsonData);

                this.Insert(0, story);
            }
        }

        public async Task<bool> IsFavExisted(Story story)
        {
            var favFolder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync("fav", CreationCollisionOption.OpenIfExists);

            var files = await favFolder.GetFilesAsync();

            return files.Any(f=>f.Name==story.Id+".json");
        }

        public async Task RemoveFav(Story story)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].Id == story.Id)
                {
                    this.RemoveAt(i);
                    break;
                }
            }
            var favFolder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync("fav", CreationCollisionOption.OpenIfExists);
            var filename = story.Id + ".json";

            // get/overwrite file
            var postFile = await favFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            var jsonData = DataService.JsonConvertHelper.JsonSerializer(story); ;

            await FileIO.WriteTextAsync(postFile, jsonData);
        }

        public override async Task Refresh()
        {
            this._loadedFiles.Clear();

            await base.Refresh();
        }

        private Dictionary<string, bool> _loadedFiles = new Dictionary<string, bool>();
    }
}
