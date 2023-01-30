using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;

    public float smoothSpeed = 0.125f;
    public Vector3 offsetX = new Vector3(2, 0);
    public Vector3 offsetY = new Vector3(0, 2f);

    private void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = player.transform.position + offsetX * (playerController.IsFacingRight ? 1 : -1) + offsetY;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, newPosition, smoothSpeed);

        float x = smoothedPosition.x;
        float y = smoothedPosition.y;

        if (x < 0f) x = 0f;
        if (x >18f) x = 18f;

        if (y < -1.5f) y = -1.5f;
        if (y > 2.5f) y = 2.5f;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
