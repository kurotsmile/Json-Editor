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
    
    public string s_type = "object";
    public int x = 0;
    public int y = 0;

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
    private List<js_object> list_child = new();
    private bool is_show_child = true;

    public void On_load(string s_type)
    {
        this.txt_name.text = this.s_type;
        this.s_type = s_type;
        this.Load_info();
        this.img_icon_expanded.gameObject.SetActive(false);
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


    public void Delete_all_child()
    {
        for (int i = 0; i < this.list_child.Count; i++) {
            if (this.list_child[i] != null) this.list_child[i].Delete();
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
            if(this.x>1) for(int i=1;i<this.x;i++) s_json = s_json + "\t";
            s_json = s_json + "\""+this.txt_name.text+"\":{";
            for (int i = 0; i < this.list_child.Count; i++)
            {
                if (i == 0) s_json = s_json + "\n";
                if (this.list_child[i]!=null)s_json = s_json+this.list_child[i].GetComponent<js_object>().get_result();
            }
            if (this.x > 1&& this.list_child.Count>0) for (int i = 1; i < this.x; i++) s_json = s_json + "\t";
            s_json = s_json + "},\n";
        }
        else if (this.s_type == "array")
        {
            if (this.x > 1) for (int i = 1; i < this.x; i++) s_json = s_json + "\t";
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
            if (this.x > 1) for (int i = 1; i < this.x; i++) s_json = s_json + "\t";
            s_json = s_json + "],\n";
        }
        else if (this.s_type == "array_item")
        {
            if (this.x > 1) for (int i = 1; i < this.x; i++) s_json = s_json + "\t";
            s_json = s_json + "\"" + this.txt_name.text + "\",";
        }
        else if (this.s_type == "properties")
        {
            string s_val = "";
            if (this.s_Properties_value == "") s_val = "null";
            else if (this.index_Properties_type == 4) s_val = this.s_Properties_value;
            else if (this.index_Properties_type == 1) s_val = this.s_Properties_value;
            else s_val = "\"" + this.s_Properties_value + "\"";
            if (this.x > 1) for (int i = 1; i < this.x; i++) s_json = s_json + "\t";
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
        this.Load_info();
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

    public js_object set_properties_value(int type,string s_val)
    {
        this.index_Properties_type = type;
        this.txt_tip.text = s_val;
        this.txt_tip.color = Color.blue;
        this.s_Properties_value = s_val;
        this.img_type_properties.sprite = this.icon_type_properties[type];
        return this;
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
}
