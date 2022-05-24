using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class DeepRunningManager : MonoBehaviour
{
    public static DeepRunningManager instance;
    public bool allOk;
    public GameObject characterPrefab;
    public StickManMoveSaveData saveData;

    string fileName = "SaveData";
    private void Update()
    {
        if (instance == null)
            instance = this;

        CharacterMove[] characters = FindObjectsOfType<CharacterMove>();

        allOk = true;
        foreach (CharacterMove character in characters)
        {
            if (character.die == false)
            {
                allOk = false;
            }
        }
        if (allOk)
        {
            allOk = false;
            GameReset();
        }
    }
    void GameReset()
    {

        for (int i = 0; i < 10; i++)
        {
            var data = from save in saveData.data
                       orderby save.resultValue
                       select save;

            saveData.data = data.ToList();

            string json = JsonUtility.ToJson(saveData);
            string path = Application.persistentDataPath + "/" + fileName + ".Json";

            File.WriteAllText(path, json);

            CharacterMove[] characters = FindObjectsOfType<CharacterMove>();
            Vector3 pos = new Vector3(0, 1, 0);
            foreach (CharacterMove character in characters)
            {
                Destroy(character.gameObject);
                Instantiate(characterPrefab, pos, Quaternion.identity);
            }
        }
    }
}
