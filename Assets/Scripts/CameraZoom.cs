using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplier = 2f;
    private float minZoom = 1f;
    private float maxZoom = 8f;
    private float velocity = 0;
    private float smoothTime = 0.1f;

    [SerializeField] private Camera cam;

    void Start(){
        zoom = cam.orthographicSize;
    }

    void Update(){
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }

}
