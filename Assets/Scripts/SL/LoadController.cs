using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Unity.VisualScripting;

public class LoadController : MonoBehaviour
{
    public static LoadController Instance;

    //是否通过读取存档进入游戏
    public bool isLoadGameFromMainMenu = false;

    //是否通过开始游戏进入游戏
    public bool isPlayGameFromMainMenu = false;

    public bool isStartRebirth = false;

    //存档的数据
    public SaveGameObjects savingData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveBySerialization(float x, float y)
    {
        SaveGameObjects save = new SaveGameObjects();

        save.savingPositionX = x;
        save.savingPositionY = y;
        save.SceneNo = SceneManager.GetActiveScene().buildIndex;
        save.StoneCount = playerControlSystem.Instance.StoneCount;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create("data.a");
        bf.Serialize(file, save);
        file.Close();
    }

    public void LoadByDeserialization()
    {
        if (File.Exists("data.a"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("data.a", FileMode.Open);
            SaveGameObjects save = bf.Deserialize(file) as SaveGameObjects;

            savingData = save;

            file.Close();
        }
        else
        {
            Debug.LogError("Data not found");
        }
    }
}