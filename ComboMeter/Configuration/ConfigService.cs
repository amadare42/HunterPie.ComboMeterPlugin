using System;
using System.IO;
using System.Linq;
using HunterPie.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ComboMeter.Configuration
{
    public static class ConfigService
    {
        public static event Action<ComboMeterConfig> ConfigUpdated;

        public static ComboMeterConfig Current { get; set; }

        private static readonly string SettingsPath = Path.Combine(Path.GetDirectoryName(typeof(ConfigService).Assembly.Location), "config.json");

        public static void Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var text = File.ReadAllText(SettingsPath);
                    Current = JsonConvert.DeserializeObject<ComboMeterConfig>(text);
                    if (Current.ComboGrades == null || Current.ComboGrades.Count == 0)
                        Current.ComboGrades = ComboMeterConfig.DefaultGrades;
                }
                else
                {
                    Current = new ComboMeterConfig();
                }
                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(Current, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    // hack to don't show ServerLogging unless specified
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new JsonConverter[] { new StringEnumConverter() }
                }));
                Apply(Current);
            }
            catch (Exception ex)
            {
                Current = new ComboMeterConfig();
                Apply(Current);
                Debugger.Error($"Error on loading config. Using default one. {ex}");
                // don't write default config to not override user changes in json and don't crash app if saving is failing
            }
            Apply(Current);
        }

        public static void Save()
        {
            File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(Current, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                // hack to don't show ServerLogging unless specified
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new JsonConverter[] { new StringEnumConverter() }
            }));
            Apply(Current);
        }

        private static void Apply(ComboMeterConfig current)
        {
            ConfigUpdated?.Invoke(current);
        }
    }
}