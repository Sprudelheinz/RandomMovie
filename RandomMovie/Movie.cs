using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomMovie.Enums;
using RandomMovie.ViewModels;

namespace RandomMovie
{
    public class Movie : ObservableViewModelBase
    {
        public int SortValue { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string LetterboxdURI { get; set; }
        public string PosterWebsiteLink { get; set; }
        public System.Drawing.Color MainColor { get; set; }
        public bool IsDark { get; set; }
        public string FilmID { get; set; }
        public List<Genre> Genres { get; set; }
        [JsonIgnore]
        public string MovieTitle => Name + " (" + Year + ")";

        private ImageSource posterImageSource;
        [JsonIgnore]
        public ImageSource PosterImageSource 
        {
            get
            {
                if (posterImageSource == null)
                    GetImageSource();
                return posterImageSource;
            }
            set
            {
                posterImageSource = value;
                RaisePropertyChanged(nameof(PosterImageSource));
                RaisePropertyChanged(nameof(PosterNotAvailable));
            }
        }

        [JsonIgnore]
        public bool PosterNotAvailable { get; set; }  = false;

        [JsonIgnore]
        public double Width => Services.Services.GetWidth();

        [JsonIgnore]
        public double Height => Services.Services.GetHeight();        

        private void GetImageSource()
        {
            var cacheFolder = Path.Combine(FileSystem.Current.AppDataDirectory, Services.Services.POSTER_CACHE_FOLDER);
            if (!Directory.Exists(cacheFolder))
                Directory.CreateDirectory(cacheFolder);
            var fileName = FilmID + ".jpg";
            var fullFileName = Path.Combine(cacheFolder, fileName);
            if (File.Exists(fullFileName))
            {
                PosterImageSource = ImageSource.FromFile(fullFileName);
                return;
            }
            MainThread.BeginInvokeOnMainThread(async () => await Services.Services.ImageDownloaderInstance.DownloadImageAsync(cacheFolder, FilmID, new Uri(PosterWebsiteLink), this));
        }
        [JsonIgnore]
        public Color MainColorMaui => GetColor(MainColor);

        private Color GetColor(System.Drawing.Color mainColor)
        {
            return Color.FromRgba(mainColor.R, mainColor.G, mainColor.B, mainColor.A);
        }
    }
}
