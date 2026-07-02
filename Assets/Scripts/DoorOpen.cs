using UnityEngine;

public class PlayerInteractionDoor: MonoBehaviour
{
    public float interactionDistance = 5f;
    public GameObject keyInHand; // Ссылка на пустой объект-родитель ключа в руке

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Находим самый главный родительский объект, в который попал луч
            // transform.root находит самый верхний объект в иерархии префаба
            GameObject mainDoorObject = hit.collider.transform.root.gameObject;

            // Проверяем тэг у ГЛАВНОГО объекта двери
            if (mainDoorObject.CompareTag("Door"))
            {
                // Проверяем, есть ли ключ в руке
                if (keyInHand != null && keyInHand.activeSelf)
                {
                    OpenDoor(mainDoorObject);
                }
                else
                {
                    Debug.Log("Дверь заперта! Нужен ключ.");
                }
            }
        }
    }

    void OpenDoor(GameObject doorRoot)
    {
        Debug.Log("Дверь открывается!");

        // Уничтожаем пустой объект ключа (все 3 дочерних объекта удалятся вместе с ним)
        Destroy(keyInHand);

        // Запускаем плавный поворот всего префаба двери
        StartCoroutine(AnimateDoor(doorRoot));
    }

    System.Collections.IEnumerator AnimateDoor(GameObject doorRoot)
    {
        // Выключаем ВСЕ коллайдеры внутри префаба двери, чтобы избежать багов
        Collider[] allColliders = doorRoot.GetComponentsInChildren<Collider>();
        foreach (Collider col in allColliders)
        {
            col.enabled = false;
        }

        Quaternion startRotation = doorRoot.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 90f, 0);

        float elapsed = 0f;
        float duration = 1.0f; // Время открытия в секундах

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            doorRoot.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            yield return null;
        }

        doorRoot.transform.rotation = endRotation;
    }
}
