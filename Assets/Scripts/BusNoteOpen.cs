using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 10f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode disableKey = KeyCode.Escape;

    // Ссылки на UI
    public GameObject keyIcon;
    public GameObject rawImageUI;

    // Ссылки на объекты сцены
    public GameObject Fonarik; // Префаб или объект на сцене
    public GameObject Player;  // Объект игрока

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (keyIcon != null) keyIcon.SetActive(false);
        if (rawImageUI != null) rawImageUI.SetActive(false);

        // На всякий случай убедимся, что курсор скрыт в начале
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- ЛОГИКА ОТКРЫТИЯ КАРТИНОК (UI) ---
        if (rawImageUI != null && rawImageUI.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Input.GetKeyDown(disableKey))
            {
                rawImageUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            return; // Прерываем выполнение, пока открыт UI
        }

        // --- ОСНОВНАЯ ЛОГИКА ВЗАИМОДЕЙСТВИЯ ---

        // 1. Пускаем один луч
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // 2. Проверяем попадание
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Получаем компонент взаимодействия
            InteractableObject target = hit.collider.GetComponent<InteractableObject>();

            if (target != null)
            {
                // Показываем иконку "E" для любого интерактивного объекта
                if (keyIcon != null) keyIcon.SetActive(true);

                // Обработка нажатия E
                if (Input.GetKeyDown(interactionKey))
                {
                    // Проверяем, является ли этот объект именно Фонариком
                    // Можно сравнивать по имени, тегу или ссылке на GameObject
                    if (hit.collider.gameObject == Fonarik || hit.collider.CompareTag("Flashlight"))
                    {
                        PickUpFlashlight();
                    }
                    else
                    {
                        // Логика для других объектов (картинки и т.д.)
                        if (rawImageUI != null) rawImageUI.SetActive(true);
                    }

                    // Скрываем иконку после нажатия
                    if (keyIcon != null) keyIcon.SetActive(false);
                }
            }
            else
            {
                HideIcon();
            }
        }
        else
        {
            HideIcon();
        }
    }

    void PickUpFlashlight()
    {
        if (Fonarik == null || Player == null) return;

        // 1. Делаем фонарик ребенком игрока
        Fonarik.transform.SetParent(Player.transform);

        // 2. Сбрасываем локальную позицию и поворот, чтобы он был "в руках" (или где нужно)
        // Настройте эти значения под свою модель!
        Fonarik.transform.localPosition = new Vector3(0.5f, -0.5f, 1f);
        Fonarik.transform.localRotation = Quaternion.identity;

        // 3. Отключаем коллайдер и физику, чтобы он не мешался
        Collider col = Fonarik.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = Fonarik.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Debug.Log("Фонарик подобран!");
    }

    void HideIcon()
    {
        if (keyIcon != null && keyIcon.activeSelf) keyIcon.SetActive(false);
    }
}