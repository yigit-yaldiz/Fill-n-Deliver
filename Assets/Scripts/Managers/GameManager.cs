using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameStates
{
    Upgrade,
    Placement,
    Delivery,
    Accident,
    Reverse
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _levelCompletedPanel;
    public static GameManager Instance { get; private set; }

    public GameStates GameState { get; private set; }

    public delegate void GameStateChangeAction();
    public static event GameStateChangeAction OnGameStateChanged;

    private bool _isLevelCompleted;

    void Awake()
    {
        Instance = this;
        GameState = GameStates.Placement;
        ProductListManager.ResetDictionary();
    }
    private void OnEnable()
    {
        AccidentController.OnAccident += OnAccident;
    }
    private void OnDisable()
    {
        AccidentController.OnAccident -= OnAccident;        
    }
    void OnAccident()
    {
        if (AccidentController.Instance.State == AccidentStates.PlayerRolling)
        {
            ChangeGameState(GameStates.Accident);
        }
    }
    public void OnAccidentFinished()
    {
        ChangeGameState(GameStates.Delivery);
    }

    #region OnClick Methods for Inspector
    public void ChangeGameStateToUpgrade()
    {
        ChangeGameState(GameStates.Upgrade);
        //MotorcycleColliderUpgrader.ChangeLidStatus(true);
    }

    public void ChangeGameStateToPlacement()
    {
        ChangeGameState(GameStates.Placement);
    }
    #endregion

    public void ChangeGameState(GameStates state)
    {
        GameState = state;
        OnGameStateChanged?.Invoke();
    }

    public void ChangeLevelStatusToFinished()
    {
        _isLevelCompleted = true;
    }

    public void ActivateCompletePanel(float delayTime)
    {
        if (!_isLevelCompleted)
        {
            return;
        }

        StartCoroutine(ActivateFinishPanel());

        IEnumerator ActivateFinishPanel()
        {
            yield return new WaitForSeconds(delayTime);
            _levelCompletedPanel.SetActive(true);
        }
    }
}
