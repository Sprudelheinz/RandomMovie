using RandomMovie.Enums;

namespace RandomMovie.ViewModels
{
    public class GenreViewModel : ObservableViewModelBase
    {
        private bool m_isSelected;

        public Genre Genre { get; set; }
        public bool IsSelected 
        { 
            get => m_isSelected; 
            set
            {
                m_isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }
        public GenreViewModel(Genre genre, bool isSelected)
        {
            Genre = genre;
            IsSelected = isSelected;
        } 
    }
}
