using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Camera))]
public sealed class GameCamera : MonoBehaviour
{
	private float speed = 10.0f * Tile.Size;
	private float minX, maxX, minZ, maxZ;

	private Vector3 dragOrigin, screenOrigin;
	private bool isHoming = false;

	private void Awake()
	{
		Serializer.ListenForLoad(LoadData);
		Serializer.ListenForSave(SaveData);

		EventManager.StartListening("StateChanged", StateChangedHandler);
	}

	private void Start()
	{
		float vertical = Camera.main.orthographicSize;
		float horizontal = vertical * Screen.width / Screen.height;

		minX = horizontal - Tile.HalfSize;
		maxX = (Map.Size * Tile.Size) - horizontal - Tile.HalfSize;
		minZ = vertical - Tile.HalfSize;
		maxZ = (Map.Size * Tile.Size) - vertical - Tile.HalfSize;
	}

	private void StateChangedHandler(Data data)
	{
		if (data.state == GameState.Editing)
			StopAllCoroutines();
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

	public void MoveToTarget(Vector3 target)
	{
		isHoming = true;
		StartCoroutine(DoMoveToTarget(target));
	}

	private IEnumerator DoMoveToTarget(Vector3 target)
	{
		float t = 0.0f;

		while (t < 1.0f)
		{
			transform.SetXY(Vector3.Lerp(transform.position, target, t));
			t += Time.deltaTime;
			yield return null;
		}

		transform.SetXY(target);
		isHoming = false;
	}

	public void MoveTowardsTarget(Vector3 target, float followSpeed)
	{
		StopAllCoroutines();
		isHoming = true;
		StartCoroutine(DoMoveTowardsTarget(target, followSpeed));
	}

	private IEnumerator DoMoveTowardsTarget(Vector3 target, float followSpeed)
	{
		target.z = transform.position.z;

		while ((transform.position - target).magnitude > 0.05f)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * followSpeed);
			yield return null;
		}

		isHoming = false;
	}

	private void LateUpdate()
	{
		switch (StateManager.CurrentState)
		{
		case GameState.Window:
			return;

		case GameState.Editing:
			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");

			Vector3 move = new Vector3(x, y, 0.0f) * speed * Time.deltaTime;
			transform.Translate(move);

			break;

		case GameState.Playing:
			if (isHoming) break;

			if (Input.GetMouseButton(0))
			{
				float h = 25.0f * Input.GetAxis("Mouse X");
				float v = 25.0f * Input.GetAxis("Mouse Y");

				transform.Translate(-h, -v, 0);
			}
	
			break;
		}

		Vector3 pos = transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.y = Mathf.Clamp(pos.y, minZ, maxZ);
		transform.position = pos;
	}
}
