using Carrot;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Type_Properties_val {string_val,number_val,color_val,null_val,date_val,bool_val}

public class Json_Editor : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Icon")]
    public Sprite sp_expanded_on;
    public Sprite sp_expanded_off;
    public Sprite sp_icon_object;
    public Sprite sp_icon_array;
    public Sprite sp_icon_array_item;
    public Sprite sp_icon_properties;
    public Sprite sp_icon_root_object;
    public Sprite sp_icon_root_array;
    public Sprite sp_icon_clear;

    public Sprite sp_icon_properties_string;
    public Sprite sp_icon_properties_number;
    public Sprite sp_icon_properties_date;
    public Sprite sp_icon_properties_color;
    public Sprite sp_icon_properties_null;
    public Sprite sp_icon_properties_bool;

    [Header("Color")]
    public Color32 color_properties_nomal;
    public Color32 color_properties_select;
    public Color32 color_btn_delete;
    public Color32 color_btn_clear;

    [Header("Obj Json")]
    public GameObject prefab_obj_js;
    public GameObject Panel_edit_Properties;
    public Color_Board Panel_select_color;
    public Transform area_all_item_editor;
    public ScrollRect ScrollRect_all_item_editor;
    public InputField inp_coder_viewer;
    public Text txt_username_login;

    public float space_x_item = 25f;
    public GameObject[] panel_model;
    public Image[] img_btn_model;

    private int index_sel_mode = 0;
    private bool is_change_coderviewer;
    private js_object Js_object_edit_temp;

    [Header("Edit Properties")]
    public Image[] img_btn_Properties;
    public Image[] img_btn_Properties_bool;
    public InputField inp_edit_Properties_name;
    public InputField inp_edit_Properties_value;
    public int index_edit_Properties_type;
    public GameObject panel_edit_Properties_value;
    public GameObject panel_edit_Properties_color;
    public GameObject panel_edit_Properties_bool;

    private Carrot_Box box = null;
    private js_object js_object_root = null;

    public void On_load()
    {
        this.Panel_edit_Properties.SetActive(false);
        this.Panel_select_color.gameObject.SetActive(false);
        this.app.carrot.clear_contain(this.area_all_item_editor);
        this.Add_node();
        this.Check_mode();
    }

    public js_object Add_node(js_object obj_father=null,string s_type="root_object")
    {
        GameObject obj_node = Instantiate(this.prefab_obj_js);
        obj_node.transform.SetParent(this.area_all_item_editor);
        obj_node.transform.localPosition = new Vector3(0f, 0f, 0f); 
        obj_node.transform.localScale = new Vector3(1f, 1f, 0f);
        obj_node.transform.localRotation = Quaternion.identity;

        js_object js_obj = obj_node.GetComponent<js_object>();
        js_obj.On_load(s_type);
        if (obj_father != null)
        {
            obj_father.Add_child(js_obj);
            float x_space = this.space_x_item * obj_father.x;
            js_obj.x = obj_father.x + 1;
            js_obj.y = obj_father.y + 1;
            js_obj.area_body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x_space, js_obj.area_body.rect.height);
        }
        else
        {
            js_obj.x = +1;
            js_object_root = js_obj;
        }

        obj_node.transform.SetSiblingIndex(js_obj.y);

        if (s_type == "root_object")
        {
            js_obj.Set_icon(sp_icon_root_object);

            Carrot_Box_Btn_Item btn_add_obj=js_obj.Create_btn("btn_add_obj");
            btn_add_obj.set_icon(this.sp_icon_object);
            btn_add_obj.set_act(() => this.Show_add_obj_to_node(js_obj));

            Carrot_Box_Btn_Item btn_add_array = js_obj.Create_btn("btn_add_array");
            btn_add_array.set_icon(this.sp_icon_array);
            btn_add_array.set_act(() => this.Add_node(js_obj, "array"));

            Carrot_Box_Btn_Item btn_add_propertie= js_obj.Create_btn("btn_add_propertie");
            btn_add_propertie.set_icon(this.sp_icon_properties);
            btn_add_propertie.set_act(() => this.Add_node(js_obj, "propertie"));

            this.Create_btn_delete(js_obj,false);
        }

        if (s_type == "root_array")
        {
            js_obj.Set_icon(sp_icon_root_array);

            Carrot_Box_Btn_Item btn_add_obj = js_obj.Create_btn("btn_add_obj");
            btn_add_obj.set_icon(this.sp_icon_object);
            btn_add_obj.set_act(() => this.Show_add_obj_to_node(js_obj));

            Carrot_Box_Btn_Item btn_add_array = js_obj.Create_btn("btn_add_array");
            btn_add_array.set_icon(this.sp_icon_array);
            btn_add_array.set_act(() => this.Add_node(js_obj, "array"));

            Carrot_Box_Btn_Item btn_add_propertie = js_obj.Create_btn("btn_add_propertie");
            btn_add_propertie.set_icon(this.sp_icon_properties);
            btn_add_propertie.set_act(() => this.Add_node(js_obj, "propertie"));

            this.Create_btn_delete(js_obj, false);
        }

        if (s_type == "array")
        {
            js_obj.Set_icon(sp_icon_array);

            Carrot_Box_Btn_Item btn_add_obj = js_obj.Create_btn("btn_add_obj");
            btn_add_obj.set_icon(this.sp_icon_object);
            btn_add_obj.set_act(() => this.Show_add_obj_to_node(js_obj));

            Carrot_Box_Btn_Item btn_add_array = js_obj.Create_btn("btn_add_array");
            btn_add_array.set_icon(this.sp_icon_array);
            btn_add_array.set_act(() => this.Add_node(js_obj, "array"));

            Carrot_Box_Btn_Item btn_add_propertie = js_obj.Create_btn("btn_add_propertie");
            btn_add_propertie.set_icon(this.sp_icon_properties);
            btn_add_propertie.set_act(() => this.Add_node(js_obj, "propertie"));

            this.Create_btn_delete(js_obj);
        }

        if (s_type == "object")
        {
            js_obj.Set_icon(sp_icon_object);

            Carrot_Box_Btn_Item btn_add_obj = js_obj.Create_btn("btn_add_obj");
            btn_add_obj.set_icon(this.sp_icon_object);
            btn_add_obj.set_act(() => this.Show_add_obj_to_node(js_obj));

            Carrot_Box_Btn_Item btn_add_array = js_obj.Create_btn("btn_add_array");
            btn_add_array.set_icon(this.sp_icon_array);
            btn_add_array.set_act(() => this.Add_node(js_obj, "array"));

            Carrot_Box_Btn_Item btn_add_propertie = js_obj.Create_btn("btn_add_propertie");
            btn_add_propertie.set_icon(this.sp_icon_properties);
            btn_add_propertie.set_act(() => this.Add_node(js_obj, "propertie"));

            Carrot_Box_Btn_Item btn_edit = js_obj.Create_btn("btn_edit");
            btn_edit.set_icon(this.app.carrot.user.icon_user_edit);
            btn_edit.set_act(() => Show_edit_key(js_obj));

            this.Create_btn_delete(js_obj);
        }

        if (s_type == "propertie")
        {
            js_obj.Set_icon(sp_icon_properties);

            Carrot_Box_Btn_Item btn_edit = js_obj.Create_btn("btn_edit");
            btn_edit.set_icon(this.app.carrot.user.icon_user_edit);
            btn_edit.set_act(() => Show_edit_key(js_obj));

            this.Create_btn_delete(js_obj);
        }

        if(s_type== "array_item")
        {
            js_obj.Set_icon(sp_icon_array_item);
            this.Create_btn_delete(js_obj);
        }

        Carrot_Box_Btn_Item btn_menu = js_obj.Create_btn("btn_menu");
        btn_menu.set_icon(this.app.carrot.icon_carrot_all_category);
        btn_menu.set_act(() => Show_menu(js_obj));
        this.Update_index_list();
        return js_obj;
    }

    private void Create_btn_delete(js_object js_obj,bool is_delete=true)
    {
        Carrot_Box_Btn_Item btn_del = js_obj.Create_btn("btn_del");
        if (is_delete)
        {
            btn_del.set_icon(this.app.carrot.sp_icon_del_data);
            btn_del.set_color(this.color_btn_delete);
            btn_del.set_act(() => Delete_node(js_obj));
        }
        else
        {
            btn_del.set_icon(this.sp_icon_clear);
            btn_del.set_color(this.color_btn_clear);
            btn_del.set_act(() => Clear_child_in_node(js_obj));
        }
    }

    private void Show_add_obj_to_node(js_object obj)
    {
        app.carrot.play_sound_click();
        box = app.carrot.Create_Box();
        box.set_icon(sp_icon_object);
        box.set_title("Add Object");

        Carrot_Box_Item item_key = box.create_item();
        item_key.set_icon(app.carrot.icon_carrot_write);
        item_key.set_title("Name Object");
        item_key.set_tip("The name of the object");
        item_key.set_type(Box_Item_Type.box_value_input);
        item_key.check_type();
        item_key.set_val(obj.txt_name.text);

        Carrot_Box_Btn_Panel panel_btn = box.create_panel_btn();
        Carrot_Button_Item btn_done = panel_btn.create_btn("btn_done");
        btn_done.set_bk_color(app.carrot.color_highlight);
        btn_done.set_icon_white(app.carrot.icon_carrot_done);
        btn_done.set_label_color(Color.white);
        btn_done.set_label("Done");
        btn_done.set_act_click(() => Act_add_emp_to_node_done(item_key.get_val(), obj));

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_bk_color(app.carrot.color_highlight);
        btn_cancel.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_act_click(() => Act_close_box());
    }

    private void Act_add_emp_to_node_done(string s_name,js_object obj)
    {
        Add_node(obj,"object").txt_name.text=s_name;
        this.Act_close_box();
    }

    private void Show_edit_key(js_object obj)
    {
        box = app.carrot.Create_Box();
        box.set_icon(app.carrot.user.icon_user_edit);
        box.set_title("Edit key");

        Carrot_Box_Item item_key = box.create_item();
        item_key.set_icon(app.carrot.icon_carrot_write);
        item_key.set_title("Key name");
        item_key.set_tip("The name of the object or property");
        item_key.set_type(Box_Item_Type.box_value_input);
        item_key.check_type();
        item_key.set_val(obj.txt_name.text);

        Carrot_Box_Btn_Panel panel_btn = box.create_panel_btn();
        Carrot_Button_Item btn_done=panel_btn.create_btn("btn_done");
        btn_done.set_bk_color(app.carrot.color_highlight);
        btn_done.set_icon_white(app.carrot.icon_carrot_done);
        btn_done.set_label_color(Color.white);
        btn_done.set_label("Done");
        btn_done.set_act_click(() => Act_edit_key_done(item_key.get_val(), obj));

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_bk_color(app.carrot.color_highlight);
        btn_cancel.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_act_click(() => Act_close_box());
    }

    private void Act_edit_key_done(string s_name,js_object obj)
    {
        obj.Set_name_key(s_name);
        this.Act_close_box();
    }

    private void Act_close_box()
    {
        app.carrot.play_sound_click();
        if (box != null) box.close();
    }

    private void Show_menu(js_object obj)
    {
        box = app.carrot.Create_Box();
        box.set_icon(app.carrot.icon_carrot_all_category);
        box.set_title("Menu");

        if (obj.get_length_item() > 0)
        {
            Carrot_Box_Item item_clear = box.create_item();
            item_clear.set_icon(sp_icon_clear);
            item_clear.set_title("Clean up");
            item_clear.set_tip("Delete all data of sub-blocks");
            item_clear.set_act(() => Clear_child_in_node(obj));
        }

        if (obj.s_type != "root_object" || obj.s_type != "root_array")
        {
            Carrot_Box_Item item_del = box.create_item();
            item_del.set_icon(app.carrot.sp_icon_del_data);
            item_del.set_title("Delete");
            item_del.set_tip("Delete all data and node");
            item_del.set_act(() => Delete_node(obj));
        }
    }

    private void Update_index_list()
    {
        foreach(Transform tr in this.area_all_item_editor)
        {
            tr.GetComponent<js_object>().y=tr.GetSiblingIndex();
        }
    }

    private void Delete_node(js_object obj)
    {
        app.carrot.play_sound_click();
        obj.Delete();
        app.carrot.show_msg("Json Editor", "Delete node " + obj.txt_name.text + " success!", Msg_Icon.Alert);
    }

    private void Clear_child_in_node(js_object obj)
    {
        app.carrot.play_sound_click();
        obj.Delete_all_child();
        app.carrot.show_msg("Json Editor", "Clear al child in node data " + obj.txt_name.text + " success!", Msg_Icon.Alert);
    }

    public void Btn_sel_mode(int index_mode)
    {
        this.index_sel_mode = index_mode;
        this.Check_mode();
        if (this.index_sel_mode == 0) this.Show_editor_by_coder();
        if (this.index_sel_mode == 1) this.Show_code_json(false);
        if (this.index_sel_mode == 2) this.Show_code_json(true);
        this.app.carrot.play_sound_click();
    }

    private void Check_mode()
    {
        this.img_btn_model[0].color = this.color_properties_nomal;
        this.img_btn_model[1].color = this.color_properties_nomal;
        this.img_btn_model[2].color = this.color_properties_nomal;
        this.panel_model[0].SetActive(false);
        this.panel_model[1].SetActive(false);
        this.img_btn_model[this.index_sel_mode].color = this.color_properties_select;
        this.panel_model[this.index_sel_mode].SetActive(true);
    }

    public void Show_code_json(bool is_no_space)
    {
        string s_json;
        if (is_no_space)
            s_json = this.Get_root().get_result_shortened();
        else
            s_json = this.Get_root().get_result();
        Debug.Log("View code:" + s_json);
        this.inp_coder_viewer.text = s_json;
        this.is_change_coderviewer = false;
    }

    public void Show_editor_by_coder()
    {
        if (this.is_change_coderviewer==false) return;
        this.Clear_list_item_editor();
        this.Paser(this.inp_coder_viewer.text);
    }

    public js_object Get_root()
    {
        return this.js_object_root;
    }

    public void Clear_list_item_editor()
    {
        this.is_change_coderviewer = false;
        this.app.carrot.clear_contain(this.area_all_item_editor);
    }

    public string Get_data_cur()
    {
        return js_object_root.get_result();
    }

    public void Paser_obj(IDictionary<string, object> obj_code, js_object js_father)
    {
        foreach (var obj_js in obj_code)
        {
            if (obj_js.Value is string)
            {
                if (obj_js.Value.ToString().IndexOf("#") == 0)
                {
                    js_object js_obj = this.Add_node(js_father, "propertie");
                    js_obj.Set_name_key(obj_js.Key.ToString());
                    js_obj.Set_val(obj_js.Value.ToString(), Type_Properties_val.color_val, this);
                }
                else
                {
                    if (int.TryParse(obj_js.Value.ToString(), out int obj_number))
                    {
                        js_object js_obj = this.Add_node(js_father, "propertie");
                        js_obj.Set_name_key(obj_js.Key.ToString());
                        js_obj.Set_val(obj_js.Value.ToString(), Type_Properties_val.number_val, this);
                    }
                    else
                    {
                        js_object js_obj = this.Add_node(js_father, "propertie");
                        js_obj.Set_name_key(obj_js.Key.ToString());
                        js_obj.Set_val(obj_js.Value.ToString(), Type_Properties_val.string_val, this);
                    }
                }
            }
            else if(obj_js.Value is null)
            {
                js_object js_obj = this.Add_node(js_father, "propertie");
                js_obj.Set_name_key(obj_js.Key.ToString());
                js_obj.Set_val(obj_js.Value.ToString(), Type_Properties_val.null_val, this);
            }
            else if (obj_js.Value is bool)
            {
                js_object js_obj = this.Add_node(js_father, "propertie");
                js_obj.Set_name_key(obj_js.Key.ToString());
                js_obj.Set_val(obj_js.Value.ToString(), Type_Properties_val.bool_val,this);
            }
            else if (obj_js.Value is IList<object>)
            {
                js_object obj_array = this.Add_node(js_father, "array");
                obj_array.Set_name_key(obj_js.Key.ToString());
                IList<object> l = (IList<object>)obj_js.Value;
                this.Paser_Array(l,obj_array);
            }else if(obj_js.Value is IDictionary)
            {
                IDictionary<string, object> obj_child = (IDictionary<string, object>)obj_js.Value;
                js_object obj_paser = this.Add_node(js_father, "object");
                obj_paser.Set_name_key(obj_js.Key.ToString());
                this.Paser_obj(obj_child, obj_paser);
            }
            else
            {
                js_object js_obj = this.Add_node(js_father, "propertie");
                js_obj.Set_name_key(obj_js.Key.ToString());
                js_obj.Set_val(obj_js.Value.ToString(), Type_Properties_val.string_val, this);
            }

        }
    }

    private void Paser_Array(IList<object> list_obj,js_object obj_father)
    {
        for (int y = 0; y < list_obj.Count; y++)
        {
            object myDict = list_obj[y];
            if (myDict is null)
            {
                js_object js_item_array = Add_node(obj_father, "array_item");
                js_item_array.Set_name_key("Array_item");
                js_item_array.Set_val(myDict.ToString(), Type_Properties_val.null_val, this);
            }
            else if (myDict is IDictionary)
            {
                js_object js_object = Add_node(obj_father, "object");
                IDictionary<string, object> datas = (IDictionary<string, object>)myDict;
                Paser_obj(datas, js_object);
            }
            else if (myDict is Array)
            {
                Paser_Array((IList<object>)myDict, obj_father);
            }
            else if (myDict is string)
            {
                js_object js_item_array = Add_node(obj_father, "array_item");
                js_item_array.Set_name_key("Array_item");
                js_item_array.Set_val(myDict.ToString(),Type_Properties_val.string_val,this);
            }
            else if (myDict is int || myDict is float || myDict is double)
            {
                js_object js_item_array = Add_node(obj_father, "array_item");
                js_item_array.Set_name_key("Array_item");
                js_item_array.Set_val(myDict.ToString(),Type_Properties_val.number_val,this);
            }
            else if (myDict is Color)
            {
                js_object js_item_array = Add_node(obj_father, "array_item");
                js_item_array.Set_name_key("Array_item");
                js_item_array.Set_val(myDict.ToString(), Type_Properties_val.color_val, this);
            }
            else if (myDict is DateTime)
            {
                js_object js_item_array = Add_node(obj_father, "array_item");
                js_item_array.Set_name_key("Array_item");
                js_item_array.Set_val(myDict.ToString(), Type_Properties_val.date_val, this);
            }
            else if (myDict is object)
            {
                js_object js_object = Add_node(obj_father, "object");
                IDictionary<string, object> datas = (IDictionary<string, object>)myDict;
                Paser_obj(datas, js_object);
            }
        }
    }


    public void Check_type_Properties()
    {
        this.panel_edit_Properties_color.SetActive(false);
        this.panel_edit_Properties_value.SetActive(false);
        this.panel_edit_Properties_bool.SetActive(false);
        for (int i = 0; i < this.img_btn_Properties.Length; i++) this.img_btn_Properties[i].color = this.color_properties_nomal;
        this.img_btn_Properties[this.index_edit_Properties_type].color = this.color_properties_select;
        if (this.index_edit_Properties_type == 0)
        {
            this.panel_edit_Properties_value.SetActive(true);
            this.inp_edit_Properties_value.contentType = InputField.ContentType.Standard;
        }

        if (this.index_edit_Properties_type == 1)
        {
            this.panel_edit_Properties_value.SetActive(true);
            this.inp_edit_Properties_value.contentType = InputField.ContentType.DecimalNumber;
            int obj_number;
            int.TryParse(this.inp_edit_Properties_value.text, out obj_number);
            this.inp_edit_Properties_value.text = obj_number.ToString();

        }
        if (this.index_edit_Properties_type == 2)
        {
            this.panel_edit_Properties_color.SetActive(true);
            this.inp_edit_Properties_value.contentType = InputField.ContentType.Standard;
            if (this.inp_edit_Properties_value.text == "") this.inp_edit_Properties_value.text = "#000000";
            this.Panel_select_color.set_color_for_edit_properties(this.inp_edit_Properties_value.text);
        }

        if (this.index_edit_Properties_type == 3)
        {
            this.inp_edit_Properties_value.contentType = InputField.ContentType.Standard;
            this.inp_edit_Properties_value.text = "null";
        }

        if (this.index_edit_Properties_type == 4)
        {
            this.panel_edit_Properties_bool.SetActive(true);
            this.inp_edit_Properties_value.contentType = InputField.ContentType.Standard;
            this.Check_val_bool_properties();
        }
    }

    public void Btn_sel_type_Properties(int index_sel)
    {
        this.app.carrot.play_sound_click();
        this.index_edit_Properties_type = index_sel;
        this.Check_type_Properties();
    }

    public void Show_select_color()
    {
        this.app.carrot.play_sound_click();
        this.Panel_select_color.show_table();
    }


    public void Btn_sel_val_bool_properties(bool is_true)
    {
        if (is_true)
            this.inp_edit_Properties_value.text = "true";
        else
            this.inp_edit_Properties_value.text = "false";

        this.Check_val_bool_properties();
        this.app.carrot.play_sound_click();
    }

    private void Check_val_bool_properties()
    {
        this.img_btn_Properties_bool[0].color = this.color_properties_nomal;
        this.img_btn_Properties_bool[1].color = this.color_properties_nomal;
        if (this.inp_edit_Properties_value.text.ToLower() == "true")
            this.img_btn_Properties_bool[0].color = this.color_properties_select;
        else
            this.img_btn_Properties_bool[1].color = this.color_properties_select;

    }

    public void Close_color_select()
    {
        this.app.carrot.play_sound_click();
        this.Panel_select_color.close();
    }

    public void Change_coder_in_view()
    {
        this.Paser(this.inp_coder_viewer.text);
    }

    public void Paser(string s_data)
    {
        this.Clear_list_item_editor();
        object data = Json.Deserialize(s_data);
        if(data is IDictionary)
        {
            Add_node(null, "root_object");
            IDictionary<string, object> data_obj = (IDictionary<string, object>)data;
            this.Paser_obj(data_obj, this.Get_root());
        }

        if (data is IList)
        {
            Add_node(null, "root_array");
            IList<object> data_obj = (IList<object>)data;
            this.Paser_Array(data_obj, this.Get_root());
        }
    }
}
