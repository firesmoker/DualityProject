using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Header("Falling")]
        public float fallingSpeed;
        public float acceleration;
        public float minimalObstaclesGap;

        public AnimationCurve cameraSizePerFallingSpeed;

        [Header("Spawning")]
        public AnimationCurve generationRateProbability;
        public AnimationCurve widthProbability;
        public Transform obstacleSpawningPosition;
        public GameObject obstaclesContainer;

        [Header("Obstacles")]
        public GameObject redObstaclePrefab;
        public GameObject blueObstaclePrefab;

        [Header("UI")]
        public TextMesh scoreText;
        public TextMesh highScoreText;

        [Header("Cameras")]
        public Camera gameCamera;
        public Camera uiCamera;

        [Header("Display")]
        public SpriteRenderer blackScreen;
        public SpriteRenderer redBackground;
        public AnimationCurve backgroundAnimation;

        private float score = 0;
        private static float highScore = 0;

        private bool isAlive = true;

        public void SetBackground(float flipState)
        {
            var alpha = backgroundAnimation.EvaluateByPolarity(flipState, Player.Single.polarity);
            var prevColor = redBackground.color;
            var nextColor = new Color(prevColor.r, prevColor.g, prevColor.b, alpha);
            redBackground.color = nextColor;
        }

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
            gameCamera.orthographicSize = cameraSizePerFallingSpeed.Evaluate(fallingSpeed);

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
