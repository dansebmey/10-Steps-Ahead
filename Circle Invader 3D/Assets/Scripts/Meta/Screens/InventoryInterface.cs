using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterface : GmAwareObject
{
    [SerializeField] private Image itemSlotPanelPrefab;
    [SerializeField] private Image itemSlotBackgroundPanelPrefab;
    
    private Image _backgroundPanel;
    private List<Image> _itemSlotPanels;

    private Inventory _inventory;

    protected override void Awake()
    {
        base.Awake();

        _inventory = FindObjectOfType<Inventory>();
        
        _backgroundPanel = GetComponentInChildren<Image>();
        var rectTransform = _backgroundPanel.rectTransform;
        rectTransform.sizeDelta = new Vector2(0, 132 * (_inventory.maxCapacity-1));
        rectTransform.localPosition = new Vector3(
            rectTransform.localPosition.x,
            -128 + 64 * _inventory.maxCapacity,
            rectTransform.localPosition.z);
        
        _itemSlotPanels = new List<Image>();
        for (int i = 0; i < _inventory.maxCapacity; i++)
        {
            Image isbp = Instantiate(itemSlotBackgroundPanelPrefab, new Vector3(0, 72 + i * 132, 0), Quaternion.identity);
            isbp.transform.SetParent(_backgroundPanel.transform, false);
            
            Image isp = Instantiate(itemSlotPanelPrefab, new Vector3(0, 72 + i * 132, 0), Quaternion.identity);
            isp.transform.SetParent(_backgroundPanel.transform, false);
            isp.name = "ItemSlot (index "+i+")";
            _itemSlotPanels.Add(isp);
        }
        UpdateItemSlots();
    }

    public void UpdateItemSlots()
    {
        for (int i = 0; i < _inventory.maxCapacity; i++)
        {
            if (_inventory.carriedPowerups.Count < i + 1)
            {
                _itemSlotPanels[i].sprite = null;
                _itemSlotPanels[i].color = Color.clear;
            }
            else
            {
                _itemSlotPanels[i].sprite = _inventory.carriedPowerups[i].inventoryIcon;
                _itemSlotPanels[i].color = Color.white;
            }
        }

        if (_inventory.carriedPowerups.Count == 1)
        {
            _inventory.HighlightedItemIndex = 0;
        }
    }

    public void HighlightItem(int itemIndex)
    {
        _itemSlotPanels[itemIndex].GetComponentsInChildren<Image>()[1].color = Color.white;
        for (int i = 0; i < _itemSlotPanels.Count; i++)
        {
            if (i != itemIndex)
            {
                _itemSlotPanels[i].GetComponentsInChildren<Image>()[1].color = Color.clear;
            }
        }
    }
}