using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Framework
{
    public class GlobalPreferences
    {
        public GlobalPreferences()
        {
            config = new ConcurrentDictionary<string, object>();
            ServerSpecific = new ServerPreferences() { globalPrefs = this };
        }

        [JsonProperty]
        private ConcurrentDictionary<string, object> config;
        [JsonProperty]
        public ServerPreferences ServerSpecific;

        public static string FilePath { get { return Path.Combine(Directory.GetCurrentDirectory(), "config.json"); } }

        public static GlobalPreferences LoadFromFile()
        {
            if (File.Exists(FilePath))
                return JsonConvert.DeserializeObject<GlobalPreferences>(File.ReadAllText(FilePath));
            var prefs = new GlobalPreferences();
            prefs.SavePreferences();
            return prefs;
        }

        public void SavePreferences()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public T GetValue<T>(string key, T defValue)
        {
            if (config.TryGetValue(key, out object value))
                return (T)value;
            else return defValue;
        }

        public void SetValue(string key, object value)
        {
            config.AddOrUpdate(key, value, (e, a) => value);
            SavePreferences();
        }
    }
}
