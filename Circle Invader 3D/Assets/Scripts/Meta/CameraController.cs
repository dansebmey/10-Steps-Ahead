using System;
using System.Collections;
using UnityEngine;

public class CameraController : GmAwareObject
{
    private Vector3 _targetPos;
    private Quaternion _targetRot;

    [SerializeField] private Transform defaultFocalPoint;
    public Transform DefaultFocalPoint => defaultFocalPoint;
    [SerializeField] private Vector3 defaultPos = new Vector3(0, 7.8f, 0);
    [SerializeField] private Vector3 defaultRotEulers = new Vector3(90, 0, 0);
    
    public AudioLowPassFilter LowPassFilter { get; private set; }
    public Camera Camera { get; private set; }

    private float _transitionSpeed = 0.05f;

    protected override void Awake()
    {
        base.Awake();
        
        Camera = GetComponent<Camera>();
        LowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    private void Start()
    {
        FocusOn(defaultFocalPoint, new Vector3(0, 1, -10), new Vector3(-15, 0, 0), 0.005f);
        // FocusOn(defaultFocalPoint, defaultPos, defaultRotEulers, 0.005f);
    }

    public void FocusOn(Transform targetObject, Vector3 targetLocalPos, Vector3 targetLocalRotEulers, float transitionSpeed = 0.05f)
    {
        _transitionSpeed = transitionSpeed;
        
        transform.SetParent(targetObject, true);

        _targetPos = targetLocalPos;
        _targetRot = Quaternion.Euler(targetLocalRotEulers);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.localPosition, _targetPos) > 0.01f)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, _targetPos, _transitionSpeed);
        }
        else
        {
            transform.localPosition = _targetPos;
        }
        
        if (Quaternion.Angle(transform.localRotation, _targetRot) > 0.001f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _targetRot, _transitionSpeed);
        }
        else
        {
            transform.localRotation = _targetRot;
        }
    }
}