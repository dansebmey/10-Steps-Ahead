using System;
using UnityEngine;
using UnityEngine.UI;

public class FlashyTextController : MonoBehaviour
{
    private Text _text;
    private float counter;
    [Range(0.0f, 1.0f)][SerializeField] private float flashSpeed = 0.1f;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        counter += flashSpeed;
        _text.color = Color.HSVToRGB(0.196f, Mathf.Sin(counter), 0.255f);
    }
}