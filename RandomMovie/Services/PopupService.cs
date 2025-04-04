﻿using CommunityToolkit.Maui.Views;
using RandomMovie.Services.Interface;

namespace RandomMovie.Services
{
    public class PopupService : IPopupService
    {
        public void ShowPopup(Popup popup)
        {
            Page page = Application.Current.Windows[0]?.Page ?? throw new NullReferenceException();
            page.ShowPopup(popup);
        }

        public Task<object> ShowPopupAsync(Popup popup)
        {
            Page page = Application.Current.Windows[0]?.Page ?? throw new NullReferenceException();
            return page.ShowPopupAsync(popup);
        }
    }
}
