//
//  Chunk.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public sealed class Chunk
{
	public const int SizeBits = 4;
	public const int Size = 1 << SizeBits;

	private Tile[] tiles;
	private Tile[] overlayTiles;

	private BoardManager boardManager;

	private Vector2i worldPos;
	private Vector3 fWorldPos;

	private static MeshData meshData = new MeshData();

	private Mesh[] meshes = new Mesh[MeshData.MaxMeshes];

	public bool FlaggedForUpdate { get; set; }
	 
	public Vector2i Position
	{
		get { return worldPos; }
	}

	public Chunk(int chunkX, int chunkZ, BoardManager boardManager)
	{
		tiles = new Tile[Size * Size];
		overlayTiles = new Tile[Size * Size];

		for (int i = 0; i < tiles.Length; i++)
		{
			tiles[i] = TileType.Air;
			overlayTiles[i] = TileType.Air;
		}
		
		this.boardManager = boardManager;

		worldPos = new Vector2i(chunkX * Size, chunkZ * Size);
		fWorldPos = new Vector3(worldPos.x, worldPos.y);
	}

	public Tile GetTile(int x, int y)
	{
		return tiles[(y * Size) + x];
	}

	public Tile GetOverlayTile(int x, int y)
	{
		return overlayTiles[(y * Size) + x];
	}

	public void SetTile(int x, int y, Tile tile)
	{
		if (tile.IsOverlay)
			overlayTiles[(y * Size) + x] = tile;
		else
			tiles[(y * Size) + x] = tile;
	}

	public void DeleteTile(int x, int y)
	{
		int index = (y * Size) + x;

		if (overlayTiles[index].ID != 0)
			overlayTiles[index] = TileType.Air;
		else
			tiles[index] = TileType.Air;
	}

	public void BuildMesh()
	{
		FlaggedForUpdate = false;

		for (int x = 0; x < Size; x++)
		{
			for (int y = 0; y < Size; y++)
			{
				Tile tile = GetTile(x, y);
				Tile overlay = GetOverlayTile(x, y);

				if (tile.ID != 0)
					tile.Build(x, y, meshData, false);

				if (overlay.ID != 0)
					overlay.Build(x, y, meshData, true);
			}
		}

		for (int i = 0; i < meshes.Length; i++)
		{
			GameObject.Destroy(meshes[i]);
			meshes[i] = meshData.GetMesh(i);
		}

		meshData.Clear();
	}

	public void Render()
	{
		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i] != null)
				Graphics.DrawMesh(meshes[i], fWorldPos, Quaternion.identity, boardManager.GetMaterial(i), 0);
		}
	}
}
