using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

        private GameObject _dogsGameObject;

        private Dictionary<int, DogRenderData> _dogRenderData;
		private Dictionary<Pack, GameObject> _packPrefabs;

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

        private void SetupDogResources()
		{
            _dogsGameObject = GameObject.Find("World/Entities/Dogs");
			_dogRenderData = new Dictionary<int, DogRenderData>();
            _packPrefabs = new Dictionary<Pack, GameObject>
			{
				[Pack.Pack1] = Resources.Load<GameObject>("Prefabs/Entities/Dogs/Doggie"),
				[Pack.Pack2] = Resources.Load<GameObject>("Prefabs/Entities/Dogs/Doggie")
			};
        }

        private void Awake() {
            SetupEvents();
            SetupTilemapResources();
            SetupDogResources();
        }

        private void SetupEvents()
        {
            MapSystem.OnUpdateMapRender += UpdateMapRender;
            EntitySystem.OnCreateDog += CreateDogRenderData;
        }

        private void CreateDogRenderData(object sender, OnDogEventArgs eventArgs)
        {
			Dog dog = eventArgs.Dog;
			DogRenderData dogRenderData = new DogRenderData();

			Vector2 startPosition = GridToWorld(dog.Position);

			dogRenderData.WorldGameObject = Instantiate(
				_packPrefabs[dog.Pack],
				startPosition,
				Quaternion.identity,
				_dogsGameObject.transform
			);
			dogRenderData.WorldGameObject.name = "Dog" + dog.Pack + dog.Id;

			// dogRenderData.Animator = dogRenderData.WorldGameObject.GetComponent<Animator>();
            Debug.Log(dog.Id);

			_dogRenderData[dog.Id] = dogRenderData;
            Debug.Log(_dogRenderData);

			// PlayAnimation(dog, DogAnimationType.Idle);
            
        }

        private void OnDisable() {
            MapSystem.OnUpdateMapRender -= UpdateMapRender;
			EntitySystem.OnCreateDog -= CreateDogRenderData;
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
        
		private Vector3 GridToWorld(int x, int y)
		{
			Vector3 worldPosition = _grid.CellToWorld(new Vector3Int(x, y, 0));
			worldPosition.y += 1 / 4f;

			return worldPosition;
		}

		private Vector3 GridToWorld(int2 position)
		{
			return GridToWorld(position.x, position.y);
		}
        
    }
}
