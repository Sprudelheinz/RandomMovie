using System.ComponentModel;

namespace RandomMovie.ViewModels
{
    internal class MainPageViewModel : ObservableViewModelBase
    {
        private List<Movie> movies;

        public List<Movie> Movies 
        { 
            get => movies;
            set
            {
                SetProperty(ref movies, value);
            }
        }
        public List<Movie> Watchlist { get; set; } = new List<Movie>();
        public string LetterBoxdUserName { get; set; } = "Sprudelheinz";

        public MainPageViewModel()
        {
            ReadJsonFiles();           
        }

        private async void ReadJsonFiles()
        {
            Movies = await Services.Services.ReadJsonFromFileAsync("brightToDark.json");
            var watchlistFileName = Path.Combine(Microsoft.Maui.Storage.FileSystem.Current.AppDataDirectory, Services.Services.WATCHLIST_FILENAME);
            if (File.Exists(watchlistFileName))
            {
                var watchlistJson = await Services.Services.ReadTextFile(watchlistFileName);
                Watchlist = Services.Services.ReadJson(watchlistJson);
            }
        }
    }
}
