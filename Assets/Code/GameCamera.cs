using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]
public sealed class GameCamera : MonoBehaviour
{
	private GameObject cameraToggle;

	public Entity followTarget;

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

		if (mode == CameraMode.Follow)	
		{
			Vector3 pos = transform.position;

			if (Vector3.Distance(pos, followTarget.Position) > 5.0f)
				transform.SetXY(Vector3.Lerp(pos, followTarget.Position, Time.smoothDeltaTime * 5.0f));
		}
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
