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

        public static bool TryGetEnemyPool(DecayModeEnemyPool pool, out List<EnemySO> enemies)
        {
            if (MassSpawnPools.TryGetValue(pool, out var massspawn))
            {
                if (LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(massspawn.ToString(), out SpawnMassivelyEverywhereUsingHealthEffect massspawneffect))
                {
                    enemies = massspawneffect._possibleEnemies;
                    return true;
                }
            }
            else if (RandomSpawnPools.TryGetValue(pool, out var randomspawn))
            {
                if (LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(randomspawn.ToString(), out SpawnRandomEnemyAnywhereEffect randomspawneffect))
                {
                    enemies = randomspawneffect._enemies;
                    return true;
                }
            }
            else if(TransformPools.TryGetValue(pool, out var transform))
            {
                if (LoadedDBsHandler.EnemyDB.TryGetEnemyPoolEffect(transform.ToString(), out CasterRandomTransformationEffect transformeffect))
                {
                    enemies = transformeffect._possibleTransformations.ConvertAll(x => x.enemyTransformation);
                    return true;
                }
            }

            enemies = null;
            return false;
        }

        public static bool TryGetRandomEnemyFromPool(DecayModeEnemyPool pool, out EnemySO enemy)
        {
            if(!TryGetEnemyPool(pool, out var enemies) || enemies == null || enemies.Count == 0)
            {
                enemy = null;
                return false;
            }

            enemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];
            return true;
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
        CadaverSynod
    }
}
