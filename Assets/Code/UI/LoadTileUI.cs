using UnityEngine;
using UnityEngine.UI;

public class LoadTileUI : MonoBehaviour
{
	private GameObject tilePrefab;
	private Transform tilesPanel;

	private void Start()
	{
		tilePrefab = Resources.Load<GameObject>("Prefabs/TileButton");
		tilesPanel = SceneItems.GetItem<Transform>("TilePanel");

		Map.OperateOnTiles(LoadTile);
	}

	private void LoadTile(TileType type)
	{
		if (type.ID == 0) return;

		GameObject tileButton = Instantiate<GameObject>(tilePrefab);
		tileButton.name = type.Name;

		Texture2D tex = type.Texture;
		tileButton.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero); 

		tileButton.transform.SetParent(tilesPanel);
	}
}
