using System;
using UnityEngine;
using UnityEngine.UI;

public class FlashyButtonController : MonoBehaviour
{
    private Button _button;
    private float counter;
    private float flashSpeed = 0.05f;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        counter += flashSpeed;
        
        float scale = 1 + 0.125f * Mathf.Sin(counter);
        _button.transform.localScale = new Vector3(scale, scale, scale);  
        //Color.HSVToRGB(0.196f, Mathf.Sin(counter), 0.255f);
    }
}