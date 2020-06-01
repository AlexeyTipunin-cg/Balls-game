using System;
using Code;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    public static event Action<float> OnUpdate;
    
    public static UIController uiController;
    private void Start()
    {
        CreateGame();
    }

    private void CreateGame()
    {
        FieldManager.Instance.CreateField();
        FieldManager.Instance.createWalls();
        
        uiController = new UIController();
        uiController.start();
    }

    void Update()
    {
        OnUpdate?.Invoke(Time.deltaTime);
    }
}