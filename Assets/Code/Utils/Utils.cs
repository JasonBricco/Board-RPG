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
		return new Vector3(tPos.x * Tile.Size, tPos.y * Tile.Size);
	}

	public static Vector3 WorldFromChunkPos(Vector2i cPos)
	{
		return WorldFromTilePos(TileFromChunkPos(cPos));
	}

	public static Vector2i TileFromWorldPos(Vector3 wPos)
	{
		return new Vector2i((int)wPos.x >> Tile.SizeBits, (int)wPos.y >> Tile.SizeBits);
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
		return new Vector2i(wPos.x >> Tile.SizeBits, wPos.y >> Tile.SizeBits);
	}

	public static Vector2i GetCursorWorldPos()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		int x = Utils.RoundToNearest(pos.x, Tile.Size);
		int y = Utils.RoundToNearest(pos.y, Tile.Size);

		return new Vector2i(x, y);
	}

	public static Vector2i GetLineEnd(Vector2i start, Vector2i dir)
	{
		Vector2i current = start;
		int distance = 0;

		for (int i = 0; i < Map.Size; i++)
		{
			Vector2i next = current + dir;

			if (!Map.InTileBounds(next.x, next.y))
				return current;
			
			Tile nextTile = Map.GetTile(0, next.x, next.y);
			Tile nextOverlay = Map.GetTile(1, next.x, next.y);

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
		