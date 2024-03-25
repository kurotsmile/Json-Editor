using Carrot;
using SimpleFileBrowser;
using System;
using System.Collections;
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
    private Carrot_Window_Input box_inp = null;
    private int sel_project_index = -1;
    private IDictionary data_project_temp = null;

    public void On_load()
    {
        this.length = PlayerPrefs.GetInt("js_length", 0);
    }

    private void Add_project(string s_name, string s_data)
    {
        IDictionary data = (IDictionary)Json.Deserialize("{}");
        data["id"] = "code" + app.carrot.generateID() + UnityEngine.Random.Range(1, 20);
        data["title"] = s_name;
        data["code"] = s_data;
        data["date"] = DateTime.Now.ToString();
        this.Add_project(data);
    }

    private void Add_project(IDictionary data)
    {
        PlayerPrefs.SetString("js_data_" + this.length, Json.Serialize(data));
        this.sel_project_index = this.length;
        this.length += 1;
        PlayerPrefs.SetInt("js_length", this.length);
    }

    public void Show_list_project()
    {
        string id_user_login = this.app.carrot.user.get_id_user_login();
        if (id_user_login != "")
        {
            box_menu = app.carrot.Create_Box();
            box_menu.set_icon(this.app.sp_icon_project);
            box_menu.set_title("Select the project archive list");

            Carrot_Box_Item item_list_offline = box_menu.create_item();
            item_list_offline.set_icon(sp_icon_open_project);
            item_list_offline.set_title("Offline project");
            item_list_offline.set_tip("Local security projects are only visible to you");
            item_list_offline.set_act(() => Show_list_offline());

            Carrot_Box_Item item_list_online = box_menu.create_item();
            item_list_online.set_icon(sp_icon_project_online);
            item_list_online.set_title("Online project");
            item_list_online.set_tip("Projects you can store online and share with the world");
            item_list_online.set_act(() => Show_list_online());
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
            if (s_data_project != "")
            {
                var index = i;
                IDictionary data = (IDictionary)Json.Deserialize(s_data_project);
                data["index"] = i;
                Carrot_Box_Item item_project = this.Add_item_to_list_box(data);

                if (s_id_user_login != "")
                {
                    Carrot_Box_Btn_Item btn_public = item_project.create_item();
                    btn_public.set_icon(sp_icon_project_online);
                    btn_public.set_color(app.carrot.color_highlight);
                    btn_public.set_act(() => Upload_project(index));
                }

                Carrot_Box_Btn_Item btn_edit = item_project.create_item();
                btn_edit.set_icon(app.carrot.user.icon_user_edit);
                btn_edit.set_color(app.carrot.color_highlight);
                btn_edit.set_act(() => Show_edit_info(data));

                Carrot_Box_Btn_Item btn_del = item_project.create_item();
                btn_del.set_icon(app.carrot.sp_icon_del_data);
                btn_del.set_color(Color.red);
                btn_del.set_act(() => Delete_project_offline(index, item_project.gameObject));
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
        if (id_user_login != "")
        {
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

            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data = fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item item_project = this.Add_item_to_list_box(data);

                Carrot_Box_Btn_Item btn_download = item_project.create_item();
                btn_download.set_color(app.carrot.color_highlight);
                btn_download.set_icon(app.carrot.icon_carrot_download);
                btn_download.set_act(() => Download_project(data));

                string s_link_share = app.carrot.mainhost + "/?p=code&id=" + data["id"].ToString();
                Carrot_Box_Btn_Item btn_share = item_project.create_item();
                btn_share.set_color(app.carrot.color_highlight);
                btn_share.set_icon(app.carrot.sp_icon_share);
                btn_share.set_act(() => Show_share_project(s_link_share));

                Carrot_Box_Btn_Item btn_del = item_project.create_item();
                btn_del.set_icon(app.carrot.sp_icon_del_data);
                btn_del.set_color(Color.red);
                btn_del.set_act(() => Delete_project_online(data["id"].ToString(), item_project.gameObject));
            }
        }
        else
        {
            this.app.carrot.show_msg(app.carrot.lang.Val("open", "Open project"), app.carrot.lang.Val("no_project", "You don't have any archived projects yet"), Carrot.Msg_Icon.Alert);
            this.app.carrot.play_vibrate();
            return;
        }
    }

    public void Show_project_by_id(string s_id)
    {
        app.carrot.play_sound_click();
        app.carrot.show_loading();
        StructuredQuery q = new("code");
        q.Add_where("id", Query_OP.EQUAL, s_id);
        q.Set_limit(1);
        app.carrot.server.Get_doc(q.ToJson(), Act_get_project_by_id,Act_server_fail);
    }

    private void Act_get_project_by_id(string s_data)
    {
        app.carrot.hide_loading();
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            IDictionary data = fc.fire_document[0].Get_IDictionary();
            this.Show_project(data);
        }
        else
        {
            this.app.carrot.show_msg("Json Editor", "This project does not exist!", Carrot.Msg_Icon.Alert);
            app.carrot.play_vibrate();
        }
    }

    private void Show_share_project(string s_link)
    {
        app.carrot.show_share(s_link, "Share this project with everyone");
    }

    private void Download_project(IDictionary data)
    {
        this.Add_project(data);
        app.carrot.show_msg("Download and import project success!");
    }

    private void Show_project(IDictionary data)
    {
        if (box != null) box.close();
        if (box_menu != null) box_menu.close();
        if (data["index"] != null)
            this.sel_project_index = int.Parse(data["index"].ToString());
        else
            this.sel_project_index = -1;

        this.app.txt_save_status.text = data["title"].ToString();


        if (this.app.get_index_sel_mode() == 2)
            this.app.json_editor.Show_code_json(true);
        else
            this.app.json_editor.Show_code_json(false);
        this.app.json_editor.Paser(data["code"].ToString());
        this.app.json_editor.ScrollRect_all_item_editor.verticalNormalizedPosition = 1f;
        this.app.carrot.play_sound_click();
    }

    public void Show_project_online(string id_project)
    {
        StructuredQuery q = new(app.carrot.Carrotstore_AppId);
        q.Add_where("project_id", Query_OP.EQUAL, id_project);
    }

    public void Delete_project_offline(int index, GameObject obj_item)
    {
        PlayerPrefs.DeleteKey("js_data_" + index);
        bool is_null_list = true;
        for (int i = 0; i < this.length; i++) if (PlayerPrefs.GetString("js_data_" + i, "") != "") { is_null_list = false; break; }

        if (is_null_list)
        {
            this.length = 0;
            PlayerPrefs.DeleteKey("p_length");
            this.app.Set_save_status_default();
            if (box != null) box.close();
        }
        Destroy(obj_item);
    }

    public void Delete_project_online(string s_id_project, GameObject obj_item)
    {
        app.carrot.show_loading();
        app.carrot.server.Delete_Doc("code", s_id_project, Act_delete_project_online_done, Act_server_fail);
        Destroy(obj_item);
    }

    private void Act_delete_project_online_done(string s_data)
    {
        app.carrot.hide_loading();
        app.carrot.show_msg("Json Editor", "Delete project online success!", Msg_Icon.Success);
    }

    public void Set_new_project()
    {
        this.sel_project_index = -1;
    }

    IEnumerator Get_data_json_by_url(string url_json)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url_json);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            app.carrot.hide_loading();
            this.app.json_editor.Clear_list_item_editor();
            this.app.txt_save_status.text = "Import file";
            this.app.json_editor.Paser(www.downloadHandler.text);
            if (this.app.get_index_sel_mode() == 2)
                this.app.json_editor.Show_code_json(true);
            else
                this.app.json_editor.Show_code_json(false);
            this.app.json_editor.ScrollRect_all_item_editor.verticalNormalizedPosition = 1f;
            this.app.carrot.play_sound_click();
            if (box != null) box.close();
            if (box_inp != null) box_inp.close();
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

        if (is_save_as == false)
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

        Carrot_Button_Item btn_save = panel_btn.create_btn("btn_save");
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
        this.app.obj_icon_save_status_new.SetActive(false);
        this.Add_project(s_name, app.json_editor.Get_data_cur());
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

        Carrot_Box_Item item_url = box.create_item("item_name");
        item_url.set_icon(app.sp_icon_import_web);
        item_url.set_title("Enter from web address");
        item_url.set_tip("Enter json web url");
        item_url.set_act(() => Import_from_web_address());

        Carrot_Box_Item item_import_file = box.create_item("item_import_file");
        item_import_file.set_icon(app.sp_icon_import_file);
        item_import_file.set_title("Import from file");
        item_import_file.set_tip("Click here to select the json file to import");
        item_import_file.set_act(() => Import_from_file());

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

    private void Import_from_web_address()
    {
        this.box_inp = app.carrot.show_input("Import json data from web address", "Enter from web address");
        this.box_inp.set_act_done(Act_import_project);
    }

    private void Act_import_project(string s_data)
    {
        if (s_data.Trim() == "")
        {
            this.app.carrot.show_msg("Import", "Input url cannot be empty", Msg_Icon.Error);
            this.app.carrot.play_vibrate();
            return;
        }

        app.carrot.show_loading();
        StartCoroutine(Get_data_json_by_url(s_data));
    }

    private void Import_from_file()
    {
        this.Check_Permissions();
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetFilters(true,
            new FileBrowser.Filter("Json data file", ".json", ".jsonl", ".json5"),
            new FileBrowser.Filter("Geo Map json", ".geojson"),
            new FileBrowser.Filter("Web App Manifest", ".webappmanifest"),
            new FileBrowser.Filter("Babel", ".babelrc"),
            new FileBrowser.Filter("Prettier,", ".prettierrc"),
            new FileBrowser.Filter("ESLint,", ".eslintrc")
        );
        FileBrowser.ShowLoadDialog(Act_Import_from_file_done, Act_Import_from_file_cancel, FileBrowser.PickMode.Files);
    }

    private void Act_Import_from_file_done(string[] paths)
    {
        string s_data = FileBrowserHelpers.ReadTextFromFile(paths[0]);
        this.app.json_editor.Paser(s_data);
        this.app.txt_save_status.text = FileBrowserHelpers.GetFilename(paths[0]);
        this.app.Set_save_status_new();
        if (box != null) box.close();
    }

    private void Act_Import_from_file_cancel() { }

    public int Get_Index_project_curent()
    {
        return this.sel_project_index;
    }

    private void Export_file(IDictionary data)
    {
        data_project_temp = data;
        this.Check_Permissions();
        FileBrowser.ShowSaveDialog(Export_file_done, Export_file_cancel, FileBrowser.PickMode.Files);

    }

    private void Export_file_done(string[] paths)
    {
        string filePath = paths[0];
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetFilters(true,
            new FileBrowser.Filter("Json data file", ".json", ".jsonl", ".json5"),
            new FileBrowser.Filter("Geo Map json", ".geojson"),
            new FileBrowser.Filter("Web App Manifest", ".webappmanifest"),
            new FileBrowser.Filter("Babel", ".babelrc"),
            new FileBrowser.Filter("Prettier,", ".prettierrc"),
            new FileBrowser.Filter("ESLint,", ".eslintrc")
        );
        FileBrowserHelpers.WriteTextToFile(filePath, data_project_temp["code"].ToString());
        app.carrot.show_msg("Export File", "File saved successfully at: " + filePath, Msg_Icon.Success);
    }

    private void Export_file_cancel()
    {

    }

    private void Upload_project(int index)
    {
        app.carrot.show_loading();
        string s_data = PlayerPrefs.GetString("js_data_" + index);
        IDictionary data = (IDictionary)Json.Deserialize(s_data);
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

    internal void Update_project_curent()
    {
        IDictionary data = (IDictionary)Json.Deserialize(PlayerPrefs.GetString("js_data_" + this.sel_project_index));
        data["code"] = app.json_editor.Get_data_cur();
        data["date"] = DateTime.Now.ToString();
        PlayerPrefs.SetString("js_data_" + this.sel_project_index, Json.Serialize(data));
    }

    private void Show_edit_info(IDictionary data)
    {
        this.box_menu = app.carrot.Create_Box();
        this.box_menu.set_icon(app.carrot.user.icon_user_edit);
        this.box_menu.set_title("Edit project information");

        Carrot_Box_Item item_title = this.box_menu.create_item();
        item_title.set_title("Title");
        item_title.set_tip("Your project name");
        item_title.set_icon(this.app.sp_icon_project);
        item_title.set_type(Box_Item_Type.box_value_input);
        item_title.check_type();
        item_title.set_val(data["title"].ToString());

        Carrot_Box_Item item_describe = this.box_menu.create_item();
        item_describe.set_title("Describe");
        item_describe.set_tip("Describe your project");
        item_describe.set_icon(this.app.carrot.icon_carrot_database);
        item_describe.set_type(Box_Item_Type.box_value_input);
        item_describe.check_type();
        if (data["describe"] != null) item_describe.set_val(data["describe"].ToString());

        Carrot_Box_Btn_Panel panel_btn = this.box_menu.create_panel_btn();
        Carrot_Button_Item btn_done = panel_btn.create_btn("btn_done");
        btn_done.set_icon_white(app.carrot.icon_carrot_done);
        btn_done.set_bk_color(app.carrot.color_highlight);
        btn_done.set_label_color(Color.white);
        btn_done.set_act_click(() => Act_update_info_project(item_title.get_val(), item_describe.get_val(), data));

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_done");
        btn_cancel.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_cancel.set_bk_color(app.carrot.color_highlight);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_act_click(() => Act_close_box_edit_info());
    }

    private void Act_update_info_project(string s_title, string s_description, IDictionary data)
    {
        data["title"] = s_title;
        data["describe"] = s_description;
        data["date"] = DateTime.Now.ToString();
        PlayerPrefs.SetString("js_data_" + data["index"].ToString(), Json.Serialize(data));
        app.carrot.show_msg("Json Editor", "Update infomation success!");
        if (this.box_menu != null) this.box_menu.close();
    }

    private void Act_close_box_edit_info()
    {
        app.carrot.play_sound_click();
        if (this.box_menu != null) this.box_menu.close();
    }

    private void Check_Permissions()
    {
        if (FileBrowser.AskPermissions == false)
        {
            FileBrowser.RequestPermission();
            app.carrot.show_msg("Json Editor", "You need to give the application permission to read and write json data files on the drive!", Msg_Icon.Alert);
        }
    }
}
