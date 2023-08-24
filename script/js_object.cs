using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class js_object : MonoBehaviour
{
    [Header("Obj Main")]
    public RectTransform area_body;
    public Image img_icon_expanded;
    public Image img_icon_obj;
    public Text txt_name;
    public Text txt_tip;
    public GameObject obj_btn_edit;
    public GameObject obj_btn_add_obj;
    public GameObject obj_btn_add_array;
    public GameObject obj_btn_add_properties;
    public GameObject obj_btn_delete;
    public GameObject obj_menu_btn;
    
    public string s_type = "object";
    public int index = 0;
    public int index_list = 0;

    [Header("Obj Update")]
    public InputField inp_name;
    public GameObject obj_btn_edit_done;
    public GameObject obj_btn_edit_close;

    [Header("Icon")]
    public Sprite sp_expanded_on;
    public Sprite sp_expanded_off;
    public Sprite sp_icon_object;
    public Sprite sp_icon_array;
    public Sprite sp_icon_array_item;
    public Sprite sp_icon_properties;
    public Sprite sp_icon_root;

    [Header("Properties")]
    public string s_Properties_value = "";
    public int index_Properties_type = 0;
    public Sprite[] icon_type_properties;
    public Image img_type_properties;

    private string s_result = "";
    private List<GameObject> list_child = new List<GameObject>();
    private bool is_show_child = true;

    public void btn_add_obj()
    {
        GameObject.Find("App").GetComponent<App>().add_obj(this,"object");
    }

    public void btn_add_array()
    {
        GameObject.Find("App").GetComponent<App>().add_obj(this,"array");
    }

    public void btn_add_properties()
    {
        GameObject.Find("App").GetComponent<App>().add_obj(this, "properties");
    }

    public void load_obj(string type,string s_name_set=null,int count_new=0)
    {
        this.s_type = type;
        if (this.s_type== "object")
        {
            this.img_icon_obj.sprite = this.sp_icon_object;
            if(s_name_set==null) this.txt_name.text = "Object "+count_new;
            this.txt_tip.text = "Object (0)";
        }

        if (this.s_type == "array")
        {
            this.img_icon_obj.sprite = this.sp_icon_array;
            if (s_name_set == null) this.txt_name.text = "Array " + count_new;
            this.txt_tip.text = "Array (0)";
        }

        if (this.s_type == "array_item")
        {
            this.img_icon_obj.sprite = this.sp_icon_array_item;
            this.txt_tip.text = "Array Item :"+ count_new;
        }

        if (this.s_type == "properties")
        {
            this.img_icon_obj.sprite = this.sp_icon_properties;
            if (s_name_set == null) this.txt_name.text = "Properties " + count_new;
            this.txt_tip.text = "Null";
        }

        if (this.s_type == "root")
        {
            this.img_icon_obj.sprite = this.sp_icon_root;
            if (s_name_set == null) this.txt_name.text = "Json";
            this.txt_tip.text = PlayerPrefs.GetString("json_root","Json Root Object");
        }
        if (s_name_set != null) this.txt_name.text = s_name_set;
    }

    public void btn_delete_obj()
    {
        this.delete_all_child();
        Destroy(this.gameObject);
        this.check_child_expanded();
        GameObject.Find("App").GetComponent<App>().set_save_status_new();
    }

    public void delete_all_child()
    {
        for (int i = 0; i < this.list_child.Count; i++) {
            if (this.list_child[i] != null)
            {
                this.list_child[i].GetComponent<js_object>().delete_all_child();
                Destroy(this.list_child[i].gameObject);
            }
        }
    }

    public string get_result()
    {
        string s_json = "";

        if (this.s_type == "root") {
            s_json = s_json + "{\n";
            for(int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null) s_json = s_json + this.list_child[i].GetComponent<js_object>().get_result();
            }
            s_json = s_json + "}";
        }
        else if(this.s_type=="object")
        {
            if(this.index>1) for(int i=1;i<this.index;i++) s_json = s_json + "\t";
            s_json = s_json + "\""+this.txt_name.text+"\":{";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (i == 0) s_json = s_json + "\n";
                if (this.list_child[i]!=null)s_json = s_json+this.list_child[i].GetComponent<js_object>().get_result();
            }
            if (this.index > 1&& this.list_child.Count>0) for (int i = 1; i < this.index; i++) s_json = s_json + "\t";
            s_json = s_json + "},\n";
        }
        else if (this.s_type == "array")
        {
            if (this.index > 1) for (int i = 1; i < this.index; i++) s_json = s_json + "\t";
            s_json = s_json + "\"" + this.txt_name.text + "\":[";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null)
                {
                    if(this.list_child[i].GetComponent<js_object>().s_type== "array_item")
                        s_json = s_json + this.list_child[i].GetComponent<js_object>().get_result();
                    else
                        s_json = s_json + "{" + this.list_child[i].GetComponent<js_object>().get_result() + "},";
                }
            }
            s_json=s_json.Replace(",}", "}");
            if (this.index > 1) for (int i = 1; i < this.index; i++) s_json = s_json + "\t";
            s_json = s_json + "],\n";
        }
        else if (this.s_type == "array_item")
        {
            if (this.index > 1) for (int i = 1; i < this.index; i++) s_json = s_json + "\t";
            s_json = s_json + "\"" + this.txt_name.text + "\",";
        }
        else if (this.s_type == "properties")
        {
            string s_val = "";
            if (this.s_Properties_value == "") s_val = "null";
            else if (this.index_Properties_type == 4) s_val = this.s_Properties_value;
            else if (this.index_Properties_type == 1) s_val = this.s_Properties_value;
            else s_val = "\"" + this.s_Properties_value + "\"";
            if (this.index > 1) for (int i = 1; i < this.index; i++) s_json = s_json + "\t";
            s_json = s_json + "\"" + this.txt_name.text + "\":"+s_val+",\n";
        }

        this.s_result = s_json;
        this.s_result = this.s_result.Replace("},\n}", "}\n}");
        this.s_result = this.s_result.Replace("\n,}", "}\n");
        this.s_result = this.s_result.Replace(",}", "}\n");
        this.s_result = this.s_result.Replace(",\n}", "\n}\n");
        this.s_result = this.s_result.Replace(",]", "]\n");
        this.s_result = this.s_result.Replace("],\n}", "]\n}");
        this.s_result = this.s_result.Replace("\n\n", "\n");
        this.s_result = this.s_result.Replace(",\n\t}", "\n\t}");
        this.s_result = this.s_result.Replace(",\n\t\t}", "\n\t\t}");
        this.s_result = this.s_result.Replace(",\n\t\t\t}", "\n\t\t\t}");
        this.s_result = this.s_result.Replace(",\n\t\t\t\t}", "\n\t\t\t\t}");
        this.s_result = this.s_result.Replace(",\n\t\t\t\t\t}", "\n\t\t\t\t\t}");
        this.s_result = this.s_result.Replace(",\n\t\t\t\t\t\t}", "\n\t\t\t\t\t\t}");
        this.s_result = this.s_result.Replace(",\n\t\t\t\t\t\t\t}", "\n\t\t\t\t\t\t\t}");
        this.s_result = this.s_result.Replace(",\n\t\t\t\t\t\t\t\t}", "\n\t\t\t\t\t\t\t\t}");
        return this.s_result;
    }

    public string get_result_shortened()
    {
        string s_json = "";

        if (this.s_type == "root")
        {
            s_json = s_json + "{";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null) s_json = s_json + this.list_child[i].GetComponent<js_object>().get_result_shortened();
            }
            s_json = s_json + "}";
        }
        else if (this.s_type == "object")
        {
            s_json = s_json + "\"" + this.txt_name.text + "\":{";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null) s_json = s_json + this.list_child[i].GetComponent<js_object>().get_result_shortened();
            }
            s_json = s_json + "},";
        }
        else if (this.s_type == "array")
        {
            s_json = s_json + "\"" + this.txt_name.text + "\":[";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null)
                {
                    if (this.list_child[i].GetComponent<js_object>().s_type == "array_item")
                        s_json = s_json + this.list_child[i].GetComponent<js_object>().get_result_shortened();
                    else
                        s_json = s_json + "{" + this.list_child[i].GetComponent<js_object>().get_result_shortened() + "},";
                }
            }
            s_json = s_json.Replace(",}", "}");
            s_json = s_json + "],";
        }
        else if (this.s_type == "array_item")
        {
            s_json = s_json + "\"" + this.txt_name.text + "\",";
        }
        else if (this.s_type == "properties")
        {
            string s_val;
            if (this.index_Properties_type == 3) s_val = "null";
            else if (this.index_Properties_type == 4) s_val = this.s_Properties_value;
            else if (this.index_Properties_type == 1) s_val = this.s_Properties_value;
            else s_val = "\"" + this.s_Properties_value + "\"";
            s_json = s_json + "\"" + this.txt_name.text + "\":" + s_val + ",";
        }

        this.s_result = s_json;
        this.s_result = this.s_result.Replace("},}", "}}");
        this.s_result = this.s_result.Replace(",}", "}");
        this.s_result = this.s_result.Replace(",}", "}");
        this.s_result = this.s_result.Replace(",}", "}");
        this.s_result = this.s_result.Replace(",]", "]");
        this.s_result = this.s_result.Replace("],}", "]}");
        return this.s_result;
    }

    public void add_child(GameObject o_child)
    {
        this.list_child.Add(o_child);
        this.check_child_expanded();
    }

    public void show_or_hide_child()
    {
        if (this.inp_name.gameObject.activeInHierarchy) return;
        if (this.is_show_child)
            this.is_show_child = false;
        else
            this.is_show_child = true;

       
        this.hide_or_show_all_child(this.is_show_child);
        this.check_child_expanded();
    }

    public void check_child_expanded()
    {
        if (this.list_child.Count > 0)
        {
            this.img_icon_expanded.gameObject.SetActive(true);
            if (this.s_type == "object") this.txt_tip.text = "Object (" + this.list_child.Count + " Item)";
            if (this.s_type == "array") this.txt_tip.text = "Array (" + this.list_child.Count + " Item)";
        }
        else
        {
            this.img_icon_expanded.gameObject.SetActive(false);
            if (this.s_type == "object") this.txt_tip.text = "Object (None)";
            if (this.s_type == "array") this.txt_tip.text = "Array (None)";
        }
    }

    public void hide_or_show_all_child(bool is_show)
    {
        if(is_show)
            this.img_icon_expanded.sprite = this.sp_expanded_off;
        else
            this.img_icon_expanded.sprite = this.sp_expanded_on;

        for (int i = 0; i < this.list_child.Count; i++)
        {
            if (this.list_child[i] != null)
            {
                this.list_child[i].GetComponent<js_object>().hide_or_show_all_child(is_show);
                this.list_child[i].SetActive(is_show);
            }
        }
    }

    public void btn_show_edit()
    {
        if (this.s_type == "root")
        {
            this.show_or_hide_child();
            return;
        }

        if (this.s_type == "properties")
        {
            GameObject.Find("App").GetComponent<App>().show_Properties(this);
            return;
        }
        GameObject.Find("App").GetComponent<App>().block_all_btn_edit(false);
        this.obj_menu_btn.SetActive(true);
        this.inp_name.text = this.txt_name.text;
        this.inp_name.gameObject.SetActive(true);
        this.obj_btn_edit_done.SetActive(true);
        this.obj_btn_edit_close.SetActive(true);
        this.obj_btn_add_array.SetActive(false);
        this.obj_btn_add_obj.SetActive(false);
        this.obj_btn_edit.SetActive(false);
        this.obj_btn_delete.SetActive(false);
        this.obj_btn_add_properties.SetActive(false);
        this.txt_tip.gameObject.SetActive(false);
    }

    public void btn_close_edit()
    {
        GameObject.Find("App").GetComponent<App>().block_all_btn_edit(true);
        this.txt_tip.gameObject.SetActive(true);
        this.load_obj_default();
    }

    public void load_obj_default()
    {
        this.inp_name.gameObject.SetActive(false);
        this.obj_btn_edit_done.SetActive(false);
        this.obj_btn_edit_close.SetActive(false);
        this.obj_btn_edit.SetActive(true);
        this.obj_btn_delete.SetActive(true);
        if (this.s_type == "properties")
        {
            this.obj_btn_add_array.SetActive(false);
            this.obj_btn_add_obj.SetActive(false);
            this.obj_btn_add_properties.SetActive(false);
        }
        else if (this.s_type == "root")
        {
            this.obj_btn_delete.SetActive(false);
            this.obj_btn_edit.SetActive(false);
        }
        else
        {
            this.obj_btn_add_array.SetActive(true);
            this.obj_btn_add_obj.SetActive(true);
            this.obj_btn_add_properties.SetActive(true);
        }
    }

    public void btn_done_edit()
    {
        this.txt_name.text = this.inp_name.text;
        this.btn_close_edit();
    }

    public int get_length_item()
    {
        return this.list_child.Count;
    }

    public void set_properties_value(int type,string s_val)
    {
        this.index_Properties_type = type;
        this.txt_tip.text = s_val;
        this.txt_tip.color = Color.blue;
        this.s_Properties_value = s_val;
        this.img_type_properties.sprite = this.icon_type_properties[type];
    }

}
