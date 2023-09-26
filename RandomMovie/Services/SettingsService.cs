using Newtonsoft.Json;

namespace RandomMovie.Services
{
    internal class SettingsService
    {
        private const string SETTINGS_FILE = "settings.json";
        public static SettingsService Instance { get; set; }
        public Settings Settings;
        public SettingsService() 
        {
            ReadSettings();
        }

        private async void ReadSettings()
        {
            Settings = await ReadSettingsAsync();
        }

        public void SaveSettingsAsync()
        {
            var json = JsonConvert.SerializeObject(Settings);
            Services.WriteTextToFileAsync(json, SETTINGS_FILE);
        }

        internal async Task<Settings> ReadSettingsAsync()
        {
            var settingsJson = await Services.ReadTextFileAsync(SETTINGS_FILE);
            if (settingsJson != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<Settings>(settingsJson);
                }
                catch
                {
                    return new Settings();
                }
            }
            else
                return new Settings();
        }

        internal static void Init()
        {
            Instance = new SettingsService();
        }
    }
}
