using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble
{
    public static class PlayerConfig
    {
        private static float MasterVolume;
        private static float BGMVolume;
        private static float SFXVolume;

        private static float CharacterDelay;

        private static bool ShowCluesAlerts;
        private static bool ShowCardGameWarning;

        private const float MasterBusVolumeDefaultValue = 1;
        private const float BGMBusVolumeDefaultValue = 1;
        private const float SFXBusVolumeDefaultValue = 1;
        private const float CharacterDelayDefaultValue = 0.01f;
        private const bool ShowCluesAlertsDefaultValue = true;
        private const bool ShowCardGameWarningDefaultValue = true;

        private const string MasterBusVolumeKey = "MasterBusVolume";
        private const string BGMBusVolumeKey = "BGMBusVolume";
        private const string SFXBusVolumeKey = "SFXBusVolume";
        private const string CharacterDelayValueKey = "CharacterDelayValue";
        private const string ShowCluesAlertsKey = "ShowCluesAlerts";
        private const string ShowCardGameWarningKey = "ShowCardGameWarning";

        public static void InitializeValues()
        {
            if (PlayerPrefs.HasKey(MasterBusVolumeKey))
            {
                MasterVolume = PlayerPrefs.GetFloat(MasterBusVolumeKey);
            }
            else
            {
                SetMasterVolume(MasterBusVolumeDefaultValue);
            }

            if (PlayerPrefs.HasKey(BGMBusVolumeKey))
            {
                BGMVolume = PlayerPrefs.GetFloat(BGMBusVolumeKey);
            }
            else
            {
                SetBGMVolume(BGMBusVolumeDefaultValue);
            }

            if (PlayerPrefs.HasKey(SFXBusVolumeKey))
            {
                SFXVolume = PlayerPrefs.GetFloat(SFXBusVolumeKey);
            }
            else
            {
                SetSFXVolume(SFXBusVolumeDefaultValue);
            }

            if (PlayerPrefs.HasKey(CharacterDelayValueKey))
            {
                CharacterDelay = PlayerPrefs.GetFloat(CharacterDelayValueKey);
            }
            else
            {
                SetCharacterDelay(CharacterDelayDefaultValue);
            }
            
            if (PlayerPrefs.HasKey(ShowCluesAlertsKey))
            {
                ShowCluesAlerts = IntToBool(PlayerPrefs.GetInt(ShowCluesAlertsKey));
            }
            else
            {
                SetShowCluesAlerts(ShowCluesAlertsDefaultValue);
            }
            
            if (PlayerPrefs.HasKey(ShowCardGameWarningKey))
            {
                ShowCardGameWarning = IntToBool(PlayerPrefs.GetInt(ShowCardGameWarningKey));
            }
            else
            {
                SetShowCardGameWarning(ShowCardGameWarningDefaultValue);
            }
        }

        public static void SetPlayerPrefs()
        {
            PlayerPrefs.SetFloat(MasterBusVolumeKey, MasterVolume);
            PlayerPrefs.SetFloat(BGMBusVolumeKey, BGMVolume);
            PlayerPrefs.SetFloat(SFXBusVolumeKey, SFXVolume);
            PlayerPrefs.SetFloat(CharacterDelayValueKey, CharacterDelay);
            PlayerPrefs.SetInt(ShowCluesAlertsKey, BoolToInt(ShowCluesAlerts));
            PlayerPrefs.SetInt(ShowCardGameWarningKey, BoolToInt(ShowCardGameWarning));
        }

        #region Setters

        public static void SetMasterVolume(float value)
        {
            MasterVolume = value;
            PlayerPrefs.SetFloat(MasterBusVolumeKey, value);
        }

        public static void SetBGMVolume(float value)
        {
            BGMVolume = value;
            PlayerPrefs.SetFloat(BGMBusVolumeKey, value);
        }

        public static void SetSFXVolume(float value)
        {
            SFXVolume = value;
            PlayerPrefs.SetFloat(SFXBusVolumeKey, value);
        }

        public static void SetCharacterDelay(float value)
        {
            CharacterDelay = value;
            PlayerPrefs.SetFloat(CharacterDelayValueKey, value);
        }

        public static void SetShowCluesAlerts(bool value)
        {
            ShowCluesAlerts = value;
            PlayerPrefs.SetInt(ShowCluesAlertsKey, BoolToInt(value));
        }

        public static void SetShowCardGameWarning(bool value)
        {
            ShowCardGameWarning = value;
            PlayerPrefs.SetInt(ShowCardGameWarningKey, BoolToInt(value));
        }

        #endregion

        #region Getters

        public static float GetMasterVolume() { return MasterVolume; }
        public static float GetBGMVolume() { return BGMVolume; }
        public static float GetSFXVolume() { return SFXVolume; }
        public static float GetCharacterDelay() { return CharacterDelay; }
        public static bool GetShowCluesAlerts() { return ShowCluesAlerts; }
        public static bool GetShowCardGameWarning() { return ShowCardGameWarning; }

        #endregion

        private static int BoolToInt(bool value)
        {
            return value ? 1 : 0;
        }

        private static bool IntToBool(int value)
        {
            return value == 1;
        }
    }
}