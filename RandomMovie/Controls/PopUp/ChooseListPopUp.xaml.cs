using CommunityToolkit.Maui.Views;
using RandomMovie.ViewModels;

namespace RandomMovie.Controls.PopUp;

public partial class ChooseListPopUp : Popup
{
    public ChooseListPopUp()
	{
		InitializeComponent();
    }

    public ChooseListPopUp(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        BindingContext = mainPageViewModel;
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var item = (KeyValuePair<string, string>)e.SelectedItem; 
        (BindingContext as MainPageViewModel).SelectedList = item;
        CloseAsync(item);
    }
}