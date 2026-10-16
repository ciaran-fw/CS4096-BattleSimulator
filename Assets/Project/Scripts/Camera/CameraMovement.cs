using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float CamSpeed = 10f;

    private void Start()
    {
        ResetCamera();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        transform.Translate(move * CamSpeed * Time.deltaTime);
    }

    public void ResetCamera()
    {
        transform.position = new Vector3(0f, 0f, -10f);
    }
}
