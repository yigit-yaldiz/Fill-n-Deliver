using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    public void TransferMoneyToPlayer(Transform target)
    {
        GetComponent<MeshRenderer>().enabled = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position + new Vector3(0, 1, 0);
        float t = 0;

        StartCoroutine(TimeCoroutine());

        transform.SetParent(target);

        IEnumerator TimeCoroutine()
        {
            while (t < 1)
            {
                targetPos = target.position + new Vector3(0, 1, 0);
                t += Time.deltaTime * 2;
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
