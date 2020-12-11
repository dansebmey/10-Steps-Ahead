public class RegistryOverlay : Overlay
{
    private CustomTextField[] _customTextFields;
    public CustomTextField HighlightedField { get; private set; }

    private int _highlightedFieldIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        _customTextFields = GetComponentsInChildren<CustomTextField>();
    }

    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        Gm.SwitchState(typeof(RegistryState));
        HighlightSelectedField();
    }

    private void HighlightSelectedField()
    {
        HighlightedField = _customTextFields[_highlightedFieldIndex];
        foreach (CustomTextField ctf in _customTextFields)
        {
            ctf.ShowArrows(ctf == HighlightedField);
        }
    }

    public void PreviousCharacter()
    {
        HighlightedField.PreviousCharacter();
    }

    public void NextCharacter()
    {
        HighlightedField.NextCharacter();
    }

    public void HighlightPreviousField()
    {
        int newIndex = (_customTextFields.Length + _highlightedFieldIndex - 1) % _customTextFields.Length;
        _highlightedFieldIndex = newIndex;
        
        HighlightSelectedField();
    }

    public void HighlightNextField()
    {
        int newIndex = (_customTextFields.Length + _highlightedFieldIndex + 1) % _customTextFields.Length;
        _highlightedFieldIndex = newIndex;
        
        HighlightSelectedField();
    }

    public void RegisterHighscore()
    {
        string result = "";
        foreach (CustomTextField ctf in _customTextFields)
        {
            result += ctf.GetCharacter();
        }
        
        Gm.HighscoreManager.RegisterHighscore(result, Gm.PlayerScore);
        Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.HIGHSCORE);
    }
}