using System;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
            _worldGenerator = new CentralLargeIslandMapGenerator();
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

		public Cell GetCell(int id)
		{
			if (id >= _worldMap.Area) {
				Debug.Log("here2: " + id + " " + _worldMap.Area);
				return null;
			}

			return _worldMap.Cells[id];
		}

		public Cell GetCell(int x, int y)
		{
			int cellId = PositionToId(new int2(x, y));
			return GetCell(cellId);
		}

		public Cell GetCell(int2 position)
		{
			return GetCell(position.x, position.y);
		}


        public bool OnMap(int x, int y)
		{
			bool insideHorizontalBounds = x >= 0 && x <= _worldMap.Width;
			bool insideVerticalBounds = y >= 0 && y <= _worldMap.Height;

			return insideHorizontalBounds && insideVerticalBounds;
		}

		public bool IsOccupied(int x, int y)
		{
			if (OnMap(x, y))
			{
				Cell cell = GetCell(x, y);
				// Utils.DumpToConsole(cell);
				if(cell == null){
					Debug.Log("mydebug: "  + x + " " + y);
					return true;
				}
				return cell.Occupied;
			}
			return true;
		}

		public bool IsOccupied(int2 position)
		{
			return IsOccupied(position.x, position.y);
		}

		public int2 GetOpenCellPosition()
		{
			int2 cellPosition;

			do
			{
				cellPosition = new int2(
					Utils.RandomRange(0, _worldMap.Width),
					Utils.RandomRange(0, _worldMap.Height)
				);
			}
			while (IsOccupied(cellPosition));

			return cellPosition;
		}

		public bool IsPassable(int2 startPosition, Direction direction)
		{
			int2 endPosition = startPosition + MapConfig.DirectionVectors[direction];

			if (IsOccupied(endPosition)) return false;

			bool cardinalMove = (
				direction == Direction.EE ||
				direction == Direction.NN ||
				direction == Direction.WW ||
				direction == Direction.SS
			);

			if (cardinalMove) return true;

			bool eastPassable = !IsOccupied(startPosition + MapConfig.DirectionVectors[Direction.EE]);
			bool northPassable = !IsOccupied(startPosition + MapConfig.DirectionVectors[Direction.NN]);
			bool westPassable = !IsOccupied(startPosition + MapConfig.DirectionVectors[Direction.WW]);
			bool southPassable = !IsOccupied(startPosition + MapConfig.DirectionVectors[Direction.SS]);

			if (direction == Direction.NE)
			{
				return northPassable && eastPassable;
			}
			else if (direction == Direction.NW)
			{
				return northPassable && westPassable;
			}
			else if (direction == Direction.SE)
			{
				return southPassable && eastPassable;
			}
			else if (direction == Direction.SW)
			{
				return southPassable && westPassable;
			}

			return false;
		}

		public List<Direction> LocatePredators(int2 startPosition, float awarenessRadius){
			List<Direction> predatorDirections = new List<Direction>();
			Array directionsArray = Enum.GetValues(typeof(Direction));
			for(int i = 0; i < directionsArray.Length; i++){
				for(int j = 0; j < awarenessRadius; j++){
					Direction direction = (Direction)directionsArray.GetValue(i);
					int2 endPosition = startPosition + (j + 1) * MapConfig.DirectionVectors[direction];
					int x = endPosition.x;				
					int y = endPosition.y;				
					Cell cell = GetCell(x, y);
					Debug.Log("searching for predator " + cell.TerrainType);
					if(cell.TerrainType == TerrainType.Test){ // change this to condition checking for actual predator
						predatorDirections.Add(direction); 
						Debug.Log("predator found");
					}
				}
			}
			return predatorDirections;
		}

		public List<Direction> DetermineEscapeDirections(int2 startPosition, float awarenessRadius){
			List<Direction> predatorDirections = LocatePredators(startPosition, awarenessRadius);
			List<Direction> directions = (Enum.GetValues(typeof(Direction))).Cast<Direction>().ToList();
		    List<Direction> escapeDirections = directions.Except(predatorDirections).ToList();
			return escapeDirections;
		}

		public List<Direction> DetermineFoodDirections(int2 startPosition, float awarenessRadius){
			List<Direction> foodDirections = new List<Direction>();
			Array directionsArray = Enum.GetValues(typeof(Direction));
			for(int i = 0; i < directionsArray.Length; i++){
				for(int j = 0; j < awarenessRadius; j++){
					Direction direction = (Direction)directionsArray.GetValue(i);
					int2 endPosition = startPosition + (j + 1) * MapConfig.DirectionVectors[direction];
					int x = endPosition.x;				
					int y = endPosition.y;				
					Cell cell = GetCell(x, y);
					Debug.Log("searching for food " + cell.TerrainType);
					// if(cell.TerrainType == TerrainType.Test){ // change this to condition checking for actual food
					// 	foodDirections.Add(direction); 
					// 	Debug.Log("food found");
					// }
				}
			}
			return foodDirections;
		}

		public bool EatingFood(int2 position){
			Cell cell = GetCell(position.x, position.y);
			return cell.TerrainType == TerrainType.Test;
		}

        		
        public int PositionToId(int x, int y)
		{
			// return (x + _worldMap.Width) + _worldMap.Width * (y + _worldMap.Height);
			return ((x * _worldMap.Height) + y);
			// return x % 250 + y;
		}

		public int PositionToId(int2 position)
		{
			return PositionToId(position.x, position.y);
		}

        protected override void Tick(object sender, OnTickArgs eventArgs)
        {
            // Debug.Log(eventArgs.Tick);
        }

        public override void Quit()
        {
            GameStateManager.OnTick -= Tick;
        }
    }
}