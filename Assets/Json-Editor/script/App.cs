using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Json_Editor json_editor;
    public Json_Properties json_properties;
    public Manager_Project manager_Project;
    public Carrot_File file;
    public Help help;
    public IronSourceAds ads;

    [Header("Icon")]
    public Sprite sp_icon_save;
    public Sprite sp_icon_save_as;
    public Sprite sp_icon_project;
    public Sprite sp_icon_import_web;
    public Sprite sp_icon_export_file;
    public Sprite sp_icon_import_file;

    [Header("Obj Json")]

    public float space_x_item = 25f;
    private int index_sel_mode = 0;
    private bool is_change_editor=false;

    [Header("Save Project")]
    public GameObject obj_icon_save_status_new;
    public Text txt_save_status;


    [Header("Sound")]
    public AudioClip soundclip_click;
    public AudioSource soundBackground;

    private string link_deep_app = "";

    private void Start()
    {
        this.link_deep_app = Application.absoluteURL;
        Application.deepLinkActivated += onDeepLinkActivated;

        this.carrot.Load_Carrot(this.Check_exit_app);
        this.carrot.change_sound_click(this.soundclip_click);
        this.carrot.game.load_bk_music(this.soundBackground);
        this.ads.On_Load();
        this.carrot.act_after_close_all_box = this.Check_data_login;
        this.carrot.game.act_click_watch_ads_in_music_bk=this.ads.ShowRewardedVideo;
        this.ads.onRewardedSuccess=this.carrot.game.OnRewardedSuccess;
        this.carrot.shop.onCarrotPaySuccess += onCarrotPaySuccess;
        this.carrot.shop.onCarrotRestoreSuccess += onCarrotRestoreSuccess;
        
        this.obj_icon_save_status_new.SetActive(false);

        this.json_editor.On_load();
        this.manager_Project.On_load();

        this.Check_data_login();
        if(this.carrot.os_app==OS.Window){
            this.file.type=Carrot_File_Type.StandaloneFileBrowser;
        }else{
            this.file.type=Carrot_File_Type.SimpleFileBrowser;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) this.carrot.delay_function(3f, this.check_link_deep_app);
    }

    private void onDeepLinkActivated(string url)
    {
        this.link_deep_app = url;
        if (this.carrot != null) this.carrot.delay_function(1f, this.check_link_deep_app);
    }

    public void check_link_deep_app()
    {
        if (this.link_deep_app.Trim() != "")
        {
            if (this.carrot.is_online())
            {
                if (this.link_deep_app.Contains("codejson:"))
                {
                    string id_project = this.link_deep_app.Replace("codejson://show/", "");
                    Debug.Log("Get Project id:" + id_project);
                    manager_Project.Show_project_by_id(id_project);
                    this.link_deep_app = "";
                }
            }
        }
    }

    [ContextMenu("Test Deep Link")]
    public void test_deep_link()
    {
        this.onDeepLinkActivated("codejson://show/code-id1687455026204");
    }

    private void Check_exit_app()
    {
        
    }

    public void new_project()
    {
        carrot.play_sound_click();
        this.json_editor.Clear_list_item_editor();
        this.json_editor.Add_node();
        this.txt_save_status.text = carrot.lang.Val("new_file","New File");
        this.manager_Project.Set_new_project();
        this.ads.show_ads_Interstitial();
    }

    public void show_project()
    {
        this.carrot.play_sound_click();
        this.manager_Project.Show_list_project();
        this.ads.show_ads_Interstitial();
    }

    public void Btn_save_project()
    {
        carrot.play_sound_click();
        if (this.manager_Project.Get_Index_project_curent() != -1)
        {
            if (this.is_change_editor) this.manager_Project.Update_project_curent();
            this.obj_icon_save_status_new.SetActive(false);
            this.is_change_editor = false;
        }
        else
        {
            manager_Project.Show_save_project(false);
        }
        this.ads.show_ads_Interstitial();
    }

    public void Btn_save_as_project()
    {
        carrot.play_sound_click();
        manager_Project.Show_save_project(true);
    }

    public void Btn_show_import_project()
    {
        carrot.play_sound_click();
        manager_Project.Show_Import();
    }

    public void Btn_show_help()
    {
        carrot.play_sound_click();
        help.Show();
    }

    public void buy_product(int index_p)
    {
        this.carrot.play_sound_click();
        this.carrot.shop.buy_product(index_p);
    }

    public void check_change_coder_viewer()
    {
        this.Set_save_status_new();
        this.json_editor.Change_coder_in_view();
    }

    public void app_rate()
    {
        this.carrot.show_rate();
    }

    public void Btn_show_setting()
    {
        this.carrot.Create_Setting();
    }

    public void btn_delete_all_data()
    {
        this.carrot.Delete_all_data();
        this.carrot.delay_function(1f,this.Start);
    }

    public void btn_share_app()
    {
        this.carrot.show_share();
    }

    public void Set_save_status_default()
    {
        this.txt_save_status.text = carrot.lang.Val("new_file", "New File");
        this.obj_icon_save_status_new.SetActive(true);
    }

    public void Set_save_status_new()
    {
        this.obj_icon_save_status_new.SetActive(true);
        this.is_change_editor = true;
    }

    public int get_index_sel_mode()
    {
        return this.index_sel_mode;
    }

    public void Btn_show_login()
    {
        this.carrot.user.show_login(()=> Check_data_login());
        this.carrot.play_sound_click();
    }

    private void Check_data_login()
    {
        if (this.carrot.user.get_id_user_login() != "")
        {
            this.json_editor.txt_username_login.text = this.carrot.user.get_data_user_login("name");
            this.carrot.img_btn_login.color = Color.white;
        }
        else
        {
            this.json_editor.txt_username_login.text = carrot.L("login","Login");
            this.carrot.img_btn_login.color = Color.black;
        }
    }

    public void Btn_show_select_lang()
    {
        this.carrot.Show_list_lang(Act_show_select_lang);
    }

    private void Act_show_select_lang(string s_data)
    {
        if(this.get_index_sel_mode()==-1) this.txt_save_status.text = carrot.L("new_file", "New File");
    }
    private void onCarrotPaySuccess(string id_product)
    {
        if (id_product == this.carrot.shop.get_id_by_index(0))
        {
            this.carrot.Show_msg("Remove Ads", "Remove Ads Success!", Msg_Icon.Success);
            this.ads.RemoveAds();
        }
    }

    private void onCarrotRestoreSuccess(string[] array_id)
    {
        foreach (string id_product in array_id)
        {
            if (id_product == this.carrot.shop.get_id_by_index(0))
            {
                this.carrot.Show_msg("Remove Ads", "Remove Ads Success!", Msg_Icon.Success);
                this.ads.RemoveAds();
            }
        }
    }
}
