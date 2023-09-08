using System.Net;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RandomMovie.ViewModels;

namespace RandomMovie;

public partial class MainPage : ContentPage
{
    private MainPageViewModel m_mainPageViewModel;
    public MainPage()
	{
		InitializeComponent();
        Carousel.CurrentItemChanged += Carousel_CurrentItemChanged;
        m_mainPageViewModel = new MainPageViewModel();
        BindingContext = m_mainPageViewModel;
}

    private void Carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        if (e.CurrentItem is Movie)
            LetterBoxd.Text = (e.CurrentItem as Movie)?.Name;
    }

    void GenerateRandomMovie()
    {
        var randgen = new Random();
        var movie = m_mainPageViewModel.Movies[randgen.Next(m_mainPageViewModel.Movies.Count)];
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
        m_mainPageViewModel.Movies = m_mainPageViewModel.AllTheMovies.Where(x => x.Name.ToLowerInvariant().Contains(searchBar.Text.ToLowerInvariant())).ToList();
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
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ActivityIndicator.IsVisible = true;
        });


        if (m_mainPageViewModel.Watchlist.Any())
        {
            m_mainPageViewModel.Movies = m_mainPageViewModel.Watchlist;
        }
        else
        {

            Services.Services.ReadWatchlistFromUser(m_mainPageViewModel);
            m_mainPageViewModel.Movies = m_mainPageViewModel.Watchlist;
        }
        GenerateRandomMovie();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ActivityIndicator.IsVisible = false;
        });
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        m_mainPageViewModel.SortAscending = !m_mainPageViewModel.SortAscending;
        m_mainPageViewModel.Movies = m_mainPageViewModel.SortAscending
                                        ? m_mainPageViewModel.Movies.OrderBy(x => x.SortValue).ToList()
                                        : m_mainPageViewModel.Movies.OrderByDescending(x => x.SortValue).ToList();
    }

    private void RestoreButton_Clicked(object sender, EventArgs e)
    {
        m_mainPageViewModel.Movies = m_mainPageViewModel.AllTheMovies;
    }

    //private void ActivityButton_Tapped(object sender, EventArgs e)
    //{
    //    activityButton.IsInProgress = true;
    //    if (m_mainPageViewModel.Watchlist.Any())
    //    {
    //        m_mainPageViewModel.Movies = m_mainPageViewModel.Watchlist;
    //    }
    //    else
    //    {

    //        Services.Services.ReadWatchlistFromUser(m_mainPageViewModel);
    //        m_mainPageViewModel.Movies = m_mainPageViewModel.Watchlist;
    //    }
    //    GenerateRandomMovie();
    //    activityButton.IsInProgress = false;
    //}
}

