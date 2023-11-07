using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using RandomMovie.Services;
using RandomMovie.Services.Interface;

namespace RandomMovie;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
        SettingsService.Init();
        builder.Services.AddTransient<IPopupService, PopupService>();
        return builder.Build();
	}
}
