using UnityEngine;
using System.Collections.Generic;

public static class Utils 
{
	public static int RoundToNearest(float value, int nearest)
	{
		return nearest * ((int)(value + nearest * 0.5f) >> 5);
	}

	public static Vector3 WorldFromTilePos(Vector2i tPos)
	{
		return new Vector3(tPos.x * TileType.Size, tPos.y * TileType.Size);
	}

	public static Vector3 WorldFromChunkPos(Vector2i cPos)
	{
		return WorldFromTilePos(TileFromChunkPos(cPos));
	}

	public static Vector2i TileFromWorldPos(Vector3 wPos)
	{
		return new Vector2i((int)wPos.x >> TileType.SizeBits, (int)wPos.y >> TileType.SizeBits);
	}

	public static Vector2i TileFromChunkPos(Vector2i cPos)
	{
		return new Vector2i(cPos.x * Chunk.Size, cPos.y * Chunk.Size);
	}

	public static void ShuffleList<T>(List<T> list)
	{
		int n = list.Count;

		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static Vector2i GetCursorTilePos()
	{
		Vector2i wPos = GetCursorWorldPos();
		return new Vector2i(wPos.x >> TileType.SizeBits, wPos.y >> TileType.SizeBits);
	}

	public static Vector2i GetCursorWorldPos()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		int x = Utils.RoundToNearest(pos.x, TileType.Size);
		int y = Utils.RoundToNearest(pos.y, TileType.Size);

		return new Vector2i(x, y);
	}

	public static Vector2i GetLineEnd(Vector2i start, Vector2i dir)
	{
		Vector2i current = start;
		int distance = 0;

		for (int i = 0; i < Map.Size; i++)
		{
			Vector2i next = current + dir;
			Tile nextTile = Map.GetTileSafe(0, next.x, next.y);
			Tile nextOverlay = Map.GetTileSafe(1, next.x, next.y);

			if (nextTile.Equals(Tiles.Stopper) || nextOverlay.Equals(Tiles.Arrow) || nextOverlay.Equals(Tiles.RandomArrow))
				return next;

			if (!Map.GetTileType(nextOverlay).IsPassable(next.x, next.y))
				return current;

			current = next;
			distance++;
		}

		return start;
	}
}
		