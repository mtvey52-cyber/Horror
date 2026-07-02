using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [System.Serializable]
    public class InteractionPair
    {
        public GameObject targetObject;
        public GameObject uiImage;
    }

    public float interactionDistance = 10f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode disableKey = KeyCode.Escape;

    public InteractionPair[] interactables;

    public GameObject Fonarik;
    public Transform playerHands;
    public GameObject DPSKey;
    public Transform KeyPlayerHands;

    public Transform door;
    public Transform miniDoor1;
    public Transform miniDoor2;
    public float doorOpenAngle = 90f;
    public float doorOpenSpeed = 2f;  

    public GameObject keyIcon;
    public GameObject rawImageUI;
    public FlashlightFollow flashlightScript;

    private Camera cam;
    private GameObject currentUI;
    private bool hasFlashlight = false;
    private bool hasKey = false;
    private bool isDoorOpening = false;  
    private float doorTargetAngle;  

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        if (keyIcon != null) keyIcon.SetActive(false);
        foreach (var pair in interactables)
        {
            if (pair.uiImage != null) pair.uiImage.SetActive(false);
        }

        
        if (door != null)
        {
            doorTargetAngle = door.eulerAngles.y;
        }
    }

    void Update()
    {
        
        if (isDoorOpening && door != null)
        {
            float currentAngle = door.eulerAngles.y;
            float newAngle = Mathf.LerpAngle(currentAngle, doorTargetAngle, doorOpenSpeed * Time.deltaTime);
            door.eulerAngles = new Vector3(door.eulerAngles.x, newAngle, door.eulerAngles.z);

            
            if (Mathf.Abs(currentAngle - doorTargetAngle) < 0.5f)
            {
                isDoorOpening = false;
            }
        }

        if (currentUI != null && currentUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Input.GetKeyDown(disableKey))
            {
                currentUI.SetActive(false);
                currentUI = null;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            return;
        }

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            GameObject hitObj = hit.collider.gameObject;

            
            if (Fonarik != null && !hasFlashlight)
            {
                bool isFlashlight = hitObj == Fonarik || hitObj.transform.IsChildOf(Fonarik.transform);

                if (isFlashlight)
                {
                    ShowIcon(keyIcon);
                    if (Input.GetKeyDown(interactionKey))
                    {
                        PickUpFlashlight();
                    }
                    return;
                }
            }

            
            if (DPSKey != null && !hasKey)
            {
                bool isKey = hitObj == DPSKey || hitObj.transform.IsChildOf(DPSKey.transform);

                if (isKey)
                {
                    ShowIcon(keyIcon);
                    if (Input.GetKeyDown(interactionKey))
                    {
                        PickUpKey();
                    }
                    return;
                }
            }

            
            if (door != null && hasKey && !isDoorOpening)
            {
                
                bool isDoor = hitObj == door.gameObject || hitObj.transform.IsChildOf(door);

                if (isDoor)
                {
                    ShowIcon(keyIcon);
                    if (Input.GetKeyDown(interactionKey))
                    {
                        OpenDoor();
                    }
                    return;
                }
            }


            
            InteractionPair found = FindPair(hitObj);

            if (found != null)
            {
                ShowIcon(keyIcon);

                if (Input.GetKeyDown(interactionKey))
                {
                    HideIcon(keyIcon);
                    found.uiImage.SetActive(true);
                    currentUI = found.uiImage;
                }
            }
            else
            {
                HideIcon(keyIcon);
            }
        }
        else
        {
            HideIcon(keyIcon);
        }
    }

    InteractionPair FindPair(GameObject obj)
    {
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].targetObject == obj)
                return interactables[i];
        }
        return null;
    }

    void PickUpFlashlight()
    {
        Fonarik.transform.SetParent(playerHands);
        Fonarik.transform.localPosition = Vector3.zero;
        Fonarik.transform.localRotation = Quaternion.identity;

        Collider[] colliders = Fonarik.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) col.enabled = false;

        Rigidbody[] rigidbodies = Fonarik.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies) rb.isKinematic = true;

        if (flashlightScript != null)
        {
            flashlightScript.Activate();
            flashlightScript.playerHands = playerHands;
        }

        hasFlashlight = true;
        HideIcon(keyIcon);
    }

    void PickUpKey()
    {
        DPSKey.transform.parent = KeyPlayerHands;
        DPSKey.transform.localPosition = Vector3.zero;
        DPSKey.transform.localRotation = Quaternion.identity;

        Collider[] colliders = DPSKey.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) col.enabled = false;

        Rigidbody[] rigidbodies = DPSKey.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies) rb.isKinematic = true;

        hasKey = true;
        HideIcon(keyIcon);
    }

    void OpenDoor()
    {
        
        if (DPSKey != null)
        {
            Destroy(DPSKey);
            DPSKey = null;
        }

        
        if (door != null)
        {
            doorTargetAngle = door.eulerAngles.y + doorOpenAngle;
            isDoorOpening = true;
        }

        hasKey = false;
        HideIcon(keyIcon);
    }

    void ShowIcon(GameObject icon)
    {
        if (icon != null && !icon.activeSelf) icon.SetActive(true);
    }

    void HideIcon(GameObject icon)
    {
        if (icon != null && icon.activeSelf) icon.SetActive(false);
    }
}