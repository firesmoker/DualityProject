using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        [NonSerialized] public Polarity polarity;
        public Transform display;
        public float flipSpeed = 4;


        private float prevRotation = 0;
        public bool isInsideObstacle;
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

            var targetRotation = PolarityToAngle(polarity);
            var nextRotation = Mathf.MoveTowards(prevRotation, targetRotation, flipSpeed * Time.deltaTime * 180);
            display.rotation = Quaternion.Euler(0, 0, nextRotation);
            prevRotation = nextRotation;
        }

        private float PolarityToAngle(Polarity polarity)
        {
            if (polarity == Polarity.Red)
                return 0;
            else
                return 180;
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
