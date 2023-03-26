using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KOI
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        public MapSystem MapSystem { get; private set; }
        public EntitySystem EntitySystem { get; private set; }
        private Dictionary<TerrainType, Tile> _terrainTiles;
        public static event EventHandler<OnTickArgs> OnTick;
        private int _tick;
        private float _tickTimer;

        private void Awake()
        {
            EnforceSingletonInstance();
            MapSystem = new MapSystem();
            EntitySystem = new EntitySystem();
            _tick = 0;
            _tickTimer = 0;
        }

        private void Start() {
            MapSystem.Init();
            EntitySystem.Init();
        }

        private void EnforceSingletonInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else{
                Instance = this;
            }
        }

        private void TestTiler(){
            if(Input.GetMouseButtonDown(0)){
                Vector3 currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int xPosition = (int)Mathf.Floor(currentPoint.x);
                int yPosition = (int)Mathf.Floor(currentPoint.y);
                Cell currentCell =  MapSystem.GetCell(xPosition, yPosition);
                Tilemap _terrainTileMap = GameObject.Find("TerrainTilemap").GetComponent<Tilemap>();
                _terrainTiles = new Dictionary<TerrainType, Tile>
			{
				[TerrainType.Water] = Resources.Load<Tile>("Tiles/Water"),
				[TerrainType.Sand] = Resources.Load<Tile>("Tiles/Sand"),
				[TerrainType.Ground] = Resources.Load<Tile>("Tiles/Grass"),
				[TerrainType.Mountain] = Resources.Load<Tile>("Tiles/Stone"),
				[TerrainType.Ice] = Resources.Load<Tile>("Tiles/Ice"),
				[TerrainType.Test] = Resources.Load<Tile>("Tiles/Test")
			};
                if(currentCell != null){
                    Vector3Int tilemapPosition = new Vector3Int(xPosition, yPosition);
                    // if(currentCell.TerrainType == TerrainType.Test){
                    //     _terrainTileMap.SetTile(tilemapPosition, _terrainTiles[TerrainType.Ground]);
                    //     currentCell.TerrainType = TerrainType.Ground;
                    //     return;
                    // }
                    _terrainTileMap.SetTile(tilemapPosition, _terrainTiles[TerrainType.Test]);
                    currentCell.TerrainType = TerrainType.Test;
				}
            }
        }

        void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer >= GameConfig.TickDuration)
            {
                _tick ++;
                _tickTimer -= GameConfig.TickDuration;

                OnTick?.Invoke(this, new OnTickArgs {Tick = _tick});
            }
           TestTiler();
        }

        private void OnDisable() {
            MapSystem.Quit();
            EntitySystem.Quit();
        }
    }
}