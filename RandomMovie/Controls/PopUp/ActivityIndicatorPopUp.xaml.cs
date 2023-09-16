using CommunityToolkit.Maui.Views;

namespace RandomMovie.Controls.PopUp;

public partial class ActivityIndicatorPopUp : Popup
{
	public ActivityIndicatorPopUp()
	{
		InitializeComponent();
        CanBeDismissedByTappingOutsideOfPopup = false;
    }
}