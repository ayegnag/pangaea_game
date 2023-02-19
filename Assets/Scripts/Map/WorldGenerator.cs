namespace KOI
{
    interface WorldGenerator
    {
        void Initialize(WorldMap worldMap){}
        WorldMap GenerateMap();
    }
}