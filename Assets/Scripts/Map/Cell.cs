using Unity.Mathematics;

namespace KOI
{
    public class Cell
    {
        public int Id;
        public bool Solid;
        public int2 Position;

        public TerrainType TerrainType;
        public FoliageType FoliageType;
    }
}