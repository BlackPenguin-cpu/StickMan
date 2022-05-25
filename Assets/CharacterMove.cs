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

        //저장 데이터를 불러옵니다
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
    #region 딥러닝 세이브 데이터를 토대로 움직입니다.
    async void deepLearning()
    {
        int MoveCount = 0;
        //이 반복문은 해당 객체가 파괴되기 전까지 반복되며 0.01초마다 반복됩니다
        while (true)
        {
            if (!die)
            {
                //Random에 시드값을 초기화 해줍니다
                float temp = Time.time * 100f;
                Random.InitState((int)temp);
                int nowBodyPart = 0;
                int movePower = 0;

                if (saveData == null || saveData.data.FirstOrDefault() == null || originalCharacter || Random.Range(0, 10) < 2 || MoveCount >= saveData.data.FirstOrDefault().torquePower.Count)
                {
                    //SaveData가 없거나 가장 열성 유전자모델 이거나 20퍼센트로 변이 유전자거나 저장된것보다 오래 살아있다면 랜덤하게 움직입니다.
                    nowBodyPart = Random.Range(0, BodyParts.Count);
                    movePower = Random.Range(-500, 500);
                }
                else
                {
                    //위 조건이 모두 아닐경우 원래 세이브에 있는 열성 유전자 데이터를 가져옵니다
                    movePower = saveData.data.FirstOrDefault().torquePower[MoveCount];
                    nowBodyPart = saveData.data.FirstOrDefault().bodyPartNumber[MoveCount];
                }

                //MoveCount를 더하고 
                MoveCount++;
                BodyParts[nowBodyPart].AddTorque(movePower);

                moveDatas.bodyPartNumber.Add(nowBodyPart);
                moveDatas.torquePower.Add(movePower);
            }
            // 캐릭터가 시간 초과 또는 플랫폼에 충돌했다면 죽음 판정이 됩니다 
            else if (die == true)
            {
                //딥러닝의 보상을 정해줍니다. 여기선 (시간만큼 간 거리 + 버틴 시간) 입니다.
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
    /// 피격판정에 Platform이 닿았을경우 죽었다 표시
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
