using Android.Graphics;
using CommunityToolkit.Maui.Views;

namespace RandomMovie.Controls.PopUp;

public partial class InfoPopUp : Popup
{
	public InfoPopUp()
	{
		InitializeComponent();    
    }

    private async void Close_PopUp(object sender, EventArgs e)
    {
       await CloseAsync();
    }
    private async void Open_Github(object sender, EventArgs e)
    {
        Uri uri = new Uri("https://github.com/Sprudelheinz/RandomMovie");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        await CloseAsync();
    }
}