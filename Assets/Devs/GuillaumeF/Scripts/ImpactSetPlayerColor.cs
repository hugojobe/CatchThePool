using UnityEngine;

public class ImpactSetPlayerColor : MonoBehaviour
{
    public Color playerColor;
    public Transform[] playerObjects;

    public ParticlesMPB.ChickenSelected targetChicken;

    public Color targetPlayerColor;

    public Transform[] targetObjects;
    private MaterialPropertyBlock mpb;

    private void OnEnable()
    {
        foreach (Transform r in targetObjects)
        {
            if (r.GetComponent<ParticlesMPB>() != null)
            {
                r.GetComponent<ParticlesMPB>().currentChicken = targetChicken;
                r.GetComponent<ParticlesMPB>().playerColor = targetPlayerColor;
            }

        }

        mpb = new MaterialPropertyBlock();
        mpb.SetColor("_PlayerColor", playerColor);

        foreach (Transform r in playerObjects)
        {
            if (r.GetComponent<ParticleSystemRenderer>() != null)
            {
                r.GetComponent<ParticleSystemRenderer>().SetPropertyBlock(mpb);
            }
            else
            {

                r.GetComponent<Renderer>().SetPropertyBlock(mpb);

            }


        }
    }

}
