using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HighScoreManager : MonoBehaviour
    {
        public Text highScoreText;

        void Start()
        {
            highScore = PlayerPrefs.GetFloat("highScore");

            highScoreText.text = ((int)GameManager.HighScore).ToString();
        }

        public void Return()
        {
            SceneManager.LoadScene("Menu");
        }

    }
}