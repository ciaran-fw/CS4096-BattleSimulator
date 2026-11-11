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
        float zoom = 0f;

        zoom += Mouse.current.scroll.ReadValue().y * 0.01f * zoomSpeed;

        // using delta time so that zoom is determined by frame rate.
        if (Keyboard.current.numpadPlusKey.isPressed || Keyboard.current.equalsKey.isPressed)
        {
            zoom += zoomSpeed * 0.1f * Time.deltaTime;
        }

        if (Keyboard.current.numpadMinusKey.isPressed || Keyboard.current.minusKey.isPressed)
        {
            zoom -= zoomSpeed * 0.1f * Time.deltaTime;
        }

        Camera.main.orthographicSize -= zoom;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, Min, Max);
    }
}
