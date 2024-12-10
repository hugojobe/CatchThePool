using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ChickenConfig", menuName = "ChickenConfig", order = 0)]
public class ChickenConfig : ScriptableObject
{
    public string chickenName;
    public string chickenSpellName;
    public int chickenHealthPSM;
    public int chickenSpeedPSM;
}
