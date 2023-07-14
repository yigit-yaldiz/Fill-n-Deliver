using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniqueGames.Saving;
using Saving;

public class Case : MonoBehaviour
{
    [SerializeField] private List<GameObject> _listables = new List<GameObject>();
    private Animator _animator;

    private bool _isItChecked;

    public GameObject LastListMember => _listables[0];

    public bool IsItChecked { get => _isItChecked; set => _isItChecked = value; }

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();

        foreach (Transform child in transform)
        {
            foreach (Transform item in child)
            {
                if (!item.GetComponent<Product>().IsItSold || child.gameObject.activeInHierarchy)
                {
                    Placer placer = item.GetComponentInChildren<Placer>();
                    placer.SetMyCase(this);
                }
            }
        }
    }

    public void CheckListables()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                break;
            }

            foreach (Transform item in child)
            {
                if (!item.GetComponent<Product>().IsItSold)
                {
                    _listables.Add(item.gameObject);
                    Placer placer = item.GetComponentInChildren<Placer>();
                    placer.SetMyCase(this);
                    placer.SetTheState(Placer.State.NonPlaced);
                }
            }
        }

        if (_listables.Count == 0)
        {
            CaseEmpty();
        }
    }

    public GameObject ChooseTheNextPlacer()
    {
        if (_listables.Count == 0)
        {
            return null;
        }

        return _listables[0];
    }   

    public void RemovePreviousPlacer()
    {
        if (_listables.Count != 0)
        {
            _listables.RemoveAt(0);

            if (_listables.Count == 0)
            {
                CaseEmpty();
            }
        }
    }

    public void RemoveSpecificPlacer(GameObject gameObject)
    {
        _listables.Remove(gameObject);
    }

    private void CaseEmpty()
    {
        if (this == Cases.Instance.CaseList[0])
        {
            transform.parent.parent.localPosition += Vector3.left * 0.75f;
        }
        else if (this != Cases.Instance.CaseList[0])
        {
            transform.parent.parent.localPosition += Vector3.right * 0.75f;
        }

        Cases.Instance.RemoveFromList(this);

        if (Cases.Instance.CaseList.Count == 0)
        {
            CasePanelInput.Instance.CompletedImage.gameObject.SetActive(true);
        }

        transform.parent.gameObject.SetActive(false);
    }

    public void AddReturnedPlacer(GameObject placer)
    {
        if (!_listables.Contains(placer))
        {
            if (!Cases.Instance.CaseList.Contains(this))
            {
                Cases.Instance.AddToList(this);

                if (this == Cases.Instance.CaseList[0])
                {
                    transform.parent.parent.localPosition -= Vector3.left * 0.75f;
                }
                else if (this != Cases.Instance.CaseList[0])
                {
                    transform.parent.parent.localPosition -= Vector3.right * 0.75f;
                }

            }

            CasePanelInput.Instance.CompletedImage.gameObject.SetActive(false);
            transform.parent.gameObject.SetActive(true);
            
            _listables.Add(placer);
            _listables = _listables.OrderBy(x => x.transform.parent.name).ToList(); //list sorted by item names
        }
    }

    public void SetTrigger()
    {
        _animator.enabled = true;
        _animator.SetTrigger("click");
    }
}
