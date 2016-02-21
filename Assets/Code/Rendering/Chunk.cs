//
//  Chunk.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public sealed class Chunk
{
	public const int SizeBits = 4;
	public const int Size = 1 << SizeBits;

	private Tile[][] tiles = new Tile[2][];

	private BoardManager boardManager;

	private Vector2i chunkPos;
	private Vector3 worldPos;

	private static MeshData meshData = new MeshData();

	private Queue<Mesh> unloadQueue = new Queue<Mesh>();

	private Mesh[] meshes = new Mesh[MeshData.MaxMeshes];

	public bool FlaggedForUpdate { get; set; }
	 
	public Vector3 Position
	{
		get { return worldPos; }
	}

	public Chunk(int cX, int cY, BoardManager boardManager)
	{
		tiles[0] = new Tile[Size * Size];
		tiles[1] = new Tile[Size * Size];

		for (int i = 0; i < tiles[0].Length; i++)
		{
			tiles[0][i] = TileStore.Air;
			tiles[1][i] = TileStore.Air;
		}
		
		this.boardManager = boardManager;

		chunkPos = new Vector2i(cX, cY);
		worldPos = Utils.WorldFromChunkPos(chunkPos);
	}

	public Tile GetTile(int index, int lX, int lY)
	{
		return tiles[index][(lY * Size) + lX];
	}

	public void SetTile(int lX, int lY, Tile tile)
	{
		tiles[tile.PosIndex][(lY * Size) + lX] = tile;
	}

	public void DeleteTile(int lX, int lY, int tX, int tY)
	{
		int index = (lY * Size) + lX;

		if (tiles[1][index].ID != 0)
		{
			tiles[1][index].OnDeleted(boardManager.GetData(), new Vector2i(tX, tY));
			tiles[1][index] = TileStore.Air;
		}
		else
		{
			tiles[0][index].OnDeleted(boardManager.GetData(), new Vector2i(tX, tY));
			tiles[0][index] = TileStore.Air;
		}
	}

	public void BuildMesh()
	{
		FlaggedForUpdate = false;

		for (int lX = 0; lX < Size; lX++)
		{
			for (int lY = 0; lY < Size; lY++)
			{
				int tX = lX * Tile.Size, tY = lY * Tile.Size;

				Tile tile = GetTile(0, lX, lY);
				Tile overlay = GetTile(1, lX, lY);

				if (tile.ID != 0)
					tile.Build(tX, tY, meshData, false);

				if (overlay.ID != 0)
					overlay.Build(tX, tY, meshData, true);
			}
		}

		for (int i = 0; i < meshes.Length; i++)
		{
			unloadQueue.Enqueue(meshes[i]);
			meshes[i] = meshData.GetMesh(i);
		}

		meshData.Clear();
	}

	public void Render()
	{
		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i] != null)
				Graphics.DrawMesh(meshes[i], worldPos, Quaternion.identity, boardManager.GetMaterial(i), 0);
		}

		while (unloadQueue.Count > 0)
			GameObject.Destroy(unloadQueue.Dequeue());
	}

	public void Destroy()
	{
		for (int i = 0; i < meshes.Length; i++)
			GameObject.Destroy(meshes[i]);
	}

	public void Save(BoardData data)
	{
		int pos = (chunkPos.y * BoardManager.WidthInChunks) + chunkPos.x;
		data.savedChunks.Add(pos);

		ChunkData saveData = new ChunkData();
		Encoder.Encode(tiles[0], saveData, 0);
		Encoder.Encode(tiles[1], saveData, 1);

		string chunkData = JsonUtility.ToJson(saveData);
		data.chunkData.Add(chunkData);
	}

	public void Load(string data)
	{
		ChunkData loadedData = JsonUtility.FromJson<ChunkData>(data);

		Encoder.Decode(tiles[0], loadedData, 0);
		Encoder.Decode(tiles[1], loadedData, 1);

		BuildMesh();
	}
}
