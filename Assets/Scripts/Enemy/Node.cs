using UnityEngine;

public class Node
{
    public Vector2 worldPosition;
    public bool isWalkable;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost => gCost + hCost;

    public Node(bool walkable, Vector2 worldPos, int x, int y)
    {
        isWalkable = walkable;
        worldPosition = worldPos;
        gridX = x;
        gridY = y;
    }
}
