using Carrot;
using System;
using UnityEngine;

public enum Type_box {add_object,add_properties,add_array,edit_array,edit_object,edit_properties,add_array_item,edit_array_item}
public class Json_Properties : MonoBehaviour
{
    [Header("Main obj")]
    public App app;
    private Carrot_Box box = null;
    private Carrot_Box box_sub = null;

    [Header("Icon")]
    public Sprite sp_icon_properties_string;
    public Sprite sp_icon_properties_number;
    public Sprite sp_icon_properties_date;
    public Sprite sp_icon_properties_null;
    public Sprite sp_icon_properties_bool;

    private Type_box type;
    private Carrot_Box_Item item_key = null;
    private Carrot_Box_Item item_val = null;

    public void Show(js_object obj,Type_box type_box=Type_box.add_object)
    {
        this.type=type_box;
        box = app.carrot.Create_Box();
        string s_name = "";
        if (type_box == Type_box.add_object)
        {
            s_name = "Object" + (obj.get_length_item() + 1);
            box.set_icon(app.sp_icon_project);
            box.set_title("Add Object");
        }

        if (type_box == Type_box.edit_object)
        {
            s_name = obj.s_name;
            box.set_icon(app.sp_icon_project);
            box.set_title("Edit Object ("+obj.s_name+")");
        }

        if (type_box == Type_box.add_properties)
        {
            s_name = "Propertie" + (obj.get_length_item() + 1);
            box.set_icon(app.json_editor.sp_icon_properties);
            box.set_title("Add Propertie");
        }

        if (type_box == Type_box.edit_properties)
        {
            s_name = obj.s_name;
            box.set_icon(app.json_editor.sp_icon_properties);
            box.set_title("Edit Propertie ("+obj.s_name+")");
        }

        if (type_box == Type_box.add_array)
        {
            s_name = "Array" + (obj.get_length_item() + 1);
            box.set_icon(app.json_editor.sp_icon_array);
            box.set_title("Add Array");
        }

        if (type_box == Type_box.edit_array)
        {
            s_name = obj.s_name;
            box.set_icon(app.json_editor.sp_icon_array);
            box.set_title("Edit Array ("+obj.s_name+")");
        }

        if (type_box == Type_box.add_array_item)
        {
            box.set_icon(app.json_editor.sp_icon_array_item);
            box.set_title("Add Array Item");
        }

        if (type_box == Type_box.edit_array_item)
        {
            box.set_icon(app.json_editor.sp_icon_array_item);
            box.set_title("Edit Array Item");
        }

        if (type_box != Type_box.edit_array_item && type_box != Type_box.add_array_item)
        {
            item_key = box.create_item();
            item_key.set_icon(app.carrot.icon_carrot_write);
            item_key.set_title("Key name");
            item_key.set_tip("The name of the object or property");
            item_key.set_type(Box_Item_Type.box_value_input);
            item_key.check_type();
            item_key.set_val(s_name);
        }

 
        if (
           type_box==Type_box.add_properties||type_box == Type_box.edit_properties||
           type_box == Type_box.add_properties || type_box == Type_box.edit_properties|| 
           type_box == Type_box.add_array_item||type_box==Type_box.edit_array_item
         )
        {
            item_val = box.create_item();
            item_val.set_icon(app.carrot.icon_carrot_database);
            item_val.set_title("Value");
            item_val.set_tip("The value of the object or property");
            item_val.set_type(Box_Item_Type.box_value_input);
            item_val.check_type();
            item_val.set_val(obj.s_val);

            Carrot_Box_Btn_Item btn_list_val = item_val.create_item();
            btn_list_val.set_icon(app.carrot.icon_carrot_add);
            btn_list_val.set_color(app.carrot.color_highlight);
            btn_list_val.set_act(() => Show_list_propertie_val(item_val));
        }


        Carrot_Box_Btn_Panel panel_btn = box.create_panel_btn();
        Carrot_Button_Item btn_done = panel_btn.create_btn("btn_done");
        btn_done.set_bk_color(app.carrot.color_highlight);
        btn_done.set_icon_white(app.carrot.icon_carrot_done);
        btn_done.set_label_color(Color.white);
        btn_done.set_label("Done");
        btn_done.set_act_click(() => Act_edit_val_done(obj));

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_bk_color(app.carrot.color_highlight);
        btn_cancel.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_act_click(() => Act_close_box());
    }

    private void Act_edit_val_done(js_object js_obj)
    {
        if (this.type == Type_box.add_object)
        {
            app.json_editor.Add_node(js_obj, "object").Set_name_key(item_key.get_val());
            app.carrot.show_msg("Json Editor", "Add object success!", Msg_Icon.Success);
        }

        if (this.type == Type_box.edit_object)
        {
            js_obj.Set_name_key(item_key.get_val());
            app.carrot.show_msg("Json Editor", "Update object success!", Msg_Icon.Success);
        }

        if (this.type == Type_box.add_properties)
        {
            js_object js_properties = app.json_editor.Add_node(js_obj, "propertie");
            js_properties.Set_name_key(item_key.get_val());
            js_properties.Set_val(item_val.get_val(), js_obj.get_Type_Properties(), app.json_editor);
            app.carrot.show_msg("Json Editor", "Add propertie success!", Msg_Icon.Success);
        }

        if (this.type == Type_box.edit_properties)
        {
            js_obj.Set_name_key(item_key.get_val());
            js_obj.Set_val(item_val.get_val(), js_obj.get_Type_Properties(), app.json_editor);
            app.carrot.show_msg("Json Editor", "Update propertie success!", Msg_Icon.Success);
        }

        if (this.type == Type_box.add_array)
        {
            js_object js_array = app.json_editor.Add_node(js_obj, "array");
            js_array.Set_name_key(item_key.get_val());
            app.carrot.show_msg("Json Editor", "Add array success!", Msg_Icon.Success);
        }

        if (this.type == Type_box.edit_array)
        {
            js_obj.Set_name_key(item_key.get_val());
            app.carrot.show_msg("Json Editor", "Update array success!", Msg_Icon.Success);
        }

        if (this.type == Type_box.add_array_item)
        {
            js_object js_array_item = app.json_editor.Add_node(js_obj, "array_item");
            js_array_item.Set_val(item_val.get_val(), js_obj.get_Type_Properties(), app.json_editor);
            app.carrot.show_msg("Json Editor", "Add array item success!", Msg_Icon.Success);
        }

        if (this.type == Type_box.edit_array_item)
        {
            js_obj.Set_val(item_val.get_val(), js_obj.get_Type_Properties(), app.json_editor);
            app.carrot.show_msg("Json Editor", "Update array item success!", Msg_Icon.Success);
        }

        this.Act_close_box();
    }

    private void Act_close_box()
    {
        app.carrot.play_sound_click();
        if (box != null) box.close();
    }

    private void Show_list_propertie_val(Carrot_Box_Item item_val)
    {
        if (box_sub != null) box_sub.close();
        app.carrot.play_sound_click();
        box_sub = app.carrot.Create_Box();
        box_sub.set_icon(app.carrot.icon_carrot_add);
        box_sub.set_title("Add built-in values to the property's value");

        Carrot_Box_Item item_color_hex = box_sub.create_item();
        item_color_hex.set_icon(app.carrot.sp_icon_theme_color);
        item_color_hex.set_title("Add value color Hex");
        item_color_hex.set_tip("Add color value in hex color table");
        item_color_hex.set_act(() => app.carrot.theme.Show_list_color(Act_add_color_hex_for_field));

        Carrot_Box_Item item_color_rgb = box_sub.create_item();
        item_color_rgb.set_icon(app.carrot.sp_icon_mixer_color);
        item_color_rgb.set_title("Add value color RGB");
        item_color_rgb.set_tip("Add color value in RGB color table");
        item_color_rgb.set_act(() => app.carrot.theme.Show_list_color(Act_add_color_rgb_for_field));

        Carrot_Box_Item item_color_hsv = box_sub.create_item();
        item_color_hsv.set_icon(app.carrot.sp_icon_table_color);
        item_color_hsv.set_title("Add value color HSV");
        item_color_hsv.set_tip("Add color value in HSV color table");
        item_color_hsv.set_act(() => app.carrot.theme.Show_list_color(Act_add_color_hsv_for_field));

        Carrot_Box_Item item_date = box_sub.create_item();
        item_date.set_icon(sp_icon_properties_date);
        item_date.set_title("Add value date");
        item_date.set_tip("Add the current time price to the data field");
        item_date.set_act(() => Act_add_date_for_field(item_val));
    }

    private void Act_add_date_for_field(Carrot_Box_Item item_box)
    {
        app.carrot.play_sound_click();
        item_box.set_val(DateTime.Now.ToString());
        if (box_sub != null) box_sub.close();
    }

    private void Act_add_color_rgb_for_field(Color32 color_val)
    {
        app.carrot.play_sound_click();
        this.item_val.set_val(color_val.ToString());
        if (box_sub != null) box_sub.close();
    }

    private void Act_add_color_hex_for_field(Color32 color_val)
    {
        app.carrot.play_sound_click();
        this.item_val.set_val("#"+ColorUtility.ToHtmlStringRGB(color_val));
        if (box_sub != null) box_sub.close();
    }

    private void Act_add_color_hsv_for_field(Color32 color_val)
    {
        app.carrot.play_sound_click();
        Color.RGBToHSV(color_val, out float hue, out float saturation, out float value);
        this.item_val.set_val("HSV("+hue+";"+ saturation + ";"+ value + ")");
        if (box_sub != null) box_sub.close();
    }
}
