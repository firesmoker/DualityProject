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


        private float prevFlipState = 0;
        private bool isInsideObstacle;
        private Obstacle currentObstacle;

        public static Player Single;

        private void Start()
        {
            Single = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                polarity = polarity.Flip();
                if (isInsideObstacle && currentObstacle.passablePolarity != polarity)
                {
                    GameManager.Single.InitiateDeath();
                }
            }

            var targetFlipState = PolarityToFlipState(polarity);
            var fallSpeed = GameManager.Single.fallingSpeed;
            var flipSpeed = flipSpeedPerFallSpeed.Evaluate(fallSpeed);
            var nextFlipState = Mathf.MoveTowards(prevFlipState, targetFlipState, flipSpeed * Time.deltaTime);
            prevFlipState = nextFlipState;

            SetRotation(nextFlipState);
            GameManager.Single.SetBackground(nextFlipState);
        }
        
        private void SetRotation(float flipState)
        {
            var angle = flipAnimation.Evaluate(flipState);
            display.rotation = Quaternion.Euler(0, 0, angle);
        }

        private float PolarityToFlipState(Polarity polarity)
        {
            if (polarity == Polarity.Red)
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
                GameManager.Single.InitiateDeath();
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
