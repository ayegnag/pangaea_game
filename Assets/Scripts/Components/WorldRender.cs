using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KOI
{
    public class WorldRender : MonoBehaviour
    {   
		private Grid _grid;
        
        private Tilemap _terrainTileMap;
        private Tilemap _treesTileMap;

        private Dictionary<TerrainType, Tile> _terrainTiles;
        private Dictionary<VegetationType, Tile> _treeTiles;

		private void SetupTilemapResources()
		{
			_grid = GameObject.Find("TerrainGrid").GetComponent<Grid>();
            
			_terrainTileMap = GameObject.Find("TerrainTilemap").GetComponent<Tilemap>();
			_treesTileMap = GameObject.Find("TreesTilemap").GetComponent<Tilemap>();

            _terrainTiles = new Dictionary<TerrainType, Tile>
			{
				[TerrainType.Water] = Resources.Load<Tile>("Tiles/Water"),
				[TerrainType.Sand] = Resources.Load<Tile>("Tiles/Sand"),
				[TerrainType.Ground] = Resources.Load<Tile>("Tiles/Grass"),
				[TerrainType.Mountain] = Resources.Load<Tile>("Tiles/Stone"),
				[TerrainType.Ice] = Resources.Load<Tile>("Tiles/Ice")
			};

            _treeTiles = new Dictionary<VegetationType, Tile>
			{
				[VegetationType.None] = null,
				[VegetationType.BeachTree] = Resources.Load<Tile>("Tiles/Tree_Two"),
				[VegetationType.PlainTree] = Resources.Load<Tile>("Tiles/Tree_One")
			};
        }

        private void Awake() {
            SetupEvents();
            SetupTilemapResources();
        }
        
        private void SetupEvents()
        {
            MapSystem.OnUpdateMapRender += UpdateMapRender;
        }
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnDisable() {
            MapSystem.OnUpdateMapRender -= UpdateMapRender;
        }

        private void UpdateMapRender(object sender, OnMapEventArgs eventArgs)
        {
            foreach(Cell cell in eventArgs.WorldMap.Cells)
            {
                Vector3Int tilemapPosition = new Vector3Int(cell.Position.x, cell.Position.y);
                _terrainTileMap.SetTile(tilemapPosition, _terrainTiles[cell.TerrainType]);
                _treesTileMap.SetTile(tilemapPosition, _treeTiles[cell.VegetationType]);
            }
        }
        
    }
}
