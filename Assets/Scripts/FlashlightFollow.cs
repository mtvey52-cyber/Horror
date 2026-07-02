using UnityEngine;

public class FlashlightFollow : MonoBehaviour
{
    public Transform cameraTarget;
    public Transform playerHands;  
    public float smoothness = 5f;

    private bool isActive = false;

    public void Activate() => isActive = true;

    void LateUpdate()
    {
        if (!isActive || cameraTarget == null || playerHands == null) return;

        
        Quaternion worldCameraRotation = cameraTarget.rotation;

        
        Quaternion localTargetRotation = Quaternion.Inverse(playerHands.rotation) * worldCameraRotation;

       
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            localTargetRotation,
            smoothness * Time.deltaTime
        );
    }
}