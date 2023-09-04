using System.IO.Compression;
using Newtonsoft.Json;

namespace RandomMovie;

public partial class MainPage : ContentPage
{
    List<Movie> Movies { get; set; }
	public MainPage()
	{
		InitializeComponent();
        ReadJsonAsync();
        Carousel.CurrentItemChanged += Carousel_CurrentItemChanged;
    }

    private void Carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        LetterBoxd.Text = (e.CurrentItem as Movie).Name;
    }

    private async void ReadJsonAsync()
    {
        using (var stream = await FileSystem.OpenAppPackageFileAsync("brightToDark.json"))
        {

            using (StreamReader reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                if (string.IsNullOrEmpty(json))
                {
                }
                var movies = JsonConvert.DeserializeObject<List<Movie>>(json);
                Movies = movies.ToList();
                Carousel.ItemsSource = Movies;
                GenerateRandomMovie();

            }
        }
    }

    void GenerateRandomMovie()
    {
        var randgen = new Random();
        var movie = Movies[randgen.Next(Movies.Count)];
        Carousel.IsScrollAnimated = false;
        Carousel.CurrentItem = movie;
        Carousel.IsScrollAnimated = true;
    }


    private void RandomMovie_Clicked(object sender, EventArgs e)
    {
        GenerateRandomMovie();
    }


    private async void LetterBoxd_Clicked(object sender, EventArgs e)
    {
        try
        {
            var movie = Carousel.CurrentItem as Movie;
            Uri uri = new Uri(movie.LetterboxdURI);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch
        { }
    }
}

