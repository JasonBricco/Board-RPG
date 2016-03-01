using UnityEngine;
using System.Collections.Generic;

public class MeshData 
{
	private readonly int MaxMeshes;

	private List<Vector3>[] vertices;
	private List<Vector2>[] uvs;
	private List<int>[] indices;
	private List<Color32>[] colors;

	public MeshData(BoardManager manager)
	{
		MaxMeshes = manager.TileCount;

		vertices = new List<Vector3>[MaxMeshes];
		uvs = new List<Vector2>[MaxMeshes];
		indices = new List<int>[MaxMeshes];
		colors = new List<Color32>[MaxMeshes];

		for (int i = 0; i < MaxMeshes; i++)
		{
			vertices[i] = new List<Vector3>(512);
			uvs[i] = new List<Vector2>(512);
			indices[i] = new List<int>(1024);
			colors[i] = new List<Color32>(512);
		}
	}

	public void AddVertex(int index, Vector3 vertex) 
	{
		vertices[index].Add(vertex);
	}

	public void AddUV(int index, Vector2 uv)
	{
		this.uvs[index].Add(uv);
	}

	public List<int> GetIndices(int index)
	{
		return indices[index];
	}

	public int GetOffset(int index)
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
