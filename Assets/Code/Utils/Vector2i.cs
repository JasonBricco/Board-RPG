//
//  Vector2i.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public struct Vector2i : System.IEquatable<Vector2i>
{
	public int x, y;

	public static readonly Vector2i zero = new Vector2i(0, 0);
	public static readonly Vector2i one = new Vector2i(1, 1);
	public static readonly Vector2i up = new Vector2i(0, 1);
	public static readonly Vector2i down = new Vector2i(0, -1);
	public static readonly Vector2i left = new Vector2i(-1, 0);
	public static readonly Vector2i right = new Vector2i(1, 0);

	public static readonly Vector2i[] directions = new Vector2i[] 
	{
		left, right,
		down, up
	};

	public Vector2i(int x, int z) 
	{
		this.x = x;
		this.y = z;
	}

	public Vector2i(Vector3 vector)
	{
		this.x = Mathf.RoundToInt(vector.x);
		this.y = Mathf.RoundToInt(vector.y);
	}

	public int DistanceSquared(Vector2i v) 
	{
		return DistanceSquared(this, v);
	}

	public static int DistanceSquared(Vector2i a, Vector2i b) 
	{
		int dx = b.x - a.x;
		int dy = b.y - a.y;

		return dx * dx + dy * dy;
	}

	public override int GetHashCode() 
	{
		return (x.GetHashCode() ^ y.GetHashCode() << 2);
	}

	public bool Equals(Vector2i vector) 
	{
		return x == vector.x && y == vector.y;
	}

	public override string ToString() 
	{
		return "(" + x + ", " + y + ")";
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, y);
	}

	public static Vector2i Min(Vector2i a, Vector2i b) 
	{
		return new Vector2i(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
	}

	public static Vector2i Max(Vector2i a, Vector2i b) 
	{
		return new Vector2i(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
	}

	public static Vector2i operator - (Vector2i a) 
	{
		return new Vector2i(-a.x, -a.y);
	}

	public static Vector2i operator - (Vector2i a, Vector2i b) 
	{
		return new Vector2i(a.x - b.x, a.y - b.y);
	}

	public static Vector2i operator + (Vector2i a, Vector2i b) 
	{
		return new Vector2i(a.x + b.x, a.y + b.y);
	}

	public static Vector2i operator * (Vector2i vector, int scalar)
	{
		return new Vector2i(vector.x * scalar, vector.y * scalar);
	}

	public static implicit operator Vector2(Vector2i v) 
	{
		return new Vector3(v.x, v.y);
	}
}
