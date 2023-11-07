using CommunityToolkit.Maui.Views;

namespace RandomMovie.Services.Interface
{
    public interface IPopupService
    {
        void ShowPopup(Popup popup);

        Task<object> ShowPopupAsync(Popup popup);
    }
}
