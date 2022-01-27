using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        [NonSerialized] public Polarity polarity;
        public Transform display;
        public float flipSpeed = 4;

        private float prevRotation = 0;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                polarity = polarity.Flip();

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

        private void OnTriggerEnter2D(Collider2D collision)
        {
        }
    }
}
