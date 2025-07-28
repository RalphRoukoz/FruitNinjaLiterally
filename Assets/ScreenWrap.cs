using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    private Camera cam;
    private float buffer = 0.1f; // Small buffer to avoid flickering

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

        // Wrap horizontally
        if (viewportPos.x > 1 + buffer)
        {
            viewportPos.x = 0 - buffer;
            transform.position = cam.ViewportToWorldPoint(viewportPos);
        }
        else if (viewportPos.x < 0 - buffer)
        {
            viewportPos.x = 1 + buffer;
            transform.position = cam.ViewportToWorldPoint(viewportPos);
        }
    }
}