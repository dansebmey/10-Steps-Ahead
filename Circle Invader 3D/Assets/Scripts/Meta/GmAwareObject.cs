using UnityEngine;

public class GmAwareObject : MonoBehaviour
{
    protected GameManager Gm;
    
    protected virtual void Awake()
    {
        Gm = FindObjectOfType<GameManager>();
    }
}