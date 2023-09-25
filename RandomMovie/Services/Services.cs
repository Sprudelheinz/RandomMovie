using Newtonsoft.Json;
using RandomMovie.ViewModels;


namespace RandomMovie.Services
{
   
    public static class Services
    {
        public const string WATCHLIST_FILENAME = "watchlist.json";
        public const string POSTER_CACHE_FOLDER = "postercache";
        public const string SETTINGS_FILE = "settings.json";
        public static ImageDownloader ImageDownloaderInstance { get; set; } = new ImageDownloader();
        internal static void SaveJson(List<Movie> watchlist)
        {
            var json = JsonConvert.SerializeObject(watchlist);
            WriteTextToFile(json, WATCHLIST_FILENAME);
        }

        internal static async Task<List<Movie>> ReadJsonFromFileAsync(string file)
        {
            using (var stream = await FileSystem.OpenAppPackageFileAsync(file)) //
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    return ReadJson(json);
                }
            }
        }
        internal static List<Movie> ReadJson(string json)
        {
            var movies = JsonConvert.DeserializeObject<List<Movie>>(json);
            return movies.ToList();
        }

        public static async void WriteTextToFile(string text, string targetFileName)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            using (var outputStream = File.Open(targetFile, FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteAsync(text);
                }
            }
        }

        public static async Task<string> ReadTextFile(string targetFileName)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            if (File.Exists(targetFile))
            {
                using (var InputStream = File.OpenRead(targetFile))
                {
                    using (var reader = new StreamReader(InputStream))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
                
            }
            return null;
        }

        internal async static Task ReadWatchlistFromUserAsync(MainPageViewModel mainPageViewModel)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            try
            {
                var pageFound = true;
                int i = 0;
                if (string.IsNullOrEmpty(mainPageViewModel.LetterBoxdUserName))
                {
                    mainPageViewModel.Settings.LetterBoxdUserName = null;
                    SaveSettings(mainPageViewModel.Settings);
                    mainPageViewModel.Watchlist = new List<Movie>();
                    return;
                }
                using (var httpClient = new HttpClient())
                {
                    mainPageViewModel.Watchlist = new List<Movie>();
                    while (pageFound)
                    {
                        var uri = $"https://letterboxd.com/" + mainPageViewModel.LetterBoxdUserName + $"/watchlist/page/" + i;
                        var webstring = await httpClient.GetStringAsync(uri);
                        doc.LoadHtml(webstring);
                        var ul = doc.DocumentNode.SelectSingleNode("//ul[contains(@class,'poster-list')]");
                        if (ul == null)
                            break;
                        var divs = ul.Descendants("div");
                        if (divs.Any())
                            foreach (var div in divs)
                            {
                                var node = div as HtmlAgilityPack.HtmlNode;
                                var value = node.GetAttributes();
                                var part = value.First(x => x.Name == "data-film-id").Value;
                                var movie = mainPageViewModel.Movies.FirstOrDefault(x => x.FilmID == part);
                                if (movie != null)
                                {
                                    mainPageViewModel.Watchlist.Add(movie);
                                }
                            }
                        else
                            pageFound = false;
                        i++;
                    }
                }
                mainPageViewModel.Watchlist = mainPageViewModel.Watchlist.OrderBy(x => x.SortValue).Distinct().ToList();
                SaveJson(mainPageViewModel.Watchlist);
                mainPageViewModel.Settings.LetterBoxdUserName = mainPageViewModel.LetterBoxdUserName;
                SaveSettings(mainPageViewModel.Settings);
            }
            catch
            {
            }
        }

        public static void SaveSettings(Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings);
            WriteTextToFile(json, SETTINGS_FILE);
        }

        internal static async Task<Settings> ReadSettings()
        {
            var settingsJson = await ReadTextFile(SETTINGS_FILE);
            if (settingsJson != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<Settings>(settingsJson);
                }
                catch 
                {
                    return new Settings();
                }
            }
            else
                return new Settings();
        }
    }
}
