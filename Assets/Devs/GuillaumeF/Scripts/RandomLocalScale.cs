using UnityEngine;


[ExecuteInEditMode]
public class RandomLocalScale : MonoBehaviour
{
    public bool uniformScale;
    public Vector3[] initialScale;

    public bool resetScale;

    public float minRange = .9f;
    public float maxRange = 1.1f;



    private void OnEnable()
    {

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            //if (initialScale[i] == null)
            //    initialScale[i] = transform.localScale;

            //if (resetScale)
            //{
            //    transform.GetChild(i).transform.localScale = initialScale[i];
            //}


            if (uniformScale)
            {
                transform.GetChild(i).transform.localScale *= Random.Range(minRange, maxRange);
            }
            else
            {
                transform.GetChild(i).transform.localScale = new Vector3(transform.GetChild(i).transform.localScale.x * Random.Range(minRange, maxRange), transform.GetChild(i).transform.localScale.y * Random.Range(minRange, maxRange), transform.GetChild(i).transform.localScale.z * Random.Range(minRange, maxRange));
            }
        }
    }
}
