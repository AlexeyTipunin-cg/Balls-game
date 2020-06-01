using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class MainHUD : UIBase
    {
        [SerializeField] private Text scoreText;

        public override void UpdateUI()
        {
            scoreText.text = "Score: " + Controller.totalScore;
        }
    }
}