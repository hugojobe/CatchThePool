using Unity.VisualScripting;
using UnityEngine;

public class PopupCanvas : MonoBehaviour
{
    private void Update()
    {
        Camera camera = Camera.main;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
        this.transform.Rotate(0,180,0);
    }
}
