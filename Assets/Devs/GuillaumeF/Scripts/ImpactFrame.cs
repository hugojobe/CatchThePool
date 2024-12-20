using System.Collections;
using UnityEngine;

public class ImpactFrame : MonoBehaviour
{
    public bool impact;
    public Material FullScreenPass;

    private void Start()
    {
        FullScreenPass.SetFloat("_ImpactStep", 0);
        FullScreenPass.SetFloat("_ImpactActive", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (impact)
            StartCoroutine(ImpactFrameProgress());
    }

    IEnumerator ImpactFrameProgress()
    {
        FullScreenPass.SetFloat("_ImpactActive", 1);


        for (int i = 0; i < 3; i++)
        {
            FullScreenPass.SetFloat("_ImpactStep", i%2);
            Debug.Log(i % 2);

            yield return new WaitForSeconds(.05f);
        }

        FullScreenPass.SetFloat("_ImpactActive", 0);
        StopCoroutine(ImpactFrameProgress());
    }
}
