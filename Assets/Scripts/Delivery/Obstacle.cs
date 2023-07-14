using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Examples;

public class Obstacle : MonoBehaviour
{
    private Coroutine _delayCoroutine;

    public bool DidItInteract;

    private void OnEnable()
    {
        DidItInteract = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool condition = collision.transform.CompareTag("Vehicle") || collision.transform.CompareTag("Player") || collision.gameObject.layer == 3;

        if (condition && !DidItInteract)
        {
            WaitAfterCrash();
        }
    }

    private void WaitAfterCrash()
    {
        DidItInteract = true;

        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
        }

        #region Layer Ignoring
        Physics.IgnoreLayerCollision(14, 15); //bike body - obstacle
        Physics.IgnoreLayerCollision(14, 3);
        Physics.IgnoreLayerCollision(11, 15); //wall - obstacle
        Physics.IgnoreLayerCollision(6, 15);  //floor wall - obstacle
        Physics.IgnoreLayerCollision(13, 16); //customer - bike
        #endregion

        AccidentController.Instance.ChangeTheState(AccidentStates.PlayerRolling); //accident state activated

        _delayCoroutine = StartCoroutine(CrashDelayCoroutine(4f));
    }

    IEnumerator CrashDelayCoroutine(float time = 0)
    {
        yield return new WaitForSeconds(time);
        DidItInteract = false;
    }
}
