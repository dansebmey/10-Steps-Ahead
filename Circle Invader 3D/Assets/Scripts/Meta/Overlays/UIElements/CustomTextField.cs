using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomTextField : MonoBehaviour
{
    private Text _text, _textShadow, _arrows;
    private int charIndex = 0;

    private char[] chars;

    private void Awake()
    {
        _text = GetComponentsInChildren<Text>()[0];
        _textShadow = GetComponentsInChildren<Text>()[1];
        _arrows = GetComponentsInChildren<Text>()[2];
    }

    private void Start()
    {
        chars = new[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G',
            'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z', '0', '1',
            '2', '3', '4', '5', '6', '7', '8',
            '9', ' '
        };
        
        _text.text = chars[charIndex].ToString();
        _textShadow.text = chars[charIndex].ToString();
    }

    public void PreviousCharacter()
    {
        int newIndex = (chars.Length + charIndex - 1) % chars.Length;
        charIndex = newIndex;
        
        _text.text = chars[charIndex].ToString();
        _textShadow.text = chars[charIndex].ToString();
    }

    public void NextCharacter()
    {
        int newIndex = (chars.Length + charIndex + 1) % chars.Length;
        charIndex = newIndex;
        
        _text.text = chars[charIndex].ToString();
        _textShadow.text = chars[charIndex].ToString();
    }

    public void ShowArrows(bool doShow)
    {
        _arrows.gameObject.SetActive(doShow);
    }

    public string GetCharacter()
    {
        return chars[charIndex].ToString();
    }
}