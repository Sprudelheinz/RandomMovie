using Android.App;
using Android.Content.PM;
using Android.OS;

namespace RandomMovie;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {

        base.OnCreate(savedInstanceState);

        RequestedOrientation = ScreenOrientation.Portrait;

    }

    protected override void OnPause()
    {
        base.OnPause();
        if (Accelerometer.Default.IsMonitoring)
        {
            Accelerometer.Default.Stop();
        }
    }

    protected override void OnResume()
    {
        base.OnResume();
        if (!Accelerometer.Default.IsMonitoring)
        {
            Accelerometer.Default.Start(SensorSpeed.Default);
        }
    }
}
