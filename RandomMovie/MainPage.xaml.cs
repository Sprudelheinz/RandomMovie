using System.Net;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RandomMovie.ViewModels;

namespace RandomMovie;

public partial class MainPage : ContentPage
{
    internal MainPageViewModel MainPageViewModel { get; set; }

    public MainPage()
	{
		InitializeComponent();
        Carousel.CurrentItemChanged += Carousel_CurrentItemChanged;
        MainPageViewModel = new MainPageViewModel();
        BindingContext = MainPageViewModel;
    }

    private void Carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        if (e.CurrentItem is Movie)
            LetterBoxd.Text = (e.CurrentItem as Movie)?.Name;
    }

    void GenerateRandomMovie()
    {
        var randgen = new Random();
        var movie = MainPageViewModel.Movies[randgen.Next(MainPageViewModel.Movies.Count)];
        SetCurrentItem(movie);
    }
    void GenerateRandomMovieWatchlist()
    {
        var randgen = new Random();
        var movie = MainPageViewModel.Watchlist[randgen.Next(MainPageViewModel.Watchlist.Count)];
        SetCurrentItem(movie);
    }

    private void SetCurrentItem(Movie movie)
    {
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
        var currentItem = Carousel.CurrentItem as Movie;
        Carousel.ItemsSource = MainPageViewModel.Movies.Where(x => x.Name.ToLowerInvariant().Contains(searchBar.Text.ToLowerInvariant()));
        if (currentItem != null && string.IsNullOrEmpty(searchBar.Text))
        {
            SetCurrentItem(currentItem);
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VerticalStackLayout vsl && vsl.BindingContext is Movie movie)
        {
            Carousel.CurrentItem = movie;
        }
    }

    private void WatchlistLetterBoxdButton_Clicked(object sender, EventArgs e)
    {
        if (MainPageViewModel.Watchlist.Any())
        {
            Carousel.ItemsSource = MainPageViewModel.Watchlist;
        }
        else
        {
            ActivityIndicator.IsRunning = true;
            Services.Services.ReadWatchlistFromUser(MainPageViewModel);
            Carousel.ItemsSource = MainPageViewModel.Watchlist;
            ActivityIndicator.IsRunning = false;
        }
        GenerateRandomMovieWatchlist();
    }

    
}

