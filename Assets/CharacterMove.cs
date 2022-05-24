using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

[System.Serializable]
public class StickManMoveData
{
    public List<int> bodyPartNumber = new List<int>();
    public List<int> torquePower = new List<int>();
    public float resultValue;
}

[System.Serializable]
public class StickManMoveSaveData
{
    public List<StickManMoveData> data =
        new List<StickManMoveData>();
}
public class CharacterMove : MonoBehaviour
{
    public float runningTime;

    Collider2D collider2D;
    List<Rigidbody2D> BodyParts;

    StickManMoveSaveData saveData;
    StickManMoveData moveDatas = new StickManMoveData();

    GameObject startPoint;

    public bool die;
    public bool originalCharacter;

    string fileName = "SaveData";
    void Start()
    {
        startPoint = GameObject.Find("StartPoint");
        collider2D = GetComponent<CircleCollider2D>();
        BodyParts = transform.GetComponentsInChildren<Rigidbody2D>().ToList();

        //Save Managing
        string path = Application.persistentDataPath + "/" + fileName + ".Json";
        string json = File.ReadAllText(path);
        saveData = JsonUtility.FromJson<StickManMoveSaveData>(json);


        StartCoroutine(deepRunniung());
    }

    void Update()
    {
        runningTime += Time.deltaTime;
    }
    IEnumerator deepRunniung()
    {
        while (true)
        {
            int MoveCount = 0;
            if (!die && runningTime < 60)
            {
                int nowBodyPart = 0;
                int movePower = 0;
                if (saveData == null || saveData.data.FirstOrDefault() == null)
                {
                    nowBodyPart = Random.Range(0, BodyParts.Count);
                    movePower = Random.Range(-500, 500);
                }
                else if (originalCharacter)
                {
                    movePower = saveData.data.FirstOrDefault().torquePower[MoveCount];
                    nowBodyPart = saveData.data.FirstOrDefault().bodyPartNumber[MoveCount];
                }
                else if (Random.Range(0, 10) == 0)
                {
                    nowBodyPart = Random.Range(0, BodyParts.Count);
                    movePower = Random.Range(-500, 500);
                }
                else
                {
                    movePower = saveData.data.FirstOrDefault().torquePower[MoveCount];
                    nowBodyPart = saveData.data.FirstOrDefault().bodyPartNumber[MoveCount];
                }

                MoveCount++;
                BodyParts[nowBodyPart].AddTorque(movePower);

                moveDatas.bodyPartNumber.Add(nowBodyPart);
                moveDatas.torquePower.Add(movePower);
            }
            else
            {
                die = true;

                float distance = transform.position.x - startPoint.transform.position.x;
                moveDatas.resultValue = distance / runningTime/* - runningTime*/;

                // Call On Character Fail
                DeepRunningManager.instance.saveData.data.Add(moveDatas);
                break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Contains("Platform"))
        {
            die = true;
        }
    }
}
