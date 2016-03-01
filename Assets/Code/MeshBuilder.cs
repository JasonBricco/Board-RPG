using UnityEngine;
using System.Collections.Generic;

public sealed class MeshBuilder
{
	public void BuildSquare(TileType type, Tile tile, int tX, int tY, MeshData meshData)
	{
		int meshIndex = type.MeshIndex;

		AddSquareIndices(meshIndex, meshData);

		float z = type.Layer == 1 ? 1.0f : 2.0f;

		meshData.AddVertex(meshIndex, new Vector3(tX + TileType.HalfSize, tY - TileType.HalfSize, z));
		meshData.AddVertex(meshIndex, new Vector3(tX + TileType.HalfSize, tY + TileType.HalfSize, z));
		meshData.AddVertex(meshIndex, new Vector3(tX - TileType.HalfSize, tY + TileType.HalfSize, z));
		meshData.AddVertex(meshIndex, new Vector3(tX - TileType.HalfSize, tY - TileType.HalfSize, z));

		type.SetUVs(tile, meshData);
	}

	private void AddSquareIndices(int index, MeshData data)
	{
		List<int> indices = data.GetIndices(index);
		int offset = data.GetOffset(index);

		indices.Add(offset + 2);
		indices.Add(offset + 1);
		indices.Add(offset + 0);

		indices.Add(offset + 3);
		indices.Add(offset + 2);
		indices.Add(offset + 0);
	}
}
