//
//  CameraControl.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum CameraMode { Free, Follow }

[RequireComponent(typeof(Camera))]
public sealed class CameraControl : MonoBehaviour
{
	private PlayerManager playerManager;

	private Transform player;
	private float speed = 10.0f * Tile.Size;
	private float minX, maxX, minZ, maxZ;

	private Vector3 velocity = Vector3.zero;

	private CameraMode mode = CameraMode.Free;

	private void Start()
	{
		playerManager = Engine.Instance.GetComponent<PlayerManager>();

		float vertical = Camera.main.orthographicSize;
		float horizontal = vertical * Screen.width / Screen.height;

		minX = horizontal - Tile.HalfSize;
		maxX = (BoardManager.Size * Tile.Size) - horizontal - Tile.HalfSize;
		minZ = vertical - Tile.HalfSize;
		maxZ = (BoardManager.Size * Tile.Size) - vertical - Tile.HalfSize;

		EventManager.StartListening("StateChanged", StateChangedHandler);
		EventManager.StartListening("CameraTogglePressed", CameraToggledHandler);
	}

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
		{
		case GameState.Playing:
			mode = CameraMode.Follow;
			break;

		case GameState.Editing:
			mode = CameraMode.Free;
			break;
		}
	}

	private void CameraToggledHandler(int data)
	{
		switch (mode)
		{
		case CameraMode.Free:
			mode = CameraMode.Follow;
			break;

		case CameraMode.Follow:
			mode = CameraMode.Free;
			break;
		}
	}

	private void LateUpdate()
	{
		if (StateManager.CurrentState == GameState.Window) return;

		if (mode == CameraMode.Follow)
			transform.SetXY(Vector3.SmoothDamp(transform.position, playerManager.CurrentEntity.transform.position, ref velocity, 0.5f));
		else
		{
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
}