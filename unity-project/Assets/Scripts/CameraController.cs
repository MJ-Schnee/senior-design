using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField]
    private float
        baseMoveSpeed = 25f,
        panSpeed = 0.4f,
        zoomSpeed = 5f,
        minZoom = 0.5f,
        maxZoom = 12f,
        zoomMultiplier = 1.3f;

    private Camera cam;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        cam = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    // Camera movement when holding WASD or M3
    void HandleMovement()
    {
        // Move camera using WASD
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDirection += new Vector3(1, 0, 1);
        if (Input.GetKey(KeyCode.S)) moveDirection += new Vector3(-1, 0, -1);
        if (Input.GetKey(KeyCode.A)) moveDirection += new Vector3(-1, 0, 1);
        if (Input.GetKey(KeyCode.D)) moveDirection += new Vector3(1, 0, -1);
        float moveSpeed = baseMoveSpeed * (cam.orthographicSize * zoomMultiplier / maxZoom);
        transform.position += moveSpeed * Time.deltaTime * moveDirection.normalized;

        // Mouse panning using middle mouse button
        if (Input.GetMouseButton(2))
        {
            float pan_x = -Input.GetAxis("Mouse X") * panSpeed * (cam.orthographicSize / maxZoom * zoomMultiplier);
            float pan_y = -Input.GetAxis("Mouse Y") * panSpeed * (cam.orthographicSize / maxZoom * zoomMultiplier);
            Vector3 panComplete = new(pan_x, pan_y, 0);
            transform.Translate(panComplete);
        }
    }

    // Camera zoom using mouse scroll
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    // Centers camera on a specific focal point
    public void CenterObject(GameObject focalPoint)
    {
        Vector3 newPos = focalPoint.transform.position;
        newPos.x -= 20;
        newPos.y = 30;
        newPos.z -= 20;
        
        cam.transform.position = newPos;
    }
}
