using UnityEngine;
using System.Collections.Generic;

public sealed class Chunk
{
	public const int SizeBits = 4;
	public const int Size = 1 << SizeBits;

	private Tile[][] tiles = new Tile[2][];

	private Vector2i chunkPos;
	public Vector2i ChunkPos { get { return chunkPos; } }

	private Vector3 worldPos;
	public Vector3 WorldPos { get { return worldPos; } }

	private static MeshData meshData;

	private Queue<Mesh> unloadQueue = new Queue<Mesh>();

	private Mesh[] meshes;
	private Material[] materials;

	private bool flaggedForUpdate;

	public Chunk(int cX, int cY)
	{
		meshes = new Mesh[Map.MaxMeshes];
		materials = new Material[Map.MaxMeshes];

		if (meshData == null)
			meshData = new MeshData();

		tiles[0] = new Tile[Size * Size];
		tiles[1] = new Tile[Size * Size];

		chunkPos = new Vector2i(cX, cY);
		worldPos = Utils.WorldFromChunkPos(chunkPos);
	}

	private void FlagForUpdate()
	{
		if (!flaggedForUpdate)
		{
			flaggedForUpdate = true;
			Map.QueueChunkForUpdate(this);
		}
	}

	public Tile GetTile(int layer, int lX, int lY)
	{
		return tiles[layer][(lY * Size) + lX];
	}

	public void SetTile(int lX, int lY, Tile tile)
	{
		int layer = Map.GetTileType(tile).Layer;
		int index = (lY * Size) + lX;

		if (tiles[layer][index].Equals(tile))
			return;
		
		tiles[layer][index] = tile;
		FlagForUpdate();
	}

	public void DeleteTile(int lX, int lY, int tX, int tY)
	{
		int index = (lY * Size) + lX;

		if (tiles[1][index].ID != 0)
		{
			Map.GetTileType(tiles[1][index]).OnDeleted(new Vector2i(tX, tY));
			tiles[1][index] = Tiles.Air;
		}
		else
		{
			Map.GetTileType(tiles[0][index]).OnDeleted(new Vector2i(tX, tY));
			tiles[0][index] = Tiles.Air;
		}

		FlagForUpdate();
	}

	public void BuildMesh()
	{
		Debug.Log("Building mesh.");
		flaggedForUpdate = false;

		for (int lX = 0; lX < Size; lX++)
		{
			for (int lY = 0; lY < Size; lY++)
			{
				int tX = lX * TileType.Size, tY = lY * TileType.Size;

				Tile tile = GetTile(0, lX, lY);
				Tile overlay = GetTile(1, lX, lY);

				if (tile.ID != 0)
					Map.GetTileType(tile).Build(tile, tX, tY, meshData);

				if (overlay.ID != 0)
					Map.GetTileType(overlay).Build(overlay, tX, tY, meshData);
			}
		}

		for (int i = 0; i < meshes.Length; i++)
		{
			unloadQueue.Enqueue(meshes[i]);
			meshes[i] = meshData.GetMesh(i);
			materials[i] = meshData.GetMaterial(i);
		}

		meshData.Clear();
	}

	public void Render()
	{
		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i] != null)
				Graphics.DrawMesh(meshes[i], worldPos, Quaternion.identity, materials[i], 0);
		}

		while (unloadQueue.Count > 0)
			GameObject.Destroy(unloadQueue.Dequeue());
	}

	public void Destroy()
	{
		for (int i = 0; i < meshes.Length; i++)
			GameObject.Destroy(meshes[i]);
	}

	public void Load(string data)
	{
		ChunkData loadedData = JsonUtility.FromJson<ChunkData>(data);

		ReadData(loadedData.layer0, tiles[0]);
		ReadData(loadedData.layer1, tiles[1]);

		BuildMesh();
	}

	public void Save(MapData data)
	{
		int pos = (chunkPos.y * Map.WidthInChunks) + chunkPos.x;
		data.savedChunks.Add(pos);

		ChunkData saveData = new ChunkData();

		WriteData(tiles[0], saveData.layer0);
		WriteData(tiles[1], saveData.layer1);

		string chunkData = JsonUtility.ToJson(saveData);
		data.chunkData.Add(chunkData);
	}

	private void WriteData(Tile[] source, List<ushort> destination)
	{
		ushort airCount = 0;

		for (int i = 0; i < source.Length; i++)
		{
			Tile nextTile = source[i];

			if (nextTile.ID != 0)
			{
				if (airCount > 0)
				{
					destination.Add(0);
					destination.Add(airCount);
					airCount = 0;
				}
					
				destination.Add(nextTile.ID);
				destination.Add(nextTile.Data);
			}
			else
			{
				airCount++;

				if (i == source.Length - 1)
				{
					destination.Add(0);
					destination.Add(airCount);
				}
			}
		}
	}

	private void ReadData(List<ushort> source, Tile[] destination)
	{
		int pos = 0;

		for (int i = 0; i <= source.Count - 2; i += 2)
		{
			ushort ID = source[i];
			ushort data = source[i + 1];

			if (ID == 0)
			{
				for (int j = 0; j < data; j++)
				{
					destination[pos] = Tiles.Air;
					pos++;
				}
			}
			else
			{
				destination[pos] = new Tile(ID, data);
				pos++;
			}
		}
	}
}
