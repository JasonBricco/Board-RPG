using UnityEngine;
using System;

public sealed class PathNode : IEquatable<PathNode>
{
	public PathNode parent;
	public Vector2i tilePos;
	public bool passable;
	public bool open;
	public bool closed;
	public byte distanceToEnd;
	public byte distanceTraveled;

	public bool Equals(PathNode other)
	{
		return tilePos.Equals(other.tilePos);
	}
}
