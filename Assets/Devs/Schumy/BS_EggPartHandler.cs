using UnityEngine;

public class BS_EggPartHandler : MonoBehaviour
{
        private Rigidbody partRb;
        private float dragOverTime;
        private float scale = 0.2f;
        private float speed = 1;

        public void Initialize(Rigidbody rb, float drag)
        {
            partRb = rb;
            dragOverTime = drag;
            StartCoroutine(ApplyDragOverTime());
        }

        private System.Collections.IEnumerator ApplyDragOverTime()
        {
            while (partRb != null && partRb.linearVelocity.magnitude > 0.1f)
            {
                partRb.linearVelocity *= 1 - dragOverTime * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(3f);
            while (scale > 0)
            {
                scale -= Time.deltaTime * speed;
                transform.localScale = Vector3.one * scale; 
                yield return null;
            }
            Destroy(gameObject);
        }
    }
