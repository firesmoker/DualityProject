using UnityEngine;

namespace Assets.Scripts
{
    public class Obstacle : MonoBehaviour
    {
        public Polarity passablePolarity;

        private void Update()
        {
            var advancement = GameManager.Single.fallingSpeed * Time.deltaTime;
            transform.position = transform.position + advancement * Vector3.up;

            if (transform.position.y >= GameManager.Single.obstacleDestroyingPosition.transform.position.y)
            {
                Destroy(gameObject);
            }
        }
    }
}
