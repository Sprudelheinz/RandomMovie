using CommunityToolkit.Maui.Views;
using RandomMovie.ViewModels;
using System.ComponentModel;

namespace RandomMovie.Controls.PopUp;

public partial class ChooseCountry : Popup, INotifyPropertyChanged
{
    private MainPageViewModel m_mainPageViewModel;
    public ChooseCountry()
	{
		InitializeComponent();        
    }

    public ChooseCountry(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        m_mainPageViewModel = mainPageViewModel;
        BindingContext = mainPageViewModel;
    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        if ((sender as Switch).BindingContext is CountryViewModel countryViewModel)
        {
            countryViewModel.IsSelected = e.Value;
        }
    }

    private void ResetCountryFilter_Clicked(object sender, EventArgs e)
    {
        foreach(var country in m_mainPageViewModel.CountryList)
        {
            country.IsSelected = false;
        }
    }

    private async void Close_PopUp(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private void HorizontalStackLayoutTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if ((sender as HorizontalStackLayout).BindingContext is CountryViewModel countryViewModel)
        {
            countryViewModel.IsSelected = !countryViewModel.IsSelected;
        }
    }
}