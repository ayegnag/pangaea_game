using System.Collections.Generic;
using Unity.Mathematics;

namespace KOI
{
    public static class MapConfig 
    {
        public static int WorldMapWidth = 250;
        public static int WorldMapHeight = 150;
        
        public static int Seed = 677999;   // 0 to 100000000
        // public static int Seed = 46419636;   // 0 to 100000000
        // public static float NoiseScale = 0.3f; // 0.8f gives realistically smoothest coastline but in-land heights were too uniform.
		public static float NoiseScale = 0.8f;
        public static int NoiseOctaves = 8; // 1 to 10

		public static float Radius = 3.0f;
		public static float Slope = 0.0003f;

        public static int ChanceGroundTree = 6; // 1 to 10; 6 = 40% chance
        public static int ChanceBeachTree = 8; // 1 to 10; 8 = 20% chance
        
		public static Dictionary<Direction, int2> DirectionVectors = new Dictionary<Direction, int2>
		{
			[Direction.EE] = new int2(+1, +0),
			[Direction.NE] = new int2(+1, +1),
			[Direction.NN] = new int2(+0, +1),
			[Direction.NW] = new int2(-1, +1),
			[Direction.WW] = new int2(-1, +0),
			[Direction.SW] = new int2(-1, -1),
			[Direction.SS] = new int2(+0, -1),
			[Direction.SE] = new int2(+1, -1),
		};

		public static Dictionary<Direction, int> DirectionCosts = new Dictionary<Direction, int>
		{
			[Direction.EE] = 10,
			[Direction.NE] = 14,
			[Direction.NN] = 10,
			[Direction.NW] = 14,
			[Direction.WW] = 10,
			[Direction.SW] = 14,
			[Direction.SS] = 10,
			[Direction.SE] = 14,
		};
    }
}

// 454530 -  4 Islands

// For larger map and larger island:
// WorldMapWidth = 800;
// WorldMapHeight = 600;
// Radius = 100.0f;
// Slope = 0.00003f;