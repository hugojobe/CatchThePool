using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionCanvas : MonoBehaviour
{
    public GameObject loaderImage;
    public Image cutout;

    public float width = 10572.84f;
    public float height = 11641.6f;
    
    private void Awake() {
        loaderImage.SetActive(false);
    }
}
