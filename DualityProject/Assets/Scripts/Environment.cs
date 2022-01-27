using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Environment : MonoBehaviour
    {
        public float fallingSpeed = 1;
        public float generationRate = 1;

        public static Environment Single;

        private void Awake()
        {
            Single = this;
        }
    }
}