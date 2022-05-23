using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

[System.Serializable]
public class StickManMoveData
{
    public List<int> bodyPartNumber;
    public List<int> torquePower;
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
    bool OnSave;

    string fileName = "SaveData";
    void Start()
    {
        startPoint = GameObject.Find("StartPoint");
        collider2D = GetComponent<CircleCollider2D>();
        BodyParts = transform.GetComponentsInChildren<Rigidbody2D>().ToList();

        //Save Managing
        string path = "Resources/" + fileName + ".Json";
        string json = File.ReadAllText(path);
        FileInfo file = new FileInfo(path);
        if (file != null)
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
            if (!die && runningTime < 60)
            {
                int nowBodyPart = Random.Range(0, BodyParts.Count);
                int movePower = Random.Range(-500, 500);

                BodyParts[nowBodyPart].AddTorque(movePower);

                moveDatas.bodyPartNumber.Add(nowBodyPart);
                moveDatas.torquePower.Add(movePower);
            }
            else
            {
                die = true;

                float distance = transform.position.x - startPoint.transform.position.x;
                moveDatas.resultValue = distance / runningTime;

                if (!OnSave)
                {
                    characterReset();
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    /// <summary>
    /// Call On Character Fail
    /// </summary>
    private void characterReset()
    {
        OnSave = true;
        saveData.data.Add(moveDatas);
        string json = JsonUtility.ToJson(saveData);
        string path = "Resources/" + fileName + ".Json";

        File.WriteAllText(path, json);

        Instantiate(Resources.Load("Character"), Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Contains("Platform"))
        {
            die = true;
        }
    }
}
