
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code
{
    public class RestartWindow : UIBase
    {
        [SerializeField] private GameObject _restartButton;
        [SerializeField] private Text textField;

        public override void StartUI()
        {
            base.StartUI();
            _restartButton.getOrAddComponent<PointerEventsDispatcher>().onClick += onClick;
        }

        public override void OnOpen()
        {
            if (Controller.hasWon)
            {
                textField.text = "!!!!!!YOU HAVE WON!!!!!!!!!";
            }
            else
            {
                textField.text = "!!!!!!YOU LOOSE!!!!!!!!!";
            }
        }

        private void onClick(PointerEventData data)
        {
            FieldManager.Instance.clearField();
            FieldManager.Instance.CreateField();
            gameObject.SetActive(false);
        }
    }
}