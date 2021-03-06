﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Discord.Net.Framework
{
    public class SettingsDictionary : Dictionary<string, object>
    {
        public SettingsDictionary()
        {
        }

        public SettingsDictionary(GlobalPreferences prefs) : base()
        {
            globalPrefs = prefs;
        }

        internal GlobalPreferences globalPrefs;
        // Quick Access Properties
        public string CommandPrefix { get { return GetValueOrGlobal<string>("prefix"); } set { AddOrUpdate("prefix", value); } }


        public T GetValue<T>(string key, T defValue)
        {
            if (ContainsKey(key))
            {
                if (this[key] is JObject)
                    return (this[key] as JObject).ToObject<T>();
                return (T)this[key];
            }
            else return defValue;
        }

        public T GetValueOrGlobal<T>(string key)
        {
            if (ContainsKey(key))
            {
                if (this[key] is JObject)
                    return (this[key] as JObject).ToObject<T>();
                return (T)this[key];
            }
            else return globalPrefs.GetValue(key, default(T));
        }

        public void AddOrUpdate(string key, object value)
        {
            if (ContainsKey(key))
                if (value == null)
                    Remove(key);
                else this[key] = value;
            else if (value != null)
                Add(key, value);
            SavePreferences();
        }

        public void SavePreferences()
        {
            globalPrefs.SavePreferences();
        }
    }
}
