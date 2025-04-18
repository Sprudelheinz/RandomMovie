﻿using RandomMovie.Enums;
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
        public string MainColor { get; set; }
        public bool IsDark { get; set; }
        public string FilmID { get; set; }
        public List<Genre> Genres { get; set; }
        public List<string> Countries { get; set; }
        public double Rating { get; set; }

        public string MovieTitle => Name + " (" + Year + ")";

        private bool isLoadingImage = false;

        private ImageSource m_posterImageSource;
        public ImageSource PosterImageSource 
        {
            get
            {
                if (m_posterImageSource == null && !isLoadingImage)
                    GetImageSource();
                return m_posterImageSource;
            }
            set
            {
                if (m_posterImageSource != value)
                {
                    m_posterImageSource = value;
                    RaisePropertyChanged(nameof(PosterImageSource));
                    RaisePropertyChanged(nameof(PosterNotAvailable));
                }
            }
        }
        public bool PosterNotAvailable { get; set; }  = false;

        public double Width => Services.Services.GetWidth();

        public double Height => Services.Services.GetHeight();

        private void GetImageSource()
        {
            if (string.IsNullOrEmpty(PosterWebsiteLink))
            {
                PosterNotAvailable = true;
                return;
            }

            var cacheFolder = Path.Combine(FileSystem.Current.AppDataDirectory, Services.Services.POSTER_CACHE_FOLDER);
            if (!Directory.Exists(cacheFolder))
                Directory.CreateDirectory(cacheFolder);
            var fileName = FilmID + ".jpg";
            var fullFileName = Path.Combine(cacheFolder, fileName);
            if (File.Exists(fullFileName))
            {
                var imageSource = ImageSource.FromFile(fullFileName);
                if (imageSource != null)
                {
                    PosterImageSource = imageSource;
                }
                else
                {
                    PosterImageSource = null;
                    PosterNotAvailable = true;
                }
                return;
            }
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    isLoadingImage = true;
                    var uri = new Uri(PosterWebsiteLink);
                    await Services.Services.ImageDownloaderInstance.DownloadImageAsync(cacheFolder, FilmID, uri, this);
                }
                catch
                {
                    PosterImageSource = null;
                    PosterNotAvailable = true;
                }
                finally
                {
                    isLoadingImage = false;
                }
            });
        }
        public Color MainColorMaui => GetColor(MainColor);

        private Color GetColor(string mainColor)
        {
            var parts = mainColor.Split(',');
            if (parts.Length == 4 &&
                byte.TryParse(parts[0], out var r) &&
                byte.TryParse(parts[1], out var g) &&
                byte.TryParse(parts[2], out var b) &&
                byte.TryParse(parts[3], out var a))
            {
                return Color.FromRgba(r, g, b, a);
            }
            return Colors.Transparent;
        }
    }
}
