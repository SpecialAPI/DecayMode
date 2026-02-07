using BrutalAPI;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecayMode
{
    [HarmonyPatch]
    public static class Patches
    {
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

            if (!en.TryGetStoredData(Plugin.PersonalizedDecayStoreData._UnitStoreDataID, out var hold) || hold.m_ObjectData is not BasePassiveAbilitySO)
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
        public static void AddDecay_OnCombatStart_Tranpsiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            while (crs.TryGotoNext(MoveType.After, x => x.MatchNewobj<EnemyCombat>()))
                crs.EmitStaticDelegate(AddDecay_OnCombatStart_Add);
        }

        public static EnemyCombat AddDecay_OnCombatStart_Add(EnemyCombat en)
        {
            AddDecayToEnemy(en);

            return en;
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.AddNewEnemy))]
        [HarmonyILManipulator]
        public static void AddDecay_OnSpawn_Tranpsiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            while (crs.TryGotoNext(MoveType.After, x => x.MatchNewobj<EnemyCombat>()))
                crs.EmitStaticDelegate(AddDecay_OnSpawn_Add);
        }

        public static EnemyCombat AddDecay_OnSpawn_Add(EnemyCombat en)
        {
            if (en == null || Array.IndexOf(ModConfig.EnemiesWithDecayOnSpawn, en.Enemy.name) < 0)
                return en;

            AddDecayToEnemy(en);

            return en;
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.TransformEnemy))]
        [HarmonyPostfix]
        public static void AddDecay_OnPostTransform_Postfix(EnemyCombat __instance)
        {
            if (!__instance.TryGetStoredData(Plugin.PersonalizedDecayStoreData._UnitStoreDataID, out var hold, false))
                return;

            if (hold == null || hold.m_ObjectData is not BasePassiveAbilitySO decay)
                return;

            __instance.AddPassiveAbility(decay);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryUnboxEnemy))]
        [HarmonyILManipulator]
        public static void AddDecay_OnUnbox_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            while (crs.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
            {
                crs.Emit(OpCodes.Ldloc_3);
                crs.EmitStaticDelegate(AddDecay_OnUnbox_Add);
            }
        }

        public static void AddDecay_OnUnbox_Add(EnemyCombat enm)
        {
            if (!enm.TryGetStoredData(Plugin.PersonalizedDecayStoreData._UnitStoreDataID, out var hold, false))
                return;

            if (hold == null || hold.m_ObjectData is not BasePassiveAbilitySO decay)
                return;

            enm.AddPassiveAbility(decay);
        }
    }
}
