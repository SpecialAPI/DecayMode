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
        private static ConfigEntry<string> EnemiesIgnoredForDecayEntry;

        private static ConfigEntry<DecayModeEnemyPool> EnemyPoolEntry;
        private static ConfigEntry<string> EnemiesWithBronzoDecayEntry;
        private static ConfigEntry<string> ModdedMassSpawnPoolEntry;
        private static ConfigEntry<string> ModdedRandomSpawnPoolEntry;
        private static ConfigEntry<string> ModdedTransformPoolEntry;
        private static ConfigEntry<string> TargetUnitTypesEntry;
        private static ConfigEntry<string> CustomEnemyPoolEntry;

        public static string[] EnemiesWithDecayOnSpawn => EnemiesWithDecayOnSpawnEntry?.Value?.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries) ?? [];
        public static string[] EnemiesIgnoredForDecay => EnemiesIgnoredForDecayEntry?.Value?.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries) ?? [];

        public static DecayModeEnemyPool EnemyPool => EnemyPoolEntry?.Value ?? DecayModeEnemyPool.Sepulchre;
        public static string[] EnemiesWithBronzoDecay => EnemiesWithBronzoDecayEntry?.Value?.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries) ?? [];
        public static string ModdedMassSpawnPool => ModdedMassSpawnPoolEntry?.Value ?? string.Empty;
        public static string ModdedRandomSpawnPool => ModdedRandomSpawnPoolEntry?.Value ?? string.Empty;
        public static string ModdedTransformPool => ModdedTransformPoolEntry?.Value ?? string.Empty;
        public static string[] TargetUnitTypes => TargetUnitTypesEntry?.Value?.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries) ?? [];
        public static string[] CustomEnemyPool => CustomEnemyPoolEntry?.Value?.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries) ?? [];

        public static void Init()
        {
            EnemiesWithDecayOnSpawnEntry = File.Bind("DecayMode", "EnemiesWithDecayOnSpawn", string.Join(Separator, EnemiesWithDecayOnSpawn_Default));
            EnemiesIgnoredForDecayEntry = File.Bind("DecayMode", "EnemiesIgnoredForDecay", string.Join(Separator, EnemiesIgnoredForDecay_Default));

            EnemyPoolEntry = File.Bind("DecayMode.EnemyPool", "EnemyPool", DecayModeEnemyPool.Sepulchre);
            EnemiesWithBronzoDecayEntry = File.Bind("DecayMode.EnemyPool", "EnemiesWithBronzoDecay", string.Join(Separator, EnemiesWithBronzoDecay_Default));
            ModdedMassSpawnPoolEntry = File.Bind("DecayMode.EnemyPool", "ModdedMassSpawnPool", "");
            ModdedRandomSpawnPoolEntry = File.Bind("DecayMode.EnemyPool", "ModdedRandomSpawnPool", "");
            ModdedTransformPoolEntry = File.Bind("DecayMode.EnemyPool", "ModdedTransformPool", "");
            TargetUnitTypesEntry = File.Bind("DecayMode.EnemyPool", "TargetUnitTypes", "");
            CustomEnemyPoolEntry = File.Bind("DecayMode.EnemyPool", "CustomEnemyPool", "");
        }
    }
}
