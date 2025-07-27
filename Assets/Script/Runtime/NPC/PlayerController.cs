using UnityEngine;

namespace NPC
{
    public class PlayerController : MonoBehaviour
    {
        private bool isFacingRight; 

        void Awake()
        {
            // Secara otomatis mendeteksi arah hadap awal berdasarkan scale
            isFacingRight = transform.localScale.x > 0;
        }

        public void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 newScale = transform.localScale;
            newScale.x *= -1; // Membalik sumbu X dari scale
            transform.localScale = newScale;
        }

        public void FaceTowards(Transform target)
        {
            // Jika target null, jangan lakukan apa-apa
            if (target == null) return;

            // Cek apakah target ada di kanan atau kiri player
            bool shouldFaceRight = (target.position.x > transform.position.x);
            
            // Jika arah hadap sekarang tidak sesuai dengan arah yang seharusnya, panggil Flip()
            if (isFacingRight != shouldFaceRight)
            {
                Flip();
            }
        }
    }
}
