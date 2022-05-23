using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class StickManMoveData
{

}

[System.Serializable]
public class StickManMoveSaveData
{
    public List<StickManMoveData> data;
}
public class CharacterMove : MonoBehaviour
{
    Collider2D collider2D = new Collider2D();
    List<Rigidbody2D> BodyParts;
    void Start()
    {
        BodyParts = transform.GetComponentsInChildren<Rigidbody2D>().ToList();
    }

    void Update()
    {
        BodyParts[Random.Range(0, BodyParts.Count)].AddTorque(Random.Range(-500, 500));
    }
}
