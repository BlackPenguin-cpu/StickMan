using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class DeepRunningManager : MonoBehaviour
{
    public static DeepRunningManager instance;
    public bool allOk;
    public CharacterMove characterPrefab;
    public StickManMoveSaveData saveData;

    string fileName = "SaveData";
    private void FixedUpdate()
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

        var data = from save in saveData.data
                   orderby save.resultValue descending
                   select save;

        saveData.data = data.Take(10).ToList();

        string json = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + "/" + fileName + ".Json";

        File.WriteAllText(path, json);

        CharacterMove[] characters = FindObjectsOfType<CharacterMove>();
        foreach (CharacterMove character in characters)
        {
            if (character.originalCharacter)
            {
                CharacterMove obj = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
                obj.originalCharacter = true;
            }
            else
                Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
            Destroy(character.gameObject);
        }
    }
}
