using CommunityToolkit.Maui.Views;
using RandomMovie.Enums;
using System.ComponentModel;

namespace RandomMovie.Controls.PopUp;

public partial class InfoMessagePopUp : Popup, INotifyPropertyChanged
{
    public string Message { get; set; }
    public bool OkVisible { get; set; } = false;
    public bool YesVisible { get; set; } = false;
    public bool NoVisible { get; set; } = false;
    public bool CloseVisible { get; set; } = false;

    public InfoMessagePopUp(string message, InfoMessageAnswer infoMessageAnswer)
	{
		InitializeComponent();
        CanBeDismissedByTappingOutsideOfPopup = false;
        Message = message;      
        switch (infoMessageAnswer)
        {
            case InfoMessageAnswer.Ok:
                OkVisible = true;
                break;
            case InfoMessageAnswer.YesNo:
                YesVisible = true;
                NoVisible = true;
                break;
            case InfoMessageAnswer.Close:
                CloseVisible = true;
                break;
        }
        BindingContext = this;
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