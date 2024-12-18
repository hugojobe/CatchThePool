using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PropertyActivation : MonoBehaviour
{
    [SerializeField]
    protected Renderer[] objectRenderer;
    protected MaterialPropertyBlock mpb;

    public Color color;

    [SerializeField]
    protected string propertyName;

    [SerializeField]
    protected AnimationCurve spawnCurve;

    protected float timer = 0;
    [SerializeField]
    protected float effectDuration = 1;
    [SerializeField]
    protected bool soloIteration = true;


    // Start is called before the first frame update
    void Start()
    {
        mpb = new MaterialPropertyBlock();
        mpb.SetColor("_PlayerColor", color);

    }
    private void OnEnable()
    {
        timer = 0;
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        if (timer <= effectDuration)
        {
            timer += Time.deltaTime;
            Spawn();

        }

        if (timer > effectDuration && soloIteration)
        {
            transform.gameObject.SetActive(false);
            Destroy(gameObject);

        }

    }

    protected virtual void Spawn()
    {
        mpb.SetFloat($"_{propertyName}", spawnCurve.Evaluate(timer));
        foreach (Renderer item in objectRenderer)
        {
            item.SetPropertyBlock(mpb);

        }
    }

    protected void Respawn()
    {
        timer = 0;
    }
}

