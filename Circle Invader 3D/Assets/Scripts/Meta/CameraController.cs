using System;
using System.Collections;
using UnityEngine;

public class CameraController : MovableObject
{
    private Quaternion _targetRot;

    [SerializeField] private Transform defaultFocalPoint;
    [SerializeField] private Vector3 defaultPos = new Vector3(0, 7.8f, 0);
    [SerializeField] private Vector3 defaultRotEulers = new Vector3(90, 0, 0);
    
    public AudioLowPassFilter LowPassFilter { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    protected override void Start()
    {
        base.Start();
        
        // transform.position = defaultPos;
        // transform.rotation = Quaternion.Euler(defaultRotEulers);
        // transform.parent = defaultFocalPoint;
        FocusOn(defaultFocalPoint, defaultPos, defaultRotEulers, 6);
    }

    public void FocusOn(Transform targetObject, Vector3 targetLocalPos, Vector3 targetLocalRotEulers, float duration)
    {
        StartCoroutine(MoveTowardsNewTarget(targetObject, targetLocalPos, targetLocalRotEulers, duration));
    }

    private IEnumerator MoveTowardsNewTarget(Transform targetObject, Vector3 targetLocalPos, Vector3 targetLocalRot, float duration)
    {
        transform.SetParent(targetObject, true);
        // note: transform.position verwijst altijd naar global position

        Vector3 newTargetPos = targetLocalPos;
        Quaternion newTargetRot = Quaternion.Euler(targetLocalRot);
        
        while (Vector3.Distance(transform.localPosition, newTargetPos) > 0.01f && Quaternion.Angle(transform.localRotation, newTargetRot) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, newTargetPos, 1.0f / (duration * 25));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, newTargetRot, 1.0f / (duration * 25));
            yield return new WaitForSeconds(0.01f);
        }

        transform.localPosition = newTargetPos;
        transform.localRotation = newTargetRot;
    }

    public void ZoomOut()
    {
        // targetPos = new Vector3(0, 11.33f, -5);
        // _targetRot = new Quaternion(40, 0, 0, 0);
    }

    public void ResetZoom()
    {
        // targetPos = defaultCamTransform.position;
        // _targetRot = new Quaternion(15, 0, 0, 0);
    }

    protected override void Update()
    {
        // if (transform.localPosition != targetPos)
        // {
        //     transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, 0.05f);
        // }
        //
        // if (transform.rotation != _targetRot)
        // {
        //     transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRot, 0.05f);
        // }
    }
}