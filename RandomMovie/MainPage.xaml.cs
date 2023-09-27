using CommunityToolkit.Maui.Views;
using RandomMovie.Controls.PopUp;
using RandomMovie.Services;
using RandomMovie.ViewModels;
using System;

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
        var width = DeviceDisplay.Current.MainDisplayInfo.Width;
        var density = DeviceDisplay.Current.MainDisplayInfo.Density;
        width = width / density;
#if ANDROID
        Carousel.PeekAreaInsets = new Thickness((width - 220) / 2);
#else
        Carousel.HeightRequest = 500;
#endif
    }

    private void Carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        if (e.CurrentItem is Movie)
            LetterBoxd.Text = (e.CurrentItem as Movie)?.MovieTitle;
    }

    void GenerateRandomMovie()
    {
        var randgen = new Random();
        if (m_mainPageViewModel.Movies.Any())
        {
            var movie = m_mainPageViewModel.Movies[randgen.Next(m_mainPageViewModel.Movies.Count)];
            SetCurrentItem(movie);
        }
    }

    private void SetCurrentItem(Movie movie)
    {
        Carousel.IsScrollAnimated = false;
        Carousel.SetBinding(CarouselView.CurrentItemProperty, "Current", BindingMode.TwoWay);
        Carousel.CurrentItem = movie;
        Carousel.ScrollTo(movie, animate: false, position: ScrollToPosition.Center);
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
    private async void ChooseList_Clicked(object sender, EventArgs e)
    {
        var popUp = new ActivityIndicatorPopUp();
        if (string.IsNullOrEmpty(m_mainPageViewModel.LetterBoxdUserName))
        {
            SettingsService.Instance.Settings.LetterBoxdUserName = null;
            SettingsService.Instance.SaveSettingsAsync();
            m_mainPageViewModel.Watchlist = new List<Movie>();
            return;
        }
        if (m_mainPageViewModel.List == null || !m_mainPageViewModel.List.Any())
        {

            this.ShowPopup(popUp);
            var lists = await Services.Services.GetListsFromUserName(m_mainPageViewModel);
            try
            {
                popUp.Close();
            }
            catch
            {
            }
            m_mainPageViewModel.List = lists;
        }
        var chooseListPopUp = new ChooseListPopUp(m_mainPageViewModel);
        var result = await this.ShowPopupAsync(chooseListPopUp);
        if (result != null && result is KeyValuePair<string, string>)
        {
            var uri = $"https://letterboxd.com" + ((KeyValuePair<string, string>)result).Value;
            GetListFromUri(uri);
        }
    }

    private async void GetListFromUri(string uri)
    {
        ActivityIndicatorPopUp popUp = new ActivityIndicatorPopUp();
        this.ShowPopup(popUp);
        await Services.Services.ReadListFromUri(uri, m_mainPageViewModel);

        if (m_mainPageViewModel.Watchlist.Any())
        {
            m_mainPageViewModel.Movies = m_mainPageViewModel.Watchlist;
            GenerateRandomMovie();
        }
        else
        {
            m_mainPageViewModel.Movies = m_mainPageViewModel.AllTheMovies;
        }
        try
        {
            popUp.Close();
        }
        catch
        {
        }
    }

    private async void WatchlistLetterBoxdButton_Clicked(object sender, EventArgs e)
    {
        if (m_mainPageViewModel.Watchlist.Any() 
            && m_mainPageViewModel.LetterBoxdUserName == SettingsService.Instance.Settings.LetterBoxdUserName)
        {
            m_mainPageViewModel.Movies = m_mainPageViewModel.Watchlist;
            GenerateRandomMovie();
        }
        else
        {
            if (string.IsNullOrEmpty(m_mainPageViewModel.LetterBoxdUserName))
            {
                SettingsService.Instance.Settings.LetterBoxdUserName = null;
                SettingsService.Instance.SaveSettingsAsync();
                m_mainPageViewModel.Watchlist = new List<Movie>();
                return;
            }
            var watchlistUri = $"https://letterboxd.com/" + m_mainPageViewModel.LetterBoxdUserName + $"/watchlist";
            GetListFromUri(watchlistUri);
        }     
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
        SetCurrentItem(m_mainPageViewModel.Movies.First());
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        var popUp = new InfoPopUp();
        this.ShowPopup(popUp);
    }
}

