using UnityEngine;
using System.Collections.Generic;

public static class Encoder 
{
	public static void Encode(Tile[] tiles, ChunkData data, int index) 
	{ 
		int i = 0;
		int length = tiles.Length;

		ushort currentCount = 0;
		ushort currentData = 0;

		for (i = 0; i < length; i++) 
		{
			ushort thisData = tiles[i].ID;

			if (thisData != currentData) 
			{
				if (i != 0) data.AddData(currentCount, currentData, index);

				currentCount = 1;
				currentData = thisData;
			}
			else
				currentCount++;

			if (i == length - 1) data.AddData(currentCount, currentData, index);
		}
	}

	public static void Decode(Tile[] tiles, ChunkData data, int index) 
	{
		List<int> runs = data.GetRuns(index);
		List<ushort> tileData = data.GetTiles(index);

		int cur = 0;

		for (int run = 0; run < runs.Count; run++)
		{
			for (int i = 0; i < runs[run]; i++)
			{
				tiles[cur] = TileStore.GetTileByID(tileData[run]);
				cur++;
			}
		}
	}
}
