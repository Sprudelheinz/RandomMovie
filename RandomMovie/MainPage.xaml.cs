using CommunityToolkit.Maui.Views;
using RandomMovie.Controls.PopUp;
using RandomMovie.Resources;
using RandomMovie.Services;
using RandomMovie.ViewModels;
using System;

namespace RandomMovie;

public partial class MainPage : ContentPage
{
    private const int CAROUSEL_HEIGHTREQUEST = 500;
    private const int CAROUSEL_PEEKAREAINSETS = 220;
    private const int MAX_MOVIES_FOR_ANIMATE = 200;
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
        Carousel.PeekAreaInsets = new Thickness((width - CAROUSEL_PEEKAREAINSETS) / 2);
#else
        Carousel.HeightRequest = CAROUSEL_HEIGHTREQUEST;
#endif

        //int i = 0;
        //var timer = Application.Current.Dispatcher.CreateTimer();
        //timer.Interval = TimeSpan.FromSeconds(1);
        //timer.Tick += (s, e) =>
        //{

        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        if (m_mainPageViewModel.Movies.Any())
        //        {
        //            Carousel.Position = (Carousel.Position + 1) % m_mainPageViewModel.Movies.Count;
        //            i++;
        //        }
        //    });
        //    if (i == 5)
        //        timer.Stop();
        //};
        //timer.Start();
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
        if (m_mainPageViewModel.Movies.Count > MAX_MOVIES_FOR_ANIMATE)
        {
            Carousel.IsScrollAnimated = false;
            //Carousel.SetBinding(CarouselView.CurrentItemProperty, "Current", BindingMode.TwoWay);
            Carousel.CurrentItem = movie;
            Carousel.ScrollTo(movie, animate: false, position: ScrollToPosition.Center);
            Carousel.IsScrollAnimated = true;
        }
        else
        { 
            //Carousel.SetBinding(CarouselView.CurrentItemProperty, "Current", BindingMode.TwoWay);
            Carousel.CurrentItem = movie;
            Carousel.ScrollTo(movie, animate: true, position: ScrollToPosition.Center);
        }
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
        m_mainPageViewModel.Movies = m_mainPageViewModel.AllTheMovies
                                        .Where(x => x.MovieTitle.ToLowerInvariant().Contains(searchBar.Text.ToLowerInvariant())).ToList();
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
        var activityIndicatorPopUp = new ActivityIndicatorPopUp();
        if (string.IsNullOrEmpty(m_mainPageViewModel.LetterBoxdUserName))
        {
            SettingsService.Instance.Settings.LetterBoxdUserName = null;
            SettingsService.Instance.SaveSettingsAsync();
            m_mainPageViewModel.SelectedLetterboxdList = new List<Movie>();
            return;
        }
        m_mainPageViewModel.LetterBoxdUserName = m_mainPageViewModel.LetterBoxdUserName.Trim();
        if (m_mainPageViewModel.LetterboxdLists == null 
            || !m_mainPageViewModel.LetterboxdLists.Any()
            || (!string.IsNullOrEmpty(m_mainPageViewModel.LetterBoxdUserName) 
                && m_mainPageViewModel.LetterBoxdUserName != SettingsService.Instance.Settings.LetterBoxdUserName))
        {

            this.ShowPopup(activityIndicatorPopUp);
            var lists = await Services.Services.GetListsFromUserName(m_mainPageViewModel);
            await activityIndicatorPopUp.CloseAsync();

            if (lists == null)
            {
                var infoMessagePopUp = new InfoMessagePopUp(Localisation.ErrorLetterboxdName, Enums.InfoMessageAnswer.Ok);
                var value = await this.ShowPopupAsync(infoMessagePopUp);
                return;
            }
            
            m_mainPageViewModel.LetterboxdLists = lists;
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

        if (m_mainPageViewModel.SelectedLetterboxdList.Any())
        {
            m_mainPageViewModel.Movies = m_mainPageViewModel.SelectedLetterboxdList;
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

