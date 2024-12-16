using UnityEngine;

public class ParticlesMPB : MonoBehaviour
{
    public enum ChickenSelected
    {
        ConPollo,
        BigNugget,
        Shi_Ken,
        Rokoco
    }

    public ChickenSelected currentChicken;

    [SerializeField]
    private ChickenColorSO chickenColors;


    public Color playerColor;
    private MaterialPropertyBlock mpb;
    private ParticleSystem pS;

    private void OnEnable()
    {
        mpb = new MaterialPropertyBlock();
        pS = transform.GetComponent<ParticleSystem>();
        var main = pS.main;

        switch (currentChicken)
        {
            case ChickenSelected.ConPollo:
                main.startColor = new ParticleSystem.MinMaxGradient(chickenColors.ConPolloColor1, chickenColors.ConPolloColor2); ;
                break;

            case ChickenSelected.BigNugget:
                main.startColor = new ParticleSystem.MinMaxGradient(chickenColors.BigNuggetColor1, chickenColors.BigNuggetColor2); ;
                break;

            case ChickenSelected.Shi_Ken:
                main.startColor = new ParticleSystem.MinMaxGradient(chickenColors.ShiKenColor1, chickenColors.ShiKenColor2); ;
                break;

            case ChickenSelected.Rokoco:
                main.startColor = new ParticleSystem.MinMaxGradient(chickenColors.RokocoColor1, chickenColors.RokocoColor2); ;
                break;

            default:
                break;
        }

        mpb.SetColor("_PlayerColor", playerColor);
        transform.GetComponent<ParticleSystemRenderer>().SetPropertyBlock(mpb);

    }

}
