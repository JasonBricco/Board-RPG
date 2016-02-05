//
//  CameraControl.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.Events;

public sealed class CameraControl : MonoBehaviour, IUpdatable
{
	private Transform player;

	private float minX, maxX, minZ, maxZ;

	private void Start()
	{
		float vertical = Camera.main.orthographicSize;
		float horizontal = vertical * Screen.width / Screen.height;

		minX = horizontal - 0.5f;
		maxX = BoardManager.Size - horizontal - 0.5f;;
		minZ = vertical - 0.5f;
		maxZ = BoardManager.Size - vertical - 0.5f;
	}

	public void UpdateTick()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		Vector3 move = new Vector3(x, y, 0.0f) * 10.0f * Time.deltaTime;
		transform.Translate(move);

		Vector3 pos = transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.y = Mathf.Clamp(pos.y, minZ, maxZ);
		transform.position = pos;
	}
}