using UnityEngine;
using UnityEngine.Events;

public enum CameraMode { Free, Follow }

[RequireComponent(typeof(Camera))]
public sealed class CameraControl : MonoBehaviour
{
	[SerializeField] private Map map;

	private GameObject cameraToggle;

	private Transform player;
	private float speed = 10.0f * TileType.Size;
	private float minX, maxX, minZ, maxZ;

	private CameraMode mode = CameraMode.Free;

	private void Start()
	{
		float vertical = Camera.main.orthographicSize;
		float horizontal = vertical * Screen.width / Screen.height;

		minX = horizontal - TileType.HalfSize;
		maxX = (Map.Size * TileType.Size) - horizontal - TileType.HalfSize;
		minZ = vertical - TileType.HalfSize;
		maxZ = (Map.Size * TileType.Size) - vertical - TileType.HalfSize;

		cameraToggle = UIStore.GetGraphic("CameraToggle");

		EventManager.StartListening("StateChanged", StateChangedHandler);
		EventManager.StartListening("CameraTogglePressed", CameraToggledHandler);
	}

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
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
			transform.SetXY(Vector3.Lerp(transform.position, map.CurrentEntity.Position, Time.deltaTime * 5.0f));
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