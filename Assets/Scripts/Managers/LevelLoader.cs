using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniqueGames.Saving;
using Saving;
using GameAnalyticsSDK;

public class LevelLoader : MonoBehaviour, ISaveable
{
    [SerializeField] Animator _transitionAnimator;
    const float _transitionTime = 1f;

    int _activeSceneIndex;
    static bool _isGAInitialized;
    static bool CanLoadRandomScene;
    public static LevelLoader Instance { get; private set; }
    public int ActiveSceneIndex { get => _activeSceneIndex; private set => _activeSceneIndex = value; }

    private void Awake()
    {
        Instance = this;
        Customer.CustomerCount = 0;
        
        if (!_isGAInitialized)
        {
            _isGAInitialized = true;
            GameAnalytics.Initialize();
        }
    }

    private void Start()
    {
        if (_activeSceneIndex == 0)//New game
        {
            _activeSceneIndex = 1;
        }
        if (SceneManager.GetActiveScene().buildIndex != _activeSceneIndex)
        {
            LoadSpecificScene(_activeSceneIndex);
        }
    }

    public void LoadSpecificScene(int buildIndex)
    {
        StartCoroutine(LoadLevel(buildIndex));
    }

    public void LoadNextLevel()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_" + _activeSceneIndex.ToString("D3"));
        if (_activeSceneIndex == SceneManager.sceneCountInBuildSettings - 1) //last scene
        {
            CanLoadRandomScene = true;
        }

        if (CanLoadRandomScene)
        {
            LoadRandomScene(); //random level 2-10
        }
        else
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1)); //next level
        }
    }

    public void LoadRandomScene()
    {
        int randomSceneIndex = Random.Range(2, SceneManager.sceneCountInBuildSettings); 

        while (_activeSceneIndex == randomSceneIndex)
        {
            randomSceneIndex = Random.Range(2, SceneManager.sceneCountInBuildSettings);

           if (_activeSceneIndex != randomSceneIndex)
           {
                StartCoroutine(LoadLevel(randomSceneIndex));
                break;
           }
        }

        Debug.Log("Random Scene Loaded");

        StartCoroutine(LoadLevel(randomSceneIndex));
    }

    IEnumerator LoadLevel(int buildIndex)
    {
        _activeSceneIndex = buildIndex;

        SaveWrapper.Instance.Save();
        SaveWrapper.Instance.SaveEmpty();

        _transitionAnimator.SetTrigger("Start");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level_" + _activeSceneIndex.ToString("D3"));
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            _transitionAnimator.SetTrigger("End");
            SceneManager.LoadScene(buildIndex);
        }
        else
        {
            yield return new WaitForSeconds(_transitionTime);
            _transitionAnimator.SetTrigger("End");
            SceneManager.LoadScene(buildIndex);
        }
    }

    [System.Serializable]
    struct SaveData
    {
        public int activeSceneIndex;
        public bool canLoadRandomScene;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();
        saveData.activeSceneIndex = _activeSceneIndex;
        saveData.canLoadRandomScene = CanLoadRandomScene;

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _activeSceneIndex = saveData.activeSceneIndex;
        CanLoadRandomScene = saveData.canLoadRandomScene;
    }
}
