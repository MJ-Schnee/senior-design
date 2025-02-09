using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField]
    private float
        baseMoveSpeed = 10f,
        maxMoveSpeed = 20f,
        zoomSpeed = 5f,
        minZoom = 0.5f,
        maxZoom = 12f,
        zoomMultiplier = 3;

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

    void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += new Vector3(1, 0, 1);
        if (Input.GetKey(KeyCode.S)) moveDirection += new Vector3(-1, 0, -1);
        if (Input.GetKey(KeyCode.A)) moveDirection += new Vector3(-1, 0, 1);
        if (Input.GetKey(KeyCode.D)) moveDirection += new Vector3(1, 0, -1);

        float currentSpeed = Math.Clamp(baseMoveSpeed * (cam.orthographicSize * zoomMultiplier / maxZoom), 0, maxMoveSpeed);
        transform.position += currentSpeed * Time.deltaTime * moveDirection.normalized;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    public void CenterObject(GameObject focalPoint)
    {
        Vector3 newPos = focalPoint.transform.position;
        newPos.x -= 20;
        newPos.y = 30;
        newPos.z -= 20;
        
        cam.transform.position = newPos;
    }
}
