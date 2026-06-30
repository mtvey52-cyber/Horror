using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 3f;
    public float maxLookUp = 60f;    
    public float maxLookDown = -60f; 

    private float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.parent.Rotate(Vector3.up * mouseX);

        
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, maxLookDown, maxLookUp);
        transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);
    }
}