using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CustomTextField : MonoBehaviour
{
    private Text _text, _textShadow, _arrows;
    private int _charIndex = 0;

    private char[] _chars;

    private void Awake()
    {
        _text = GetComponentsInChildren<Text>()[0];
        _textShadow = GetComponentsInChildren<Text>()[1];
        _arrows = GetComponentsInChildren<Text>()[2];
        
        _chars = new[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G',
            'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z',
            // '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            ' '
        };
        
        _text.text = _chars[_charIndex].ToString();
        _textShadow.text = _chars[_charIndex].ToString();
    }

    public void PreviousCharacter()
    {
        int newIndex = (_chars.Length + _charIndex - 1) % _chars.Length;
        _charIndex = newIndex;
        
        _text.text = _chars[_charIndex].ToString();
        _textShadow.text = _chars[_charIndex].ToString();
    }

    public void NextCharacter()
    {
        int newIndex = (_chars.Length + _charIndex + 1) % _chars.Length;
        _charIndex = newIndex;
        
        _text.text = _chars[_charIndex].ToString();
        _textShadow.text = _chars[_charIndex].ToString();
    }

    public void ShowArrows(bool doShow)
    {
        _arrows.gameObject.SetActive(doShow);
    }

    public string GetCharacter()
    {
        return _chars[_charIndex].ToString();
    }

    public void SetCharacter(char c)
    {
        _charIndex = Array.IndexOf(_chars, c);
        
        _text.text = _chars[_charIndex].ToString();
        _textShadow.text = _chars[_charIndex].ToString();
    }
}