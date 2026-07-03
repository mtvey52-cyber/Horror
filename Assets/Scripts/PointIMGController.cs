using UnityEngine;

public class PointIMGController : MonoBehaviour
{
    public GameObject keyIcon;
    public GameObject pointIcon;
    public GameObject BinNote;
    public GameObject BusNote;
    public GameObject Car1Note;
    public GameObject PauseMenu;
    public GameObject DpsNote;

    private void Start()
    {
        pointIcon.SetActive(true);
    }

    void Update()
    {
        //if (keyIcon != null && keyIcon.activeSelf)
        //{
           // pointIcon.SetActive(false);
        //}
        //else
        //{
         //   pointIcon.SetActive(true);
       // }
       if (BinNote.activeSelf || BusNote.activeSelf || Car1Note.activeSelf || DpsNote.activeSelf ||  PauseMenu.activeSelf)
        {
            pointIcon.SetActive(false);

        }
       else
        {
            pointIcon.SetActive(true);
        }

    }
}
