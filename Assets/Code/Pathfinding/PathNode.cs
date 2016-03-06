using UnityEngine;

public sealed class PathNode
{
	public Vector2i tilePos;
	public bool passable;
	public PathNode[] neighbors;
}
