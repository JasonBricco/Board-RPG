//
//  BoardData.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/6/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BoardData : ISerializationCallbackReceiver
{
	public List<int> savedChunks = new List<int>();
	public List<string> chunkData = new List<string>();
	public List<Vector2i> startTiles = new List<Vector2i>();

	public List<Vector2i> triggerKeys = new List<Vector2i>();
	public List<string> triggerValues = new List<string>();

	public Dictionary<Vector2i, string> triggerData = new Dictionary<Vector2i, string>();

	public void OnBeforeSerialize()
	{
		foreach (var item in triggerData)
		{
			triggerKeys.Add(item.Key);
			triggerValues.Add(item.Value);
		}
	}

	public void OnAfterDeserialize()
	{
		for (int i = 0; i < triggerKeys.Count; i++)
			triggerData.Add(triggerKeys[i], triggerValues[i]);

		triggerKeys.Clear();
		triggerValues.Clear();
	}

	public void ClearChunkData()
	{
		savedChunks.Clear();
		chunkData.Clear();
	}
}

[System.Serializable]
public class ChunkData
{
	public List<int> runs = new List<int>();
	public List<ushort> tiles = new List<ushort>();

	public List<int> overlayRuns = new List<int>();
	public List<ushort> overlayTiles = new List<ushort>();

	public void AddData(int count, ushort data, int index)
	{
		if (index == 0) 
		{
			runs.Add(count);
			tiles.Add(data);
		}
		else
		{
			overlayRuns.Add(count);
			overlayTiles.Add(data);
		}
	}

	public List<int> GetRuns(int index)
	{
		return index == 0 ? runs : overlayRuns;
	}

	public List<ushort> GetTiles(int index)
	{
		return index == 0 ? tiles : overlayTiles;
	}

	public void Clear()
	{
		runs.Clear();
		tiles.Clear();
		overlayRuns.Clear();
		overlayTiles.Clear();
	}
}
