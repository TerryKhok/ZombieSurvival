using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera _cam;

    private void Start() {
        _cam = Camera.main;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position -_cam.transform.position);
    }
}
