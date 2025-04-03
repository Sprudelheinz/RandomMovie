using RandomMovie.Controls.PopUp;
using RandomMovie.ViewModels;

namespace RandomMovie.Services
{
   
    public static class Services
    {
        public const string LETTERBOXDLIST_FILENAME = "watchlist.json";
        public const string POSTER_CACHE_FOLDER = "postercache";

        public static ImageDownloader ImageDownloaderInstance { get; set; } = new ImageDownloader();
        internal static void SaveJson(List<Movie> letterboxdList)
        {
            var json = Utf8Json.JsonSerializer.ToJsonString(letterboxdList);
            WriteTextToFileAsync(json, LETTERBOXDLIST_FILENAME);
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
            var movies = Utf8Json.JsonSerializer.Deserialize<List<Movie>>(json);
            return movies.ToList();
        }

        public static async void WriteTextToFileAsync(string text, string targetFileName)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            using (var outputStream = File.Create(targetFile))
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteAsync(text);
                }
            }
        }

        public static string ReadTextFile(string targetFileName)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            if (File.Exists(targetFile))
            {
                using (var InputStream = File.OpenRead(targetFile))
                {
                    using (var reader = new StreamReader(InputStream))
                    {
                        return reader.ReadToEnd();
                    }
                }        
            }
            return null;
        }

        internal async static Task ReadListFromUri(string uri, MainPageViewModel mainPageViewModel, ActivityIndicatorPopUp activityIndicatorPopUp)
        {        
            try
            {
                SettingsService.Instance.Settings.LetterBoxdUserName = mainPageViewModel.LetterBoxdUserName;
                SettingsService.Instance.SaveSettingsAsync();
                using (var httpClient = new HttpClient())
                {
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    int pages = await GetPageNumbersAsync(httpClient, uri);
                    //if (pages > 20)
                    //{
                    //    await activityIndicatorPopUp.CloseAsync();
                    //    Thread.Sleep(100);
                    //    var infoMessage = new InfoMessagePopUp(Localisation.WarnTooMuchToLoad, Enums.InfoMessageAnswer.YesNo);
                    //    var popUpService = ServiceProvider.GetService<IPopupService>();
                        
                    //    var value = await popUpService.ShowPopupAsync(infoMessage);
                    //    if (value is bool returnValue && !returnValue)
                    //        return;
                    //    activityIndicatorPopUp = new ActivityIndicatorPopUp();
                    //    _ = popUpService.ShowPopupAsync(activityIndicatorPopUp);
                    //}
                    mainPageViewModel.SelectedLetterboxdList = new List<Movie>();
                    var webStrings = await GetWebStrings(httpClient, uri, pages);
                    foreach (var webstring in webStrings)
                    {
                        doc.LoadHtml(webstring);
                        var ul = doc.DocumentNode.SelectSingleNode("//ul[contains(@class,'poster-list')]");
                        if (ul == null)
                            break;
                        var divs = ul.Descendants("div");
                        if (divs.Any())
                            foreach (var div in divs)
                            {
                                var value = div.GetAttributes();
                                var part = value.First(x => x.Name == "data-film-id").Value;
                                var movie = mainPageViewModel.AllTheMovies.FirstOrDefault(x => x.FilmID == part);
                                if (movie != null)
                                {
                                    mainPageViewModel.SelectedLetterboxdList.Add(movie);
                                }
                            }
                    }
                }
                mainPageViewModel.SelectedLetterboxdList = mainPageViewModel.SelectedLetterboxdList.OrderBy(x => x.SortValue).Distinct().ToList();               
            }
            catch
            {
            }
        }

        internal async static Task<Dictionary<string, string>> GetListsFromUserName(MainPageViewModel mainPageViewModel)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            var returnList = new Dictionary<string, string>
            {
                { "Watchlist", $"/" + mainPageViewModel.LetterBoxdUserName + $"/watchlist" }
            };
            try
            {
                using (var httpClient = new HttpClient())
                {
                    
                    var listsUri = $"https://letterboxd.com/" + mainPageViewModel.LetterBoxdUserName + $"/lists";
                    var pages = await GetPageNumbersAsync(httpClient, listsUri);
                    var webstrings = await GetWebStrings(httpClient, listsUri, pages);
                    foreach (var webstring in webstrings)
                    {
                        doc.LoadHtml(webstring);
                        var headlines = doc.DocumentNode.SelectNodes("//h2[contains(@class,'title-2 title prettify')]");
                        foreach (var headline in headlines)
                        {
                            var a = headline.ChildNodes.First(x => x.Name == "a");
                            var link = a.Attributes.First().Value;
                            var name = a.InnerHtml;
                            returnList.Add(name, link);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return returnList;
        }

        public static async Task<IEnumerable<string>> GetWebStrings(HttpClient client, string letterboxdUri, int pages)
        {
            var getWebStringTasks = new List<Task<string>>();
            for (int i = 1;i <= pages; i++)
            {
                var uri = letterboxdUri + "/page/" + i;
                getWebStringTasks.Add(client.GetStringAsync(uri));
            }

            return await Task.WhenAll(getWebStringTasks);
        }

        private static async Task<int> GetPageNumbersAsync(HttpClient httpClient, string uri)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            var webstring = await httpClient.GetStringAsync(uri);
            doc.LoadHtml(webstring);
            var nodes = doc.DocumentNode.SelectNodes("//li[contains(@class,'paginate-page')]");
            if (nodes != null && nodes.Any())
            {
                var allPages = nodes.Select(x => x.InnerText);
                var numbers = new List<int>();
                foreach (var page in allPages)
                {
                    int.TryParse(page, out int pageNumber);
                    numbers.Add(pageNumber);
                }
                return numbers.Max();
            }
            return 1;
        }

        private static double? m_width;
        public static double GetWidth()
        {
            if (m_width != null && m_width.HasValue)
                return m_width.Value;
            var width = DeviceDisplay.Current.MainDisplayInfo.Width;
            var density = DeviceDisplay.Current.MainDisplayInfo.Density;
            width = width / density;
            if (width is double.NaN)
            {
                m_width = 230;
                return m_width.Value;
            }
            m_width = width * 0.55;
            if (m_width.Value > 230)
                m_width = 230;
            return m_width.Value;
        }

        private static double? m_height;
        public static double GetHeight()
        {
            if (m_height != null && m_height.HasValue)
                return m_height.Value;
            if (m_width != null && m_width.HasValue)
            {
                m_height = m_width * 1.5; 
            }
            else
            {
                var width = GetWidth();
                m_height = width * 1.5;
            }
            if (m_height.Value > 345)
                m_height = 345;
            return m_height.Value;
        }

        public static void ResetList(MainPageViewModel mainPageViewModel)
        {
            foreach (var genre in mainPageViewModel.GenresList)
                genre.IsSelected = false;
            foreach (var country in mainPageViewModel.CountryList)
                country.IsSelected = false;
            mainPageViewModel.SearchText = null;
            mainPageViewModel.SortAscending = true;
            mainPageViewModel.SelectedLetterboxdList.Clear();
            mainPageViewModel.Rating = null;            
            mainPageViewModel.Movies = mainPageViewModel.AllTheMovies;
            mainPageViewModel.GreaterThanSmallerThan = true;
        }
    }
}
