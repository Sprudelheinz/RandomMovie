namespace RandomMovie;

public partial class AppShell : Shell
{
    public AppShell()
	{
		InitializeComponent();
    }

    private void MenuItem_Clicked(object sender, EventArgs e)
    {
        var di = new DirectoryInfo(FileSystem.AppDataDirectory);
        foreach(var file in di.GetFiles()) 
        {
            file.Delete();
        }

        di = new DirectoryInfo(FileSystem.CacheDirectory);
        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }
    }
}
