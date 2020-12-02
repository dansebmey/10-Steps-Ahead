using UnityEngine;

public class CIObject : MonoBehaviour
{
    protected static GameManager Gm => GameManager.Instance;
    [HideInInspector] public Vector3 targetPos;

    protected virtual void Start()
    {
        targetPos = transform.position;
    }

    protected virtual void Update()
    {
        if (transform.position != targetPos)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.25f);
        }
    }
}