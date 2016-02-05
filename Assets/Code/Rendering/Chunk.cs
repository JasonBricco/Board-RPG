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

	private BoardManager boardManager;
	private Vector2i worldPos;

	private static MeshData meshData = new MeshData();

	private Mesh[] meshes = new Mesh[MeshData.MaxMeshes];

	public bool FlaggedForUpdate { get; set; }
	 
	public Vector2i Position
	{
		get { return worldPos; }
	}

	public Chunk(int chunkX, int chunkZ, BoardManager boardManager)
	{
		this.boardManager = boardManager;
		worldPos = new Vector2i(chunkX * Size, chunkZ * Size);
	}

	public void BuildMesh()
	{
		FlaggedForUpdate = false;

		for (int x = worldPos.x; x < worldPos.x + Size; x++)
		{
			for (int y = worldPos.y; y < worldPos.y + Size; y++)
			{
				Tile tile = boardManager.GetTile(x, y);

				if (tile.ID != 0)
					tile.Build(x, y, meshData);
			}
		}

		for (int i = 0; i < meshes.Length; i++)
			meshes[i] = meshData.GetMesh(i);

		meshData.Clear();
	}

	public void Render()
	{
		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i] != null)
				Graphics.DrawMesh(meshes[i], Vector3.zero, Quaternion.identity, boardManager.GetMaterial(i), 0);
		}
	}
}
