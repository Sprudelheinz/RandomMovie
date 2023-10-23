using RandomMovie.Services;
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
            foreach (var genre in mainPageViewModel.GenresList)
                genre.IsSelected = false;
            mainPageViewModel.SearchText = null;
            mainPageViewModel.SortAscending = true;
            mainPageViewModel.SelectedLetterboxdList.Clear();
            mainPageViewModel.Rating = null;
            mainPageViewModel.Movies = mainPageViewModel.AllTheMovies;  
        }

        var letterboxdListFileName = Path.Combine(FileSystem.Current.AppDataDirectory, Services.Services.LETTERBOXDLIST_FILENAME);
        if (File.Exists(letterboxdListFileName))
        {
            File.Delete(letterboxdListFileName);
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
        SettingsService.Instance.Settings.Theme = Application.Current.UserAppTheme;
        SettingsService.Instance.SaveSettingsAsync();
        FlyoutIsPresented = false;
    }
}
