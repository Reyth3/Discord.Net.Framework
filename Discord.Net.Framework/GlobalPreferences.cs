using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Framework
{
    public class GlobalPreferences
    {
        public GlobalPreferences()
        {
        }

        public GlobalPreferences(string instanceId)
        {
            config = new ConcurrentDictionary<string, object>();
            _instance = instanceId;
            ServerSpecific = new ServerPreferences(this);
        }

        [JsonProperty]
        private ConcurrentDictionary<string, object> config;
        [JsonProperty]
        public ServerPreferences ServerSpecific;

        [JsonProperty]
        internal string _instance;

        public static string FilePath { get { return Path.Combine(Directory.GetCurrentDirectory(), "config{iId}.json"); } }

        public static GlobalPreferences LoadFromFile(string instanceId)
        {
            var path = FilePath.Replace("{iId}", instanceId);
            if (File.Exists(path))
            {
                var p = JsonConvert.DeserializeObject<GlobalPreferences>(File.ReadAllText(path));
                return p;
            }
            var prefs = new GlobalPreferences(instanceId);
            prefs.SavePreferences();
            return prefs;
        }

        public void SavePreferences()
        {
            var path = FilePath.Replace("{iId}", _instance);
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public T GetValue<T>(string key, T defValue)
        {
            if (config.TryGetValue(key, out object value))
                if (value is JObject jo)
                    return jo.ToObject<T>();
                else if (value is JArray ja)
                    return ja.ToObject<T>();
                else return (T)value;
            else return defValue;
        }

        public void SetValue(string key, object value)
        {
            config.AddOrUpdate(key, value, (e, a) => value);
            SavePreferences();
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            ServerSpecific.globalPrefs = this;
            foreach (var d in ServerSpecific.config)
                d.Value.globalPrefs = this;
        }
    }
}
