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
	public const int Size = 512;
	public const int WidthInChunks = Size / Chunk.Size;

	private Material[] materials;

	private Chunk[,] chunks = new Chunk[WidthInChunks, WidthInChunks];

	private List<Chunk> chunksToRebuild = new List<Chunk>();

	public BoardManager(Material[] materials)
	{
		this.materials = materials;
	}

	public Material GetMaterial(int index)
	{
		return materials[index];
	}

	public Tile GetTile(int x, int y)
	{
		if (!InTileBounds(x, y)) return TileType.Air;

		Chunk chunk = chunks[x >> Chunk.SizeBits, y >> Chunk.SizeBits];
		return chunk == null ? TileType.Air : chunk.GetTile(x & Chunk.Size - 1, y & Chunk.Size - 1);
	}

	public void SetTile(Vector2i pos, Tile tile)
	{
		GetChunkSafe(pos.x, pos.y).SetTile(pos.x & Chunk.Size - 1, pos.y & Chunk.Size - 1, tile);
	}

	public void DeleteTile(Vector2i pos)
	{
		GetChunkSafe(pos.x, pos.y).DeleteTile(pos.x & Chunk.Size - 1, pos.y & Chunk.Size - 1);
	}

	private Chunk GetChunk(int worldX, int worldY)
	{
		return chunks[worldX >> Chunk.SizeBits, worldY >> Chunk.SizeBits];
	}

	private Chunk GetChunkSafe(int worldX, int worldY)
	{
		Vector2i pos = new Vector2i(worldX >> Chunk.SizeBits, worldY >> Chunk.SizeBits);
		Chunk chunk = chunks[pos.x, pos.y];

		if (chunk == null)
		{
			chunk = new Chunk(pos.x, pos.y, this);
			chunks[pos.x, pos.y] = chunk;
		}

		return chunk;
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
