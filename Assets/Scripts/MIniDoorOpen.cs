using UnityEngine;

public class MiniDoorOpen : MonoBehaviour
{
    [Header("Двери (перетащи из иерархии)")]
    public Transform MiniDoor1;
    public Transform MiniDoor2;
    public Transform MiniDoor3;
    public Transform MiniDoor4;

    [Header("Углы открытия для каждой двери (90 или -90)")]
    public float angle1 = 90f;
    public float angle2 = -90f;
    public float angle3 = 90f;
    public float angle4 = -90f;

    [Header("Настройки")]
    public KeyCode interactionKey = KeyCode.E;
    public GameObject keyIcon;
    public float interactionDistance = 3f;
    public float openSpeed = 300f;

    private Camera cam;
    private Transform currentTargetDoor;
    private bool[] isOpen = new bool[4];
    private float[] openAngles;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        // Проверка keyIcon
        if (keyIcon == null)
        {
            Debug.LogError("KEY ICON НЕ НАЗНАЧЕН В ИНСПЕКТОРЕ!");
        }
        else
        {
            Debug.Log("KeyIcon назначен: " + keyIcon.name);
            keyIcon.SetActive(false);
        }

        openAngles = new float[] { angle1, angle2, angle3, angle4 };

        if (cam == null)
            Debug.LogError("КАМЕРА НЕ НАЙДЕНА!");
    }

    private void Update()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        currentTargetDoor = null;
        bool isLookingAtDoor = false;

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            int doorIndex = GetDoorIndex(hit.transform);

            if (doorIndex >= 0)
            {
                currentTargetDoor = hit.transform;
                isLookingAtDoor = true;

                if (Input.GetKeyDown(interactionKey))
                {
                    isOpen[doorIndex] = !isOpen[doorIndex];
                    Debug.Log("Дверь " + (doorIndex + 1) + " переключена");
                }
            }
        }

        // Управление иконкой
        if (keyIcon != null)
        {
            keyIcon.SetActive(isLookingAtDoor);

            // Отладка
            if (isLookingAtDoor && !keyIcon.activeSelf)
            {
                Debug.LogWarning("Пытаемся показать keyIcon, но он не активен!");
            }
        }

        // Анимация всех дверей
        AnimateDoor(MiniDoor1, isOpen[0], openAngles[0]);
        AnimateDoor(MiniDoor2, isOpen[1], openAngles[1]);
        AnimateDoor(MiniDoor3, isOpen[2], openAngles[2]);
        AnimateDoor(MiniDoor4, isOpen[3], openAngles[3]);
    }

    int GetDoorIndex(Transform t)
    {
        if (t == MiniDoor1) return 0;
        if (t == MiniDoor2) return 1;
        if (t == MiniDoor3) return 2;
        if (t == MiniDoor4) return 3;
        return -1;
    }

    void AnimateDoor(Transform door, bool open, float openAngle)
    {
        if (door == null) return;

        float targetY = open ? openAngle : 0f;
        Quaternion target = Quaternion.Euler(0, targetY, 0);
        door.localRotation = Quaternion.RotateTowards(
            door.localRotation, target, openSpeed * Time.deltaTime);
    }
}