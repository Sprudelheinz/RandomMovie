using Android.App;
using Android.Runtime;
using Android.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace RandomMovie;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
        Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("LightSearch", (h, v) =>
        {

            var children = h.PlatformView.GetChildrenOfType<ImageView>();
            foreach (var child in children)
            {
                child.SetColorFilter(Colors.DarkGray.ToPlatform());
            }
        });
        ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => PrependToMappingImageSource(handler, view));
    }

    public static void PrependToMappingImageSource(IImageHandler handler, Microsoft.Maui.IImage image)
    {
        handler.PlatformView?.Clear();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
