using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class Manager_Project : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    private int length = 0;
    public Sprite sp_icon_open_project;
    public Sprite sp_icon_project_online;

    private Carrot_Box box = null;
    private Carrot_Box box_menu = null;
    private int sel_project_index = -1;

    public void On_load()
    {
        this.length = PlayerPrefs.GetInt("js_length", 0);
    }

    private void Add_project(string s_name, string s_data)
    {
        IDictionary data =(IDictionary) Json.Deserialize("{}");
        data["id"] = "code" + app.carrot.generateID() + UnityEngine.Random.Range(1, 20);
        data["title"] = s_name;
        data["code"] = s_data;
        data["date"] = DateTime.Now.ToString();
        this.Add_project(data);
    }

    private void Add_project(IDictionary data)
    {
        PlayerPrefs.SetString("js_data_" + this.length, Json.Serialize(data));
        this.length += 1;
        PlayerPrefs.SetInt("js_length", this.length);
    }

    public void Show_list_project()
    {
        string id_user_login= this.app.carrot.user.get_id_user_login();
        if (id_user_login != "")
        {
            box_menu = app.carrot.Create_Box();
            box_menu.set_icon(this.app.sp_icon_project);
            box_menu.set_title("Select the project archive list");

            Carrot_Box_Item item_list_offline = box_menu.create_item();
            item_list_offline.set_icon(sp_icon_open_project);
            item_list_offline.set_title("Offline project");
            item_list_offline.set_tip("Local security projects are only visible to you");
            item_list_offline.set_act(()=>Show_list_offline());

            Carrot_Box_Item item_list_online = box_menu.create_item();
            item_list_online.set_icon(sp_icon_project_online);
            item_list_online.set_title("Online project");
            item_list_online.set_tip("Projects you can store online and share with the world");
            item_list_online.set_act(()=>Show_list_online());
        }
        else
        {
            this.Show_list_offline();
        }
    }

    private void Show_list_offline()
    {
        if (this.length == 0)
        {
            this.app.carrot.show_msg(app.carrot.lang.Val("open", "Open project"), app.carrot.lang.Val("no_project", "You don't have any archived projects yet"), Carrot.Msg_Icon.Alert);
            this.app.carrot.play_vibrate();
            return;
        }

        string s_id_user_login = app.carrot.user.get_id_user_login();
        if (this.box != null) this.box.close();
        this.box = this.app.carrot.Create_Box(PlayerPrefs.GetString("open", "Open project"), this.sp_icon_open_project);
        for (int i = this.length - 1; i >= 0; i--)
        {
            string s_data_project = PlayerPrefs.GetString("js_data_" + i, "");
            if (s_data_project!="")
            {
                var index = i;
                IDictionary data = (IDictionary) Json.Deserialize(s_data_project);
                Carrot_Box_Item item_project = this.Add_item_to_list_box(data);

                if (s_id_user_login != "")
                {
                    Carrot_Box_Btn_Item btn_public = item_project.create_item();
                    btn_public.set_icon(sp_icon_project_online);
                    btn_public.set_color(app.carrot.color_highlight);
                    btn_public.set_act(() => Upload_project(index));
                }

                Carrot_Box_Btn_Item btn_del = item_project.create_item();
                btn_del.set_icon(app.carrot.sp_icon_del_data);
                btn_del.set_color(Color.red);
                btn_del.set_act(() => Delete_project_offline(index));
            }
        }
    }

    private Carrot_Box_Item Add_item_to_list_box(IDictionary data)
    {
        Carrot_Box_Item item_project = box.create_item("item_project");
        item_project.set_icon(this.app.sp_icon_project);
        item_project.set_title(data["title"].ToString());
        item_project.set_tip(data["date"].ToString());

        Carrot_Box_Btn_Item btn_export = item_project.create_item();
        btn_export.set_icon(app.sp_icon_export_file);
        btn_export.set_color(app.carrot.color_highlight);
        btn_export.set_act(() => Export_file(data));

        item_project.set_act(() => Show_project(data));
        return item_project;
    }

    private void Show_list_online()
    {
        app.carrot.show_loading();
        string id_user_login = this.app.carrot.user.get_id_user_login();
        if (id_user_login != "") {
            StructuredQuery q = new("code");
            q.Add_where("user_id", Query_OP.EQUAL, id_user_login);
            app.carrot.server.Get_doc(q.ToJson(), Act_list_online_done, Act_server_fail);
        }
    }

    private void Act_list_online_done(string s_data)
    {
        app.carrot.hide_loading();
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.box = app.carrot.Create_Box();
            this.box.set_title("List of projects archived online");
            this.box.set_icon(this.sp_icon_project_online);

            for(int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data = fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item item_project=this.Add_item_to_list_box(data);

                Carrot_Box_Btn_Item btn_download = item_project.create_item();
                btn_download.set_color(app.carrot.color_highlight);
                btn_download.set_icon(app.carrot.icon_carrot_download);
                btn_download.set_act(()=>Add_project(data));

                string s_link_share = app.carrot.mainhost+"/?p=code&id="+data["id"].ToString();
                Carrot_Box_Btn_Item btn_share = item_project.create_item();
                btn_share.set_color(app.carrot.color_highlight);
                btn_share.set_icon(app.carrot.sp_icon_share);
                btn_share.set_act(() => Show_share_project(s_link_share));

                Carrot_Box_Btn_Item btn_del = item_project.create_item();
                btn_del.set_icon(app.carrot.sp_icon_del_data);
                btn_del.set_color(Color.red);
                btn_del.set_act(() => delete_project_online(data["id"].ToString()));
            }
        }
        else
        {
            this.app.carrot.show_msg(app.carrot.lang.Val("open", "Open project"), app.carrot.lang.Val("no_project", "You don't have any archived projects yet"), Carrot.Msg_Icon.Alert);
            this.app.carrot.play_vibrate();
            return;
        }
    }

    private void Show_share_project(string s_link)
    {
        app.carrot.show_share(s_link, "Share this project with everyone");
    }

    private void Show_project(IDictionary data)
    {
        if (box != null) box.close();
        if (box_menu != null) box_menu.close();
        if (data["index"] != null) this.sel_project_index = int.Parse(data["index"].ToString());

        this.app.clear_list_item_editor();
        this.app.txt_save_status.text = data["title"].ToString();
        IDictionary<string, object> thanh = (IDictionary<string, object>)Json.Deserialize(data["code"].ToString());
        this.paser_obj(thanh, this.app.get_root());
        this.app.update_option_list();
        if (this.app.get_index_sel_mode() == 2)
            this.app.show_code_json(true);
        else
            this.app.show_code_json(false);

        this.app.carrot.close();
        this.app.ScrollRect_all_item_editor.verticalNormalizedPosition = 1f;
        this.app.carrot.play_sound_click();
        this.app.Update_option_list_obj();
    }

    public void Show_project_online(string id_project)
    {
        StructuredQuery q = new(app.carrot.Carrotstore_AppId);
        q.Add_where("project_id",Query_OP.EQUAL, id_project);
    }

    public void paser_obj(IDictionary<string, object> thanh,js_object js_father)
    {
        if (thanh is  object)
        foreach (var obj_js in thanh)
        {
            GameObject item_editor = Instantiate(this.app.prefab_obj_js);
            item_editor.transform.SetParent(this.app.area_all_item_editor);
            item_editor.transform.localScale = new Vector3(1f, 1f, 1f);
            item_editor.GetComponent<js_object>().txt_name.text = obj_js.Key;
            float x_space = this.app.space_x_item * js_father.index;
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
            this.app.add_obj_list_main(item_editor);
        }
    }

    private void add_item_array(string s_val,int index_item, js_object js_father)
    {
        GameObject item_editor = Instantiate(this.app.prefab_obj_js);
        item_editor.transform.SetParent(this.app.area_all_item_editor);
        item_editor.transform.localScale = new Vector3(1f, 1f, 1f);
        float x_space = this.app.space_x_item * js_father.index;
        item_editor.GetComponent<js_object>().index = js_father.index + 1;
        item_editor.GetComponent<js_object>().area_body.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x_space, item_editor.GetComponent<js_object>().area_body.rect.height);
        item_editor.GetComponent<js_object>().load_obj("array_item", s_val, index_item);
        item_editor.GetComponent<js_object>().check_child_expanded();
        item_editor.GetComponent<js_object>().load_obj_default();
        js_father.add_child(item_editor);
        this.app.add_obj_list_main(item_editor);
    }

    public void Delete_project_offline(int index)
    {
        bool is_null_list = true;
        this.delete_project_data(index);
        for (int i = 0; i < this.length; i++) if (PlayerPrefs.GetString("p_name_" + i, "") != "") { is_null_list = false; break; }

        if (is_null_list)
        {
            this.length = 0;
            PlayerPrefs.DeleteKey("p_length");
            this.app.carrot.close();
            this.app.set_save_status_default();
        }
        else
            this.Show_list_project();
    }

    public void Delete_project_online(string s_id_project)
    {
        app.carrot.show_loading();
        app.carrot.server.Delete_Doc("code", s_id_project, Act_delete_project_online_done, Act_server_fail);
    }

    private void Act_delete_project_online_done(string s_data)
    {
        app.carrot.hide_loading();
        app.carrot.show_msg("Json Editor", "Delete project online success!", Msg_Icon.Success);
    }

    private void delete_project_data(int index)
    {
        PlayerPrefs.DeleteKey("p_name_" + index);
        PlayerPrefs.DeleteKey("p_data_" + index);
        PlayerPrefs.DeleteKey("p_date_" + index);
    }

    public void delete_project_online(string s_id_project)
    {
        app.carrot.server.Delete_Doc(this.app.carrot.Carrotstore_AppId, s_id_project, Act_delete_project);
    }

    private void Act_delete_project(string s_data)
    {
        this.Show_list_project();
    }

    public void Update_project_curent()
    {
        PlayerPrefs.SetString("p_data_" + this.sel_project_index, app.Get_data_cur());
        PlayerPrefs.SetString("p_date_" + this.sel_project_index, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        this.app.carrot.show_msg(PlayerPrefs.GetString("p_update_success", "Project update successful!"));
    }

    private void act_update_project_online(string s_data)
    {
        this.app.carrot.show_msg(PlayerPrefs.GetString("p_update_success", "Project update successful!"));
    }

    public void update_project_name(int id_project,string s_new_name)
    {
        PlayerPrefs.SetString("p_name_" + id_project, s_new_name);
        this.app.carrot.show_msg(PlayerPrefs.GetString("p_name_update_success","Project name update successful!"));
    }

    public void Show_project_last()
    {
        this.sel_project_index = this.length - 1;
    }

    public void Set_new_project()
    {
        this.sel_project_index = -1;
    }

    public void Import_json_url(string url_data)
    {
        app.carrot.show_loading();
        StartCoroutine(Get_data_json_by_url(url_data));
    }

    IEnumerator Get_data_json_by_url(string url_json)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url_json);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success) 
        {
            app.carrot.hide_loading();
            Debug.Log(www.downloadHandler.text);
            this.app.clear_list_item_editor();
            this.app.txt_save_status.text ="Import file";
            IDictionary<string, object> obj_js = (IDictionary<string, object>)Json.Deserialize(www.downloadHandler.text);
            this.paser_obj(obj_js, this.app.get_root());
            this.app.update_option_list();
            if (this.app.get_index_sel_mode() == 2)
                this.app.show_code_json(true);
            else
                this.app.show_code_json(false);

            this.app.carrot.close();
            this.app.ScrollRect_all_item_editor.verticalNormalizedPosition = 1f;
            this.app.carrot.play_sound_click();
            this.app.Update_option_list_obj();
            if (box != null) box.close();
        }
        else
        {
            app.carrot.hide_loading();
            app.carrot.show_msg("Error", www.error.ToString(), Msg_Icon.Error);
        }
    }

    public void Show_save_project(bool is_save_as)
    {
        box = app.carrot.Create_Box();
        box.set_icon(app.sp_icon_save);
        box.set_title("Project Archives");

        if (is_save_as==false)
        {
            Carrot_Box_Item item_tip = box.create_item("item_tip");
            item_tip.set_icon(app.sp_icon_save);
            item_tip.set_title("Project Archives");
            item_tip.set_tip("Store your json string for easy management and retrieval");
        }
        else
        {
            Carrot_Box_Item item_tip = box.create_item("item_tip");
            item_tip.set_icon(app.sp_icon_save_as);
            item_tip.set_title("Save As...");
            item_tip.set_tip("Archive this project with another name");
        }

        Carrot_Box_Item item_name_project = box.create_item("item_name");
        item_name_project.set_icon(app.sp_icon_project);
        item_name_project.set_title("Name of project");
        item_name_project.set_tip("Name the project you want to save");
        item_name_project.set_type(Box_Item_Type.box_value_input);
        item_name_project.check_type();

        Carrot_Box_Btn_Panel panel_btn = box.create_panel_btn();

        Carrot_Button_Item btn_save=panel_btn.create_btn("btn_save");
        btn_save.set_icon_white(app.carrot.icon_carrot_done);
        btn_save.set_bk_color(app.carrot.color_highlight);
        btn_save.set_label_color(Color.white);
        btn_save.set_label("Done");
        btn_save.set_act_click(() => Act_save_project(item_name_project.get_val()));

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_cancel.set_bk_color(app.carrot.color_highlight);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_act_click(() => Act_close_save());
    }

    private void Act_save_project(string s_name)
    {
        app.carrot.play_sound_click();
        if (s_name.Trim() == "")
        {
            this.app.carrot.show_msg(app.carrot.lang.Val("project_name_error", "Project name cannot be empty!"));
            this.app.carrot.play_vibrate();
            return;
        }

        this.app.txt_save_status.text = s_name;
        this.Add_project(s_name, app.Get_data_cur());
        if (box != null) box.close();
    }

    private void Act_close_save()
    {
        app.carrot.play_sound_click();
        if (box != null) box.close();
    }

    public void Show_Import()
    {
        box = app.carrot.Create_Box();
        box.set_icon(app.sp_icon_save);
        box.set_title("Project Import");

        Carrot_Box_Item item_tip = box.create_item("item_tip");
        item_tip.set_icon(app.sp_icon_save_as);
        item_tip.set_title("Import");
        item_tip.set_tip("Import json data from web address");

        Carrot_Box_Item item_url = box.create_item("item_name");
        item_url.set_icon(app.sp_icon_project);
        item_url.set_title("URL");
        item_url.set_tip("Enter json web url");
        item_url.set_type(Box_Item_Type.box_value_input);
        item_url.check_type();

        Carrot_Box_Btn_Panel panel_btn = box.create_panel_btn();

        Carrot_Button_Item btn_save = panel_btn.create_btn("btn_save");
        btn_save.set_icon_white(app.carrot.icon_carrot_done);
        btn_save.set_bk_color(app.carrot.color_highlight);
        btn_save.set_label_color(Color.white);
        btn_save.set_label("Done");
        btn_save.set_act_click(() => Act_import_project(item_url.get_val()));

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_cancel.set_bk_color(app.carrot.color_highlight);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_act_click(() => Act_close_save());
    }

    private void Act_import_project(string s_data)
    {
        if (s_data.Trim() == "")
        {
            this.app.carrot.show_msg("Import","Input url cannot be empty", Msg_Icon.Error);
            this.app.carrot.play_vibrate();
            return;
        }
        this.Import_json_url(s_data);
    }

    public int Get_Index_project_curent()
    {
        return this.sel_project_index;
    }

    private void Export_file(IDictionary data)
    {
        string s_data = data["code"].ToString();
        string s_name = data["title"].ToString();

#if UNITY_EDITOR
        string filePath = EditorUtility.SaveFilePanel("Save File", "", s_name, "json");
        if (!string.IsNullOrEmpty(filePath))
        {
            try
            {
                System.IO.File.WriteAllText(filePath, s_data);
                app.carrot.show_input("Save", "File saved successfully at: ", filePath, Window_Input_value_Type.input_field);
            }
            catch (System.Exception ex)
            {
                app.carrot.show_msg("Error", "Failed to save file: " + ex.Message, Msg_Icon.Error);
            }
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        string filePath = DirectoryHelper.GetAndroidExternalFilesDir() + "/" + s_name + ".json";

        try
        {
            File.WriteAllText(filePath, s_data);
            app.carrot.show_input("Save", "File saved successfully at: ", filePath, Window_Input_value_Type.input_field);
        }
        catch (System.Exception ex)
        {
            app.carrot.show_msg("Error", "Failed to save file: " + ex.Message, Msg_Icon.Error);
        }
#endif
    }

    private void Upload_project(int index)
    {
        app.carrot.show_loading();
        string s_data = PlayerPrefs.GetString("js_data_" + index);
        IDictionary data = (IDictionary) Json.Deserialize(s_data);
        data["code_theme"] = "docco.min.css";
        data["code_type"] = "json";
        data["code"] = data["code"].ToString().Replace("\"", "\\\"");
        data["user_id"] = app.carrot.user.get_id_user_login();
        data["user_lang"] = app.carrot.user.get_lang_user_login();
        data["status"] = "pending";

        string s_data_json = app.carrot.server.Convert_IDictionary_to_json(data);
        app.carrot.server.Add_Document_To_Collection("code", data["id"].ToString(), s_data_json, Act_upload_project_done, Act_server_fail);
    }

    private void Act_upload_project_done(string s_data)
    {
        app.carrot.hide_loading();
        app.carrot.show_msg("Upload Project", "Public project success!", Msg_Icon.Success);
    }

    private void Act_server_fail(string s_error)
    {
        app.carrot.hide_loading();
        app.carrot.show_msg("Error", s_error, Msg_Icon.Error);
        app.carrot.play_vibrate();
    }
}
