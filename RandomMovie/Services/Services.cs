
using Newtonsoft.Json;
using RandomMovie.ViewModels;
using System.Net;

namespace RandomMovie.Services
{
    public static class Services
    {
        public const string WATCHLIST_FILENAME = "watchlist.json";

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
            using FileStream outputStream = File.OpenWrite(targetFile);
            using StreamWriter streamWriter = new StreamWriter(outputStream);
            await streamWriter.WriteAsync(text);
        }

        public static async Task<string> ReadTextFile(string targetFileName)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            using FileStream InputStream = File.OpenRead(targetFile);
            using StreamReader reader = new StreamReader(InputStream);
            return await reader.ReadToEndAsync();
        }

        internal async static Task ReadWatchlistFromUserAsync(MainPageViewModel mainPageViewModel)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            try
            {
                var pageFound = true;
                int i = 0;
                using (var httpClient = new HttpClient())
                {
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
            }
            catch
            { }
        }
    }
}
