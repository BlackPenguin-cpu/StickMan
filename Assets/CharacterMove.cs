using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

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
    new Collider2D collider2D;
    List<Rigidbody2D> BodyParts;

    StickManMoveSaveData saveData;
    StickManMoveData moveDatas = new StickManMoveData();

    GameObject startPoint;

    public bool die;
    public bool originalCharacter;

    private readonly string fileName = "SaveData";
    void Start()
    {
        startPoint = GameObject.Find("StartPoint");
        collider2D = GetComponent<CircleCollider2D>();
        BodyParts = transform.GetComponentsInChildren<Rigidbody2D>().ToList();

        //Save Managing
        string path = Application.persistentDataPath + "/" + fileName + ".Json";
        string json = File.ReadAllText(path);
        saveData = JsonUtility.FromJson<StickManMoveSaveData>(json);

        deepLearning();
    }

    void Update()
    {
        runningTime += Time.deltaTime;
        if (runningTime > 60) die = true;
    }
    async void deepLearning()
    {
        int MoveCount = 0;
        while (true)
        {
            if (!die)
            {
                float temp = Time.time * 100f;
                Random.InitState((int)temp);
                int nowBodyPart = 0;
                int movePower = 0;
                if (saveData == null || saveData.data.FirstOrDefault() == null)
                {
                    nowBodyPart = Random.Range(0, BodyParts.Count);
                    movePower = Random.Range(-100, 100);
                }
                else if (originalCharacter)
                {
                    if (MoveCount >= saveData.data.FirstOrDefault().torquePower.Count)
                    {
                        Debug.Assert(false);
                        nowBodyPart = Random.Range(0, BodyParts.Count);
                        movePower = Random.Range(-100, 100);
                    }
                    else
                    {
                        movePower = saveData.data.FirstOrDefault().torquePower[MoveCount];
                        nowBodyPart = saveData.data.FirstOrDefault().bodyPartNumber[MoveCount];
                    }
                }
                else if (Random.Range(0, 10) == 0)
                {
                    nowBodyPart = Random.Range(0, BodyParts.Count);
                    movePower = Random.Range(-100, 100);
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
                moveDatas.resultValue = distance / runningTime + runningTime;
                Debug.Log(moveDatas.resultValue);

                // Call On Character Fail
                DeepRunningManager.instance.saveData.data.Add(moveDatas);
                break;
            }
            await Task.Delay(10);
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
