using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Discord.Net.Framework
{
    public class ServerPreferences
    {
        public ServerPreferences()
        {
            config = new Dictionary<ulong, SettingsDictionary>();
        }

        public ServerPreferences(GlobalPreferences prefs)
        {
            config = new Dictionary<ulong, SettingsDictionary>();
            globalPrefs = prefs;
        }

        [JsonProperty]
        internal Dictionary<ulong, SettingsDictionary> config;

        internal GlobalPreferences globalPrefs;

        public SettingsDictionary GetFor(ulong id)
        {
            if (config.ContainsKey(id))
                return config[id];
            else
            {
                var dict = new SettingsDictionary(globalPrefs);
                config.Add(id, dict);
                globalPrefs.SavePreferences();
                return dict;
            }
        }

        public T GetValueFor<T>(ulong id, string key, T defValue)
        {
            var d = GetFor(id);
            if (d.ContainsKey(key))
                return (T)d[key];
            else return defValue;
        }

        public T GetValueForOrGlobal<T>(ulong id, string key)
        {
            var d = GetFor(id);
            if (d.ContainsKey(key))
                return (T)d[key];
            else return globalPrefs.GetValue(key, default(T));
        }

        public void AddOrUpdateFor(ulong id, string key, object value)
        {
            var d = GetFor(id);
            if (d.ContainsKey(key))
                if (value == null)
                    d.Remove(key);
                else d[key] = value;
            else if(value != null)
                d.Add(key, value);
            globalPrefs.SavePreferences();
        }
    }
}
