using UnityEngine;

namespace Code
{
    public class SaveManager
    {
        public static readonly SaveManager Instance = new SaveManager();

        private int _bestScore;

        public const string BEST_SCORE = "Score";

        public int bestScore
        {
            get { return _bestScore; }
            set
            {
                if (value > _bestScore)
                {
                    _bestScore = value;
                    PlayerPrefs.SetInt(BEST_SCORE, _bestScore);
                }
            }
        }

        private SaveManager()
        {
            if (PlayerPrefs.HasKey(BEST_SCORE))
            {
                _bestScore = PlayerPrefs.GetInt(BEST_SCORE);
            }
            else
            {
                PlayerPrefs.SetInt(BEST_SCORE, 0);
            }
        }

        public void ResetBestScore()
        {
            _bestScore = 0;
            PlayerPrefs.SetInt(BEST_SCORE, 0);
        }
    }
}