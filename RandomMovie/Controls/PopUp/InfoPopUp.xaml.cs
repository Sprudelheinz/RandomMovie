using CommunityToolkit.Maui.Views;
using RandomMovie.Resources;

namespace RandomMovie.Controls.PopUp;

public partial class InfoPopUp : Popup
{
    public string Version => Localisation.Version + ": " + AppInfo.Current.VersionString;
    public InfoPopUp()
	{
		InitializeComponent();
        BindingContext = this;
    }

    private async void Close_PopUp(object sender, EventArgs e)
    {
       await CloseAsync();
    }
    private async void Open_Github(object sender, EventArgs e)
    {
        Uri uri = new("https://github.com/Sprudelheinz/RandomMovie");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        await CloseAsync();
    }

    private async void Open_Market(object sender, EventArgs e)
    {
        Uri uri = new("https://play.google.com/store/apps/details?id=com.sprudelheinz.randommovie");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        await CloseAsync();
    }
}