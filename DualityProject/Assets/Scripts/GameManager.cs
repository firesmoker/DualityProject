using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public float fallingSpeed;
        public AnimationCurve generationRateProbability;
        public AnimationCurve widthProbability;
        public GameObject redObstaclePrefab, blueObstaclePrefab;
        public GameObject obstaclesContainer;
        public Transform obstacleSpawningPosition;
        public SpriteRenderer blackScreen;
        public float acceleration;
        public float minimalObstaclesGap;
        public TextMesh scoreText;
        public TextMesh highScoreText;

        private float score = 0;
        private static float highScore = 0;

        private bool isAlive = true;
        private float timeToSpawn = 0;

        public static GameManager Single;

        public void Start()
        {
            Single = this;
            score = 0;
        }

        public void Update()
        {
            timeToSpawn -= Time.deltaTime;
            
            if (timeToSpawn <= 0)
            {
                var obstacleHeight = SpawnAndReturnHeight();
                var initialTimeToSpawn = generationRateProbability.Evaluate(Random.value);
                var timeHeightAddition = (obstacleHeight + minimalObstaclesGap) / fallingSpeed;
                timeToSpawn = initialTimeToSpawn + timeHeightAddition;
            }

            fallingSpeed += acceleration * Time.deltaTime;

            if (isAlive)
            {
                UpdateScore();
            }
            else
            {
                FadeAndRestart();
            }    
        }

        private void UpdateScore()
        {
            score += Time.deltaTime * 10;
            highScore = Mathf.Max(score, highScore);
            scoreText.text = ((int)score).ToString();
            highScoreText.text = ((int)highScore).ToString();
        }

        private void FadeAndRestart()
        {
            Time.timeScale = Mathf.Max(Time.timeScale, 0.5f);

            var currentOpacity = blackScreen.color.a;
            var nextOpacity = currentOpacity + Time.deltaTime;
            var newColor = new Color(1, 1, 1, nextOpacity);
            blackScreen.color = newColor;

            if (nextOpacity >= 1)
            {
                SceneManager.LoadScene("Level1");
            }
        }

        private float SpawnAndReturnHeight()
        {
            var position = obstacleSpawningPosition.position;
            var parnet = obstaclesContainer.transform;

            var spawningObstaclePrefab = GetRandomObstaclePrefab();
            var newObstacle = Instantiate(spawningObstaclePrefab, position, Quaternion.identity, parnet);

            var scale = newObstacle.transform.localScale;
            var height = widthProbability.Evaluate(Random.value);
            newObstacle.transform.localScale = new Vector3(scale.x, height, scale.z);

            return height;
        }

        private GameObject GetRandomObstaclePrefab()
        {
            GameObject spawningObstaclePrefab;
            if (Random.Range(0, 2) == 1)
                spawningObstaclePrefab = redObstaclePrefab;
            else
                spawningObstaclePrefab = blueObstaclePrefab;

            return spawningObstaclePrefab;
        }

        public void InitiateDeath()
        {
            Destroy(Player.Single.gameObject);
            isAlive = false;
        }
    }
}
