using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public abstract void Activate(PlayerController player);
}