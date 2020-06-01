using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code
{
    public class UIController
    {
        public Canvas canvas;
        private GameObject _back;
        private GameObject _backButton;
        private GameObject _restartButton;

        private UIBase _restartWindow;
        private UIBase _mainHUD;
        
        public static readonly UIController Instance = new UIController();

        public void start()
        {
            canvas = Object.FindObjectOfType<Canvas>();

            _mainHUD = canvas.gameObject.findChildWithName("MainHUD").createUI();
            _restartWindow = canvas.gameObject.findChildWithName("RestartWindow").createUI();
            
            GameLoader.OnUpdate += updateUI;
        }

        public void openWindow()
        {
            _restartWindow.ActivateUI();
        }

        private void updateUI(float deltaTime)
        {
            _mainHUD.UpdateUI();
        }


    }
}