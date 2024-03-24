using Carrot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class js_object : MonoBehaviour
{
    [Header("Obj Main")]
    public GameObject btn_carrot_prefab;
    public RectTransform area_body;
    public Transform area_btn_extension;
    public Image img_icon_expanded;
    public Image img_icon_obj;
    public Text txt_name;
    public Text txt_tip;
    
    public string s_type = "";
    public string s_name = "";
    public string s_val = "";

    public int x = 0;
    public int y = 0;

    [Header("Icon")]
    public Sprite sp_expanded_on;
    public Sprite sp_expanded_off;

    [Header("Properties")]
    public Image img_type_properties;

    private string s_result = "";
    private List<js_object> list_child = new();
    private Type_Properties_val type_Propertie = Type_Properties_val.string_val;
    private bool is_show_child = true;

    public void On_load(string s_type)
    {
        this.txt_name.text = this.s_type;
        this.s_type = s_type;
        this.s_name = s_type;
        this.Load_info();
        this.img_icon_expanded.gameObject.SetActive(false);
        if (s_type == "propertie") this.s_val = null;
    }

    private void Load_info()
    {
        string s_count_child;
        if (this.list_child.Count > 0) s_count_child = " (" + this.list_child.Count + ")";
        else s_count_child = " (None)";
        this.txt_tip.text = this.s_type+" "+s_count_child;
    }

    public void Add_child(js_object obj)
    {
        this.list_child.Add(obj);
        this.check_child_expanded();
        this.Load_info();
    }

    public void Set_name_key(string s_name)
    {
        this.txt_name.text = s_name;
        this.txt_name.color = new Color32(115, 0, 48, 255);
        this.s_name = s_name;
    }

    public void Remove_name_key()
    {
        this.txt_name.text = this.s_type;
        this.txt_name.color = Color.black;
        this.s_name ="";
    }

    public void Set_type(Type_Properties_val type)
    {
        this.type_Propertie = type;
    }

    public void Set_val(string s_val,Type_Properties_val type_properties, Json_Editor js_edit)
    {
        this.type_Propertie = type_properties;
        if (this.type_Propertie == Type_Properties_val.string_val) this.img_type_properties.sprite = js_edit.app.json_properties.sp_icon_properties_string;
        if (this.type_Propertie == Type_Properties_val.number_val) this.img_type_properties.sprite = js_edit.app.json_properties.sp_icon_properties_number;
        if (this.type_Propertie == Type_Properties_val.color_val) this.img_type_properties.sprite = js_edit.app.carrot.sp_icon_theme_color;
        if (this.type_Propertie == Type_Properties_val.date_val) this.img_type_properties.sprite = js_edit.app.json_properties.sp_icon_properties_date;
        if (this.type_Propertie == Type_Properties_val.null_val) this.img_type_properties.sprite = js_edit.app.json_properties.sp_icon_properties_null;
        if (this.type_Propertie == Type_Properties_val.bool_val) this.img_type_properties.sprite = js_edit.app.json_properties.sp_icon_properties_bool;
        this.txt_tip.text = s_val;
        this.s_val = s_val;
        this.txt_tip.color = Color.blue;
    }

    public void Delete_all_child()
    {
        for (int i = 0; i < this.list_child.Count; i++) {
            if (this.list_child[i] != null) this.list_child[i].Delete();
        }
        this.list_child = new();
    }

    public string get_result()
    {
        string s_json = "";

        if (this.s_type == "root_object")
        {
            s_json += "{\n";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null) s_json += this.list_child[i].get_result();
            }
            s_json += "}";
        }
        else if (this.s_type == "root_array")
        {
            s_json += "[\n";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null) s_json += this.list_child[i].get_result();
            }
            s_json += "]";
        }
        else if (this.s_type == "object")
        {
            if (this.x > 1) for (int i = 1; i < this.x; i++) s_json += "\t";
            if(s_name.Trim()!="") s_json +=s_json + "\"" + this.s_name + "\":";
            s_json += "{";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (i == 0) s_json += "\n";
                if (this.list_child[i] != null) s_json = s_json + this.list_child[i].get_result();
            }
            if (this.x > 1 && this.list_child.Count > 0) for (int i = 1; i < this.x; i++) s_json = s_json + "\t";
            s_json = s_json + "},\n";
        }
        else if (this.s_type == "array")
        {
            if (this.x > 1) for (int i = 1; i < this.x; i++) s_json = s_json + "\t";
            if(this.s_name!="") s_json = s_json + "\"" + this.s_name + "\":";
            s_json +="[";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (i == 0) s_json += "\n";
                if (this.list_child[i] != null)
                {
                    if (this.list_child[i].s_type == "array_item")
                        s_json = s_json + this.list_child[i].get_result();
                    else
                        s_json = s_json + "{" + this.list_child[i].get_result() + "},";
                }
            }
            s_json = s_json.Replace(",}", "}");
            if (this.x > 1&&this.list_child.Count>0) for (int i = 1; i < this.x; i++) s_json+="\t";
            s_json = s_json + "],\n";
        }
        else if (this.s_type == "array_item")
        {
            if (this.x > 1) for (int i = 1; i < this.x; i++) s_json += "\t";
            s_json = s_json + "\"" + this.s_val + "\",\n";
        }
        else if (this.s_type == "propertie")
        {
            string s_val_propertie = "";
            if (this.type_Propertie==Type_Properties_val.null_val) s_val = "null";
            else if (this.type_Propertie==Type_Properties_val.number_val) s_val_propertie = this.s_val;
            else s_val_propertie = "\"" + this.s_val + "\"";
            if (this.x > 1) for (int i = 1; i < this.x; i++) s_json = s_json + "\t";
            s_json = s_json + "\"" + this.s_name + "\":" + s_val_propertie + ",\n";
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

        if (this.s_type == "root_object")
        {
            s_json += "{";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null) s_json += this.list_child[i].get_result_shortened();
            }
            s_json += "}";
        }
        else if (this.s_type == "root_array")
        {
            s_json += "[";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null) s_json += this.list_child[i].get_result_shortened();
            }
            s_json += "]";
        }
        else if (this.s_type == "object")
        {
            s_json +="\"" + this.txt_name.text + "\":{";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null) s_json += this.list_child[i].get_result_shortened();
            }
            s_json += "},";
        }
        else if (this.s_type == "array")
        {
            s_json = s_json + "\"" + this.s_name + "\":[";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (this.list_child[i] != null)
                {
                    if (this.list_child[i].s_type == "array_item")
                        s_json += this.list_child[i].get_result_shortened();
                    else
                        s_json = s_json + "{" + this.list_child[i].get_result_shortened() + "},";
                }
            }
            s_json = s_json.Replace(",}", "}");
            s_json = s_json + "],";
        }
        else if (this.s_type == "array_item")
        {
            s_json = s_json + "\"" + this.s_val + "\",";
        }
        else if (this.s_type == "propertie")
        {
            string s_val_propertie = "";
            if (this.type_Propertie == Type_Properties_val.null_val) s_val = "null";
            else if (this.type_Propertie == Type_Properties_val.number_val) s_val_propertie = this.s_val;
            else s_val_propertie = "\"" + this.s_val + "\"";
            s_json +="\"" + this.s_name + "\":" + s_val_propertie + ",";
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

    public void show_or_hide_child()
    {
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
            this.img_icon_expanded.gameObject.SetActive(true);
        else
            this.img_icon_expanded.gameObject.SetActive(false);
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
                this.list_child[i].hide_or_show_all_child(is_show);
                this.list_child[i].gameObject.SetActive(is_show);
            }
        }
    }

    public int get_length_item()
    {
        return this.list_child.Count;
    }

    public Carrot_Box_Btn_Item Create_btn(string s_name = "btn_item")
    {
        GameObject bnt_footer = Instantiate(btn_carrot_prefab);
        bnt_footer.name = s_name;
        bnt_footer.transform.SetParent(this.area_btn_extension);
        bnt_footer.transform.localPosition = new Vector3(bnt_footer.transform.position.x, bnt_footer.transform.position.y, 0f);
        bnt_footer.transform.localScale = new Vector3(1f, 1f, 1f);
        bnt_footer.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Carrot_Box_Btn_Item btn_item = bnt_footer.GetComponent<Carrot_Box_Btn_Item>();
        return btn_item;
    }

    public void Set_icon(Sprite sp_icon)
    {
        this.img_icon_obj.sprite = sp_icon;
    }

    public void Delete()
    {
        this.Delete_all_child();
        Destroy(this.gameObject);
    }

    public Type_Properties_val get_Type_Properties()
    {
        return this.type_Propertie;
    }
}
