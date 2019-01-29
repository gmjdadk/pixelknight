using System;
using System.Globalization;
using PixelKnight.Utils;

namespace PixelKnight.Models
{
    public static class PlayerPrefsUtility
    {
        public const string KEY_PREFIX = "ENC-";
        public const string VALUE_FLOAT_PREFIX = "0";
        public const string VALUE_INT_PREFIX = "1";
        public const string VALUE_STRING_PREFIX = "2";

        public static bool IsEncryptedKey(string key)
        {
            return key.StartsWith("ENC-");
        }

        public static string DecryptKey(string encryptedKey)
        {
            if (encryptedKey.StartsWith("ENC-"))
                return SimpleEncryption.DecryptString(encryptedKey.Substring("ENC-".Length));
            throw new InvalidOperationException("Could not decrypt item, no match found in known encrypted key prefixes");
        }

        public static void SetEncryptedFloat(string key, float value)
        {
            PlayerPrefs.SetString("ENC-" + SimpleEncryption.EncryptString(key), "0" + SimpleEncryption.EncryptFloat(value));
        }

        public static void SetEncryptedInt(string key, int value)
        {
            PlayerPrefs.SetString("ENC-" + SimpleEncryption.EncryptString(key), "1" + SimpleEncryption.EncryptInt(value));
        }

        public static void SetEncryptedString(string key, string value)
        {
            PlayerPrefs.SetString("ENC-" + SimpleEncryption.EncryptString(key), "2" + SimpleEncryption.EncryptString(value));
        }

        public static object GetEncryptedValue(string encryptedKey, string encryptedValue)
        {
            if (encryptedValue.StartsWith("0"))
                return (object)PlayerPrefsUtility.GetEncryptedFloat(SimpleEncryption.DecryptString(encryptedKey.Substring("ENC-".Length)), 0.0f);
            if (encryptedValue.StartsWith("1"))
                return (object)PlayerPrefsUtility.GetEncryptedInt(SimpleEncryption.DecryptString(encryptedKey.Substring("ENC-".Length)), 0);
            if (encryptedValue.StartsWith("2"))
                return (object)PlayerPrefsUtility.GetEncryptedString(SimpleEncryption.DecryptString(encryptedKey.Substring("ENC-".Length)), string.Empty);
            throw new InvalidOperationException("Could not decrypt item, no match found in known encrypted key prefixes");
        }

        public static float GetEncryptedFloat(string key, float defaultValue = 0.0f)
        {
            string str = PlayerPrefs.GetString("ENC-" + SimpleEncryption.EncryptString(key));
            if (!string.IsNullOrEmpty(str))
                return SimpleEncryption.DecryptFloat(str.Remove(0, 1));
            return defaultValue;
        }

        public static int GetEncryptedInt(string key, int defaultValue = 0)
        {
            string str = PlayerPrefs.GetString("ENC-" + SimpleEncryption.EncryptString(key));
            if (!string.IsNullOrEmpty(str))
                return SimpleEncryption.DecryptInt(str.Remove(0, 1));
            return defaultValue;
        }

        public static string GetEncryptedString(string key, string defaultValue = "")
        {
            string str = PlayerPrefs.GetString("ENC-" + SimpleEncryption.EncryptString(key));
            if (!string.IsNullOrEmpty(str))
                return SimpleEncryption.DecryptString(str.Remove(0, 1));
            return defaultValue;
        }

        public static void SetBool(string key, bool value)
        {
            if (value)
                PlayerPrefs.SetInt(key, 1);
            else
                PlayerPrefs.SetInt(key, 0);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            if (!PlayerPrefs.HasKey(key))
                return defaultValue;
            return PlayerPrefs.GetInt(key) != 0;
        }

        public static void SetEnum(string key, Enum value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static T GetEnum<T>(string key, T defaultValue = default(T)) where T : struct
        {
            string str = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(str))
                return (T)Enum.Parse(typeof(T), str);
            return defaultValue;
        }

        public static object GetEnum(string key, System.Type enumType, object defaultValue)
        {
            string str = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(str))
                return Enum.Parse(enumType, str);
            return defaultValue;
        }

        public static void SetDateTime(string key, DateTime value)
        {
            PlayerPrefs.SetString(key, value.ToString("o", (IFormatProvider)CultureInfo.InvariantCulture));
        }

        public static DateTime GetDateTime(string key, DateTime defaultValue = default(DateTime))
        {
            string s = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(s))
                return DateTime.Parse(s, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return defaultValue;
        }

        public static void SetTimeSpan(string key, TimeSpan value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static TimeSpan GetTimeSpan(string key, TimeSpan defaultValue = default(TimeSpan))
        {
            string s = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(s))
                return TimeSpan.Parse(s);
            return defaultValue;
        }
    }
}
