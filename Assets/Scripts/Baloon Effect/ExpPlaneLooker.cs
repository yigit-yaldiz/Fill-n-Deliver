using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPlaneLooker : MonoBehaviour
{
    void Update()
    {
        if(GameManager.Instance.GameState == GameStates.Accident)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(Vector3.right * 90);
        }
    }
}
