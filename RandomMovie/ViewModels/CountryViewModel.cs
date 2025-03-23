using RandomMovie.Enums;

namespace RandomMovie.ViewModels
{
    public class CountryViewModel : ObservableViewModelBase
    {
        private bool m_isSelected;

        public string Country { get; set; }
        public bool IsSelected 
        { 
            get => m_isSelected; 
            set
            {
                m_isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }
        public CountryViewModel(string country, bool isSelected)
        {
            Country = country;
            IsSelected = isSelected;
        } 
    }
}
