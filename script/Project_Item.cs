using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Project_Item : MonoBehaviour
{
    public Image img_icon;
    public string s_id_online;
    public Text txt_name;
    public Text txt_date;
    public int index_project;
    public InputField input_name_edit;
    public GameObject obj_btn_open;
    public GameObject obj_btn_edit;
    public GameObject obj_btn_del;
    public GameObject obj_btn_done_edit;
    public GameObject obj_btn_cancel_edit;
    public GameObject obj_btn_upload;
    public GameObject obj_btn_export_web;
    public GameObject obj_btn_export_file;
    public GameObject obj_btn_share;

    public void click_open()
    {
        if(this.s_id_online!="")
            GameObject.Find("App").GetComponent<Manager_Project>().show_project_online(this);
        else
            GameObject.Find("App").GetComponent<Manager_Project>().show_project_offline(this);
    }

    public void delete_project()
    {
        if(this.s_id_online!="")
            GameObject.Find("App").GetComponent<Manager_Project>().delete_project_online(this.s_id_online);
        else
            GameObject.Find("App").GetComponent<Manager_Project>().delete_project(this.index_project);
    }

    public void edit_project()
    {
        this.obj_btn_open.SetActive(false);
        this.obj_btn_edit.SetActive(false);
        this.obj_btn_del.SetActive(false);
        this.obj_btn_export_file.SetActive(false);
        this.obj_btn_export_web.SetActive(false);
        this.obj_btn_share.SetActive(false);
        this.obj_btn_upload.SetActive(false);

        this.obj_btn_cancel_edit.SetActive(true);
        this.obj_btn_done_edit.SetActive(true);
        this.input_name_edit.text = this.txt_name.text;
        this.input_name_edit.gameObject.SetActive(true);
    }

    public void close_edit_project()
    {
        this.obj_btn_open.SetActive(true);
        this.obj_btn_edit.SetActive(true);
        this.obj_btn_del.SetActive(true);

        if (this.s_id_online != "") { 
            this.obj_btn_export_file.SetActive(true);
            this.obj_btn_export_web.SetActive(true);
            this.obj_btn_share.SetActive(true);
            this.obj_btn_upload.SetActive(true);
        }

        this.obj_btn_cancel_edit.SetActive(false);
        this.obj_btn_done_edit.SetActive(false);
        this.input_name_edit.gameObject.SetActive(false);
    }

    public void btn_done_edit_project()
    {
        if (this.input_name_edit.text.Trim() == "")
        {
            GameObject.Find("App").GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("project_name_error","Project name cannot be empty!"));
            return;
        }

        this.txt_name.text = this.input_name_edit.text;
        if(this.s_id_online!="")
            GameObject.Find("App").GetComponent<Manager_Project>().update_project_name_online(this.s_id_online, this.input_name_edit.text);
        else
            GameObject.Find("App").GetComponent<Manager_Project>().update_project_name(this.index_project, this.input_name_edit.text);
        this.close_edit_project();
    }

    public void btn_upload_project()
    {
        GameObject.Find("App").GetComponent<Manager_Project>().upload_project(this);
    }

    public void btn_export_web_project()
    {
        string url_export = GameObject.Find("App").GetComponent<App>().carrot.mainhost + "?p=json_export&id=" + this.s_id_online; 
        Application.OpenURL(url_export);
    }

    public void btn_share_project()
    {
        string url_share = GameObject.Find("App").GetComponent<App>().carrot.mainhost + "?p=json_view&id=" + this.s_id_online;
        GameObject.Find("App").GetComponent<App>().carrot.show_share(url_share,PlayerPrefs.GetString("share_project_tip","Share this json project with others"));
    }

    public void btn_download_file_json()
    {
        string url_download = GameObject.Find("App").GetComponent<App>().carrot.mainhost;
        url_download = url_download.Replace("app.php", "export_file.php?id=" + this.s_id_online);
        Application.OpenURL(url_download);

    }
}
