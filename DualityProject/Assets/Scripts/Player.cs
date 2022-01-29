using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        [NonSerialized] public Polarity polarity;
        public Transform display;
        public AnimationCurve flipSpeedPerFallSpeed;

        [Header("Juice")]
        public AnimationCurve flipAnimation;
        public AnimationCurve swingAnimation;
        public AnimationCurve horizontalMovement;
        public float shakeSpeed;
        public float shakeAmplitude;
        public ParticleSystem lightDismembermentParticles;
        public ParticleSystem darkDismembermentParticles;

        public SpriteRenderer lightCharacter;
        public SpriteRenderer darkCharacter;


        private float prevFlipState = 0;
        private bool isInsideObstacle;
        private Obstacle currentObstacle;

        public float FlipState => prevFlipState;

        public static Player Single;

        private void Start()
        {
            Single = this;
        }

        private void Update()
        {
            if (!GameManager.Single.IsAlive)
                return;

            if (IsInteracting())
            {
                polarity = polarity.Flip();
                if (isInsideObstacle && currentObstacle.passablePolarity != polarity)
                {
                    InitiateDeath();
                }
            }

            var targetFlipState = PolarityToFlipState(polarity);
            var fallSpeed = GameManager.Single.fallingSpeed;
            var flipSpeed = flipSpeedPerFallSpeed.Evaluate(fallSpeed);
            var flipState = Mathf.MoveTowards(prevFlipState, targetFlipState, flipSpeed * Time.deltaTime);
            prevFlipState = flipState;

            SetRotation(flipState);
            SetFade(flipState);
            MoveHorizontally();
            GameManager.Single.SetBackgroundAlpha(flipState);
            GameManager.Single.SetUIColors(flipState);
        }

        private bool IsInteracting()
        {
            var keyDown = Input.GetKeyDown(KeyCode.Space);
            var touching = Input.touchCount > 0;
            return keyDown && touching;
        }

        private void InitiateDeath()
        {
            if (polarity == Polarity.Light)
                lightDismembermentParticles.Play();
            else
                darkDismembermentParticles.Play();

            GameManager.Single.InitiateDeath();
            Destroy(display.gameObject);
        }

        private void MoveHorizontally()
        {
            var swing = horizontalMovement.Evaluate(Time.time);
            var shake = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) * shakeAmplitude;
            transform.position = new Vector3(swing + shake, transform.position.y, transform.position.z);
        }

        private void SetFade(float flipState)
        {
            lightCharacter.SetAlpha(1 - flipState);
            darkCharacter.SetAlpha(flipState);
        }

        private void SetRotation(float flipState)
        {
            var angle = flipAnimation.EvaluateByPolarity(flipState, polarity);
            if (polarity == Polarity.Dark) angle = - angle;
            var swing = swingAnimation.Evaluate(Time.time);
            display.rotation = Quaternion.Euler(0, 0, angle + swing);
        }

        private float PolarityToFlipState(Polarity polarity)
        {
            if (polarity == Polarity.Light)
                return 0;
            else
                return 1;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var obstacle = collider.gameObject.GetComponent<Obstacle>();
            if (obstacle == null) return;

            var obstaclePolarity = obstacle.passablePolarity;
            if (polarity != obstaclePolarity)
            {
                InitiateDeath();
            }

            isInsideObstacle = true;
            currentObstacle = obstacle;
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            var obstacle = collider.gameObject.GetComponents<Obstacle>();
            var colliderWasObstacle = obstacle != null;

            if (colliderWasObstacle)
            {
                isInsideObstacle = false;
                currentObstacle = null;
            }
        }

    }
}
