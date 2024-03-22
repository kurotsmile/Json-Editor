using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_color_pick : MonoBehaviour
{
    public void click()
    {
        GameObject.Find("App").GetComponent<App>().json_editor.Panel_select_color.set_color(this.GetComponent<Image>().color);
    }
}
