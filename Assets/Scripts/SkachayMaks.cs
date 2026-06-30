using UnityEngine;

public class SkachayMaks : MonoBehaviour
{
    AudioSource _ZombieScream;

    private void Awake()
    {
        _ZombieScream = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(_ZombieScream != null)
        {
            _ZombieScream.PlayOneShot(_ZombieScream.clip);
        }
    }
}
