using UnityEngine;

public class PostAnimParentDisabler : MonoBehaviour
{
    public void PostAnimDisabler()
    {
        transform.parent.gameObject.SetActive(false);
    }
}