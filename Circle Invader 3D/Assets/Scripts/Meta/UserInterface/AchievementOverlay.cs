using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AchievementOverlay : Overlay
{
    [SerializeField] private AchievementEntry achievementEntryPrefab;
    [SerializeField] private Transform achievementTierHeader;
    [SerializeField] private Transform contentContainer;

    private List<AchievementEntry> _entries;

    protected override void Awake()
    {
        base.Awake();
        
        _entries = new List<AchievementEntry>();
        CreateAchievementEntries();
        
        // this fixes a buggo where only one AchievementOverlay would be filled with achievements.
        // however, doesn't feel very safe to call on external components on Awake()...
    }

    private void CreateAchievementEntries()
    {
        AchievementManager am = FindObjectOfType<AchievementManager>();
        
        List<Achievement> easyList = new List<Achievement>();
        List<Achievement> mediumList = new List<Achievement>();
        List<Achievement> hardList = new List<Achievement>();
        
        foreach (Achievement a in am.achievements)
        {
            switch (a.tier)
            {
                case AchievementManager.AchievementTier.Easy:
                    easyList.Add(a);
                    break;
                case AchievementManager.AchievementTier.Medium:
                    mediumList.Add(a);
                    break;
                case AchievementManager.AchievementTier.Hard:
                    hardList.Add(a);
                    break;
            }
        }

        CreateHeader("Easy");
        foreach (Achievement a in easyList)
        {
            CreateEntry(a);
        }
        CreateHeader("Medium");
        foreach (Achievement a in mediumList)
        {
            CreateEntry(a);
        }
        CreateHeader("Hard");
        foreach (Achievement a in hardList)
        {
            CreateEntry(a);
        }
    }

    private void CreateHeader(string headerText)
    {
        Transform header = Instantiate(achievementTierHeader, Vector3.zero, Quaternion.identity);
        header.GetComponentInChildren<Text>().text = headerText;
        header.transform.SetParent(contentContainer.transform);

        Vector3 cachedPos = header.transform.localPosition;
        header.transform.localPosition = new Vector3(cachedPos.x, cachedPos.y, 0);
        header.transform.localScale = Vector2.one;
        header.transform.localRotation = Quaternion.identity;
    }

    private void CreateEntry(Achievement achievement)
    {
        AchievementEntry entry = Instantiate(achievementEntryPrefab, Vector3.zero, Quaternion.identity);
        entry.AssignTo(achievement);
        entry.transform.SetParent(contentContainer.transform, true);

        Vector3 cachedPos = entry.transform.localPosition;
        entry.transform.localPosition = new Vector3(cachedPos.x, cachedPos.y, 0);
        entry.transform.localScale = Vector2.one;
        entry.transform.localRotation = Quaternion.identity;

        _entries.Add(entry);
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnShow()
    {
        base.OnShow();
        
        Gm.CameraController.FocusOn(Gm.CameraController.DefaultFocalPoint, new Vector3(0, 15, 0), new Vector3(90, 180, 0));

        foreach (AchievementEntry entry in _entries)
        {
            entry.UpdateProgress();
        }
    }
}