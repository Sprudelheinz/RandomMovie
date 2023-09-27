using RandomMovie.Controls.PopUp;
using RandomMovie.Services;
using System.ComponentModel;

namespace RandomMovie.ViewModels
{
    public class MainPageViewModel : ObservableViewModelBase
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
        private string m_letterBoxdUserName;
        private KeyValuePair<string, string> selectedList;

        public string LetterBoxdUserName 
        { 
            get => m_letterBoxdUserName;
            set
            {
                m_letterBoxdUserName = value;
                RaisePropertyChanged(nameof(LetterBoxdUserName));
            }
        }
        public bool ActivityRunning { get; set; } = false;
        public List<Movie> AllTheMovies { get; set; }
        public bool SortAscending { get; set; } = true;
        public Dictionary<string, string> List { get; internal set; }
        public KeyValuePair<string, string> SelectedList 
        { 
            get => selectedList;
            internal set
            {
                selectedList = value;
            }
        }

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
                    try
                    {
                        var watchlistJson = Services.Services.ReadTextFile(watchlistFileName);
                        Watchlist = Services.Services.ReadJson(watchlistJson);
                    }
                    catch
                    {
                        Watchlist = new List<Movie>();
                        File.Delete(watchlistFileName);
                    }
                }
                Movies = AllTheMovies;
                
                LetterBoxdUserName = SettingsService.Instance.Settings.LetterBoxdUserName;
                Application.Current.UserAppTheme = SettingsService.Instance.Settings.Theme;
            });
        }
    }
}
