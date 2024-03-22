using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Json_Editor : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Obj Json")]
    public GameObject prefab_obj_js;
    public GameObject Panel_edit_Properties;
    public Color_Board Panel_select_color;
    public Transform area_all_item_editor;
    public ScrollRect ScrollRect_all_item_editor;
    public InputField inp_coder_viewer;
    public Text txt_username_login;

    public float space_x_item = 25f;
    private List<GameObject> list_item_obj = new();
    public GameObject[] panel_model;
    public Image[] img_btn_model;

    private int index_sel_mode = 0;
    private bool is_change_coderviewer;
    private bool is_change_editor = false;
    private js_object Js_object_edit_temp;

    [Header("Edit Properties")]
    public Image[] img_btn_Properties;
    public Image[] img_btn_Properties_bool;
    public Color32 color_properties_nomal;
    public Color32 color_properties_select;
    public InputField inp_edit_Properties_name;
    public InputField inp_edit_Properties_value;
    public int index_edit_Properties_type;
    public GameObject panel_edit_Properties_value;
    public GameObject panel_edit_Properties_color;
    public GameObject panel_edit_Properties_bool;

    public void On_load()
    {
        this.Panel_edit_Properties.SetActive(false);
        this.Panel_select_color.gameObject.SetActive(false);
        this.app.carrot.clear_contain(this.area_all_item_editor);
        this.add_obj(prefab_obj_js.GetComponent<js_object>(), "root");
        this.check_mode();
    }

    public void add_obj(js_object o, string s_type)
    {
        GameObject obj = Instantiate(this.prefab_obj_js);
        js_object js_obj = obj.GetComponent<js_object>();
        float x_space = this.space_x_item * o.index;
        js_obj.index = o.index + 1;
        js_obj.area_body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x_space, js_obj.area_body.rect.height);
        obj.transform.SetParent(this.area_all_item_editor);
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.SetSiblingIndex(o.index_list + 1);
        js_obj.load_obj(s_type, null, o.get_length_item() + 1);
        if (this.list_item_obj.Count > 0) o.add_child(obj);
        js_obj.check_child_expanded();
        js_obj.load_obj_default();
        this.list_item_obj.Add(obj);
        this.update_option_list();
        if (s_type != "root")
        {
            this.is_change_coderviewer = true;
            this.is_change_editor = true;
            this.app.obj_icon_save_status_new.SetActive(true);
            this.app.carrot.play_sound_click();
        }
        this.app.carrot.ads.show_ads_Interstitial();
    }


    public void Update_option_list_obj()
    {
        for (int i = 0; i < this.list_item_obj.Count; i++)
        {
            this.list_item_obj[i].GetComponent<js_object>().index_list = i;
        }
    }

    public void Btn_sel_mode(int index_mode)
    {
        this.index_sel_mode = index_mode;
        this.check_mode();
        if (this.index_sel_mode == 0) this.show_editor_by_coder();
        if (this.index_sel_mode == 1) this.show_code_json(false);
        if (this.index_sel_mode == 2) this.show_code_json(true);
        this.app.carrot.play_sound_click();
    }

    public void check_mode()
    {
        this.img_btn_model[0].color = this.color_properties_nomal;
        this.img_btn_model[1].color = this.color_properties_nomal;
        this.img_btn_model[2].color = this.color_properties_nomal;
        this.panel_model[0].SetActive(false);
        this.panel_model[1].SetActive(false);
        this.img_btn_model[this.index_sel_mode].color = this.color_properties_select;
        this.panel_model[this.index_sel_mode].SetActive(true);
    }

    public void update_option_list()
    {
        int i = 0;
        foreach (Transform c in this.area_all_item_editor)
        {
            c.GetComponent<js_object>().index_list = i;
            i++;
        }
    }


    public void show_code_json(bool is_no_space)
    {
        string s_json = "";
        if (is_no_space)
            s_json = this.get_root().get_result_shortened();
        else
            s_json = this.get_root().get_result();
        this.inp_coder_viewer.text = s_json;
        this.is_change_coderviewer = false;
    }

    public void show_editor_by_coder()
    {
        if (!this.is_change_coderviewer) return;
        this.clear_list_item_editor();
        IDictionary<string, object> thanh = (IDictionary<string, object>)Carrot.Json.Deserialize(this.inp_coder_viewer.text);
        this.GetComponent<Manager_Project>().paser_obj(thanh, this.get_root());
        this.update_option_list();
    }

    public js_object get_root()
    {
        return this.list_item_obj[0].GetComponent<js_object>();
    }

    public void clear_list_item_editor()
    {
        this.is_change_coderviewer = false;
        this.list_item_obj = new List<GameObject>();
        this.app.carrot.clear_contain(this.area_all_item_editor);
        this.add_obj(prefab_obj_js.GetComponent<js_object>(), "root");
    }

    public void block_all_btn_edit(bool is_show)
    {
        for (int i = 0; i < this.list_item_obj.Count; i++) if (this.list_item_obj[i] != null) this.list_item_obj[i].GetComponent<js_object>().obj_menu_btn.SetActive(is_show);
    }

    public void add_obj_list_main(GameObject obj_new_js)
    {
        this.list_item_obj.Add(obj_new_js);
    }

    public void Act_check_lang()
    {
        if (this.list_item_obj[0] != null) this.list_item_obj[0].GetComponent<js_object>().txt_tip.text = "Json Root Object";
    }

    public string Get_data_cur()
    {
        return list_item_obj[0].GetComponent<js_object>().get_result();
    }

    public void paser_obj(IDictionary<string, object> thanh, js_object js_father)
    {
        if (thanh is object)
            foreach (var obj_js in thanh)
            {
                GameObject item_editor = Instantiate(this.prefab_obj_js);
                item_editor.transform.SetParent(this.area_all_item_editor);
                item_editor.transform.localScale = new Vector3(1f, 1f, 1f);
                item_editor.GetComponent<js_object>().txt_name.text = obj_js.Key;
                float x_space = this.app.space_x_item * js_father.index;
                item_editor.GetComponent<js_object>().index = js_father.index + 1;
                item_editor.GetComponent<js_object>().index_list = js_father.index_list + 1;
                item_editor.GetComponent<js_object>().area_body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x_space, item_editor.GetComponent<js_object>().area_body.rect.height);

                if (obj_js.Value is string)
                {
                    if (obj_js.Value.ToString().IndexOf("#") == 0)
                    {
                        item_editor.GetComponent<js_object>().load_obj("properties", obj_js.Key);
                        item_editor.GetComponent<js_object>().set_properties_value(2, obj_js.Value.ToString());
                    }
                    else
                    {
                        int obj_number;
                        if (int.TryParse(obj_js.Value.ToString(), out obj_number))
                        {
                            item_editor.GetComponent<js_object>().load_obj("properties", obj_js.Key);
                            item_editor.GetComponent<js_object>().set_properties_value(1, obj_number.ToString());
                        }
                        else
                        {
                            item_editor.GetComponent<js_object>().load_obj("properties", obj_js.Key);
                            item_editor.GetComponent<js_object>().set_properties_value(0, obj_js.Value.ToString());
                        }
                    }

                }
                else if (obj_js.Value is bool)
                {
                    item_editor.GetComponent<js_object>().load_obj("properties", obj_js.Key);
                    item_editor.GetComponent<js_object>().set_properties_value(4, obj_js.Value.ToString());
                }
                else if (obj_js.Value is IList<object>)
                {
                    item_editor.GetComponent<js_object>().load_obj("array", obj_js.Key);
                    IList<object> l = (IList<object>)obj_js.Value;
                    for (int y = 0; y < l.Count; y++)
                    {
                        if (l[y] is string)
                        {
                            this.add_item_array(l[y].ToString(), y, item_editor.GetComponent<js_object>());
                        }
                        else
                        {
                            IDictionary<string, object> child_obj = (IDictionary<string, object>)l[y];
                            paser_obj(child_obj, item_editor.GetComponent<js_object>());
                        }
                    }
                }
                else
                {
                    if (obj_js.Value != null)
                    {
                        try
                        {
                            IDictionary<string, object> obj_child = (IDictionary<string, object>)obj_js.Value;
                            item_editor.GetComponent<js_object>().load_obj("object", obj_js.Key);
                            this.paser_obj(obj_child, item_editor.GetComponent<js_object>());
                        }
                        catch
                        {
                            item_editor.GetComponent<js_object>().load_obj("properties", obj_js.Key);
                            item_editor.GetComponent<js_object>().set_properties_value(1, obj_js.Value.ToString());
                        }
                    }
                    else
                    {
                        item_editor.GetComponent<js_object>().load_obj("properties", obj_js.Key);
                        item_editor.GetComponent<js_object>().set_properties_value(3, "Null");
                    }
                }
                item_editor.GetComponent<js_object>().check_child_expanded();
                item_editor.GetComponent<js_object>().load_obj_default();
                js_father.add_child(item_editor);
                this.app.json_editor.add_obj_list_main(item_editor);
            }
    }

    private void add_item_array(string s_val, int index_item, js_object js_father)
    {
        GameObject item_editor = Instantiate(this.prefab_obj_js);
        item_editor.transform.SetParent(this.area_all_item_editor);
        item_editor.transform.localScale = new Vector3(1f, 1f, 1f);
        float x_space = this.app.space_x_item * js_father.index;
        item_editor.GetComponent<js_object>().index = js_father.index + 1;
        item_editor.GetComponent<js_object>().area_body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x_space, item_editor.GetComponent<js_object>().area_body.rect.height);
        item_editor.GetComponent<js_object>().load_obj("array_item", s_val, index_item);
        item_editor.GetComponent<js_object>().check_child_expanded();
        item_editor.GetComponent<js_object>().load_obj_default();
        js_father.add_child(item_editor);
        this.app.json_editor.add_obj_list_main(item_editor);
    }


    public void close_Properties()
    {
        this.app.carrot.play_sound_click();
        this.Panel_edit_Properties.SetActive(false);
    }

    public void btn_done_Properties()
    {
        this.app.carrot.play_sound_click();
        this.Js_object_edit_temp.txt_name.text = this.inp_edit_Properties_name.text;
        this.Js_object_edit_temp.set_properties_value(this.index_edit_Properties_type, this.inp_edit_Properties_value.text);
        this.Panel_edit_Properties.SetActive(false);
    }

    public void show_Properties(js_object o_edit)
    {
        this.Js_object_edit_temp = o_edit;
        this.inp_edit_Properties_name.text = o_edit.txt_name.text;
        this.inp_edit_Properties_value.text = o_edit.s_Properties_value;
        this.index_edit_Properties_type = o_edit.index_Properties_type;
        this.Panel_edit_Properties.SetActive(true);
        this.check_type_Properties();
    }

    public void check_type_Properties()
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
            this.check_val_bool_properties();
        }
    }

    public void btn_sel_type_Properties(int index_sel)
    {
        this.app.carrot.play_sound_click();
        this.index_edit_Properties_type = index_sel;
        this.check_type_Properties();
    }

    public void show_select_color()
    {
        this.app.carrot.play_sound_click();
        this.Panel_select_color.show_table();
    }


    public void btn_sel_val_bool_properties(bool is_true)
    {
        if (is_true)
            this.inp_edit_Properties_value.text = "true";
        else
            this.inp_edit_Properties_value.text = "false";

        this.check_val_bool_properties();
        this.app.carrot.play_sound_click();
    }

    private void check_val_bool_properties()
    {
        this.img_btn_Properties_bool[0].color = this.color_properties_nomal;
        this.img_btn_Properties_bool[1].color = this.color_properties_nomal;
        if (this.inp_edit_Properties_value.text.ToLower() == "true")
            this.img_btn_Properties_bool[0].color = this.color_properties_select;
        else
            this.img_btn_Properties_bool[1].color = this.color_properties_select;

    }

    public void close_color_select()
    {
        this.app.carrot.play_sound_click();
        this.Panel_select_color.close();
    }

}
