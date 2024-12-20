using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerFeathers : MonoBehaviour
{
    public FeatherStruct[] feathersList;
    
    public void ResetFeathers()
    {
        for (int i = 0; i < feathersList.Length; i++)
        {
            feathersList[i].feathersInItem.ToList().ForEach(g => g.SetActive(true));
        }
    }
}

[System.Serializable]
public struct FeatherStruct
{
    public GameObject[] feathersInItem;
}