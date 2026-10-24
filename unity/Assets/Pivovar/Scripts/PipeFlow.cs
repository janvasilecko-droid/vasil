// Animované proudění v potrubí: emisní textura s čárkami se posouvá po délce trubky.
using UnityEngine;

namespace Pivovar
{
    public class PipeFlow : MonoBehaviour
    {
        public Material mat;
        public Color liquid = new Color(0.3f, 0.6f, 0.9f);
        /// <summary>0 = prázdná, 1 = plná</summary>
        public float fill;
        /// <summary>rychlost posunu (tile/s), znaménko = směr, 0 = stojí</summary>
        public float speed;
        public float glow = 2.2f;
        float off;

        void Update()
        {
            if (mat == null) return;
            off -= speed * Time.deltaTime;
            if (off < -1000f) off += 1000f;
            HdrpUtil.SetEmissiveOffset(mat, new Vector2(off, 0f));
            HdrpUtil.SetEmissive(mat, liquid, glow * fill);
        }
    }
}
