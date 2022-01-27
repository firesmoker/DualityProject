﻿using UnityEngine;

namespace Assets.Scripts
{
    public class Obstacle : MonoBehaviour
    {
        public Polarity passablePolarity;

        private void Update()
        {
            var advancement = Environment.Single.fallingSpeed * Time.deltaTime;
            transform.position = transform.position + advancement * Vector3.up;
        }
    }
}
