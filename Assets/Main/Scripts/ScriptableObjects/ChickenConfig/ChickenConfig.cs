using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ChickenConfig", menuName = "ChickenConfig", order = 0)]
public class ChickenConfig : ScriptableObject
{
    public string chickenName;
    public string chickenSpellName;
    public Sprite chickenImage;
    public int chickenHealthPSM;
    public int chickenSpeedPSM;
    [Space]
    public int chickenHealthGameplay;

    [Space] 
    public float chickenSpeed;
    public float dashSpeed;
    public float dashDistance;
    public float dashCooldown;

    [Space] 
    public Color[] chickenColors;
}
