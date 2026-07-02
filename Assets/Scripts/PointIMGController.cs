using UnityEngine;

public class PointIMGController : MonoBehaviour
{
    public GameObject keyIcon;
    public GameObject pointIcon;

    private void Start()
    {
        pointIcon.SetActive(true);
    }

    void Update()
    {
        if (keyIcon != null && keyIcon.activeSelf)
        {
            pointIcon.SetActive(false);
        }
        else
        {
            pointIcon.SetActive(true);
        }
    }
}
