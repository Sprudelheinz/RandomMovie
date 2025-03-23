using RandomMovie.Enums;
using RandomMovie.Services;

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

        public List<GenreViewModel> GenresList { get; set; } = new List<GenreViewModel>();

        public List<CountryViewModel> CountryList { get; set; } = new List<CountryViewModel>();

        private string m_letterBoxdUserName = "Sprudelheinz";
        public string LetterBoxdUserName 
        { 
            get => m_letterBoxdUserName;
            set
            {
                m_letterBoxdUserName = value;
                RaisePropertyChanged(nameof(LetterBoxdUserName));
            }
        }
        private double? m_rating = null;
        public double? Rating
        {
            get => m_rating;
            set
            {
                m_rating = value;
                RaisePropertyChanged(nameof(Rating));
            }
        }

        private bool m_filterVisible = false;
        public bool FilterVisible
        {
            get => m_filterVisible;
            set
            {
                m_filterVisible = value;
                RaisePropertyChanged(nameof(FilterVisible));
            }
        }
        public bool ActivityRunning { get; set; } = false;
        public List<Movie> AllTheMovies { get; set; }
        public bool SortAscending { get; set; } = true;

        private string searchText;
        public string SearchText 
        { 
            get => searchText;
            set
            {
                searchText = value;
                RaisePropertyChanged(nameof(SearchText));
            }
        }

        private bool m_greaterThanSmallerThan = true;
        public bool GreaterThanSmallerThan
        {
            get => m_greaterThanSmallerThan;
            set
            {
                m_greaterThanSmallerThan = value;
                RaisePropertyChanged(nameof(GreaterThanSmallerThanText));
            }
        }
        public string GreaterThanSmallerThanText
        {
            get => GreaterThanSmallerThan ? Resources.Localisation.GreaterThan : Resources.Localisation.SmallerThan;
            set
            {     
            }
        }
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
                //var letterboxdListFileName = Path.Combine(FileSystem.Current.AppDataDirectory, Services.Services.LETTERBOXDLIST_FILENAME);
                //if (File.Exists(letterboxdListFileName))
                //{
                //    try
                //    {
                //        var letterboxdListJson = Services.Services.ReadTextFile(letterboxdListFileName);
                //        SelectedLetterboxdList = Services.Services.ReadJson(letterboxdListJson);
                //    }
                //    catch
                //    {
                //        SelectedLetterboxdList = new List<Movie>();
                //        File.Delete(letterboxdListFileName);
                //    }
                //}
                Movies = AllTheMovies;
                
                LetterBoxdUserName = SettingsService.Instance.Settings.LetterBoxdUserName;
                Application.Current.UserAppTheme = SettingsService.Instance.Settings.Theme;

                var list = Enum.GetValues<Genre>();
                foreach (var item in list.OrderBy(x => x.ToString()))
                {
                    GenresList.Add(new GenreViewModel(item, false));
                }

                foreach (var country in Country.Countries)
                {
                    CountryList.Add(new CountryViewModel(country, false));
                }
            });
        }
    }
}
