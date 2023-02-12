using System.Collections.Generic;

namespace KOI
{
    public class WorldMap
    {
        public int Width {get; private set;}
        public int Height {get; private set;}
        public int Area => Width * Height;
        public List<Cell> Cells;

        public WorldMap(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new List<Cell>(Area);
        }
    }
}