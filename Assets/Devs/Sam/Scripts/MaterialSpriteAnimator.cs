using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialSpriteAnimator : MonoBehaviour
{

    [SerializeField]
    private float maxLifetime = 1f;
    [SerializeField]
    private bool repeat;
    [SerializeField]
    private List<string> properties;
    [SerializeField]
    private List<AnimationCurve> propertiesCurves;


    private float currentLifetime;
    private Renderer rend;
    private MaterialPropertyBlock mpb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLifetime();
    }

    private float NormalizeLifetime()
    {
        return currentLifetime / maxLifetime;
    }


    private void CheckLifetime()
    {
        if(NormalizeLifetime() < 1f)
        {
            currentLifetime += Time.deltaTime;
            float normalizedLifetime = NormalizeLifetime();
            ChangeProperties(normalizedLifetime);
            return;
        }
        if(repeat)
        {
            currentLifetime = 0;
            return;
        }
        currentLifetime = maxLifetime;
        Destroy(gameObject);
    }

    private void ChangeProperties(float normalizedLifetime)
    {
        for(int i = 0; i < properties.Count; i++)
        {
            mpb.SetFloat(properties[i], propertiesCurves[i].Evaluate(normalizedLifetime));
        }
        rend.SetPropertyBlock(mpb);
    }
        
}
