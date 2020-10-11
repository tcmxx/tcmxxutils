using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public abstract class PlayerPrefValue<T>
    {
        protected static Dictionary<string, T> playerPrefabCache = new Dictionary<string, T>();

        public string Key { get; private set; }
        public T DefaultValue { get; set; }

        public PlayerPrefValue(string key, T defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                T result;
                if (playerPrefabCache.TryGetValue(Key, out result))
                    return result;
                result = GetFromPlayerPreferece();
                playerPrefabCache[Key] = result;
                return result;
            }
            set
            {
                SetToPlayerPreferece(value);
                playerPrefabCache[Key] = value;
            }
        }

        protected abstract T GetFromPlayerPreferece();
        protected abstract void SetToPlayerPreferece(T value);
    }


    public class PlayerPrefInt : PlayerPrefValue<int>
    {
        public PlayerPrefInt(string key, int defaultValue = 0) : base(key, defaultValue)
        {
        }

        protected override int GetFromPlayerPreferece()
        {
            return PlayerPrefs.GetInt(Key, DefaultValue);
        }

        protected override void SetToPlayerPreferece(int value)
        {
            PlayerPrefs.SetInt(Key, value);
        }
    }

    public class PlayerPrefFloat : PlayerPrefValue<float>
    {
        public PlayerPrefFloat(string key, float defaultValue = 0) : base(key, defaultValue)
        {
        }

        protected override float GetFromPlayerPreferece()
        {
            return PlayerPrefs.GetFloat(Key, DefaultValue);
        }

        protected override void SetToPlayerPreferece(float value)
        {
            PlayerPrefs.SetFloat(Key, value);
        }
    }

    public class PlayerPrefBool : PlayerPrefValue<bool>
    {
        public PlayerPrefBool(string key, bool defaultValue = false) : base(key, defaultValue)
        {
        }

        protected override bool GetFromPlayerPreferece()
        {
            return PlayerPrefs.GetInt(Key, DefaultValue ? 1 : 0) > 0;
        }

        protected override void SetToPlayerPreferece(bool value)
        {
            PlayerPrefs.SetInt(Key, value ? 1 : 0);
        }
    }

    public class PlayerPrefString : PlayerPrefValue<string>
    {
        public PlayerPrefString(string key, string defaultValue = "") : base(key, defaultValue)
        {
        }

        protected override string GetFromPlayerPreferece()
        {
            return PlayerPrefs.GetString(Key, DefaultValue);
        }

        protected override void SetToPlayerPreferece(string value)
        {
            PlayerPrefs.SetString(Key, value);
        }
    }
}