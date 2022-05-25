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

        //���� ���� ��� ������ ���� �������� ��������� ������ �ٽ� �����մϴ�.
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
    /// ������ ���� �����ݴϴ�.
    /// </summary>
    #region GameReset
    void GameReset()
    {
        //����� ���� ������������ �����մϴ� (�����ͺ��� ������ ��)
        var data = from save in saveData.data
                   orderby save.resultValue descending
                   select save;

        //���� 10���� �����͸� ����� �������� �߶� ����Ʈ�� �����մϴ�.
        saveData.data = data.Take(10).ToList();

        string json = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + "/" + fileName + ".Json";
        File.WriteAllText(path, json);

        //���� �����ϴ� �𵨿� ������ŭ �ݺ��Ͽ� ���� �ٽ� �����մϴ�.
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
