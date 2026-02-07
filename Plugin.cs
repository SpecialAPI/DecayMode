using BepInEx;
using BepInEx.Configuration;
using BrutalAPI;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DecayMode
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "SpecialAPI.DecayMode";
        public const string NAME = "Decay Mode";
        public const string VERSION = "1.2.0";

        public static MethodInfo ad_adi = AccessTools.Method(typeof(Plugin), nameof(AddDecay_ActuallyDoIt));
        public static MethodInfo ad_os_adi = AccessTools.Method(typeof(Plugin), nameof(AddDecay_OnSpawn_ActuallyDoIt));
        public static MethodInfo adob_adi = AccessTools.Method(typeof(Plugin), nameof(AddDecayOnUnbox_ActuallyDoIt));

        public static UnitStoreData_BasicSO PersonalizedDecayStoreData;

        public void Awake()
        {
            ModConfig.File = Config;
            ModConfig.Init();

            PersonalizedDecayStoreData = ScriptableObject.CreateInstance<UnitStoreData_BasicSO>();
            PersonalizedDecayStoreData.name = PersonalizedDecayStoreData._UnitStoreDataID = "DecayMode_PersonalizedDecay_USD";
            LoadedDBsHandler.MiscDB.AddNewUnitStoreData(PersonalizedDecayStoreData._UnitStoreDataID, PersonalizedDecayStoreData);

            var removeDecayEffect = ScriptableObject.CreateInstance<RemovePassiveEffect>();
            removeDecayEffect.m_PassiveID = PassiveType_GameIDs.Decay.ToString();
            var removeDecayFromAllies = Effects.GenerateEffect(removeDecayEffect, 0, Targeting.Unit_OtherAllies);

            var bronzo1 = LoadedAssetsHandler.GetEnemy("Bronzo1_EN");
            if (bronzo1.passiveAbilities.Count > 0 && bronzo1.passiveAbilities[0] is PerformEffectPassiveAbility bronzo1Decay)
                bronzo1Decay.effects = [removeDecayFromAllies, ..bronzo1Decay.effects];

            var bronzo2 = LoadedAssetsHandler.GetEnemy("Bronzo2_EN");
            if (bronzo2.passiveAbilities.Count > 2 && bronzo2.passiveAbilities[2] is PerformEffectPassiveAbility bronzo2Decay)
                bronzo2Decay.effects = [removeDecayFromAllies, .. bronzo2Decay.effects];

            var bronzo3 = LoadedAssetsHandler.GetEnemy("Bronzo3_EN");
            if (bronzo3.passiveAbilities.Count > 2 && bronzo3.passiveAbilities[2] is PerformEffectPassiveAbility bronzo3Decay)
                bronzo3Decay.effects = [removeDecayFromAllies, .. bronzo3Decay.effects];

            var bronzo4 = LoadedAssetsHandler.GetEnemy("Bronzo4_EN");
            if (bronzo4.passiveAbilities.Count > 3 && bronzo4.passiveAbilities[3] is PerformEffectPassiveAbility bronzo4Decay)
                bronzo4Decay.effects = [removeDecayFromAllies, .. bronzo4Decay.effects];

            new Harmony(GUID).PatchAll();
        }

        public static EnemyCombat AddDecay_OnSpawn_ActuallyDoIt(EnemyCombat en)
        {
            if (en == null || Array.IndexOf(ModConfig.EnemiesWithDecayOnSpawn, en.Enemy.name) < 0)
                return en;

            AddDecayToEnemy(en);

            return en;
        }

        public static void AddDecayToEnemy(EnemyCombat en)
        {
            if (en == null || Array.IndexOf(ModConfig.EnemiesIgnoredForDecay, en.Enemy.name) >= 0)
                return;

            var bronzo = Array.IndexOf(ModConfig.EnemiesWithBronzoDecay, en.Enemy.name) >= 0;
            if (ModConfig.DecayMean)
                bronzo = true;

            if (!TryGetDecayEnemy(bronzo, out var rngEn))
                return;

            if (en.TryGetPassiveAbility(PassiveType_GameIDs.Decay.ToString(), out var existing))
            {
                en.PassiveAbilities.Remove(existing);
                existing.OnTriggerDettached(en);
            }

            if (en.ContainsPassiveAbility(PassiveType_GameIDs.Decay.ToString()))
                return; // WTF?

            var personalizedDecay = Passives.DecayGenerator(rngEn, 100, false);

            en.PassiveAbilities.Add(personalizedDecay);
            personalizedDecay.OnTriggerAttached(en);

            if (!en.TryGetStoredData(PersonalizedDecayStoreData._UnitStoreDataID, out var hold) || hold.m_ObjectData is not BasePassiveAbilitySO)
                hold.m_ObjectData = personalizedDecay;
        }

        public static bool TryGetDecayEnemy(bool bronzoPool, out EnemySO enemy)
        {
            enemy = null;
            List<EnemySO> pool;

            if (bronzoPool)
            {
                if (!LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(PoolList_GameIDs.Bronzo.ToString(), out SpawnRandomEnemyAnywhereEffect bronz) || bronz == null)
                    return false;

                pool = bronz._enemies;
            }
            else
            {
                if (!LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(PoolList_GameIDs.Sepulchre.ToString(), out SpawnMassivelyEverywhereUsingHealthEffect sepulch) || sepulch == null)
                    return false;

                pool = sepulch._possibleEnemies;
            }

            if (pool == null || pool.Count <= 0)
                return false;

            enemy = pool[UnityEngine.Random.Range(0, pool.Count)];
            return true;
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.Initialization))]
        [HarmonyILManipulator]
        public static void AddDecay_Tranpsiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            while (crs.TryGotoNext(MoveType.After, x => x.MatchNewobj<EnemyCombat>()))
                crs.Emit(OpCodes.Call, ad_adi);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.AddNewEnemy))]
        [HarmonyILManipulator]
        public static void AddDecay_OnSpawn_Tranpsiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            while (crs.TryGotoNext(MoveType.After, x => x.MatchNewobj<EnemyCombat>()))
                crs.Emit(OpCodes.Call, ad_os_adi);
        }

        public static EnemyCombat AddDecay_ActuallyDoIt(EnemyCombat en)
        {
            AddDecayToEnemy(en);

            return en;
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.TransformEnemy))]
        [HarmonyPostfix]
        public static void AddDecayOnPostTransform(EnemyCombat __instance)
        {
            if (!__instance.TryGetStoredData(PersonalizedDecayStoreData._UnitStoreDataID, out var hold, false))
                return;

            if (hold == null || hold.m_ObjectData is not BasePassiveAbilitySO decay)
                return;

            __instance.AddPassiveAbility(decay);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryUnboxEnemy))]
        [HarmonyILManipulator]
        public static void AddDecayOnUnbox_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            while (crs.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
            {
                crs.Emit(OpCodes.Ldloc_3);
                crs.Emit(OpCodes.Call, adob_adi);
            }
        }
        public static void AddDecayOnUnbox_ActuallyDoIt(EnemyCombat enm)
        {
            if (!enm.TryGetStoredData(PersonalizedDecayStoreData._UnitStoreDataID, out var hold, false))
                return;

            if (hold == null || hold.m_ObjectData is not BasePassiveAbilitySO decay)
                return;

            enm.AddPassiveAbility(decay);
        }
    }
}
