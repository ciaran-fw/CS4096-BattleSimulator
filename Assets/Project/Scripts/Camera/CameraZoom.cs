using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 50f;
    public float Min = 2f;
    public float Max = 30f;

    // Update is called once per frame
    void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y * 0.01f;
        Camera.main.orthographicSize -= scroll * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, Min, Max);
    }
}
