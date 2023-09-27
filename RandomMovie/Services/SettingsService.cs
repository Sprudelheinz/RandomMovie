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
            Settings = ReadSettings();
        }

        public void SaveSettingsAsync()
        {
            var json = JsonConvert.SerializeObject(Settings);
            Services.WriteTextToFileAsync(json, SETTINGS_FILE);
        }

        internal Settings ReadSettings()
        {
            var settingsJson = Services.ReadTextFile(SETTINGS_FILE);
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
