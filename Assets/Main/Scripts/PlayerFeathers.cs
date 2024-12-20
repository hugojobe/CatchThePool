using System.Collections.Generic;
using UnityEngine;

public class PlayerFeathers : MonoBehaviour
{
    public List<List<GameObject>> feathers = new List<List<GameObject>>();
    
    public void InitFeathers(int playerCount)
    {
        for (int i = 0; i < feathers.Count; i++)
        {
            feathers[i].ForEach(g => g.SetActive(true));
        }
    }
}