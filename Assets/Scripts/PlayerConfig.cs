using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble
{
    public static class PlayerConfig
    {
        #region Settings

        public static PlayerSettingFloat MasterVolume;
        public static PlayerSettingFloat BGMVolume;
        public static PlayerSettingFloat SFXVolume;
        public static PlayerSettingFloat CharacterDelay;

        public static PlayerSettingBool ShowCluesAlerts;
        public static PlayerSettingBool ShowCardGameWarning;

        private static List<PlayerSetting> settings;

        #endregion

        #region Default Values

        private const float MasterBusVolumeDefaultValue = 1;
        private const float BGMBusVolumeDefaultValue = 1;
        private const float SFXBusVolumeDefaultValue = 1;
        private const float CharacterDelayDefaultValue = 0.01f;
        private const bool ShowCluesAlertsDefaultValue = true;
        private const bool ShowCardGameWarningDefaultValue = true;

        #endregion

        #region Keys

        private const string MasterBusVolumeKey = "MasterBusVolume";
        private const string BGMBusVolumeKey = "BGMBusVolume";
        private const string SFXBusVolumeKey = "SFXBusVolume";
        private const string CharacterDelayValueKey = "CharacterDelayValue";
        private const string ShowCluesAlertsKey = "ShowCluesAlerts";
        private const string ShowCardGameWarningKey = "ShowCardGameWarning";

        #endregion

        public static void InitializeValues()
        {
            MasterVolume = new PlayerSettingFloat(MasterBusVolumeKey, MasterBusVolumeDefaultValue);
            BGMVolume = new PlayerSettingFloat(BGMBusVolumeKey, BGMBusVolumeDefaultValue);
            SFXVolume = new PlayerSettingFloat(SFXBusVolumeKey, SFXBusVolumeDefaultValue);
            CharacterDelay = new PlayerSettingFloat(CharacterDelayValueKey, CharacterDelayDefaultValue);
            ShowCluesAlerts = new PlayerSettingBool(ShowCluesAlertsKey, ShowCluesAlertsDefaultValue);
            ShowCardGameWarning = new PlayerSettingBool(ShowCardGameWarningKey, ShowCardGameWarningDefaultValue);

            settings = new List<PlayerSetting>()
            {
                MasterVolume, BGMVolume, SFXVolume, CharacterDelay, ShowCluesAlerts, ShowCardGameWarning
            };
        }

        public static void SetPlayerPrefs()
        {
            foreach(PlayerSetting setting in settings)
            {
                setting.SetValue();
            }
        }

        #region Classes

        public abstract class PlayerSetting
        {
            protected string key;

            public PlayerSetting(string key)
            {
                this.key = key;
            }

            public abstract void SetValue();

        }

        public class PlayerSettingFloat : PlayerSetting
        {
            public float Value { private set; get; }

            public PlayerSettingFloat(string key, float defaultValue) : base(key)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    Value = PlayerPrefs.GetFloat(key);
                }
                else
                {
                    Value = defaultValue;
                }
            }

            public override void SetValue()
            {
                PlayerPrefs.SetFloat(key, Value);
            }

            public void SetValue(float value)
            {
                Value = value;
                PlayerPrefs.SetFloat(key, value);
            }

        }

        public class PlayerSettingBool : PlayerSetting
        {
            public bool Value { private set; get; }

            public PlayerSettingBool(string key, bool defaultValue) : base(key)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    Value = IntToBool(PlayerPrefs.GetInt(key));
                }
                else
                {
                    SetValue(defaultValue);
                }
            }

            public override void SetValue()
            {
                PlayerPrefs.SetInt(key, BoolToInt(Value));
            }

            public void SetValue(bool value)
            {
                Value = value;
                PlayerPrefs.SetInt(key, BoolToInt(value));
            }

            private static int BoolToInt(bool value)
            {
                return value ? 1 : 0;
            }

            private static bool IntToBool(int value)
            {
                return value == 1;
            }
        }

        #endregion
    }
}