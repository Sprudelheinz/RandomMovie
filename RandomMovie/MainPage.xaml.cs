using CommunityToolkit.Maui.Views;
using RandomMovie.Controls.PopUp;
using RandomMovie.Resources;
using RandomMovie.Services;
using RandomMovie.ViewModels;
using System;
using System.Reflection.Metadata;

namespace RandomMovie;

public partial class MainPage : ContentPage
{
    private const int CAROUSEL_HEIGHTREQUEST = 500;
    private const int MAX_MOVIES_FOR_ANIMATE = 200;
    private const int MARGIN_PEEK = 20;
    private MainPageViewModel m_mainPageViewModel;
    public MainPage()
	{
		InitializeComponent();
        Carousel.CurrentItemChanged += Carousel_CurrentItemChanged;
        m_mainPageViewModel = new MainPageViewModel();
        BindingContext = m_mainPageViewModel;
        var width = DeviceDisplay.Current.MainDisplayInfo.Width;
        var density = DeviceDisplay.Current.MainDisplayInfo.Density;
        var peekAreaInsets = Services.Services.GetWidth() + MARGIN_PEEK;
        width = width / density;
#if ANDROID
        Carousel.PeekAreaInsets = new Thickness((width - peekAreaInsets) / 2);
#else
        Carousel.HeightRequest = CAROUSEL_HEIGHTREQUEST;
#endif
        ToggleAcclerometer();
        //ToggleMagnetometer();
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
            var infoMessagePopUp = new InfoMessagePopUp(Localisation.ErrorLetterboxdName, Enums.InfoMessageAnswer.Ok);
            var value = await this.ShowPopupAsync(infoMessagePopUp);
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

    private void ToggleAcclerometer()
    {
        if (Accelerometer.Default.IsSupported)
        {
            if (!Accelerometer.Default.IsMonitoring)
            {
                // Turn on accelerometer
                Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
                Accelerometer.Default.ReadingChanged += Default_ReadingChanged;
                Accelerometer.Default.Start(SensorSpeed.Game);
            }
            else
            {
                // Turn off accelerometer
                Accelerometer.Default.Stop();
                Accelerometer.Default.ReadingChanged -= Default_ReadingChanged;
                Accelerometer.Default.ShakeDetected -= Accelerometer_ShakeDetected;
            }
        }
    }

    private void Accelerometer_ShakeDetected(object sender, EventArgs e)
    {
        GenerateRandomMovie();
    }

    private void Default_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        m_accelerometerFieldX = Math.Round(e.Reading.Acceleration.X, 2);
        var magneticFieldY = Math.Round(e.Reading.Acceleration.Y, 2);
        var magneticFieldZ = Math.Round(e.Reading.Acceleration.Z, 2);

        AccelerometerLabel.Text = $"x: {m_accelerometerFieldX} y: {magneticFieldY} z: {magneticFieldZ}";
        if (m_accelerometerFieldX < -0.15)
            StartScroll();
        if (m_accelerometerFieldX > 0.15)
            StartScroll();
        if (m_timer != null && m_timer.IsRunning && m_accelerometerFieldX > -0.15 && m_accelerometerFieldX < 0.15)
        {
            m_timer.Stop();
            m_oldInterval = null;
        }
    }

    
    private IDispatcherTimer m_timer;
    private double m_accelerometerFieldX;

    public double Map(double value, double fromSource, double toSource, double fromTarget, double toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
    private double? m_oldInterval = null;
    private void StartScroll()
    {
        int i = 0;
        if (m_timer == null)
        {
            m_timer = Application.Current.Dispatcher.CreateTimer();
            m_timer.Tick += (s, e) =>
            {
                SetNewMovie();
            };
        }
        var xAbsValue = Math.Abs(m_accelerometerFieldX);
        double interval;
        if (xAbsValue > 0.6)
            interval = 0.3;
        else
            interval = Math.Round(Map(xAbsValue, 0.15, 0.6, 0.8, 0.3), 1);
        if (m_oldInterval == null)
        { 
            m_timer.Interval = TimeSpan.FromSeconds(interval);
            m_oldInterval = interval;
            m_timer.Start();
        }
        else if (m_oldInterval != interval)
        {
            m_timer.Stop();
            m_timer.Interval = TimeSpan.FromSeconds(interval);
            m_oldInterval = interval;
            m_timer.Start();
        }
    }

    private void SetNewMovie()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (m_mainPageViewModel.Movies.Any())
            {
                var movie = Carousel.CurrentItem as Movie;
                var index = m_mainPageViewModel.Movies.IndexOf(movie);
                Movie newMovie = null;
                if (m_accelerometerFieldX < 0)
                    newMovie = m_mainPageViewModel.Movies[++index];
                if (m_accelerometerFieldX > 0 && index > 0)
                    newMovie = m_mainPageViewModel.Movies[--index];
                if (newMovie != null)
                    Carousel.CurrentItem = newMovie;
            }
        });
    }
}

