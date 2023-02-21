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

		public Cell GetCell(int id)
		{
			if (id >= _worldMap.Area) return null;

			return _worldMap.Cells[id];
		}

		public Cell GetCell(int x, int y)
		{
			int cellId = PositionToId(new int2(x, y));
            Debug.Log(x +" " + y + " " + cellId);
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
				Utils.DumpToConsole(cell);
				return cell.Occupied;
			}
			else
			{
				return true;
			}
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