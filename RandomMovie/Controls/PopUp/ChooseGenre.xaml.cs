using CommunityToolkit.Maui.Views;
using RandomMovie.ViewModels;
using System.ComponentModel;

namespace RandomMovie.Controls.PopUp;

public partial class ChooseGenre : Popup, INotifyPropertyChanged
{
    private MainPageViewModel m_mainPageViewModel;
    public ChooseGenre()
	{
		InitializeComponent();        
    }

    public ChooseGenre(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        m_mainPageViewModel = mainPageViewModel;
        BindingContext = mainPageViewModel;
    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        if ((sender as Switch).BindingContext is GenreViewModel genreViewModel)
        {
            genreViewModel.IsSelected = e.Value;
        }
    }

    private void ResetGenreFilter_Clicked(object sender, EventArgs e)
    {
        foreach(var genre in m_mainPageViewModel.GenresList)
        {
            genre.IsSelected = false;
        }
    }

    private async void Close_PopUp(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private void HorizontalStackLayoutTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if ((sender as HorizontalStackLayout).BindingContext is GenreViewModel genreViewModel)
        {
            genreViewModel.IsSelected = !genreViewModel.IsSelected;
        }
    }
}