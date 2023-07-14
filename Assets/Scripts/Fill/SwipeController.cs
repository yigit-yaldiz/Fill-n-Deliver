using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    private Vector2 _oldMousePos;
    private const float _deltaTreshhold = 0.001f;

    public delegate void SwipeAction(float delta);
    public static event SwipeAction OnSwipe;

    #region Singleton Pattern
    public static SwipeController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    void Update()
    {
        if (!(Input.mousePosition.y < 444))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _oldMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {   
            Vector2 currentMousePos = Input.mousePosition;
            Vector2 deltaPos = currentMousePos - _oldMousePos;

            if (Mathf.Abs(deltaPos.x / Screen.width) > _deltaTreshhold && OnSwipe != null)
            {
                OnSwipe(deltaPos.x / Screen.width);
            }

            _oldMousePos = currentMousePos;
        }
    }
}
