//using UnityEngine;

//public class NodeGrid
//{
//    public Node[,] nodes;
//    public int Width => Map.MapWidth;
//    public int Height => Map.MapHeight;

//    private Map map;

//    public NodeGrid(Map map)
//    {
//        this.map = map;
//        nodes = new Node[Width, Height];

//        for (int y = 0; y < Height; y++)
//        {
//            for (int x = 0; x < Width; x++)
//            {
//                // Get terrain and walkability from the existing Map
//                Map.Terrain terrain = map.GetTerrainAt(x, y);
//                bool walkable = map.IsNavigatable(x, y);

//                float cost = 1f;
//                switch (terrain)
//                {
//                    case Map.Terrain.Grass:
//                        cost = 1f;
//                        break;
//                    case Map.Terrain.Mud:
//                        cost = 2f;
//                        break;
//                    case Map.Terrain.Water:
//                        cost = 5f; // higher cost (slower)
//                        break;
//                    case Map.Terrain.Tree:
//                        // trees are blocked
//                        walkable = false;
//                        cost = 100000f; // huge cost (failsafe)
//                        break;
//                    default:
//                        cost = 1f;
//                        break;
//                }

//                nodes[x, y] = new Node(x, y, walkable, cost);
//            }
//        }
//    }

//    public Node GetNode(int x, int y)
//    {
//        if (x < 0 || x >= Width || y < 0 || y >= Height) return null;
//        return nodes[x, y];
//    }
//}
