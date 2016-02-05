//
//  MeshBuilder.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public sealed class MeshBuilder
{
	private readonly Vector3[] squareVertices =
	{
		new Vector3(0.5f, -0.5f),
		new Vector3(0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f)
	};

	public void BuildSquare(byte meshIndex, int x, int z, MeshData meshData)
	{
		AddSquareIndices(meshIndex, meshData);

		for (int i = 0; i < squareVertices.Length; i++)
			meshData.AddVertex(meshIndex, squareVertices[i], x, z);

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
