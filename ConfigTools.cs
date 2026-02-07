using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecayMode
{
    public static class ConfigTools
    {
        public static string SerializeDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string elementSeparator, string keyValueSeparator)
        {
            if (dictionary == null)
                return string.Empty;

            var i = 0;
            var output = "";
            foreach(var kvp in dictionary)
            {
                if (i++ > 0)
                    output += elementSeparator;

                var keyStr = TomlTypeConverter.ConvertToString(kvp.Key, typeof(TKey));
                var valueStr = TomlTypeConverter.ConvertToString(kvp.Value, typeof(TValue));
                output += $"{keyStr}{keyValueSeparator}{valueStr}";
            }

            return output;
        }

        public static Dictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(string str, string elementSeparator, string keyValueSeparator)
        {
            if(string.IsNullOrEmpty(str))
                return [];

            var elems = str.Split([elementSeparator], StringSplitOptions.RemoveEmptyEntries);
            var result = new Dictionary<TKey, TValue>();

            foreach(var elemStr in elems)
            {
                var sepIdx = elemStr.IndexOf(keyValueSeparator);

                if (sepIdx < 0)
                    continue;

                var keyStr = elemStr.Substring(0, sepIdx);
                var key = TomlTypeConverter.ConvertToValue<TKey>(keyStr);

                var valueStr = elemStr.Substring(sepIdx + 1);
                var value = TomlTypeConverter.ConvertToValue<TValue>(valueStr);

                result[key] = value;
            }

            return result;
        }
    }
}
