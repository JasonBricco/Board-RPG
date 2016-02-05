using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridObject : MonoBehaviour 
{
	public bool updateVertices = true;
	public bool updateUVs = true;
	public bool updateColors = true;
	public bool updateTriangles = true;

	private Mesh mesh;
	private VectorLine vectorLine;
			
	public void SetVectorLine(VectorLine vectorLine, Material mat) 
	{
		MeshRenderer rend = gameObject.AddComponent<MeshRenderer>();
		rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		rend.receiveShadows = false;
		rend.useLightProbes = false;
		rend.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		rend.sharedMaterial = mat;

		gameObject.AddComponent<MeshFilter>();
		this.vectorLine = vectorLine;
		SetupMesh();
	}
	
	public void Enable(bool enable) 
	{
		if (this == null) return;
		GetComponent<MeshRenderer>().enabled = enable;
	}
	
	private void SetupMesh() 
	{
		mesh = new Mesh();
		mesh.name = vectorLine.LineName;
		mesh.hideFlags = HideFlags.HideAndDontSave;
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}
	
	private void LateUpdate()
	{
		if (updateVertices)
		{
			mesh.vertices = vectorLine.Vertices;
			updateVertices = false;
		}

		if (updateUVs) 
		{
			mesh.uv = vectorLine.UVs;
			updateUVs = false;
		}

		if (updateColors) 
		{
			mesh.colors32 = vectorLine.LineColors;
			updateColors = false;
		}

		if (updateTriangles) 
		{
			mesh.SetTriangles(vectorLine.Triangles, 0);
			updateTriangles = false;
		}
	}
	
	public void SetName(string name) 
	{
		if (mesh == null) return;
		mesh.name = name;
	}
	
	public void UpdateMeshAttributes() 
	{
		mesh.Clear();
		updateVertices = true;
		updateUVs = true;
		updateColors = true;
		updateTriangles = true;
	}
	
	public void ClearMesh() 
	{
		if (mesh == null) return;
		mesh.Clear();
	}
}
	