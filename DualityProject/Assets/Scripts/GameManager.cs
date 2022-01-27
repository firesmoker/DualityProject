using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public float fallingSpeed = 1;
        public float generationRate = 1;
        public GameObject redObstaclePrefab, blueObstaclePrefab;

        public static GameManager Single;

        public void Start()
        {
            Single = this;

        }
    }
}
