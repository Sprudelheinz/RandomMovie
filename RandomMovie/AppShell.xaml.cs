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
}
