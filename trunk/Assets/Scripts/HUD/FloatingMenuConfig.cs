using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.ModernUIPack;

public enum FloatingMenuType { PANEL, LABEL };
public enum FloatingMenuItem { TEST, DELETE };

public enum DropdownMenuType { FILE, WINDOW };
public enum DropdownMenuItem { SAVE, LOAD, IMPORT_CSV, EXPORT_CSV, EXPORT_TRACERY, COLOR, HOT_KEY };

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/MenuConfig")]
public class FloatingMenuConfig : ScriptableObject
{
    // menus
    public List<FloatingMenu> floatingMenus = new List<FloatingMenu>();
    public List<DropdownMenu> dropdownMenus = new List<DropdownMenu>();
    // items
    public List<FloatingItem> floatingItems = new List<FloatingItem>();
    public List<DropdownItem> dropdownItems = new List<DropdownItem>();

    // menus
    [System.Serializable]
    public class FloatingMenu
    {
        public FloatingMenuType menu;
        [SerializeField]
        public List<FloatingMenuItem> items = new List<FloatingMenuItem>();
    }

    [System.Serializable]
    public class DropdownMenu
    {
        public DropdownMenuType menu;
        [SerializeField]
        public List<DropdownMenuItem> items = new List<DropdownMenuItem>();
    }

    // items
    [System.Serializable]
    public class FloatingItem
    {
        public FloatingMenuItem itemType;
        [SerializeField]
        public UICustomDropdown.Item item;
    }

    [System.Serializable]
    public class DropdownItem
    {
        public DropdownMenuItem itemType;
        [SerializeField]
        public UICustomDropdown.Item item;
    }

    #region getter/ setter
    public List<FloatingItem> GetFloatingItems(FloatingMenuType _type)
    {
        FloatingMenu menu = floatingMenus.Find(x => x.menu == _type);

        List<FloatingItem> items = new List<FloatingItem>();
        foreach (var item in menu.items)
        {
            int findId = floatingItems.FindIndex(x => x.itemType == item);
            if (findId != -1)
                items.Add(floatingItems[findId]);
        }

        return items;
    }

    public List<DropdownItem> GetDowndownItems(DropdownMenuType _type)
    {
        DropdownMenu menu = dropdownMenus.Find(x => x.menu == _type);

        List<DropdownItem> items = new List<DropdownItem>();
        foreach (var item in menu.items)
        {
            int findId = dropdownItems.FindIndex(x => x.itemType == item);
            if (findId != -1)
                items.Add(dropdownItems[findId]);
        }

        return items;
    }
    #endregion
}
