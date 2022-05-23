using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepRunningManager : MonoBehaviour
{
    public static DeepRunningManager instance;
    public bool allOk;
    private void Update()
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
            GameReset();
            allOk = false;
        }
    }
    void GameReset()
    {

    }
}
