using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegistryState : State
{
    private RegistryOverlay _registryOverlay;
    private Dictionary<KeyCode, Action> _actionMapping;

    public override void OnEnter()
    {
        _registryOverlay = Gm.OverlayManager.GetOverlay(OverlayManager.OverlayEnum.Registry) as RegistryOverlay;

        _actionMapping = new Dictionary<KeyCode, Action>
        {
            {KeyCode.W, _registryOverlay.NextCharacter},
            {KeyCode.S, _registryOverlay.PreviousCharacter},
            {KeyCode.A, _registryOverlay.HighlightPreviousField},
            {KeyCode.D, _registryOverlay.HighlightNextField}
            // {KeyCode.Return, RegisterHighscore},
            // {KeyCode.Escape, ReturnToMainMenu}
        };
    }

    private void RegisterHighscore()
    {
        _registryOverlay.RegisterHighscore();
    }

    public override void OnUpdate()
    {
        foreach (KeyValuePair<KeyCode, Action> entry in _actionMapping)
        {
            if (Input.GetKeyUp(entry.Key))
            {
                Gm.AudioManager.Play("ButtonHover", 0.05f);
                entry.Value.Invoke();
                // StartCoroutine(RepeatKeyPress(entry));
            }
        }
    }

    private IEnumerator RepeatKeyPress(KeyValuePair<KeyCode, Action> entry)
    {
        yield return new WaitForSeconds(0.25f);
        while (Input.GetKey(entry.Key))
        {
            entry.Value.Invoke();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void OnExit()
    {
        
    }
}