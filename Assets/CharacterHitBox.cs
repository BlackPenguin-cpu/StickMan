using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitBox : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Contains("Platform"))
        {
            transform.GetComponentInParent<CharacterMove>().die = true;
        }
    }
}
