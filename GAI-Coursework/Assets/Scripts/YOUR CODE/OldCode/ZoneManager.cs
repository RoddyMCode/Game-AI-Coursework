using UnityEngine;
using System.Collections.Generic;

public class ZoneManager
{
    public int zoneWidth = 20;  // 100 / 20 = 5 zones across
    public int zoneHeight = 20;

    private int zonesX;
    private int zonesY;
    private Map map;

    // Track which zone has been visited
    private HashSet<int> visitedZones = new HashSet<int>();

    public ZoneManager(Map mapRef)
    {
        map = mapRef;

        zonesX = Map.MapWidth / zoneWidth;
        zonesY = Map.MapHeight / zoneHeight;
    }

    // Get zone index from world-space tile position
    public int GetZoneIndex(int x, int y)
    {
        int zx = x / zoneWidth;
        int zy = y / zoneHeight;
        return zx + zy * zonesX;
    }

    public void MarkZoneVisited(int zoneIndex)
    {
        visitedZones.Add(zoneIndex);
    }

    public bool IsVisited(int zoneIndex)
    {
        return visitedZones.Contains(zoneIndex);
    }

    // Pick a zone that has not been visited yet
    public int GetUnvisitedZone()
    {
        List<int> unvisited = new List<int>();

        int totalZones = zonesX * zonesY;
        for (int i = 0; i < totalZones; i++)
        {
            if (!visitedZones.Contains(i))
                unvisited.Add(i);
        }

        if (unvisited.Count == 0)
            return -1; // all done

        return unvisited[Random.Range(0, unvisited.Count)];
    }

    // Returns a random *walkable* tile inside a zone
    public int GetRandomTileInZone(int zoneIndex)
    {
        int zx = zoneIndex % zonesX;
        int zy = zoneIndex / zonesX;

        int startX = zx * zoneWidth;
        int startY = zy * zoneHeight;

        for (int tries = 0; tries < 200; tries++)
        {
            int x = Random.Range(startX, startX + zoneWidth);
            int y = Random.Range(startY, startY + zoneHeight);

            if (map.IsNavigatable(x, y))
                return map.MapIndex(x, y);
        }

        return -1;
    }
}
