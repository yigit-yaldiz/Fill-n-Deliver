using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniqueGames.Saving;
using Saving;

public class Cases : MonoBehaviour
{
    public List<Case> CaseList = new List<Case>();

    public static Cases Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        foreach (Transform item in transform)
        {
            if (item.gameObject.activeInHierarchy)
            {
                foreach (Transform box in item)
                {
                    CaseList.Add(box.GetComponent<Case>());
                }
            }
        }
    }

    private void Start()
    {
        if (CaseList.Count == 0)
        {
            CasePanelInput.Instance.CompletedImage.gameObject.SetActive(true);
            LevelLoader.Instance.LoadNextLevel();
        }
    }

    public void AddToList(Case box)
    {
        CaseList.Add(box);
        CaseList = CaseList.OrderBy(x => x.transform.parent.name).ToList();
    }

    public void RemoveFromList(Case box)
    {
        CaseList.Remove(box);
    }
}
