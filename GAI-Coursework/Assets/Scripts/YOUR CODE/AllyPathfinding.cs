using System.Collections.Generic;
using UnityEngine;

public class AllyPathfinding : MonoBehaviour
{
    private Map map;  

    public void Initialise(Map mapRef)
    {
        map = mapRef;
    }
    public List<int> FindPath(int startIndex, int targetIndex)
    {
        var openList = new List<PathNode>();                   // Nodes to be evaluated
        var closedSet = new HashSet<int>();                    // Nodes already evaluated
        var createdNodes = new Dictionary<int, PathNode>();    // dictionary of created nodes

        PathNode startNode = new PathNode(startIndex);
        openList.Add(startNode);
        createdNodes[startIndex] = startNode;

        while (openList.Count > 0)
        {
            // Sort so the node with the lowest final cost (g + h) is processed first
            openList.Sort((a, b) => a.FinalCost.CompareTo(b.FinalCost));

            PathNode currentNode = openList[0];
            openList.RemoveAt(0);

            // Goal reached → build and return path
            if (currentNode.nodeNumber == targetIndex)
                return RetracePath(currentNode);

            closedSet.Add(currentNode.nodeNumber);

            foreach (int neighbourIndex in GetNeighbours(currentNode.nodeNumber))
            {
                if (closedSet.Contains(neighbourIndex))
                    continue;

                if (!map.IsNavigatable(neighbourIndex))
                    continue;

                // Cost of moving to this neighbour
                Map.Terrain neighbourTerrain = map.GetTerrainAt(neighbourIndex);
                int terrainCost = TerrainCost.GetCost(neighbourTerrain);

                // Encourage pathfinder to avoid trees by increasing cost
                if (IsNearTree(neighbourIndex))
                    terrainCost += 5;

                int newGCost = currentNode.goalCost + terrainCost;

                // Retrieve or create the node
                PathNode neighbourNode;
                if (!createdNodes.ContainsKey(neighbourIndex))
                {
                    neighbourNode = new PathNode(neighbourIndex);
                    createdNodes[neighbourIndex] = neighbourNode;
                }
                else
                {
                    neighbourNode = createdNodes[neighbourIndex];
                }

                // If new path is better, or this is the first time visiting
                bool isBetterPath = newGCost < neighbourNode.goalCost || !openList.Contains(neighbourNode);
                if (isBetterPath)
                {
                    neighbourNode.goalCost = newGCost;
                    neighbourNode.heuristicCost = Heuristic(neighbourIndex, targetIndex);
                    neighbourNode.parent = currentNode;

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        return null; // No valid path found
    }

   // gets the distance
    private int Heuristic(int indexA, int indexB)
    {
        int ax = map.MapIndexToX(indexA);
        int ay = map.MapIndexToY(indexA);
        int bx = map.MapIndexToX(indexB);
        int by = map.MapIndexToY(indexB);

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    
    // creates a path by following parent links back to start, then reversing.
  
    private List<int> RetracePath(PathNode endNode)
    {
        List<int> path = new List<int>();
        PathNode current = endNode;

        while (current != null)
        {
            path.Add(current.nodeNumber);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

   
    // gets grid nodes from all 4 directions
    
    private IEnumerable<int> GetNeighbours(int index)
    {
        int x = map.MapIndexToX(index);
        int y = map.MapIndexToY(index);

        if (map.IsNavigatable(x - 1, y)) yield return map.MapIndex(x - 1, y);
        if (map.IsNavigatable(x + 1, y)) yield return map.MapIndex(x + 1, y);
        if (map.IsNavigatable(x, y - 1)) yield return map.MapIndex(x, y - 1);
        if (map.IsNavigatable(x, y + 1)) yield return map.MapIndex(x, y + 1);
    }

   
    
   // movement near trees costs more any tiles contain a tree.
    private bool IsNearTree(int index)
    {
        int x = map.MapIndexToX(index);
        int y = map.MapIndexToY(index);

        // Check 8 surrounding cells
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int nx = x + dx;
                int ny = y + dy;

                if (nx < 0 || nx >= Map.MapWidth || ny < 0 || ny >= Map.MapHeight)
                    continue;

                if (map.GetTerrainAt(nx, ny) == Map.Terrain.Tree)
                    return true;
            }
        }

        return false;
    }
}
