using System;
using Unity.Mathematics;
using UnityEngine;

namespace KOI
{
    public class MapSystem : GameSystem
    {
        private WorldMap _worldMap;
        public override void Init()
        {
            SetupEvents();
        }

        private void GenerateWorlMap()
        {
            _worldMap = new WorldMap(MapConfig.WorldMapWidth, MapConfig.WorldMapHeight);

            for (int id = 0; id < _worldMap.Area; id ++)
            {
                Cell cell = new Cell
                {
                    Id = id,
                    Solid = false,
                    Position = IdToPosition(id),
                    TerrainType = TerrainType.Water,
                    FoliageType = FoliageType.None
                };
            }
        }
        private void SetupEvents()
        {
            GameStateManager.OnTick += Tick;
        }

        protected override void Tick(object sender, OnTickArgs eventArgs)
        {
            Debug.Log(eventArgs.Tick);
        }
        public override void Quit()
        {
            GameStateManager.OnTick -= Tick;
        }

        private int2 IdToPosition(int id)
        {
            int x = id % _worldMap.Width - _worldMap.Width;
            int y = id / _worldMap.Height - _worldMap.Width;
            return new int2(x, y);
        }
    }
}