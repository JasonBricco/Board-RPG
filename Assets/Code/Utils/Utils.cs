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
		return WorldFromTilePos(new Vector2i(cPos.x * Chunk.Size, cPos.y * Chunk.Size));
	}

	public static Vector2i TileFromWorldPos(Vector3 wPos)
	{
		return new Vector2i((int)wPos.x >> TileType.SizeBits, (int)wPos.y >> TileType.SizeBits);
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
}
		