using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RefInt 
{
	public int i;

	public RefInt(int value) 
	{
		i = value;
	}
}

public class VectorLine 
{
	public const int MaxPoints = 32768;

	private Vector3[] vertices;
	private Vector2[] uvs;
	private Color32[] lineColors;
	private List<int> triangles;
	private GridObject gridObject;

	private int vertexCount;
	private GameObject go;
	private RectTransform rectTransform;

	private List<Vector3> points;
	private int pointsCount;
	private Vector3[] screenPoints;

	private float[] lineWidths;
	private float lineWidth;

	private string name;
	private Material material;
	private bool active = true;

	private int drawStart = 0;
	private int drawEnd = 0;

	private float lineUVBottom;
	private float lineUVTop;

	private static Vector3 v3zero = Vector3.zero;
	private static Transform camTransform;
	private static Camera cam;

	public Vector3[] Vertices 
	{
		get { return vertices; }
	}

	public Vector2[] UVs 
	{
		get { return uvs; }
	}

	public Color32[] LineColors 
	{
		get {return lineColors;}
	}

	public List<int> Triangles 
	{
		get { return triangles; }
	}
		
	public int PointsCount 
	{
		get { return points.Count; }
	}

	public string LineName
	{
		get { return name; }
		set 
		{
			name = value;

			if (go != null)
				go.name = value;

			if (gridObject != null)
				gridObject.SetName(value);
		}
	}

	public bool Active
	{
		get { return active; }
		set 
		{
			active = value;

			if (gridObject != null)
				gridObject.Enable (value);
		}
	}

	public VectorLine(string name, List<Vector3> points, float width) 
	{
		this.points = points;
		SetCamera(Camera.main);
		SetupLine(name, width);
		SetupCanvasState();
	}

	private void SetupLine(string lineName, float width) 
	{
		pointsCount = (points.Capacity > 0 && points.Count == 0)? points.Capacity : points.Count;
		int count = pointsCount - points.Count;

		for (int i = 0; i < count; i++)
			points.Add (Vector3.zero);

		SetVertexCount();

		go = new GameObject(LineName);
		go.isStatic = true;
		rectTransform = go.AddComponent<RectTransform>();
		SetupTransform(rectTransform);

		vertices = new Vector3[vertexCount];
		uvs = new Vector2[vertexCount];
		lineColors = new Color32[vertexCount];
		lineUVBottom = 0.0f;
		lineUVTop = 1.0f;
		SetUVs(0, GetSegmentNumber());
		triangles = new List<int>();

		LineName = lineName;
		lineWidths = new float[1];
		lineWidths[0] = width * 0.5f;
		lineWidth = width;

		screenPoints = new Vector3[vertexCount];

		drawStart = 0;
		drawEnd = pointsCount - 1;

		SetupTriangles(0);
	}

	private void SetupTriangles(int startVert) 
	{
		int triangleCount = 0, end = 0;

		if (PointsCount > 0) 
		{
			triangleCount = (PointsCount >> 1) * 6;
			end = PointsCount * 2;
		}

		if (triangles.Count > triangleCount) 
		{
			triangles.RemoveRange(triangleCount, triangles.Count - triangleCount);

			if (gridObject != null)
				gridObject.updateTriangles = true;

			return;
		}

		for (int i = startVert; i < end; i += 4) 
		{
			triangles.Add(i); 
			triangles.Add(i + 1); 
			triangles.Add(i + 3);

			triangles.Add(i + 1); 
			triangles.Add(i + 2); 
			triangles.Add(i + 3);
		}

		if (gridObject != null)
			gridObject.updateTriangles = true;
	}

	private static void SetupTransform(RectTransform rectTransform) 
	{
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.zero;
		rectTransform.pivot = Vector2.zero;
		rectTransform.anchoredPosition = Vector2.zero;
	}

	private void ResizeMeshArrays(int newCount) 
	{
		System.Array.Resize(ref vertices, newCount);
		System.Array.Resize(ref uvs, newCount);
		System.Array.Resize(ref lineColors, newCount);
	}

	private void Resize() 
	{
		int oldCount = pointsCount;
		int originalSegmentCount = pointsCount;

		originalSegmentCount = pointsCount >> 1;

		bool adjustDrawEnd = (drawEnd == pointsCount - 1 || drawEnd < 1);

		SetVertexCount();

		pointsCount = PointsCount;
		int baseArrayLength = vertices.Length;

		if (baseArrayLength < vertexCount) 
		{
			if (baseArrayLength == 0)
				baseArrayLength = 4;
		}

		while (baseArrayLength < pointsCount) 
		{
			baseArrayLength *= 2;

			baseArrayLength = Mathf.Min(baseArrayLength, MaxPoints);
			ResizeMeshArrays (baseArrayLength*4);

			System.Array.Resize (ref screenPoints, baseArrayLength * 4);
		}

		if (lineWidths.Length > 1) 
		{
			baseArrayLength = baseArrayLength >> 1;

			if (baseArrayLength > lineWidths.Length)
				System.Array.Resize(ref lineWidths, baseArrayLength);
		}

		if (adjustDrawEnd)
			drawEnd = pointsCount - 1;

		drawStart = Mathf.Clamp(drawStart, 0, pointsCount - 1);
		drawEnd = Mathf.Clamp(drawEnd, 0, pointsCount - 1);

		if (pointsCount > originalSegmentCount) 
		{
			SetColor(Color.black, originalSegmentCount, GetSegmentNumber());
			SetUVs(originalSegmentCount, GetSegmentNumber());

			if (lineWidths.Length > 1)
				SetWidth(lineWidth, originalSegmentCount, GetSegmentNumber());
		}

		if (pointsCount < oldCount)
			ZeroVertices(pointsCount, oldCount);

		SetupTriangles(originalSegmentCount * 4);

		if (gridObject != null) {
			gridObject.UpdateMeshAttributes();
		}
	}

	private void SetUVs(int startIndex, int endIndex) 
	{
		var uv1 = new Vector2(0.0f, lineUVTop);
		var uv2 = new Vector2(1.0f, lineUVTop);
		var uv3 = new Vector2(1.0f, lineUVBottom);
		var uv4 = new Vector2(0.0f, lineUVBottom);
		int idx = startIndex * 4;

		for (int i = startIndex; i < endIndex; i++)
		{
			uvs[idx] = uv1;
			uvs[idx + 1] = uv2;
			uvs[idx + 2] = uv3;
			uvs[idx + 3] = uv4;
			idx += 4;
		}

		if (gridObject != null)
			gridObject.updateUVs = true;
	}

	private void SetVertexCount() 
	{
		vertexCount = Mathf.Max (0, GetSegmentNumber() * 4);

		if ((PointsCount & 1) != 0) {
			vertexCount += 4;
		}
	}

	private static void SetCamera(Camera camera) 
	{
		camTransform = camera.transform;
		cam = camera;
	}

	private int GetSegmentNumber() 
	{
		return PointsCount >> 1;
	}

	private void SetColor(Color32 color, int startIndex, int endIndex) 
	{
		int max = GetSegmentNumber();
		startIndex = Mathf.Clamp(startIndex * 4, 0, max * 4);
		endIndex = Mathf.Clamp((endIndex + 1) * 4, 0, max * 4);

		for (int i = startIndex; i < endIndex; i++)
			lineColors[i] = color;

		if (gridObject != null)
			gridObject.updateColors = true;
	}

	private void SetupWidths(int max) 
	{
		if ((max >= 2 && lineWidths.Length == 1) || (max >= 2 && lineWidths.Length != max)) 
		{
			System.Array.Resize(ref lineWidths, max);

			for (int i = 0; i < max; i++)
				lineWidths[i] = lineWidth * 0.5f;
		}
	}

	private void SetWidth(float width, int startIndex, int endIndex) 
	{
		int max = GetSegmentNumber();
		SetupWidths (max);
		startIndex = Mathf.Clamp (startIndex, 0, Mathf.Max (max-1, 0));
		endIndex = Mathf.Clamp (endIndex, 0, Mathf.Max (max-1, 0));
		for (int i = startIndex; i <= endIndex; i++) {
			lineWidths[i] = width * .5f;
		}
	}

	private bool CheckPointCount() 
	{
		if (PointsCount < 2) 
		{
			triangles.Clear();
			gridObject.ClearMesh();
			pointsCount = PointsCount;
			return false;
		}

		return true;
	}

	private void SetupDrawStartEnd(out int start, out int end, bool clearVertices) 
	{
		start = 0;
		end = pointsCount - 1;

		if (drawStart > 0) 
		{
			start = drawStart;
			if (clearVertices) ZeroVertices (0, start);
		}

		if (drawEnd < pointsCount - 1) 
		{
			end = drawEnd;

			if (end < 0) end = 0;
			if (clearVertices) ZeroVertices(end, pointsCount);
		}
	}

	private void ZeroVertices(int startIndex, int endIndex) 
	{
		startIndex *= 2;
		endIndex *= 2;

		for (int i = startIndex; i < endIndex; i += 2) 
		{
			vertices[i] = v3zero;
			vertices[i + 1] = v3zero;
		}
	}

	private void SetupCanvasState() 
	{
		if (go == null) return;
		go.transform.SetParent(null);

		if (go.GetComponent<GridObject>() == null) 
		{
			gridObject = go.AddComponent<GridObject>();
			if (material == null) material = Resources.Load("GridMaterial") as Material;
		}
		else
			gridObject = go.GetComponent<GridObject>();

		gridObject.SetVectorLine(this, material);
	}

	public void Draw() 
	{
		if (!active) return;

		if (!CheckPointCount() || lineWidths == null) return;
		if (PointsCount != pointsCount) Resize();

		int start = 0, end = 0, add = 0, widthIdx = 0;
		SetupDrawStartEnd(out start, out end, true);

		int idx = 0, widthIdxAdd = 0;

		if (lineWidths.Length > 1) 
		{
			widthIdx = start;
			widthIdxAdd = 1;
		}

		widthIdx /= 2;
		add = 2;
		idx = start * 2;

		Vector3 thisLine = v3zero, px = v3zero, pos1 = v3zero, pos2 = v3zero, p1 = v3zero, p2 = v3zero;
		var cameraPlane = new Plane(camTransform.forward, camTransform.position + camTransform.forward * cam.nearClipPlane);
		var ray = new Ray(v3zero, v3zero);
		float screenHeight = Screen.height;

		for (int i = start; i < end; i += add) 
		{
			p1 = points[i];
			p2 = points[i + 1];

			pos1 = cam.WorldToScreenPoint(p1);
			pos2 = cam.WorldToScreenPoint(p2);

			if ((pos1.x == pos2.x && pos1.y == pos2.y) || IntersectAndDoSkip(ref pos1, ref pos2, ref p1, ref p2, ref screenHeight, ref ray, ref cameraPlane)) 
			{
				SkipQuad(ref idx, ref widthIdx, ref widthIdxAdd);
				continue;
			}

			px.x = pos2.y - pos1.y; 
			px.y = pos1.x - pos2.x;

			thisLine = px / (float)System.Math.Sqrt(px.x * px.x + px.y * px.y);
			px.x = thisLine.x * lineWidths[widthIdx]; 
			px.y = thisLine.y * lineWidths[widthIdx];

			screenPoints[idx].x = pos1.x - px.x; 
			screenPoints[idx].y = pos1.y - px.y; 
			screenPoints[idx].z = pos1.z - px.z;
			screenPoints[idx + 3].x = pos1.x + px.x; 
			screenPoints[idx + 3].y = pos1.y + px.y; 
			screenPoints[idx + 3].z = pos1.z + px.z; 

			vertices[idx] = cam.ScreenToWorldPoint(screenPoints[idx]);
			vertices[idx + 3] = cam.ScreenToWorldPoint(screenPoints[idx + 3]);

			screenPoints[idx + 2].x = pos2.x + px.x; 
			screenPoints[idx + 2].y = pos2.y + px.y; 
			screenPoints[idx + 2].z = pos2.z + px.z;
			screenPoints[idx + 1].x = pos2.x - px.x; 
			screenPoints[idx + 1].y = pos2.y - px.y; 
			screenPoints[idx + 1].z = pos2.z - px.z;
			vertices[idx + 2] = cam.ScreenToWorldPoint(screenPoints[idx + 2]);
			vertices[idx + 1] = cam.ScreenToWorldPoint(screenPoints[idx + 1]);

			idx += 4;
			widthIdx += widthIdxAdd;
		}

		gridObject.updateVertices = true;
	}

	private bool IntersectAndDoSkip(ref Vector3 pos1, ref Vector3 pos2, ref Vector3 p1, ref Vector3 p2, ref float screenHeight, ref Ray ray, ref Plane cameraPlane) 
	{
		if (pos1.z < 0.0f) 
		{
			if (pos2.z < 0.0f) return true;
			
			pos1 = cam.WorldToScreenPoint(PlaneIntersectionPoint(ref ray, ref cameraPlane, ref p2, ref p1));

			Vector3 relativeP = camTransform.InverseTransformPoint(p1);

			if ((relativeP.y < -1.0f && pos1.y > screenHeight) || (relativeP.y > 1.0f && pos1.y < 0.0f))
				return true;
		}

		if (pos2.z < 0.0f) 
		{
			pos2 = cam.WorldToScreenPoint(PlaneIntersectionPoint(ref ray, ref cameraPlane, ref p1, ref p2));
			Vector3 relativeP = camTransform.InverseTransformPoint(p2);

			if ((relativeP.y < -1.0f && pos2.y > screenHeight) || (relativeP.y > 1.0f && pos2.y < 0.0f))
				return true;
		}

		return false;
	}

	private Vector3 PlaneIntersectionPoint(ref Ray ray, ref Plane plane, ref Vector3 p1, ref Vector3 p2) 
	{
		ray.origin = p1;
		ray.direction = p2 - p1;
		float rayDistance = 0.0f;
		plane.Raycast (ray, out rayDistance);

		return ray.GetPoint(rayDistance);
	}

	private void SkipQuad(ref int idx, ref int widthIdx, ref int widthIdxAdd) 
	{
		vertices[idx] = v3zero;
		vertices[idx + 1] = v3zero;
		vertices[idx + 2] = v3zero;
		vertices[idx + 3] = v3zero;

		screenPoints[idx] = v3zero;
		screenPoints[idx + 1] = v3zero;
		screenPoints[idx + 2] = v3zero;
		screenPoints[idx + 3] = v3zero;

		idx += 4;
		widthIdx += widthIdxAdd;
	}
}
