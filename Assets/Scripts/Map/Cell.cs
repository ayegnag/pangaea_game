using Unity.Mathematics;

namespace KOI
{
    public class Cell
    {

        public int Id;
        public bool Occupied;
        public int2 Position;

        public TerrainType TerrainType;
        public VegetationType VegetationType;

        public Cell(int id)
        {
            Id = id;
        }
    }
}