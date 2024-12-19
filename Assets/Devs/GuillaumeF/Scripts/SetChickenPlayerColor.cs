using UnityEngine;

public class SetChickenPlayerColor : MonoBehaviour
{
    public Color chickenColor = Color.white;
    public SkinnedMeshRenderer[] chickenRenderer;
    private MaterialPropertyBlock mpb;
    private void OnEnable()
    {
        if (transform.parent != null)
            if (transform.parent.transform.GetComponent<PlayerController>() != null)
                chickenColor = transform.parent.transform.GetComponent<PlayerController>().chickenColor;


        foreach (SkinnedMeshRenderer renderer in chickenRenderer)
        {
            mpb = new MaterialPropertyBlock();
            mpb.SetColor("_PlayerColor", chickenColor);
            renderer.SetPropertyBlock(mpb);
        }


    }
}
