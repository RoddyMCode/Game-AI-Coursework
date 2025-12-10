public static class TerrainCost
{
    public static int GetCost(Map.Terrain terrain)
    {
        switch (terrain)
        {
            case Map.Terrain.Grass: return 1;
            case Map.Terrain.Mud: return 3;
            case Map.Terrain.Water: return 5;
            default: return 100; //tree or other unavigable area
        }
    }
}
