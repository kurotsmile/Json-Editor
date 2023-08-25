using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Manager_Project : MonoBehaviour
{
    private int length = 0;
    public Sprite sp_icon_open_project;
    public Sprite sp_icon_project_online;
    public GameObject Project_Item_Prefab;
    private Project_Item project_item_temp;

    public void load_project()
    {
        this.length = PlayerPrefs.GetInt("p_length", 0);
    }

    public void add_project(string s_name, string s_data)
    {
        string id_user_login = this.GetComponent<App>().carrot.user.get_id_user_login();
        if (id_user_login!= "")
        {
            WWWForm frm_add_project = this.GetComponent<App>().carrot.frm_act("add_project");
            frm_add_project.AddField("project_name", s_name);
            frm_add_project.AddField("project_data", s_data);
            frm_add_project.AddField("project_user", id_user_login);
            frm_add_project.AddField("project_lang", this.GetComponent<App>().carrot.user.get_id_user_login());
            //this.GetComponent<App>().carrot.send(frm_add_project,act_add_project);
        }
        else
        {
            PlayerPrefs.SetString("p_name_" + length, s_name);
            PlayerPrefs.SetString("p_data_" + length, s_data);
            PlayerPrefs.SetString("p_date_" + length, System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            this.length++;
            PlayerPrefs.SetInt("p_length", this.length);
        }
    }

    private void act_add_project(string s_data)
    {
        Debug.Log("Add projec:" + s_data);
        this.project_item_temp = new Project_Item();
        this.project_item_temp.s_id_online = s_data;
    }

    public void show_list()
    {
        bool is_login_user = false;
        if (this.GetComponent<App>().carrot.user.get_id_user_login() != "") is_login_user = true;

        if (this.length == 0 && is_login_user == false)
        {
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("open","Open project"),PlayerPrefs.GetString("no_project","You don't have any archived projects yet"),Carrot.Msg_Icon.Error);
            return;
        }

        this.GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("open", "Open project"), this.sp_icon_open_project);
        for (int i = this.length-1; i >=0; i--)
        {
            if (PlayerPrefs.GetString("p_name_" + i, "") != "")
            {
                GameObject obj_project = Instantiate(this.Project_Item_Prefab);
                //obj_project.transform.SetParent(this.GetComponent<App>().carrot.area_body_box);
                obj_project.transform.localScale = new Vector3(1f, 1f, 1f);
                obj_project.GetComponent<Project_Item>().txt_name.text = PlayerPrefs.GetString("p_name_" + i);
                obj_project.GetComponent<Project_Item>().txt_date.text = PlayerPrefs.GetString("p_date_" + i);
                obj_project.GetComponent<Project_Item>().index_project = i;
                obj_project.GetComponent<Project_Item>().obj_btn_export_file.SetActive(false);
                obj_project.GetComponent<Project_Item>().obj_btn_export_web.SetActive(false);
                obj_project.GetComponent<Project_Item>().obj_btn_share.SetActive(false);
                if (is_login_user) obj_project.GetComponent<Project_Item>().obj_btn_upload.SetActive(true);
                else obj_project.GetComponent<Project_Item>().obj_btn_upload.SetActive(false);
            }
        }

        this.get_list_project_online();
    }

    private void get_list_project_online()
    {
        string id_user_login = this.GetComponent<App>().carrot.user.get_id_user_login();
        if (id_user_login != "") {
            WWWForm frm_list_project = this.GetComponent<App>().carrot.frm_act("list_project");
            frm_list_project.AddField("project_user", id_user_login);
            //this.GetComponent<App>().carrot.send(frm_list_project,show_list_project_online);
        }
    }

    private void show_list_project_online(string s_data)
    {
        IList list_project_online = (IList)Carrot.Json.Deserialize(s_data);
        for(int i = 0; i < list_project_online.Count; i++)
        {
            IDictionary data_project = (IDictionary)list_project_online[i];
            GameObject obj_project = Instantiate(this.Project_Item_Prefab);
           // obj_project.transform.SetParent(this.GetComponent<App>().carrot.area_body_box);
            obj_project.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_project.GetComponent<Project_Item>().txt_name.text = data_project["name"].ToString();
            obj_project.GetComponent<Project_Item>().txt_date.text = data_project["date"].ToString();
            obj_project.GetComponent<Project_Item>().s_id_online = data_project["id"].ToString();
            obj_project.GetComponent<Project_Item>().index_project = -2;
            obj_project.GetComponent<Project_Item>().img_icon.sprite = this.sp_icon_project_online;
            obj_project.GetComponent<Project_Item>().obj_btn_export_web.SetActive(true);
            obj_project.GetComponent<Project_Item>().obj_btn_export_file.SetActive(true);
            obj_project.GetComponent<Project_Item>().obj_btn_share.SetActive(true);
            obj_project.GetComponent<Project_Item>().obj_btn_upload.SetActive(false);
        }
    }

    public void show_project_offline(Project_Item p_item)
    {
        this.project_item_temp = p_item;
        string s_data = PlayerPrefs.GetString("p_data_" + p_item.index_project);
        string s_name = PlayerPrefs.GetString("p_name_" + p_item.index_project);
        this.GetComponent<App>().sel_project_index = p_item.index_project;
        this.show_project(s_name, s_data);
    }

    public void show_project(string s_name,string s_data)
    {
        this.GetComponent<App>().clear_list_item_editor();
        this.GetComponent<App>().txt_save_status.text = s_name;
        IDictionary<string, object> thanh = (IDictionary<string, object>)Carrot.Json.Deserialize(s_data);
        this.paser_obj(thanh, this.GetComponent<App>().get_root());
        this.GetComponent<App>().update_option_list();
        if (this.GetComponent<App>().get_index_sel_mode() == 2)
            this.GetComponent<App>().show_code_json(true);
        else
            this.GetComponent<App>().show_code_json(false);

        this.GetComponent<App>().carrot.close();
        this.GetComponent<App>().ScrollRect_all_item_editor.verticalNormalizedPosition = 1f;
        this.GetComponent<App>().carrot.play_sound_click();
        this.GetComponent<App>().update_option_list_obj();
    }

    public void show_project_online(Project_Item p_item)
    {
        this.project_item_temp = p_item;
        this.GetComponent<App>().sel_project_index = p_item.index_project;
        WWWForm frm_get_project = this.GetComponent<App>().carrot.frm_act("get_project");
        frm_get_project.AddField("project_id",p_item.s_id_online);
        //this.GetComponent<App>().carrot.send(frm_get_project, act_show_project_online);
    }

    public void act_show_project_online(string s_data_online)
    {
        IDictionary data_project =(IDictionary) Carrot.Json.Deserialize(s_data_online);
        this.show_project(data_project["name"].ToString(), data_project["data"].ToString());
    }

    public void paser_obj(IDictionary<string, object> thanh,js_object js_father)
    {
        if (thanh is  object)
        foreach (var obj_js in thanh)
        {
            GameObject item_editor = Instantiate(this.GetComponent<App>().prefab_obj_js);
            item_editor.transform.SetParent(this.GetComponent<App>().area_all_item_editor);
            item_editor.transform.localScale = new Vector3(1f, 1f, 1f);
            item_editor.GetComponent<js_object>().txt_name.text = obj_js.Key;
            float x_space = this.GetComponent<App>().space_x_item * js_father.index;
            item_editor.GetComponent<js_object>().index = js_father.index + 1;
            item_editor.GetComponent<js_object>().index_list = js_father.index_list + 1;
            item_editor.GetComponent<js_object>().area_body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x_space, item_editor.GetComponent<js_object>().area_body.rect.height);

            if (obj_js.Value is string)
            {
                if (obj_js.Value.ToString().IndexOf("#")==0)
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
                for (int y = 0; y < l.Count;y++)
                {
                    if (l[y] is string)
                    {
                        this.add_item_array(l[y].ToString(),y, item_editor.GetComponent<js_object>());
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
                    try {
                        IDictionary<string, object> obj_child = (IDictionary<string, object>)obj_js.Value;
                        item_editor.GetComponent<js_object>().load_obj("object", obj_js.Key);
                        this.paser_obj(obj_child, item_editor.GetComponent<js_object>());
                    } catch
                    {
                        item_editor.GetComponent<js_object>().load_obj("properties", obj_js.Key);
                        item_editor.GetComponent<js_object>().set_properties_value(1, obj_js.Value.ToString());
                    }
                }
                else
                {
                    item_editor.GetComponent<js_object>().load_obj("properties", obj_js.Key);
                    item_editor.GetComponent<js_object>().set_properties_value(3,"Null");
                }
            }
            item_editor.GetComponent<js_object>().check_child_expanded();
            item_editor.GetComponent<js_object>().load_obj_default();
            js_father.add_child(item_editor);
            this.GetComponent<App>().add_obj_list_main(item_editor);
        }
    }

    private void add_item_array(string s_val,int index_item, js_object js_father)
    {
        GameObject item_editor = Instantiate(this.GetComponent<App>().prefab_obj_js);
        item_editor.transform.SetParent(this.GetComponent<App>().area_all_item_editor);
        item_editor.transform.localScale = new Vector3(1f, 1f, 1f);
        float x_space = this.GetComponent<App>().space_x_item * js_father.index;
        item_editor.GetComponent<js_object>().index = js_father.index + 1;
        item_editor.GetComponent<js_object>().area_body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x_space, item_editor.GetComponent<js_object>().area_body.rect.height);
        item_editor.GetComponent<js_object>().load_obj("array_item", s_val, index_item);
        item_editor.GetComponent<js_object>().check_child_expanded();
        item_editor.GetComponent<js_object>().load_obj_default();
        js_father.add_child(item_editor);
        this.GetComponent<App>().add_obj_list_main(item_editor);
    }

    public void delete_project(int index)
    {
        bool is_null_list = true;
        this.delete_project_data(index);
        for (int i = 0; i < this.length; i++) if (PlayerPrefs.GetString("p_name_" + i, "") != "") { is_null_list = false; break; }

        if (is_null_list)
        {
            this.length = 0;
            PlayerPrefs.DeleteKey("p_length");
            this.GetComponent<App>().carrot.close();
            this.GetComponent<App>().set_save_status_default();
        }
        else
            this.show_list();
    }

    private void delete_project_data(int index)
    {
        PlayerPrefs.DeleteKey("p_name_" + index);
        PlayerPrefs.DeleteKey("p_data_" + index);
        PlayerPrefs.DeleteKey("p_date_" + index);
    }

    public void delete_project_online(string s_id_project)
    {
        WWWForm frm_delete_project = this.GetComponent<App>().carrot.frm_act("delete_project");
        frm_delete_project.AddField("project_id", s_id_project);
        //this.GetComponent<App>().carrot.send(frm_delete_project, this.act_delete_project);
    }

    private void act_delete_project(string s_data)
    {
        this.show_list();
    }

    public void update_project(int index_project,string s_data)
    {
        if (this.project_item_temp.s_id_online != "")
        {
            WWWForm frm_update_project = this.GetComponent<App>().carrot.frm_act("update_project_data");
            frm_update_project.AddField("project_id", this.project_item_temp.s_id_online);
            frm_update_project.AddField("project_data", s_data);
            //this.GetComponent<App>().carrot.send(frm_update_project,this.act_update_project_online);
        }
        else
        {
            PlayerPrefs.SetString("p_data_" + index_project, s_data);
            PlayerPrefs.SetString("p_date_" + index_project, System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("p_update_success","Project update successful!"));
        }
    }

    private void act_update_project_online(string s_data)
    {
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("p_update_success", "Project update successful!"));
    }

    public void update_project_name(int id_project,string s_new_name)
    {
        PlayerPrefs.SetString("p_name_" + id_project, s_new_name);
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("p_name_update_success","Project name update successful!"));
    }

    public void update_project_name_online(string id_project, string s_new_name)
    {
        WWWForm frm_update_project = this.GetComponent<App>().carrot.frm_act("update_project_name");
        frm_update_project.AddField("project_id", id_project);
        frm_update_project.AddField("new_name", s_new_name);
        //this.GetComponent<App>().carrot.send(frm_update_project, this.act_update_project_name_online);
    }

    private void act_update_project_name_online(string s_data)
    {
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("p_name_update_success", "Project name update successful!"));
    }

    public void show_project_last()
    {
        this.project_item_temp = new Project_Item();
        this.project_item_temp.index_project = this.length - 1;
        this.show_project_offline(this.project_item_temp);
    }

    public void upload_project(Project_Item p_item)
    {
        this.project_item_temp = p_item;
        WWWForm frm_upload_project = this.GetComponent<App>().carrot.frm_act("upload_project");
        string s_data = PlayerPrefs.GetString("p_data_" + p_item.index_project);
        string s_name = PlayerPrefs.GetString("p_name_" + p_item.index_project);
        frm_upload_project.AddField("project_name", s_name);
        frm_upload_project.AddField("project_data", s_data);
        frm_upload_project.AddField("project_user", this.GetComponent<App>().carrot.user.get_id_user_login());
        //this.GetComponent<App>().carrot.send(frm_upload_project,act_upload_project);
    }

    private void act_upload_project(string s_data)
    {
        this.delete_project_data(this.project_item_temp.index_project);
        this.project_item_temp.s_id_online = s_data;
        this.project_item_temp.img_icon.sprite = this.sp_icon_project_online;
        this.project_item_temp.obj_btn_upload.SetActive(false);
        this.project_item_temp.obj_btn_export_web.SetActive(true);
        this.project_item_temp.obj_btn_share.SetActive(true);
        this.project_item_temp.obj_btn_export_file.SetActive(true);
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("p_upload_success","Successful online project hosting!"));
    }

    public void set_new_project()
    {
        this.project_item_temp = null;
    }

    public void import_json_url(string url_data)
    {
        StartCoroutine(get_data_json_by_url(url_data));
    }

    IEnumerator get_data_json_by_url(string url_json)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url_json))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                this.GetComponent<App>().clear_list_item_editor();
                this.GetComponent<App>().txt_save_status.text =PlayerPrefs.GetString("import_file", "Import file");
                IDictionary<string, object> obj_js = (IDictionary<string, object>)Carrot.Json.Deserialize(www.downloadHandler.text);
                this.paser_obj(obj_js, this.GetComponent<App>().get_root());
                this.GetComponent<App>().update_option_list();
                if (this.GetComponent<App>().get_index_sel_mode() == 2)
                    this.GetComponent<App>().show_code_json(true);
                else
                    this.GetComponent<App>().show_code_json(false);

                this.GetComponent<App>().carrot.close();
                this.GetComponent<App>().ScrollRect_all_item_editor.verticalNormalizedPosition = 1f;
                this.GetComponent<App>().carrot.play_sound_click();
                this.GetComponent<App>().update_option_list_obj();
                this.GetComponent<App>().panel_import.SetActive(false);
            }
        }
    }
}
