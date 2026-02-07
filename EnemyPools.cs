using System;
using System.Collections.Generic;
using System.Text;

namespace DecayMode
{
    public static class EnemyPools
    {
        private static readonly Dictionary<DecayModeEnemyPool, PoolList_GameIDs> MassSpawnPools = new()
        {
            [DecayModeEnemyPool.Sepulchre] = PoolList_GameIDs.Sepulchre,
            [DecayModeEnemyPool.Kekastle] = PoolList_GameIDs.Kekastle
        };
        private static readonly Dictionary<DecayModeEnemyPool, PoolList_GameIDs> RandomSpawnPools = new()
        {
            [DecayModeEnemyPool.Bronzo] = PoolList_GameIDs.Bronzo,
            [DecayModeEnemyPool.Incarnate] = PoolList_GameIDs.Incarnate,
            [DecayModeEnemyPool.SmallEnemy] = PoolList_GameIDs.SmallEnemy,
            [DecayModeEnemyPool.Visage] = PoolList_GameIDs.Visage,
            [DecayModeEnemyPool.SilverSuckle] = PoolList_GameIDs.SilverSuckle,
            [DecayModeEnemyPool.GildedGulper] = PoolList_GameIDs.GildedGulper,
            [DecayModeEnemyPool.CadaverSynod] = PoolList_GameIDs.CadaverSynod
        };
        private static readonly Dictionary<DecayModeEnemyPool, PoolList_GameIDs> TransformPools = new()
        {
            [DecayModeEnemyPool.MusicMan] = PoolList_GameIDs.MusicMan,
            [DecayModeEnemyPool.Conductor] = PoolList_GameIDs.Conductor
        };

        public static bool TryGetRandomEnemyFromPool(DecayModeEnemyPool pool, out EnemySO enemy)
        {
            if (!TryGetEnemyPool(pool, out var enemies) || enemies == null || enemies.Count == 0)
            {
                enemy = null;
                return false;
            }

            enemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];
            return true;
        }

        public static bool TryGetEnemyPool(DecayModeEnemyPool pool, out List<EnemySO> enemies)
        {
            if(pool == DecayModeEnemyPool.CustomEnemyPool)
            {
                enemies = [];
                foreach(var id in ModConfig.CustomEnemyPool)
                {
                    var enm = LoadedAssetsHandler.GetEnemy(id);

                    if (enm == null)
                        continue;

                    enemies.Add(enm);
                }

                return true;
            }
            else if(pool == DecayModeEnemyPool.EnemiesWithUnitType)
            {
                enemies = [];
                foreach(var id in LoadedDBsHandler.EnemyDB.EnemiesList)
                {
                    var enm = LoadedAssetsHandler.GetEnemy(id);

                    if (enm == null)
                        continue;

                    foreach(var unitType in ModConfig.TargetUnitTypes)
                    {
                        if (!enm.unitTypes.Contains(unitType))
                            continue;

                        enemies.Add(enm);
                        break;
                    }
                }

                return true;
            }
            else if (IsMassSpawnPool(pool, out var massSpawnId))
            {
                if (LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(massSpawnId, out SpawnMassivelyEverywhereUsingHealthEffect massSpawnEffect))
                {
                    enemies = massSpawnEffect._possibleEnemies;
                    return true;
                }
            }
            else if (IsRandomSpawnPool(pool, out var randomSpawnId))
            {
                if (LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(randomSpawnId, out SpawnRandomEnemyAnywhereEffect randomSpawnEffect))
                {
                    enemies = randomSpawnEffect._enemies;
                    return true;
                }
            }
            else if (IsTransformPool(pool, out var transformId))
            {
                if (LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(transformId, out CasterRandomTransformationEffect transformEffect))
                {
                    enemies = transformEffect._possibleTransformations.ConvertAll(x => x.enemyTransformation);
                    return true;
                }
            }

            enemies = null;
            return false;
        }

        private static bool IsMassSpawnPool(DecayModeEnemyPool pool, out string massSpawnId)
        {
            if(pool == DecayModeEnemyPool.ModdedMassSpawnPool)
            {
                massSpawnId = ModConfig.ModdedRandomSpawnPool;
                return true;
            }
            if(MassSpawnPools.TryGetValue(pool, out var poolList))
            {
                massSpawnId = poolList.ToString();
                return true;
            }

            massSpawnId = string.Empty;
            return false;
        }

        private static bool IsRandomSpawnPool(DecayModeEnemyPool pool, out string randomSpawnId)
        {
            if(pool == DecayModeEnemyPool.ModdedRandomSpawnPool)
            {
                randomSpawnId = ModConfig.ModdedRandomSpawnPool;
                return true;
            }
            if(RandomSpawnPools.TryGetValue(pool, out var poolList))
            {
                randomSpawnId = poolList.ToString();
                return true;
            }

            randomSpawnId = string.Empty;
            return false;
        }

        private static bool IsTransformPool(DecayModeEnemyPool pool, out string transformId)
        {
            if(pool == DecayModeEnemyPool.ModdedTransformPool)
            {
                transformId = ModConfig.ModdedTransformPool;
                return true;
            }
            if(TransformPools.TryGetValue(pool, out var poolList))
            {
                transformId = poolList.ToString();
                return true;
            }

            transformId = string.Empty;
            return false;
        }
    }

    public enum DecayModeEnemyPool
    {
        Sepulchre,
        Kekastle,
        Bronzo,
        Incarnate,
        SmallEnemy,
        Visage,
        SilverSuckle,
        GildedGulper,
        MusicMan,
        Conductor,
        CadaverSynod,

        ModdedMassSpawnPool,
        ModdedRandomSpawnPool,
        ModdedTransformPool,

        EnemiesWithUnitType,
        CustomEnemyPool
    }
}
