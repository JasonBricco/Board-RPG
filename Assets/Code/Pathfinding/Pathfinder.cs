﻿using UnityEngine;
using System.Collections.Generic;

public sealed class Pathfinder
{
	private PathNode[,] searchArea = new PathNode[Map.Size, Map.Size];

	private List<PathNode> openList = new List<PathNode>();
	private List<PathNode> closedList = new List<PathNode>();

	public List<Vector2i> FindPath(Vector2i start, Vector2i end)
	{
		if (start.Equals(end))
			return new List<Vector2i>();

		ResetNodes();

		PathNode startNode = searchArea[start.x, start.y];
		PathNode endNode = searchArea[end.x, end.y];

		startNode.open = true;

		startNode.distanceToEnd = Heuristic(start, end);
		startNode.accumCost = 0;

		openList.Add(startNode);

		while (openList.Count > 0)
		{
			PathNode current = FindBestNode();

			if (current == null)
				break;

			if (current == endNode)
				return FindFinalPath(startNode, endNode);

			for (int i = 0; i < 4; i++)
			{
				Vector2i neighborPos = current.tilePos + Vector2i.directions[i];

				PathNode neighbor = null;

				if (Map.InTileBounds(neighborPos.x, neighborPos.y))
					neighbor = searchArea[neighborPos.x, neighborPos.y];

				if (neighbor == null || !neighbor.passable)
					continue;

				byte cost = (byte)(current.accumCost + neighbor.tileCost);
				byte heuristic = Heuristic(neighbor.tilePos, end);

				if (!neighbor.open && !neighbor.closed)
				{
					neighbor.accumCost = cost;
					neighbor.distanceToEnd = (byte)(cost + heuristic);
					neighbor.parent = current;
					neighbor.open = true;
					openList.Add(neighbor);
				}
				else
				{
					if (neighbor.accumCost > cost)
					{
						neighbor.accumCost = cost;
						neighbor.distanceToEnd = (byte)(cost + heuristic);

						neighbor.parent = current;
					}
				}
			}

			openList.Remove(current);
			current.closed = true;
		}

		return new List<Vector2i>();
	}

	public void FillSearchGrid()
	{
		for (int cX = 0; cX < Map.WidthInChunks; cX++)
		{
			for (int cY = 0; cY < Map.WidthInChunks; cY++)
			{
				if (Map.GetChunkDirect(cX, cY) != null)
					BuildChunk(cX, cY);
			}
		}
	}

	public void BuildChunk(int cX, int cY)
	{
		Vector2i tPos = Utils.TileFromChunkPos(new Vector2i(cX, cY));

		for (int x = tPos.x; x < tPos.x + Chunk.Size; x++)
		{
			for (int y = tPos.y; y < tPos.y + Chunk.Size; y++)
			{
				PathNode node = new PathNode();
				node.tilePos = new Vector2i(x, y);

				TileType l0 = Map.GetTileType(0, x, y);
				TileType l1 = Map.GetTileType(1, x, y);

				node.tileCost = (byte)Mathf.Max(l0.PathCost, l1.PathCost);
				node.passable = l1.IsPassable(x, y);

				searchArea[x, y] = node;
			}
		}
	}

	private byte Heuristic(Vector2i start, Vector2i end)
	{
		return (byte)(Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y));
	}

	private PathNode FindBestNode()
	{
		if (openList.Count == 0)
			return null;
		
		PathNode current = openList[0];
		byte smallestDistance = byte.MaxValue;

		for (int i = 0; i < openList.Count; i++)
		{
			if (openList[i].distanceToEnd < smallestDistance)
			{
				current = openList[i];
				smallestDistance = current.distanceToEnd;
			}
		}

		return current;
	}

	private List<Vector2i> FindFinalPath(PathNode start, PathNode end)
	{
		closedList.Add(end);
		PathNode parent = end.parent;

		while (!parent.Equals(start))
		{
			closedList.Add(parent);
			parent = parent.parent;
		}

		List<Vector2i> finalPath = new List<Vector2i>();

		for (int i = closedList.Count - 1; i >= 0; i--)
			finalPath.Add(closedList[i].tilePos);

		return finalPath;
	}

	private void ResetNodes()
	{
		openList.Clear();
		closedList.Clear();

		for (int cX = 0; cX < Map.WidthInChunks; cX++)
		{
			for (int cY = 0; cY < Map.WidthInChunks; cY++)
			{
				if (Map.GetChunkDirect(cX, cY) != null)
					ResetChunk(cX, cY);
			}
		}
	}

	private void ResetChunk(int cX, int cY)
	{
		Vector2i tPos = Utils.TileFromChunkPos(new Vector2i(cX, cY));

		for (int x = tPos.x; x < tPos.x + Chunk.Size; x++)
		{
			for (int y = tPos.y; y < tPos.y + Chunk.Size; y++)
			{
				PathNode node = searchArea[x, y];
				node.open = false;
				node.closed = false;
				node.distanceToEnd = byte.MaxValue;
				node.accumCost = byte.MaxValue;
			}
		}
	}
}
