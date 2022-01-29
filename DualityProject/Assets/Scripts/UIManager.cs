using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public GameObject menu;
        public void OpenMenu()
        {
            Time.timeScale = 0;
            menu.SetActive(true);
        }

        public void Resume()
        {
            Time.timeScale = 1;
            menu.SetActive(false);
        }

        public void Quit()
        {
            SceneManager.LoadScene("Menu");
        }

        public void ToggleMenu()
        {
            if (GameManager.Single.IsAlive)
            { 
                if (menu.activeSelf)
                    Resume();
                else
                    OpenMenu();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu();
            }
        }
    }
}