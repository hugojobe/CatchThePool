using System;
using UnityEngine;

public class TransitionCanvas : MonoBehaviour
{
    public GameObject loaderImage;

    private void Awake() {
        loaderImage.SetActive(false);
    }
}
