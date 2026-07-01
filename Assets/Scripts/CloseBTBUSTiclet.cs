using UnityEngine;

public class CloseBTBUSTiclet : MonoBehaviour
{
    public GameObject Button;

    private void DisableTicket()
    {
        Button.SetActive(false);    
    }
}
