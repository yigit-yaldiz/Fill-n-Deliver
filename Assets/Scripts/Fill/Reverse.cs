using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Reverse : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _reverseCountText;
    public static bool HasItDoneReversed;

    private int _reverseCount = 100;
    private bool _isReverseActive;
    private Button _button;
    private Image _buttonImage;

    public static Reverse Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _button = GetComponent<Button>();
        _buttonImage = GetComponent<Image>();
        ChangeButtonScale(Vector3.one * 3); //default scale
        _reverseCountText.text = _reverseCount.ToString();
    }

    public void ReverseModButton()
    {
        if (PlacementRay.Instance.FirstTime && PlacementRay.Instance.Animator != null)
        {
            PlacementRay.Instance.Animator.ResetTrigger("placed");
            PlacementRay.Instance.Animator.SetTrigger("reverseClicked");
        }

        if (!_isReverseActive)
        {
            _isReverseActive = true;
            GameManager.Instance.ChangeGameState(GameStates.Reverse);
            ChangeButtonScale(Vector3.one * 2.5f);
            _buttonImage.color = Color.green;
            ProductListFreezer.Instance.FreezeAllProductsInBasket();
        }
        else if (_isReverseActive)
        {
            _isReverseActive = false;
            DecreaseReverseCount();
            GameManager.Instance.ChangeGameState(GameStates.Placement);
            ChangeButtonScale(Vector3.one * 3);
            _buttonImage.color = Color.white;
            ProductListFreezer.Instance.DefrostAllProductsInBasket();
        }
    }

    public void ChangeButtonScale(Vector3 scale)
    {
        _button.transform.localScale = scale;
    }

    public void DecreaseReverseCount()
    {
        if (HasItDoneReversed)
        {
            _reverseCount--;
            _reverseCountText.text = _reverseCount.ToString();
            HasItDoneReversed = false;
        }
    }

    void DeatcivateReverseMod()
    {
        _isReverseActive = false;
        DecreaseReverseCount();
        GameManager.Instance.ChangeGameState(GameStates.Placement);
        ChangeButtonScale(Vector3.one * 3);
        _buttonImage.color = Color.white;
        ProductListFreezer.Instance.DefrostAllProductsInBasket();
        Debug.Log("Deactivated");
    }
}
