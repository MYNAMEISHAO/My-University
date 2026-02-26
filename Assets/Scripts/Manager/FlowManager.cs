using System;
using UnityEngine;
using System.Collections;

public class FlowManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static FlowManager instance;
    public GameState state;
    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Tutorial:
                HandleTutorial();
                break;
            case GameState.Main:
                HandleMain();
                break;
            case GameState.Minigame:
                HandleMiniGame();
                break;
            case GameState.Advertisement:
                HandleAdvertisement();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"Game state updated to: {newState}");
    }

 
    public void HandleAdvertisement()
    {
        //throw new NotImplementedException();
    }

    private void HandleShopping()
    {
        //throw new NotImplementedException();
    }

    private void HandleMiniGame()
    {
        //throw new NotImplementedException();
    }

    private void HandleMain()
    {
        //throw new NotImplementedException();
    }

  
    private void HandleTutorial()
    {
        //throw new NotImplementedException();
    }

    public enum GameState
    {
        Tutorial,
        Main,
        Minigame,
        Advertisement
    }
}
