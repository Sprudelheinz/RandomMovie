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
        public List<Movie> SelectedLetterboxdList { get; set; } = new List<Movie>();
        private string m_letterBoxdUserName;

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
        public Dictionary<string, string> LetterboxdLists { get; internal set; }

        public MainPageViewModel()
        {
            ReadJsonFiles();
        }

        private void ReadJsonFiles()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                AllTheMovies = await Services.Services.ReadJsonFromFileAsync("brightToDark.json");
                var letterboxdListFileName = Path.Combine(FileSystem.Current.AppDataDirectory, Services.Services.LETTERBOXDLIST_FILENAME);
                if (File.Exists(letterboxdListFileName))
                {
                    try
                    {
                        var letterboxdListJson = Services.Services.ReadTextFile(letterboxdListFileName);
                        SelectedLetterboxdList = Services.Services.ReadJson(letterboxdListJson);
                    }
                    catch
                    {
                        SelectedLetterboxdList = new List<Movie>();
                        File.Delete(letterboxdListFileName);
                    }
                }
                Movies = AllTheMovies;
                
                LetterBoxdUserName = SettingsService.Instance.Settings.LetterBoxdUserName;
                Application.Current.UserAppTheme = SettingsService.Instance.Settings.Theme;
            });
        }
    }
}
