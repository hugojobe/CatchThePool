using Unity.Mathematics;
using UnityEngine;

public class Hadoukoeuf : MonoBehaviour
{
    [SerializeField] private GameObject hadoukoeufob;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Instantiate(hadoukoeufob,transform.position,quaternion.identity);
    }
}
