using System.Collections.Generic;
using UnityEngine;

public class GetAllChildren
{

public static void GetChildRecursive(GameObject obj)
{
    if (null == obj)
        return;

    foreach (Transform child in obj.transform)
    {
        if (null == child)
            continue;
        //child.gameobject contains the current child you can do whatever you want like add it to an array

        GetChildRecursive(child.gameObject);
    }
}
}
