using Carrot;
using System.Collections;
using UnityEngine;

public class Help : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;

    [Header("Help Obj")]
    public Sprite icon_help;
    public Sprite[] sp_help;

    public void Show()
    {
        Carrot_Box box_help=app.carrot.Create_Box();
        box_help.set_icon(icon_help);
        box_help.set_title("Help");
    }
}
