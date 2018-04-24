using Newtonsoft.Json;
using System;
using System.IO;

namespace DotsGame.GUI
{
    public class Settings
    {
        private static string settingsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DotsGame.json");
        private static readonly object saveLock = new object();

        public string CurrentGameSgf { get; set; } = "";

        public string OpenedFileName { get; set; } = "";

        public static Settings Load()
        {
            if (File.Exists(settingsFileName))
            {
                try
                {
                    var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsFileName)) ?? new Settings();
                    return settings;
                }
                catch
                {
                    return new Settings();
                }
            }
            else
            {
                return new Settings();
            }
        }

        public void Save()
        {
            lock (saveLock)
            {
                File.WriteAllText(settingsFileName, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }
    }
}
