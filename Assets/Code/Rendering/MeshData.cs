//
//  MeshData.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public class MeshData 
{
	public const int MaxMeshes = 2;

	private List<Vector3>[] vertices = new List<Vector3>[MaxMeshes];
	private List<Vector2>[] uvs = new List<Vector2>[MaxMeshes];
	private List<int>[] indices = new List<int>[MaxMeshes];
	private List<Color32>[] colors = new List<Color32>[MaxMeshes];

	public MeshData()
	{
		for (int i = 0; i < MaxMeshes; i++)
		{
			vertices[i] = new List<Vector3>(512);
			uvs[i] = new List<Vector2>(512);
			indices[i] = new List<int>(1024);
			colors[i] = new List<Color32>(512);
		}
	}

	public void AddVertex(byte index, Vector3 vertex) 
	{
		vertices[index].Add(vertex);
	}

	public void AddUV(byte index, Vector2 uv)
	{
		this.uvs[index].Add(uv);
	}

	public List<int> GetIndices(byte index)
	{
		return indices[index];
	}

	public int GetOffset(byte index)
	{
		return vertices[index].Count;
	}

	public Mesh GetMesh(int index) 
	{
		if (vertices[index].Count == 0)
			return null;

		Mesh mesh = new Mesh();

		mesh.SetVertices(vertices[index]);
		mesh.SetUVs(0, uvs[index]);
		mesh.SetColors(colors[index]);
		mesh.SetTriangles(indices[index], 0);

		return mesh;
	}

	public void Clear()
	{
		for (int i = 0; i < MaxMeshes; i++)
		{
			vertices[i].Clear();
			uvs[i].Clear();
			indices[i].Clear();
			colors[i].Clear();
		}
	}
}
