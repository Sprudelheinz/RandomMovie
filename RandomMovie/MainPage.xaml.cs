using System.Diagnostics;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RandomMovie.Extensions;

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
        if (e.CurrentItem is Movie)
            LetterBoxd.Text = (e.CurrentItem as Movie)?.Name;
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
                //GenerateRandomMovie();

            }
        }
    }

    void GenerateRandomMovie()
    {
        var randgen = new Random();
        var movie = Movies[randgen.Next(Movies.Count)];
        Carousel.IsScrollAnimated = false;
        Carousel.SetBinding(CarouselView.CurrentItemProperty, "Current", BindingMode.TwoWay);
        Carousel.CurrentItem = movie;
        Carousel.ScrollTo(movie, animate: false);
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
            if (movie == null)
                return;
            Uri uri = new Uri(movie.LetterboxdURI);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch
        { }
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        Carousel.ItemsSource = Movies.Where(x => x.Name.ToLowerInvariant().Contains(searchBar.Text.ToLowerInvariant()));
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VerticalStackLayout vsl && vsl.BindingContext is Movie movie)
        {
            Carousel.CurrentItem = movie;
        }
    }
}

