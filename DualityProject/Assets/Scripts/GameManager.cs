using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public AnimationCurve bgCameraSizePerFallingSpeed;

        [Header("Spawning")]
        public AnimationCurve generationRateProbability;
        public AnimationCurve widthProbability;
        public Transform obstacleSpawningPosition;
        public Transform obstacleDestroyingPosition;
        public GameObject obstaclesContainer;

        [Header("Obstacles")]
        public GameObject lightObstaclePrefab;
        public GameObject darkObstaclePrefab;

        [Header("UI")]
        public TextMesh scoreText;
        public TextMesh highScoreText;
        public TextMesh scoreOutlineText;
        public TextMesh highScoreOutlineText;
        public Gradient uiColors;
        public Gradient uiOutlineColors;

        [Header("Cameras")]
        public Camera gameCamera;
        public Camera bgCamera;

        [Header("Display")]
        public SpriteRenderer deathScreen;
        public SpriteRenderer lightBG;
        public SpriteRenderer lightBG2;
        public SpriteRenderer darkBG;
        public SpriteRenderer darkBG2;
        public AnimationCurve backgroundAnimation;

        [Header("Juice")]
        public ParticleSystem scoreParticles;
        public ParticleSystem highScoreParticles;
        public Gradient lightParticlesColors;
        public Gradient darkParticlesColors;

        [Header("Music")]
        public AudioSource music;

        public bool IsAlive => isAlive;

        private float score = 0;
        private static float highScore = 0;


        private bool isAlive = true;

        public void SetBackgroundAlpha(float flipState)
        {
            var alpha = backgroundAnimation.EvaluateByPolarity(flipState, Player.Single.polarity);
            var prevColor = lightBG.color;
            var nextColor = new Color(prevColor.r, prevColor.g, prevColor.b, alpha);
            lightBG.color = nextColor;
            lightBG2.color = nextColor;
        }

        private float timeToSpawn = 0;

        public static GameManager Single;

        public void Start()
        {
            Single = this;
            score = 0;

            var resY = GetResolutionY(lightBG);

            lightBG.transform.position = Vector3.up * resY / 2 + Vector3.forward / 2;
            lightBG2.transform.position = Vector3.down * resY / 2 + Vector3.forward / 2;
            darkBG.transform.position = Vector3.up * resY / 2 + Vector3.forward;
            darkBG2.transform.position = Vector3.down * resY / 2 + Vector3.forward;

            deathScreen.SetAlpha(1);

            if (!music.isPlaying)
            {
                music.Play();
                DontDestroyOnLoad(music);
            }
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
            bgCamera.orthographicSize = bgCameraSizePerFallingSpeed.Evaluate(fallingSpeed);

            ScrollBGs();

            if (isAlive)
            {
                UpdateScore();
                var newAlpha = Mathf.MoveTowards(deathScreen.color.a, 0, Time.deltaTime * 4);
                deathScreen.SetAlpha(newAlpha); 
            }
            else
            {
                FadeAndRestart();
            }
        }

        public void SetUIColors(float flipState)
        {
            var color = uiColors.Evaluate(flipState);
            scoreText.color = color;
            highScoreText.color = color;
            var outlineColor = uiOutlineColors.Evaluate(flipState);
            scoreOutlineText.color = outlineColor;
            highScoreOutlineText.color = outlineColor;

            var transparentPrimaryColor = new Color(color.r, color.g, color.b, deathScreen.color.a);
            deathScreen.color = transparentPrimaryColor;
        }

        private void ScrollBGs()
        {
            ScrollBG(lightBG);
            ScrollBG(darkBG);
            ScrollBG(lightBG2);
            ScrollBG(darkBG2);
        }

        private void ScrollBG(SpriteRenderer bg)
        {
            var parallaxFallingSpeed = fallingSpeed / 2;
            var oldPosition = bg.transform.position;
            var height = oldPosition.y + parallaxFallingSpeed * Time.deltaTime;
            var spriteYResolution = GetResolutionY(bg);
            if (height > spriteYResolution)
                height = -spriteYResolution;
            bg.transform.position = new Vector3(oldPosition.x, height, oldPosition.z);
        }

        private float GetResolutionY(SpriteRenderer bg) => bg.sprite.bounds.size.y * bg.transform.localScale.y;

        private void UpdateScore()
        {
            score += Time.deltaTime * 10;
            var scoreEmission = scoreParticles.emission;
            var highScoreEmission = highScoreParticles.emission;
            var scoreMain = scoreParticles.main;
            var highScoreMain = highScoreParticles.main;
            if (score > highScore)
            {
                highScore = score;
                scoreEmission.enabled = true;
                highScoreEmission.enabled = true;
            }
            else
            {
                scoreEmission.enabled = false;
                highScoreEmission.enabled = false;
            }

            var particlesPalette =
                Player.Single.polarity == Polarity.Light ?
                    lightParticlesColors :
                    darkParticlesColors;

            scoreMain.startColor = particlesPalette;
            highScoreMain.startColor = particlesPalette;

            scoreText.text = ((int)score).ToString();
            highScoreText.text = ((int)highScore).ToString();

            scoreOutlineText.text = ((int)score).ToString();
            highScoreOutlineText.text = ((int)highScore).ToString();
        }

        private void FadeAndRestart()
        {
            Time.timeScale = Mathf.Max(Time.timeScale, 0.5f);

            var currentOpacity = deathScreen.color.a;
            var nextOpacity = currentOpacity + Time.deltaTime;
            var startBGColor = uiColors.Evaluate(Player.Single.FlipState);
            var endBGColor = uiColors.colorKeys.First().color;
            deathScreen.color = Color.Lerp(startBGColor, endBGColor, nextOpacity);
            deathScreen.SetAlpha(nextOpacity);


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
                spawningObstaclePrefab = lightObstaclePrefab;
            else
                spawningObstaclePrefab = darkObstaclePrefab;

            return spawningObstaclePrefab;
        }

        public void InitiateDeath()
        {
            isAlive = false;
        }
    }
}
