using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class MenuManager : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("Level1");
        }

        public void ShowHighScore()
        {
            SceneManager.LoadScene("HighScore");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}