using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "gameSave.json");
    }

    public void SaveGame()
    {
        List<GameObjectData> objectsToSave = new List<GameObjectData>();

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Savable"))
            {
                GameObjectData data = new GameObjectData
                {
                    name = obj.name,
                    position = new float[] { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z },
                    rotation = new float[] { obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z },
                    scale = new float[] { obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z }
                };
                objectsToSave.Add(data);
            }
        }

        string json = JsonUtility.ToJson(new Serialization<GameObjectData>(objectsToSave));
        File.WriteAllText(filePath, json);
    }

    public void LoadGame()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            List<GameObjectData> objectsToLoad = JsonUtility.FromJson<Serialization<GameObjectData>>(json).ToList();

            foreach (GameObjectData data in objectsToLoad)
            {
                GameObject obj = GameObject.Find(data.name);
                if (obj == null)
                {
                    obj = new GameObject(data.name);
                }
                obj.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
                obj.transform.rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);
                obj.transform.localScale = new Vector3(data.scale[0], data.scale[1], data.scale[2]);
            }
        }
    }
}

[System.Serializable]
public class Serialization<T>
{
    public List<T> target;

    public Serialization(List<T> target)
    {
        this.target = target;
    }

    public List<T> ToList()
    {
        return target;
    }
}
