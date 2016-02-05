//
//  BoardManager.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public sealed class BoardManager 
{
	public const int Size = 1024;
	public const int WidthInChunks = Size / Chunk.Size;

	private Material[] materials;

	private Chunk[,] chunks = new Chunk[WidthInChunks, WidthInChunks];
	private Tile[] tiles = new Tile[Size * Size];

	private List<Chunk> chunksToRebuild = new List<Chunk>();

	public BoardManager(Material[] materials)
	{
		this.materials = materials;

		for (int i = 0; i < tiles.Length; i++)
			tiles[i] = TileType.Air;

		for (int x = 0; x < chunks.GetLength(0); x++)
		{
			for (int y = 0; y < chunks.GetLength(1); y++)
				chunks[x, y] = new Chunk(x, y, this);
		}
	}

	public Material GetMaterial(int index)
	{
		return materials[index];
	}

	public Tile GetTile(int x, int y)
	{
		return tiles[(y * Size) + x];
	}

	public void SetTile(Vector2i pos, Tile tile)
	{
		tiles[(pos.y * Size) + pos.x] = tile;
	}

	public Chunk GetChunk(int worldX, int worldY)
	{
		return chunks[worldX >> Chunk.SizeBits, worldY >> Chunk.SizeBits];
	}

	public Chunk GetChunkDirect(int chunkX, int chunkY)
	{
		return chunks[chunkX, chunkY];
	}

	public void FlagChunkForRebuild(Vector2i worldPos)
	{
		Chunk chunk = GetChunk(worldPos.x, worldPos.y);

		if (!chunk.FlaggedForUpdate)
		{
			chunk.FlaggedForUpdate = true;
			chunksToRebuild.Add(chunk);
		}
	}

	public void RebuildChunks()
	{
		for (int i = 0; i < chunksToRebuild.Count; i++)
			chunksToRebuild[i].BuildMesh();

		chunksToRebuild.Clear();
	}

	public bool InTileBounds(int x, int y)
	{
		return x >= 0 && y >= 0 && x < Size && y < Size;
	}

	public bool InChunkBounds(int x, int y)
	{
		return x >= 0 && y >= 0 && x < WidthInChunks && y < WidthInChunks; 
	}
}
