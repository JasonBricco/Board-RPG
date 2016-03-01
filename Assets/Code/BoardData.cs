using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class BoardData
{
	public List<int> savedChunks = new List<int>();
	public List<string> chunkData = new List<string>();

	public List<Vector2i> triggerKeys = new List<Vector2i>();
	public List<string> triggerValues = new List<string>();
}

[Serializable]
public class ChunkData
{
	public List<ushort> layer0 = new List<ushort>();
	public List<ushort> layer1 = new List<ushort>();
}
