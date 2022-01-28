using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Utils
    {
        public static void SetAlpha(this SpriteRenderer renderer, float alpha)
        {
            var prevColor = renderer.color;
            var nextColor = new Color(prevColor.r, prevColor.g, prevColor.b, alpha);
            renderer.color = nextColor;
        }
    }
}
