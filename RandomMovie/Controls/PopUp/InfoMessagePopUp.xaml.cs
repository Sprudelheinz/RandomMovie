using CommunityToolkit.Maui.Views;
using RandomMovie.Enums;
using RandomMovie.ViewModels;
using System.ComponentModel;

namespace RandomMovie.Controls.PopUp;

public partial class InfoMessagePopUp : Popup, INotifyPropertyChanged
{
    public InfoMessagePopUp(string message, InfoMessageAnswer infoMessageAnswer)
	{
		InitializeComponent();
        CanBeDismissedByTappingOutsideOfPopup = false;
        BindingContext = new InfoMessagePopUpViewModel(message, infoMessageAnswer);
    }

    private async void YesClose_PopUp(object sender, EventArgs e)
    {
        await CloseAsync(true);
    }


    private async void NoClose_PopUp(object sender, EventArgs e)
    {
        await CloseAsync(false);
    }
}