using Carrot;
using UnityEngine;

public class Json_Properties : MonoBehaviour
{
    [Header("Main obj")]
    public App app;
    private Carrot_Box box = null;

    public void Show_edit_val(js_object obj)
    {
        box = app.carrot.Create_Box();
        box.set_icon(app.sp_icon_project);
        box.set_title("Edit value (" + obj.s_name + ")");

        Carrot_Box_Item item_key = box.create_item();
        item_key.set_icon(app.carrot.icon_carrot_write);
        item_key.set_title("Key name");
        item_key.set_tip("The name of the object or property");
        item_key.set_type(Box_Item_Type.box_value_input);
        item_key.check_type();
        item_key.set_val(obj.s_name);

        Carrot_Box_Item item_val = box.create_item();
        item_val.set_icon(app.carrot.icon_carrot_database);
        item_val.set_title("Value");
        item_val.set_tip("The value of the object or property");
        item_val.set_type(Box_Item_Type.box_value_input);
        item_val.check_type();
        item_val.set_val(obj.s_val);

        Carrot_Box_Btn_Panel panel_btn = box.create_panel_btn();
        Carrot_Button_Item btn_done = panel_btn.create_btn("btn_done");
        btn_done.set_bk_color(app.carrot.color_highlight);
        btn_done.set_icon_white(app.carrot.icon_carrot_done);
        btn_done.set_label_color(Color.white);
        btn_done.set_label("Done");
        btn_done.set_act_click(() => Act_edit_val_done(item_key.get_val(), item_val.get_val(), obj));

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_bk_color(app.carrot.color_highlight);
        btn_cancel.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_act_click(() => Act_close_box());
    }

    private void Act_edit_val_done(string s_key, string s_val, js_object js_obj)
    {
        js_obj.Set_name_key(s_key);
        js_obj.Set_val(s_val, js_obj.get_Type_Properties(), app.json_editor);
        app.carrot.show_msg("Json Editor", "Update json propertie success", Msg_Icon.Success);
        this.Act_close_box();
    }

    private void Act_close_box()
    {
        app.carrot.play_sound_click();
        if (box != null) box.close();
    }
}
