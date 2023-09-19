using RandomMovie.ViewModels;

namespace RandomMovie;

public partial class AppShell : Shell
{
    public AppShell()
	{
		InitializeComponent();
    }

    private void MenuItem_Clicked(object sender, EventArgs e)
    {
        var mainPage = CurrentPage as MainPage;
        if (mainPage != null) 
        {
            var mainPageViewModel = mainPage.BindingContext as MainPageViewModel;
            mainPageViewModel.Watchlist.Clear();
            mainPageViewModel.Movies = mainPageViewModel.AllTheMovies;
            mainPageViewModel.LetterBoxdUserName = string.Empty;
        }

        var watchlistFileName = Path.Combine(FileSystem.Current.AppDataDirectory, Services.Services.WATCHLIST_FILENAME);
        if (File.Exists(watchlistFileName))
        {
            File.Delete(watchlistFileName);
        }
        FlyoutIsPresented = false;
    }

    private void ResetPosterCache_Clicked(object sender, EventArgs e)
    {       
        var di = new DirectoryInfo(Path.Combine(FileSystem.Current.AppDataDirectory, Services.Services.POSTER_CACHE_FOLDER));
        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }
        var mainPage = CurrentPage as MainPage;
        if (mainPage != null)
        {
            var mainPageViewModel = mainPage.BindingContext as MainPageViewModel;
            foreach (var movie in mainPageViewModel.Movies)
                movie.PosterImageSource = null;
        }
        FlyoutIsPresented = false;
    }

    private void ChangeMode_Clicked(object sender, EventArgs e)
    {
        var currentTheme = Application.Current.RequestedTheme;
        if (currentTheme == AppTheme.Dark)
            Application.Current.UserAppTheme = AppTheme.Light;
        else
            Application.Current.UserAppTheme = AppTheme.Dark;
        var mainPage = CurrentPage as MainPage;
        if (mainPage != null)
        {
            var mainPageViewModel = mainPage.BindingContext as MainPageViewModel;
            mainPageViewModel.Settings.Theme = Application.Current.UserAppTheme;
            Services.Services.SaveSettings(mainPageViewModel.Settings);
        }
        FlyoutIsPresented = false;
    }
}
