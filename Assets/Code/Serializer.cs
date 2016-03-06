using UnityEngine;
using System.IO;

public class Serializer : MonoBehaviour 
{
	public delegate void SerializationDelegate(MapData data);

	private static event SerializationDelegate OnSave;
	private static event SerializationDelegate OnLoad;

	private void Start()
	{
		Load();
	}

	public static void ListenForSave(SerializationDelegate func)
	{
		OnSave += func;
	}

	public static void ListenForLoad(SerializationDelegate func)
	{
		OnLoad += func;
	}

	public static void Save()
	{
		MapData data = new MapData();

		if (OnSave != null) 
			OnSave(data);

		FileStream stream = new FileStream(Application.persistentDataPath + "/Data.txt", FileMode.Create);
		StreamWriter dataWriter = new StreamWriter(stream);

		string json = JsonUtility.ToJson(data);
		dataWriter.Write(json);
		dataWriter.Close();
	}

	public static void Load()
	{
		string path = Application.persistentDataPath + "/Data.txt";

		if (File.Exists(path))
		{
			StreamReader reader = new StreamReader(path);
			string json = reader.ReadToEnd();
			MapData data = JsonUtility.FromJson<MapData>(json);
			reader.Close();

			if (OnLoad != null)
				OnLoad(data);
		}
	}

	private void OnApplicationQuit()
	{
		if (StateManager.CurrentState != GameState.Playing)
			Save();
	}
}
