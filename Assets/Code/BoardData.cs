using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
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

[Serializable]
public class ChunkData
{
	public Tile[] tiles;
	public Tile[] overlays;
}
