using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text3D : MonoBehaviour
{
    private Camera _cameraToLookAt;
    private TextMesh _textMesh;

    [SerializeField] private Color warningColour1;
    [SerializeField] private Color warningColour2;

    private void Awake()
    {
        _cameraToLookAt = FindObjectOfType<Camera>();
        _textMesh = GetComponent<TextMesh>();
    }
    
    void Update()
    {
        transform.LookAt(_cameraToLookAt.transform);
    }

    public string Text
    {
        set
        {
            if (_textMesh)
            {
                _textMesh.text = value;
            }
        }
    }

    public void EnableWarningColour1()
    {
        if (_textMesh)
        {
            _textMesh.color = warningColour1;
        }
    }
    
    public void EnableWarningColour2()
    {
        if (_textMesh)
        {
            _textMesh.color = warningColour2;
        }
    }
}
