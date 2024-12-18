using UnityEngine;
using UnityEngine.InputSystem.Interactions;


//DON'T USE IN EDITOR!

public class EGSpritesheetAnimator : MonoBehaviour
{

    [SerializeField]
    private bool repeat = false;
    [SerializeField]
    private int frames = 64;
    [SerializeField]
    private float maxLifetime = .5f;
    private float currentLifetime = 0f;
    [SerializeField]
    private AnimationCurve lifetimeCurve;
    private Renderer meshRend;
    private MaterialPropertyBlock mpb;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // meshRend = GetComponent<Renderer>();
        // mpb = new MaterialPropertyBlock();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        meshRend.GetPropertyBlock(mpb);
        
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
            float curvePos = lifetimeCurve.Evaluate(normalizedLifetime);
            meshRend.material.SetFloat("_Frame", frames*curvePos);
            return;
        }
        if(repeat)
        {
            currentLifetime = 0;
        }
        else{
            Destroy(gameObject);
        }
            
        
        
    }
}
