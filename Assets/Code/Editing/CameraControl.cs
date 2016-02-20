//
//  CameraControl.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]
public sealed class CameraControl : MonoBehaviour
{
	private Transform player;
	private float speed = 10.0f * Tile.Size;
	private float minX, maxX, minZ, maxZ;

	private void Start()
	{
		float vertical = Camera.main.orthographicSize;
		float horizontal = vertical * Screen.width / Screen.height;

		minX = horizontal - Tile.HalfSize;
		maxX = (BoardManager.Size * Tile.Size) - horizontal - Tile.HalfSize;
		minZ = vertical - Tile.HalfSize;
		maxZ = (BoardManager.Size * Tile.Size) - vertical - Tile.HalfSize;
	}

	private void Update()
	{
		if (StateManager.CurrentState == GameState.Window) return;

		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		Vector3 move = new Vector3(x, y, 0.0f) * speed * Time.deltaTime;
		transform.Translate(move);

		Vector3 pos = transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.y = Mathf.Clamp(pos.y, minZ, maxZ);
		transform.position = pos;
	}
}