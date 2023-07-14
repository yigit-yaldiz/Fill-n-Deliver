using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplotionEffect : MonoBehaviour
{
    [SerializeField] float _startR = 0.5f;
    [SerializeField] float _finalR = 2f;
    [SerializeField] LayerMask _expLayerMask;
    const float c_expScalingSpeed = 2;
    Coroutine _coroutine;

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && GameManager.Instance.GameState == GameStates.Accident)
        {
            RaycastHit hit;
            Ray ray;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, _expLayerMask))
            {
                transform.position = hit.point;
            }
            //if (Input.GetMouseButtonDown(0))
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }
                _coroutine = StartCoroutine(ScaleExplotion());
            }
        }
    }
    
    IEnumerator ScaleExplotion()
    {
        GetComponent<Renderer>().enabled = true;
        float t = 0;
        while (t<1)
        {
            t += c_expScalingSpeed * Time.deltaTime;
            transform.localScale = Vector3.one*Mathf.Lerp(_startR, _finalR, t);
            yield return new WaitForFixedUpdate();
        }
        while (t > 0)
        {
            t -= c_expScalingSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * Mathf.Lerp(_startR, _finalR, t);
            yield return new WaitForFixedUpdate();
        }
        GetComponent<Renderer>().enabled = false;
    }
}
