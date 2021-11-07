using UnityEngine;

public class PostAnimParentDestroyer : MonoBehaviour
{
    public void PostAnimDestroy()
    {
        if (transform.parent.GetComponent<EnemyLayer>())
        {
            Destroy(transform.parent.gameObject);   
        }
        else
        {
            Debug.LogError("Could not destroy layer: PostAnimParentDestroyer must be a direct child of EnemyLayer.");
        }
    }
}