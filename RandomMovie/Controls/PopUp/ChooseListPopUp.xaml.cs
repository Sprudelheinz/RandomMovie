using CommunityToolkit.Maui.Views;
using RandomMovie.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;

namespace RandomMovie.Controls.PopUp;

public partial class ChooseListPopUp : Popup, INotifyPropertyChanged
{
    public Dictionary<string, string> List { get; internal set; }
    public ChooseListPopUp()
	{
		InitializeComponent();
    }

    public ChooseListPopUp(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        List = mainPageViewModel.LetterboxdLists;
        BindingContext = this;
        ListChoosen = new Command<object>(OnTapped);
    }

    public Command<object> ListChoosen { get; set; }

    private async void OnTapped(object obj)
    {
        var item = (KeyValuePair<string, string>)obj;
        await CloseAsync(item);
    }
}