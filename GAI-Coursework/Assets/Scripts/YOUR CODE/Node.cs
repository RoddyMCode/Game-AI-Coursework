public class PathNode
{       
   // represents a single node used in A* pathfinding.
 

    public int nodeNumber;
    public int goalCost;
    public int heuristicCost;
    public int FinalCost => goalCost + heuristicCost;
    public PathNode parent;

    public PathNode(int index)      // stores cost values and a reference to the parent node for path reconstruction.
    {
        this.nodeNumber = index;
    }
}
