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
		public enum DogAnimationType
		{
			Idle,
			Walk,
		}

		private Grid _grid;
        
        private Tilemap _terrainTileMap;
        private Tilemap _treesTileMap;
		private Vector3Int terrainTileMapOrigin;
		private int2 tileMapOrigin;

        private Dictionary<TerrainType, TileBase> _terrainTiles;
        private Dictionary<VegetationType, Tile> _treeTiles;

        private GameObject _dogsGameObject;

        private Dictionary<int, DogRenderData> _dogRenderData;
		private Dictionary<EntityType, GameObject> _packPrefabs;

		// public static event EventHandler<OnDogMouseEventArgs> OnUpdateDisplayPanel;

		private void SetupTilemapResources()
		{
			_grid = GameObject.Find("TerrainGrid").GetComponent<Grid>();
            
			_terrainTileMap = GameObject.Find("TerrainTilemap").GetComponent<Tilemap>();
			_treesTileMap = GameObject.Find("TreesTilemap").GetComponent<Tilemap>();

            _terrainTiles = new Dictionary<TerrainType, TileBase>
			{
				[TerrainType.Water] = Resources.Load<RuleTile>("Tiles/Water Rule Tile"),
				[TerrainType.Sand] = Resources.Load<RuleTile>("Tiles/Sand Rule Tile"),
				[TerrainType.Ground] = Resources.Load<WeightedRandomTile>("Tiles/Grass Variation Tile"),
				[TerrainType.Mountain] = Resources.Load<RuleTile>("Tiles/Dirt Rule Tile"),
				[TerrainType.Ice] = Resources.Load<Tile>("Tiles/Ice"),
				[TerrainType.Test] = Resources.Load<Tile>("Tiles/Test")
			};

            _treeTiles = new Dictionary<VegetationType, Tile>
			{
				[VegetationType.None] = null,
				[VegetationType.BeachTree] = Resources.Load<Tile>("Tiles/Tree_Two"),
				[VegetationType.PlainTree] = Resources.Load<Tile>("Tiles/Tree_One")
			};

			terrainTileMapOrigin = _terrainTileMap.origin;	// [Important!] A fix for Tilemap pivot offset issue causing blank gap at edges.
			tileMapOrigin = new int2(terrainTileMapOrigin.x, terrainTileMapOrigin.y);
        }

        private void SetupDogResources()
		{
            _dogsGameObject = GameObject.Find("World/Entities/Dogs");
			_dogRenderData = new Dictionary<int, DogRenderData>();

			// [TODO:] See if this can be used to generate other entities as well
            _packPrefabs = new Dictionary<EntityType, GameObject>
			{
				[EntityType.Dog] = Resources.Load<GameObject>("Prefabs/Entities/Dogs/Doggie"),
				[EntityType.Deer] = Resources.Load<GameObject>("Prefabs/Entities/Deers/Deer")
			};
        }

        private void Awake()
		{
            SetupEvents();
            SetupTilemapResources();
            SetupDogResources();
        }

        private void SetupEvents()
        {
            MapSystem.OnUpdateMapRender += UpdateMapRender;
            EntitySystem.OnCreateDog += CreateDogRenderData;

			Dog.OnUpdateDogRenderDirection += UpdateDogRenderDirection;
			Dog.OnUpdateDogRenderPosition += UpdateDogRenderPosition;
        }

        // private void GetHoveredEntity(object sender, OnHoverEntityArgs entity)
        // {
        //     if(entity.Type == "dog") {
		// 		DogRenderData dog = _dogRenderData[entity.Id];
		// 		DogAttributes dogAttributes = dog.WorldGameObject.Attribute;
		// 		OnUpdateDisplayPanel?.Invoke(this, new OnDogMouseEventArgs { attribute =  });
		// 	}
        // }

        private void CreateDogRenderData(object sender, OnDogEventArgs eventArgs)
        {
			Dog dog = eventArgs.Dog;
			DogRenderData dogRenderData = new DogRenderData();

			Vector2 startPosition = GridToWorld(dog.Position);

			dogRenderData.WorldGameObject = Instantiate(
				_packPrefabs[dog.EntityType],
				startPosition,
				Quaternion.identity,
				_dogsGameObject.transform
			);
			// dogRenderData.WorldGameObject.name = Utils.GenerateRandomName() + "_" + dog.EntityType + dog.Id;
			dogRenderData.WorldGameObject.name = "dog_" + dog.Id;
			dogRenderData.WorldGameObject.AddComponent(typeof(BoxCollider2D));
			dogRenderData.WorldGameObject.GetComponent<BoxCollider2D>().isTrigger = true;

			// Attach Player Interaction Events Script to the Prefabs without making the Entities inherit Mono.
			dogRenderData.WorldGameObject.AddComponent(typeof(KOI.EntityInteraction));

			// dogRenderData.WorldGameObject.name = "Dog" + dog.EntityType + dog.Id;

			// dogRenderData.Animator = dogRenderData.WorldGameObject.GetComponent<Animator>();
            // Debug.Log(dog.Id);

			_dogRenderData[dog.Id] = dogRenderData;
            // Debug.Log(_dogRenderData);

			// PlayAnimation(dog, DogAnimationType.Idle);
            
        }

        private void OnDisable()
		{
            MapSystem.OnUpdateMapRender -= UpdateMapRender;
			EntitySystem.OnCreateDog -= CreateDogRenderData;
			
			Dog.OnUpdateDogRenderDirection -= UpdateDogRenderDirection;
			Dog.OnUpdateDogRenderPosition -= UpdateDogRenderPosition;
        }

        private void UpdateMapRender(object sender, OnMapEventArgs eventArgs)
        {
            foreach(Cell cell in eventArgs.WorldMap.Cells)
            {
                Vector3Int tilemapPosition = new Vector3Int(cell.Position.x + terrainTileMapOrigin.x , cell.Position.y + terrainTileMapOrigin.y);
                _terrainTileMap.SetTile(tilemapPosition, _terrainTiles[cell.TerrainType]);
                _treesTileMap.SetTile(tilemapPosition, _treeTiles[cell.VegetationType]);
            }
        }
        
		private Vector3 GridToWorld(int x, int y)
		{
			Vector3 worldPosition = _grid.CellToWorld(new Vector3Int(x, y, 0));
			worldPosition.x += 0.5f;
			worldPosition.y += 0.5f;

			return worldPosition;
		}

		private Vector3 GridToWorld(int2 position)
		{
			return GridToWorld(position.x + terrainTileMapOrigin.x, position.y + terrainTileMapOrigin.y);
		}

		
		private void UpdateDogRenderDirection(object sender, OnDogEventArgs eventArgs)
		{
			PlayAnimation(eventArgs.Dog, DogAnimationType.Idle);
		}

		private void UpdateDogRenderPosition(object sender, OnDogEventArgs eventArgs)
		{
			StartCoroutine(MoveDog(eventArgs.Dog));
		}

		private IEnumerator MoveDog(Dog dog)
		{
			float timer = 0;
			float duration = dog.Cooldown * GameConfig.TickDuration;

			DogRenderData dogRenderData = _dogRenderData[dog.Id];

			Vector3 startPosition = dogRenderData.WorldGameObject.transform.position;

			Vector3 endPosition = GridToWorld(dog.Position);
			// endPosition.z = dog.Id * 0.00001f;	// for isometric z sorting

			// PlayAnimation(dog, DogAnimationType.Walk);

			while (timer < duration)
			{
				timer += Time.deltaTime;

				Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, timer / duration);

				dogRenderData.WorldGameObject.transform.position = newPosition;

				yield return null;
			}

			dogRenderData.WorldGameObject.transform.position = endPosition;
		}

		private void PlayAnimation(Dog dog, DogAnimationType animationType)
		{
			DogRenderData dogRenderData = _dogRenderData[dog.Id];

			// dogRenderData.Animator.Play($"Base Layer.{ dog.EntityType }-{animationType}-{dog.Direction}");
		}

		// private void OnMouseEnter() {
		// 	object identity = GetEntityTypeIdFromName(name);
		// 	OnUpdateDisplayPanel?.Invoke(this, new OnDogEventArgs { Dog = this });
		// }

        
    }
}
