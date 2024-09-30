using BepInEx;
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
        public const string VERSION = "1.0.0";

        public static MethodInfo ad_adi = AccessTools.Method(typeof(Plugin), nameof(AddDecay_ActuallyDoIt));
        public static MethodInfo ad_os_adi = AccessTools.Method(typeof(Plugin), nameof(AddDecay_OnSpawn_ActuallyDoIt));
        public static MethodInfo adob_adi = AccessTools.Method(typeof(Plugin), nameof(AddDecayOnUnbox_ActuallyDoIt));

        public static string[] EnemiesWithDecayOnSpawn = new string[]
        {
            "HeavensGateBlue_BOSS",
            "HeavensGatePurple_BOSS",
            "HeavensGateRed_BOSS",
            "HeavensGateYellow_BOSS",

            "OsmanLeft_BOSS",
            "OsmanRight_BOSS"
        };

        public static string[] EnemiesIgnoredForDecay = new string[]
        {
            "OsmanSinnoks_BOSS"
        };

        public static UnitStoreData_BasicSO PersonalizedDecayStoreData;

        public void Awake()
        {
            PersonalizedDecayStoreData = ScriptableObject.CreateInstance<UnitStoreData_BasicSO>();

            PersonalizedDecayStoreData.name = PersonalizedDecayStoreData._UnitStoreDataID = "DecayMode_PersonalizedDecay_SV";
            LoadedDBsHandler.MiscDB.AddNewUnitStoreData(PersonalizedDecayStoreData._UnitStoreDataID, PersonalizedDecayStoreData);

            new Harmony(GUID).PatchAll();
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

        public static EnemyCombat AddDecay_OnSpawn_ActuallyDoIt(EnemyCombat en)
        {
            if(en == null || Array.IndexOf(EnemiesWithDecayOnSpawn, en.Enemy.name) < 0)
                return en;

            AddDecayToEnemy(en);

            return en;
        }

        public static void AddDecayToEnemy(EnemyCombat en)
        {
            if (en == null || Array.IndexOf(EnemiesIgnoredForDecay, en.Enemy.name) >= 0)
                return;

            if (!LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(PoolList_GameIDs.Sepulchre.ToString(), out SpawnMassivelyEverywhereUsingHealthEffect sepulch) || sepulch == null || sepulch._possibleEnemies == null || sepulch._possibleEnemies.Count <= 0)
                return;

            var rngEn = sepulch._possibleEnemies[UnityEngine.Random.Range(0, sepulch._possibleEnemies.Count)];

            if (en.TryGetPassiveAbility(PassiveType_GameIDs.Decay.ToString(), out var existing))
            {
                en.PassiveAbilities.Remove(existing);
                existing.OnTriggerDettached(en);
            }

            if (en.ContainsPassiveAbility(PassiveType_GameIDs.Decay.ToString()))
                return; // WTF?

            var personalizedDecay = Passives.DecayGenerator(rngEn);

            en.PassiveAbilities.Add(personalizedDecay);
            personalizedDecay.OnTriggerAttached(en);

            if (!en.TryGetStoredData(PersonalizedDecayStoreData._UnitStoreDataID, out var hold) || hold.m_ObjectData is not BasePassiveAbilitySO)
                hold.m_ObjectData = personalizedDecay;
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.TransformEnemy))]
        [HarmonyPostfix]
        public static void AddDecayOnPostTransform(EnemyCombat __instance)
        {
            if (__instance.TryGetStoredData(PersonalizedDecayStoreData._UnitStoreDataID, out var hold, false) && hold != null && hold.m_ObjectData is BasePassiveAbilitySO decay)
                __instance.AddPassiveAbility(decay);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryUnboxEnemy))]
        [HarmonyILManipulator]
        public static void AddDecayOnUnbox_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            while (crs.TryGotoNext(MoveType.After, x => x.MatchCall<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
            {
                crs.Emit(OpCodes.Ldloc_3);
                crs.Emit(OpCodes.Call, adob_adi);
            }
        }
        public static void AddDecayOnUnbox_ActuallyDoIt(EnemyCombat enm)
        {
            if (enm.TryGetStoredData(PersonalizedDecayStoreData._UnitStoreDataID, out var hold, false) && hold != null && hold.m_ObjectData is BasePassiveAbilitySO decay)
                enm.AddPassiveAbility(decay);
        }
    }
}
