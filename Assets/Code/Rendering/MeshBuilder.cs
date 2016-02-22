using UnityEngine;
using System.Collections.Generic;

public sealed class MeshBuilder
{
	public void BuildSquare(byte meshIndex, int tX, int tY, MeshData meshData, bool overlay)
	{
		AddSquareIndices(meshIndex, meshData);

		float z = overlay ? 1.0f : 2.0f;

		meshData.AddVertex(meshIndex, new Vector3(tX + Tile.HalfSize, tY - Tile.HalfSize, z));
		meshData.AddVertex(meshIndex, new Vector3(tX + Tile.HalfSize, tY + Tile.HalfSize, z));
		meshData.AddVertex(meshIndex, new Vector3(tX - Tile.HalfSize, tY + Tile.HalfSize, z));
		meshData.AddVertex(meshIndex, new Vector3(tX - Tile.HalfSize, tY - Tile.HalfSize, z));

		AddSquareUVs(meshIndex, meshData);
	}

	private void AddSquareIndices(byte index, MeshData data)
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

	private void AddSquareUVs(byte index, MeshData data)
	{
		data.AddUV(index, new Vector2(1.0f, 0.0f));
		data.AddUV(index, new Vector2(1.0f, 1.0f));
		data.AddUV(index, new Vector2(0.0f, 1.0f));
		data.AddUV(index, new Vector2(0.0f, 0.0f));
	}
}
