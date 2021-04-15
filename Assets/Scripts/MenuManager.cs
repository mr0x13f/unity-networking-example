using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private Menu[] menus;

    void Awake() {
        instance = this;
    }

    public void OpenMenu(string menuName) {
        foreach (Menu menu in menus)
            if (menu.menuName == menuName)
                OpenMenu(menu);

    }

    public void OpenMenu(Menu menu) {
        foreach (Menu m in menus)
            if (m == menu)
                m.Open();
            else if (m.open)
                m.Close();
    }

}
