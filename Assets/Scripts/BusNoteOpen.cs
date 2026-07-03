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
    public GameObject CafeKey;
    public Transform KeyPlayerHands;
    public Transform door;
    public Transform miniDoor1;
    public Transform miniDoor2;
    public float doorOpenAngle = 90f;
    public float doorOpenSpeed = 2f;  

    public GameObject keyIcon;
    public GameObject CenterPoint;
    public GameObject rawImageUI;
    public FlashlightFollow flashlightScript;

    private Camera cam;
    private GameObject currentUI;
    private bool hasFlashlight = false;
    private bool hasDpsKey = false;
    private bool hasCafeKey = false;
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
            Debug.Log("Смотрю на: " + hitObj.name);


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

            
            if (DPSKey != null && !hasDpsKey)
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


            Debug.Log("hasDpsKey = " + hasDpsKey);
            if (door != null && hasDpsKey && !isDoorOpening)
            {
                
                bool isDoor = hitObj == door.gameObject || hitObj.transform.IsChildOf(door);

                if (isDoor)
                {
                    Debug.Log("Это дверь!");
                    ShowIcon(keyIcon);
                    if (Input.GetKeyDown(interactionKey))
                    {
                        OpenDoor();
                    }
                    return;
                }
            }
            if (CafeKey != null && !hasCafeKey)
            {
                bool isCafeKey = hitObj == CafeKey || hitObj.transform.IsChildOf(CafeKey.transform);
                if (isCafeKey)
                {
                    ShowIcon(keyIcon);
                    if (Input.GetKeyDown(interactionKey))
                    {
                        PickUpCafeKey();
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
                    HideIcon(CenterPoint);
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
            if (interactables[i].targetObject == obj || obj.transform.IsChildOf(interactables[i].targetObject.transform))
            {
                return interactables[i];
            }
                
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

        hasDpsKey = true;
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

        hasDpsKey = false;
        HideIcon(keyIcon);
    }

    void PickUpCafeKey()
    {
        CafeKey.transform.parent = KeyPlayerHands;
        CafeKey.transform.localPosition = Vector3.zero;
        CafeKey.transform.localRotation = Quaternion.identity;

        Collider[] colliders = CafeKey.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) col.enabled = false;

        Rigidbody[] rigidbodies = CafeKey.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies) rb.isKinematic = true;

        hasCafeKey = true;
        HideIcon(keyIcon);
    }

    void ShowIcon(GameObject icon)
    {
        if (icon == null)
        {
            Debug.Log("KeyIcon = NULL");
            return;
        }

        Debug.Log("Включаю: " + icon.name);

        icon.SetActive(true);

        Debug.Log("activeSelf = " + icon.activeSelf);
        Debug.Log("activeInHierarchy = " + icon.activeInHierarchy);
    }

    void HideIcon(GameObject icon)
    {
        if (icon != null)
        {
            icon.SetActive(false);
            Debug.Log("HideIcon: " + icon.name + ", active = " + icon.activeSelf);
        }
    }
}