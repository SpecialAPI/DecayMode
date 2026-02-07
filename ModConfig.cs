using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecayMode
{
    public static class ModConfig
    {
        public static ConfigFile File;

        private static ConfigEntry<ConfigList<string>> EnemiesWithDecayOnSpawnEntry;
        private static ConfigEntry<ConfigList<string>> EnemiesIgnoredForDecayEntry;

        private static ConfigEntry<int> DecayChanceEntry;
        private static ConfigEntry<bool> IgnoreWitheringEntry;
        private static ConfigEntry<bool> HideDecayEnemyEntry;

        private static ConfigEntry<DecayModeEnemyPool> EnemyPoolEntry;
        private static ConfigEntry<ConfigDictionary<string, DecayModeEnemyPool>> EnemyPoolExceptionsEntry;
        private static ConfigEntry<string> ModdedMassSpawnPoolEntry;
        private static ConfigEntry<string> ModdedRandomSpawnPoolEntry;
        private static ConfigEntry<string> ModdedTransformPoolEntry;
        private static ConfigEntry<ConfigList<string>> TargetUnitTypesEntry;
        private static ConfigEntry<ConfigList<string>> CustomEnemyPoolEntry;

        public static ConfigList<string> EnemiesWithDecayOnSpawn => EnemiesWithDecayOnSpawnEntry.Value;
        public static ConfigList<string> EnemiesIgnoredForDecay => EnemiesIgnoredForDecayEntry.Value;

        public static int DecayChance => DecayChanceEntry.Value;
        public static bool IgnoreWithering => IgnoreWitheringEntry.Value;
        public static bool HideDecayEnemy => HideDecayEnemyEntry.Value;

        public static DecayModeEnemyPool EnemyPool => EnemyPoolEntry.Value;
        public static ConfigDictionary<string, DecayModeEnemyPool> EnemyPoolExceptions => EnemyPoolExceptionsEntry.Value;
        public static string ModdedMassSpawnPool => ModdedMassSpawnPoolEntry.Value;
        public static string ModdedRandomSpawnPool => ModdedRandomSpawnPoolEntry.Value;
        public static string ModdedTransformPool => ModdedTransformPoolEntry.Value;
        public static ConfigList<string> TargetUnitTypes => TargetUnitTypesEntry.Value;
        public static ConfigList<string> CustomEnemyPool => CustomEnemyPoolEntry.Value;

        public static void Init()
        {
            ConfigTools.RegisterListConfigType<string>();
            ConfigTools.RegisterDictionaryConfigType<string, DecayModeEnemyPool>();

            EnemiesWithDecayOnSpawnEntry    = File.Bind("DecayMode", "EnemiesWithDecayOnSpawn", new ConfigList<string>()
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
            });
            EnemiesIgnoredForDecayEntry     = File.Bind("DecayMode", "EnemiesIgnoredForDecay", new ConfigList<string>()
            {
                "OsmanSinnoks_BOSS",

                "Bronzo1_EN",
                "Bronzo2_EN",
                "Bronzo3_EN",
                "Bronzo4_EN",
                "Bronzo5_EN",
            });

            DecayChanceEntry                = File.Bind("DecayMode.DecayParameters", "DecayChance", 100);
            IgnoreWitheringEntry            = File.Bind("DecayMode.DecayParameters", "IgnoreWithering", false);
            HideDecayEnemyEntry             = File.Bind("DecayMode.DecayParameters", "HideDecayEnemy", false);

            EnemyPoolEntry                  = File.Bind("DecayMode.EnemyPool", "EnemyPool", DecayModeEnemyPool.Sepulchre);
            EnemyPoolExceptionsEntry        = File.Bind("DecayMode.EnemyPool", "EnemyPoolExceptions", new ConfigDictionary<string, DecayModeEnemyPool>()
            {
                ["Bronzo_MoneyPile_EN"] = DecayModeEnemyPool.Bronzo,
                ["BronzoExtra_EN"] = DecayModeEnemyPool.Bronzo,
                ["Bronzo_Bananas_Mean_EN"] = DecayModeEnemyPool.Bronzo
            });
            ModdedMassSpawnPoolEntry        = File.Bind("DecayMode.EnemyPool", "ModdedMassSpawnPool", "");
            ModdedRandomSpawnPoolEntry      = File.Bind("DecayMode.EnemyPool", "ModdedRandomSpawnPool", "");
            ModdedTransformPoolEntry        = File.Bind("DecayMode.EnemyPool", "ModdedTransformPool", "");
            TargetUnitTypesEntry            = File.Bind("DecayMode.EnemyPool", "TargetUnitTypes", new ConfigList<string>());
            CustomEnemyPoolEntry            = File.Bind("DecayMode.EnemyPool", "CustomEnemyPool", new ConfigList<string>());
        }
    }
}
