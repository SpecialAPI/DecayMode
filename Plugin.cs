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
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "SpecialAPI.DecayMode";
        public const string NAME = "Decay Mode";
        public const string VERSION = "1.2.0";

        public static Harmony HarmonyInstance;

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

            HarmonyInstance = new Harmony(GUID);
            HarmonyInstance.PatchAll();
        }
    }
}
