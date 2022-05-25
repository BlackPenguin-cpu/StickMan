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
    private void Start()
    {
        //Time.timeScale = 10;
    }
    #region FixedUpdate
    private void FixedUpdate()
    {
        if (instance == null)
            instance = this;

        //만약 씬에 모든 딥러닝 모델이 딥러닝이 끝났을경우 러닝을 다시 시작합니다.
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
    #endregion
    /// <summary>
    /// 게임을 리셋 시켜줍니다.
    /// </summary>
    #region GameReset
    void GameReset()
    {
        //저장된 값을 내립차순으로 정렬합니다 (높은것부터 낮은거 순)
        var data = from save in saveData.data
                   orderby save.resultValue descending
                   select save;

        //상위 10개의 데이터만 남기고 나머지를 잘라서 리스트로 저장합니다.
        saveData.data = data.Take(10).ToList();

        string json = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + "/" + fileName + ".Json";
        File.WriteAllText(path, json);

        //씬에 존재하는 모델에 갯수만큼 반복하여 모델을 다시 제작합니다.
        CharacterMove[] characters = FindObjectsOfType<CharacterMove>();
        foreach (CharacterMove character in characters)
        {
            if (character.originalCharacter)
            {
                CharacterMove obj = Instantiate(characterPrefab, character.startPos, Quaternion.identity);
                obj.originalCharacter = true;
            }
            else
                Instantiate(characterPrefab, character.startPos, Quaternion.identity);
            Destroy(character.gameObject);
        }
    }
    #endregion
}
