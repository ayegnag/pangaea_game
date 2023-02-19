using System;
using Unity.Mathematics;
using UnityEngine;

namespace KOI
{
    public class MapSystem : GameSystem
    {
        public static event EventHandler<OnMapEventArgs> OnUpdateMapRender;
        private WorldMap _worldMap;
        private WorldGenerator _worldGenerator;

        public override void Init()
        {
            SetupEvents();
            SelectMapType();
            GenerateWorlMap();
        }

        private void SelectMapType()
        {   
            // Add logic here to select between different Map Generators
            _worldGenerator = new StandardMapGenerator();
        }

        private void GenerateWorlMap()
        {
            _worldMap = new WorldMap(MapConfig.WorldMapWidth, MapConfig.WorldMapHeight);
            _worldGenerator.Initialize(_worldMap);
            _worldMap = _worldGenerator.GenerateMap();
            OnUpdateMapRender?.Invoke(this, new OnMapEventArgs { WorldMap = _worldMap});
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
    }
}