using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HighScoreManager : MonoBehaviour
    {
        public Text highScoreText;

        void Start()
        {
            highScoreText.text = ((int)GameManager.HighScore).ToString();
        }

    }
}