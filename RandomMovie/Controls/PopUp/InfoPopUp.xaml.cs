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
}