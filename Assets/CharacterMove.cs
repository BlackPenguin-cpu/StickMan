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
    List<Rigidbody2D> BodyParts;

    StickManMoveSaveData saveData;
    StickManMoveData moveDatas = new StickManMoveData();

    GameObject startPoint;
    public Vector3 startPos;

    public bool die;
    public bool originalCharacter;

    private readonly string fileName = "SaveData";
    void Start()
    {
        startPos = transform.position;
        startPoint = GameObject.Find("StartPoint");
        BodyParts = transform.GetComponentsInChildren<Rigidbody2D>().ToList();

        //���� �����͸� �ҷ��ɴϴ�
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
    #region ������ ���̺� �����͸� ���� �����Դϴ�.
    async void deepLearning()
    {
        int MoveCount = 0;
        //�� �ݺ����� �ش� ��ü�� �ı��Ǳ� ������ �ݺ��Ǹ� 0.01�ʸ��� �ݺ��˴ϴ�
        while (true)
        {
            if (!die)
            {
                //Random�� �õ尪�� �ʱ�ȭ ���ݴϴ�
                float temp = Time.time * 100f;
                Random.InitState((int)temp);
                int nowBodyPart = 0;
                int movePower = 0;

                if (saveData == null || saveData.data.FirstOrDefault() == null || originalCharacter || Random.Range(0, 10) < 2 || MoveCount >= saveData.data.FirstOrDefault().torquePower.Count)
                {
                    //SaveData�� ���ų� ���� ���� �����ڸ� �̰ų� 20�ۼ�Ʈ�� ���� �����ڰų� ����Ȱͺ��� ���� ����ִٸ� �����ϰ� �����Դϴ�.
                    nowBodyPart = Random.Range(0, BodyParts.Count);
                    movePower = Random.Range(-500, 500);
                }
                else
                {
                    //�� ������ ��� �ƴҰ�� ���� ���̺꿡 �ִ� ���� ������ �����͸� �����ɴϴ�
                    movePower = saveData.data.FirstOrDefault().torquePower[MoveCount];
                    nowBodyPart = saveData.data.FirstOrDefault().bodyPartNumber[MoveCount];
                }

                //MoveCount�� ���ϰ� 
                MoveCount++;
                BodyParts[nowBodyPart].AddTorque(movePower);

                moveDatas.bodyPartNumber.Add(nowBodyPart);
                moveDatas.torquePower.Add(movePower);
            }
            // ĳ���Ͱ� �ð� �ʰ� �Ǵ� �÷����� �浹�ߴٸ� ���� ������ �˴ϴ� 
            else if (die == true)
            {
                //�������� ������ �����ݴϴ�. ���⼱ (�ð���ŭ �� �Ÿ� + ��ƾ �ð�) �Դϴ�.
                float distance = transform.position.x - startPoint.transform.position.x;
                moveDatas.resultValue = distance / runningTime + runningTime;
                Debug.Log(moveDatas.resultValue);

                DeepRunningManager.instance.saveData.data.Add(moveDatas);
                break;
            }
            await Task.Delay(10);
        }
    }
    #endregion
    /// <summary>
    /// �ǰ������� Platform�� �������� �׾��� ǥ��
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Contains("Platform"))
        {
            die = true;
        }
    }
}
