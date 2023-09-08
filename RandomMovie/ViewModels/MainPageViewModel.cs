using System.ComponentModel;

namespace RandomMovie.ViewModels
{
    internal class MainPageViewModel : ObservableViewModelBase
    {
        private List<Movie> movies = new List<Movie>();

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
        public bool ActivityRunning { get; set; } = false;
        public List<Movie> AllTheMovies { get; set; }

        public bool SortAscending { get; set; } = true;
        public MainPageViewModel()
        {
            ReadJsonFiles();           
        }

        private void ReadJsonFiles()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                AllTheMovies = await Services.Services.ReadJsonFromFileAsync("brightToDark.json");
                var watchlistFileName = Path.Combine(FileSystem.Current.AppDataDirectory, Services.Services.WATCHLIST_FILENAME);
                if (File.Exists(watchlistFileName))
                {
                    var watchlistJson = await Services.Services.ReadTextFile(watchlistFileName);
                    Watchlist = Services.Services.ReadJson(watchlistJson);
                }
                Movies = AllTheMovies;
            });
        }
    }
}
