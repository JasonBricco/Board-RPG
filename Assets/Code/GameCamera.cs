﻿using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]
public sealed class GameCamera : MonoBehaviour
{
	private GameObject cameraToggle;

	public Entity followTarget;

	private float speed = 10.0f * Tile.Size;
	private float minX, maxX, minZ, maxZ;

	private CameraMode mode = CameraMode.Free;

	private void Awake()
	{
		Serializer.ListenForLoad(LoadData);
		Serializer.ListenForSave(SaveData);
	}

	private void Start()
	{
		float vertical = Camera.main.orthographicSize;
		float horizontal = vertical * Screen.width / Screen.height;

		minX = horizontal - Tile.HalfSize;
		maxX = (Map.Size * Tile.Size) - horizontal - Tile.HalfSize;
		minZ = vertical - Tile.HalfSize;
		maxZ = (Map.Size * Tile.Size) - vertical - Tile.HalfSize;

		cameraToggle = SceneItems.GetItem("CameraToggle");

		EventManager.StartListening("StateChanged", StateChangedHandler);
		EventManager.StartListening("CameraTogglePressed", CameraToggledHandler);
	}

	private void StateChangedHandler(Data data)
	{
		switch (data.state)
		{
		case GameState.Playing:
			cameraToggle.SetActive(true);
			mode = CameraMode.Follow;
			break;

		case GameState.Editing:
			cameraToggle.SetActive(false);
			mode = CameraMode.Free;
			break;
		}
	}

	public void LoadData(MapData data)
	{
		if (data.cameraPos.x == -1)
		{
			Vector3 worldMid = Utils.WorldFromTilePos(new Vector2i(Map.Size >> 1, Map.Size >> 1));
			transform.position = new Vector3(worldMid.x, worldMid.y, -10.0f);
		}
		else
			transform.position = data.cameraPos;
	}

	public void SaveData(MapData data)
	{
		data.cameraPos = transform.position;
	}

	private void CameraToggledHandler(Data data)
	{
		switch (mode)
		{
		case CameraMode.Free:
			mode = CameraMode.Follow;
			transform.SetParent(null, true);
			break;

		case CameraMode.Follow:
			mode = CameraMode.Free;
			break;
		}
	}

	private void LateUpdate()
	{
		if (StateManager.CurrentState == GameState.Window) return;

		Vector3 pos;

		if (mode == CameraMode.Follow)	
		{
			pos = transform.position;

			if (Vector3.Distance(pos, followTarget.Position) > 5.0f)
				transform.SetXY(Vector3.Lerp(pos, followTarget.Position, Time.smoothDeltaTime * 5.0f));
		}
		else
		{
			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");

			Vector3 move = new Vector3(x, y, 0.0f) * speed * Time.deltaTime;
			transform.Translate(move);
		}

		pos = transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.y = Mathf.Clamp(pos.y, minZ, maxZ);
		transform.position = pos;
	}
}
