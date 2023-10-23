using CommunityToolkit.Maui.Views;
using RandomMovie.Enums;
using RandomMovie.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace RandomMovie.Controls.PopUp;

public partial class ChooseRatingFilter : Popup, INotifyPropertyChanged
{
    private MainPageViewModel m_mainPageViewModel;
    public ChooseRatingFilter()
	{
		InitializeComponent();        
    }

    public ChooseRatingFilter(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        m_mainPageViewModel = mainPageViewModel;
        BindingContext = mainPageViewModel;
        if (mainPageViewModel.Rating.HasValue)
            RatingSlider.Value = mainPageViewModel.Rating.Value;
    }

    private void ResetGenreFilter_Clicked(object sender, EventArgs e)
    {
        m_mainPageViewModel.Rating = null;
    }

    private async void Close_PopUp(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        m_mainPageViewModel.Rating = Math.Round(e.NewValue, 1);
    }
}