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
        }
        var di = new DirectoryInfo(FileSystem.AppDataDirectory);
        foreach(var file in di.GetFiles()) 
        {
            file.Delete();
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
}
