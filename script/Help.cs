using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

        Carrot_Box_Item item_help1 = box_help.create_item("help_1");
        item_help1.set_icon_white(sp_help[0]);
        item_help1.set_act(() => Show_photo_help(sp_help[0]));
        Add_btn_view(item_help1);

        Carrot_Box_Item item_help2 = box_help.create_item("help_2");
        item_help2.set_icon_white(sp_help[0]);
        item_help2.set_act(() => Show_photo_help(sp_help[1]));
        Add_btn_view(item_help2);

        Carrot_Box_Item item_help3 = box_help.create_item("help_3");
        item_help3.set_icon_white(sp_help[0]);
        item_help3.set_act(() => Show_photo_help(sp_help[2]));
        Add_btn_view(item_help3);
    }

    private void Show_photo_help(Sprite sp)
    {
        app.carrot.play_sound_click();
        app.carrot.camera_pro.show_photoshop(sp.texture);
    }

    private void Add_btn_view(Carrot_Box_Item item)
    {
        Carrot_Box_Btn_Item btn_view = item.create_item();
        btn_view.set_icon(app.carrot.icon_carrot_visible_off);
        btn_view.set_color(app.carrot.color_highlight);
        Destroy(btn_view.GetComponent<Button>());
    }
}
