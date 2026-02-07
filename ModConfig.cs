using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecayMode
{
    public static class ModConfig
    {
        public static ConfigFile File;

        private const string Separator = ",";
        private static readonly string[] SeparatorArray = [Separator];

        public static readonly string[] EnemiesWithDecayOnSpawn_Default = new string[]
        {
            "NextOfKin_EN",
            "Moone_EN",

            "HeavensGateBlue_BOSS",
            "HeavensGatePurple_BOSS",
            "HeavensGateRed_BOSS",
            "HeavensGateYellow_BOSS",

            "OsmanLeft_BOSS",
            "OsmanRight_BOSS",

            "Bronzo_MoneyPile_EN",
            "BronzoExtra_EN",
            "Bronzo_Bananas_Mean_EN"
        };
        public static readonly string[] EnemiesWithBronzoDecay_Default = new string[]
        {
            "Bronzo_MoneyPile_EN",
            "BronzoExtra_EN",
            "Bronzo_Bananas_Mean_EN"
        };
        public static readonly string[] EnemiesIgnoredForDecay_Default = new string[]
        {
            "OsmanSinnoks_BOSS",

            "Bronzo1_EN",
            "Bronzo2_EN",
            "Bronzo3_EN",
            "Bronzo4_EN",
            "Bronzo5_EN",
        };

        private static ConfigEntry<string> EnemiesWithDecayOnSpawnEntry;
        private static ConfigEntry<string> EnemiesWithBronzoDecayEntry;
        private static ConfigEntry<string> EnemiesIgnoredForDecayEntry;
        private static ConfigEntry<bool> DecayMeanEntry;

        public static string[] EnemiesWithDecayOnSpawn => EnemiesWithDecayOnSpawnEntry?.Value?.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries) ?? [];
        public static string[] EnemiesWithBronzoDecay => EnemiesWithBronzoDecayEntry?.Value?.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries) ?? [];
        public static string[] EnemiesIgnoredForDecay => EnemiesIgnoredForDecayEntry?.Value?.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries) ?? [];
        public static bool DecayMean => DecayMeanEntry?.Value ?? false;

        public static void Init()
        {
            EnemiesWithDecayOnSpawnEntry = File.Bind("DecayMode", "EnemiesWithDecayOnSpawn", string.Join(Separator, EnemiesWithDecayOnSpawn_Default));
            EnemiesWithBronzoDecayEntry = File.Bind("DecayMode", "EnemiesWithBronzoDecay", string.Join(Separator, EnemiesWithBronzoDecay_Default));
            EnemiesIgnoredForDecayEntry = File.Bind("DecayMode", "EnemiesIgnoredForDecay", string.Join(Separator, EnemiesIgnoredForDecay_Default));
            DecayMeanEntry = File.Bind("DecayMode", "DecayMean", false, "Bronzo mode. If set to true, Decay Mode will draw from the Bronzo pool instead of the Sepulchre pool.");
        }
    }
}
