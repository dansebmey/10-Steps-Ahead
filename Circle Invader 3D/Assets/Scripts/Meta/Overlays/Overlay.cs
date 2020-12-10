using UnityEngine;

public abstract class Overlay : GmAwareObject
{
    public abstract void OnHide();
    public abstract void OnShow();
}