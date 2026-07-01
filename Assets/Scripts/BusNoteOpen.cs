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

    public GameObject keyIcon;
    public GameObject rawImageUI;

    private Camera cam;
    private GameObject currentUI;
    private bool hasFlashlight = false; 
    private bool hasKey = false;

    void Start()
    {
        
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        if (keyIcon != null) keyIcon.SetActive(false);
        foreach (var pair in interactables)
        {
            if (pair.uiImage != null) pair.uiImage.SetActive(false);
        }
    }

    void Update()
    {
        // --- Закрыть UI по Escape ---
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

            
            if (Fonarik != null && hitObj == Fonarik && !hasFlashlight)
            {
                ShowIcon(keyIcon);
                if (Input.GetKeyDown(interactionKey))
                {
                    PickUpFlashlight();
                }
                return;
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
       
        Fonarik.transform.parent = playerHands;
        Fonarik.transform.localPosition = Vector3.zero;
        Fonarik.transform.localRotation = Quaternion.identity;


        for (int i = 0; i < DPSKey.transform.childCount; i++)
        {
            Transform child = DPSKey.transform.GetChild(i);

            Collider col = child.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }


        hasFlashlight = true;
        HideIcon(keyIcon);
    }
    void PickUpKey()
    {
        DPSKey.transform.parent = KeyPlayerHands;
        DPSKey.transform.localPosition = Vector3.zero;
        DPSKey.transform.localRotation = Quaternion.identity;

        Collider col = DPSKey.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = DPSKey.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        hasKey = true;
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