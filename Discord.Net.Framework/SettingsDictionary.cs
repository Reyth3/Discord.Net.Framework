using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Net.Framework
{
    public class SettingsDictionary : Dictionary<string, object>
    {
        public SettingsDictionary(GlobalPreferences prefs) : base()
        {
            globalPrefs = prefs;
        }

        private GlobalPreferences globalPrefs;

        // Quick Access Properties
        public string CommandPrefix => GetValueOrGlobal<string>("prefix");


        public T GetValue<T>(string key, T defValue)
        {
            if (ContainsKey(key))
                return (T)this[key];
            else return defValue;
        }

        public T GetValueOrGlobal<T>(string key)
        {
            if (ContainsKey(key))
                return (T)this[key];
            else return globalPrefs.GetValue(key, default(T));
        }

        public void AddOrUpdate(ulong id, string key, object value)
        {
            if (ContainsKey(key))
                if (value == null)
                    Remove(key);
                else this[key] = value;
            else if (value != null)
                Add(key, value);
            globalPrefs.SavePreferences();
        }

        public void SavePreferences()
        {
            globalPrefs.SavePreferences();
        }
    }
}
