using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public float fallingSpeed = 1;
        public AnimationCurve generationRateProbability;
        public AnimationCurve widthProbability;
        public GameObject redObstaclePrefab, blueObstaclePrefab;
        public GameObject obstaclesContainer;
        public Transform obstacleSpawningPosition;
        public SpriteRenderer blackScreen;
        public float acceleration = 1;

        private bool isAlive = true;
        private float timeToSpawn = 0;

        public static GameManager Single;

        public void Start()
        {
            Single = this;
        }

        public void Update()
        {
            timeToSpawn -= Time.deltaTime;
            
            if (timeToSpawn <= 0)
            {
                Spawn();
                timeToSpawn = generationRateProbability.Evaluate(Random.value);
            }

            fallingSpeed += acceleration * Time.deltaTime;

            if (!isAlive)
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
        }

        private void Spawn()
        {
            var position = obstacleSpawningPosition.position;
            var parnet = obstaclesContainer.transform;

            GameObject spawningObstaclePrefab;
            if (Random.Range(0, 2) == 1)
                spawningObstaclePrefab = redObstaclePrefab;
            else
                spawningObstaclePrefab = blueObstaclePrefab;

            var newObstacle = Instantiate(spawningObstaclePrefab, position, Quaternion.identity, parnet);
            var scale = newObstacle.transform.localScale;
            newObstacle.transform.localScale = new Vector3(scale.x, widthProbability.Evaluate(Random.value), scale.z);
        }

        public void Die()
        {
            Destroy(Player.Single.gameObject);
            isAlive = false;
        }
    }
}
