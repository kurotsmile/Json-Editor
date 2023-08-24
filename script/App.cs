using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public Carrot.Carrot carrot;
    public GameObject prefab_obj_js;
    public GameObject Panel_edit_Properties;
    public GameObject Panel_setting;
    public GameObject Panel_help;
    public Color_Board Panel_select_color;
    public Transform area_all_item_editor;
    public ScrollRect ScrollRect_all_item_editor;
    public InputField inp_coder_viewer;
    public Text txt_username_login;

    public float space_x_item = 25f;
    private List<GameObject> list_item_obj = new List<GameObject>();
    private int index_sel_mode = 0;
    public GameObject[] panel_model;
    public Image[] img_btn_model;
    private bool is_change_coderviewer;
    private bool is_change_editor=false;

    [Header("Save Project")]
    public GameObject Panel_save;
    public GameObject[] Panel_save_item;
    public GameObject obj_icon_save_status_new;
    public InputField inp_save_project_name;
    public Text txt_save_status;
    public int sel_project_index = -1;
    private int sel_type_save = 0;

    [Header("Edit Properties")]
    public Color32 color_properties_nomal;
    public Color32 color_properties_select;
    public InputField inp_edit_Properties_name;
    public InputField inp_edit_Properties_value;
    public int index_edit_Properties_type;
    public Image[] img_btn_Properties;
    public Image[] img_btn_Properties_bool;
    private js_object Js_object_edit_temp;
    public GameObject panel_edit_Properties_value;
    public GameObject panel_edit_Properties_color;
    public GameObject panel_edit_Properties_bool;

    [Header("Sound")]
    public AudioSource[] sound;
    private bool is_sound = true;

    [Header("Setting")]
    public GameObject panel_setting_removeads;
    public GameObject btn_main_removeads;
    public Image img_icon_setting_sound;
    public Sprite sp_sound_on;
    public Sprite sp_sound_off;

    [Header("Ads")]
    public string id_ads_unity;
    public string Vungle_ads_appID;
    public string Vungle_ads_placementID;
    private bool is_ads = false;
    private int count_ads = 0;

    [Header("Help")]
    public Sprite[] sp_help;
    public Image img_show_help;
    private int sel_index_help;

    [Header("Import")]
    public GameObject panel_import;
    public InputField inp_import_url;

    private void Start()
    {
        this.carrot.Load_Carrot(this.check_exit_app);
        this.carrot.shop.onCarrotPaySuccess = on_success_carrot_pay;
        this.carrot.shop.onCarrotRestoreSuccess=on_success_carrot_restore;
        this.carrot.act_after_close_all_box = this.check_data_login;

        this.Panel_edit_Properties.SetActive(false);
        this.Panel_save.SetActive(false);
        this.Panel_setting.SetActive(false);
        this.Panel_help.SetActive(false);
        this.panel_import.SetActive(false);

        this.Panel_select_color.gameObject.SetActive(false);
        this.obj_icon_save_status_new.SetActive(false);
        this.carrot.clear_contain(this.area_all_item_editor);
        this.add_obj(prefab_obj_js.GetComponent<js_object>(), "root");
        this.check_mode();
        this.GetComponent<Manager_Project>().load_project();

        if (PlayerPrefs.GetInt("is_sound", 0) == 0) this.is_sound = true; else this.is_sound = false;
        this.check_sound_status();

        if (PlayerPrefs.GetInt("is_ads", 0) == 0)
        {
#if UNITY_WSA_10_0 || UNITY_WINRT_8_1 || UNITY_METRO
            Vungle.init(Vungle_ads_appID);
            Vungle.setSoundEnabled(this.is_sound);
#else
            Advertisement.Initialize(this.id_ads_unity, false);
#endif

            this.is_ads = true;
            Advertisement.Initialize(this.id_ads_unity, false);
            this.panel_setting_removeads.SetActive(true);
            this.btn_main_removeads.SetActive(true);
        }
        else
        {
            this.panel_setting_removeads.SetActive(false);
            this.btn_main_removeads.SetActive(false);
            this.is_ads = false;
        }

        this.check_data_login();
    }

    public void check_exit_app()
    {
        if (this.Panel_edit_Properties.activeInHierarchy)
        {
            this.close_Properties();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.Panel_help.activeInHierarchy)
        {
            this.close_help();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.Panel_setting.activeInHierarchy)
        {
            this.close_setting();
            this.carrot.set_no_check_exit_app();
        }
        else if(this.Panel_save.activeInHierarchy)
        {
            this.close_save();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.panel_import.activeInHierarchy)
            {
                this.btn_close_import();
                this.carrot.set_no_check_exit_app();
            }
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
            this.obj_icon_save_status_new.SetActive(true);
            this.play_sound();
        }
        this.check_and_show_ads();
    }

    public void new_project()
    {
        this.clear_list_item_editor();
        this.txt_save_status.text = PlayerPrefs.GetString("new_file","New File");
        this.sel_project_index = -1;
        this.GetComponent<Manager_Project>().set_new_project();
        this.check_and_show_ads();
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

    public void update_option_list_obj()
    {
        for(int i = 0; i < this.list_item_obj.Count; i++)
        {
            this.list_item_obj[i].GetComponent<js_object>().index_list = i;
        }
    }

    public void btn_sel_mode(int index_mode)
    {
        this.index_sel_mode = index_mode;
        this.check_mode();
        if (this.index_sel_mode == 0) this.show_editor_by_coder();
        if (this.index_sel_mode == 1) this.show_code_json(false);
        if (this.index_sel_mode == 2) this.show_code_json(true);
        this.play_sound();
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

    public void block_all_btn_edit(bool is_show)
    {
        for (int i = 0; i < this.list_item_obj.Count; i++) if (this.list_item_obj[i] != null) this.list_item_obj[i].GetComponent<js_object>().obj_menu_btn.SetActive(is_show);
    }

    public void close_Properties()
    {
        this.play_sound();
        this.Panel_edit_Properties.SetActive(false);
    }

    public void btn_done_Properties()
    {
        this.play_sound();
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

    public void btn_sel_type_Properties(int index_sel)
    {
        this.play_sound();
        this.index_edit_Properties_type = index_sel;
        this.check_type_Properties();
    }

    public void btn_sel_val_bool_properties(bool is_true)
    {
        if (is_true)
            this.inp_edit_Properties_value.text = "true";
        else
            this.inp_edit_Properties_value.text = "false";

        this.check_val_bool_properties();
        this.play_sound();
    }

    private void check_val_bool_properties()
    {
        this.img_btn_Properties_bool[0].color = this.color_properties_nomal;
        this.img_btn_Properties_bool[1].color = this.color_properties_nomal;
        if(this.inp_edit_Properties_value.text.ToLower()=="true")
            this.img_btn_Properties_bool[0].color = this.color_properties_select;
        else
            this.img_btn_Properties_bool[1].color = this.color_properties_select;

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
            this.inp_edit_Properties_value.text =obj_number.ToString();
            
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

    public void show_project()
    {
        this.play_sound();
        this.GetComponent<Manager_Project>().show_list();
        this.check_and_show_ads();
    }

    public void show_save()
    {
        this.play_sound();
        if (this.sel_project_index != -1)
        {
            if (this.is_change_editor) this.GetComponent<Manager_Project>().update_project(this.sel_project_index, this.get_root().get_result());
            this.obj_icon_save_status_new.SetActive(false);
            this.is_change_editor = false;
        }
        else
        {
            this.sel_type_save = 0;
            this.check_show_save();
        }
        this.check_and_show_ads();
    }

    public void show_save_as()
    {
        this.play_sound();
        this.inp_save_project_name.text = "";
        this.sel_type_save = 1;
        this.check_show_save();
    }

    private void check_show_save()
    {
        this.Panel_save_item[0].SetActive(false);
        this.Panel_save_item[1].SetActive(false);
        this.Panel_save_item[this.sel_type_save].SetActive(true);
        this.Panel_save.SetActive(true);
    }

    public void btn_done_save()
    {
        this.play_sound();
        if (this.inp_save_project_name.text.Trim() == "")
        {
            this.carrot.show_msg(PlayerPrefs.GetString("project_name_error","Project name cannot be empty!"));
            return;
        }
        this.carrot.show_msg(PlayerPrefs.GetString("save_success","Save Project Success!!!"));
        this.GetComponent<Manager_Project>().add_project(this.inp_save_project_name.text, this.list_item_obj[0].GetComponent<js_object>().get_result());
        if (this.sel_type_save == 1){
            if(this.carrot.user.get_id_user_login()=="") this.GetComponent<Manager_Project>().show_project_last();
        }
        this.Panel_save.SetActive(false);
    }

    public void close_save()
    {
        this.play_sound();
        this.Panel_save.SetActive(false);
    }

    public void buy_product(int index_p)
    {
        this.play_sound();
        this.carrot.shop.buy_product(index_p);
    }

    public void clear_list_item_editor(){
        this.is_change_coderviewer = false;
        this.list_item_obj = new List<GameObject>();
        this.carrot.clear_contain(this.area_all_item_editor);
        this.add_obj(prefab_obj_js.GetComponent<js_object>(), "root");
    }

    public js_object get_root()
    {
        return this.list_item_obj[0].GetComponent<js_object>();
    }

    public void add_obj_list_main(GameObject obj_new_js)
    {
        this.list_item_obj.Add(obj_new_js);
    }

    public void check_change_coder_viewer()
    {
        this.is_change_coderviewer = true;
        this.set_save_status_new();
    }

    public void show_editor_by_coder()
    {
        if (!this.is_change_coderviewer) return;
        this.clear_list_item_editor();
        IDictionary<string, object> thanh = (IDictionary<string, object>)Carrot.Json.Deserialize(this.inp_coder_viewer.text);
        this.GetComponent<Manager_Project>().paser_obj(thanh, this.get_root());
        this.update_option_list();
    }

    public void app_rate()
    {
        this.play_sound();
        this.carrot.show_rate();
    }

    public void show_setting()
    {
        this.play_sound();
        this.Panel_setting.SetActive(true);
    }

    public void close_setting()
    {
        this.play_sound();
        this.Panel_setting.SetActive(false);
    }

    public void close_color_select()
    {
        this.play_sound();
        this.Panel_select_color.close();
    }

    public void btn_in_app_restore()
    {
        this.play_sound();
        this.carrot.shop.restore_product();
    }

    public void btn_delete_all_data()
    {
        this.play_sound();
        this.carrot.delete_all_data();
        this.carrot.delay_function(1f,this.Start);
        this.list_item_obj = new List<GameObject>();
    }

    public void btn_share_app()
    {
        this.play_sound();
        this.carrot.show_share();
    }

    public void set_save_status_default()
    {
        this.txt_save_status.text = "New File";
        this.obj_icon_save_status_new.SetActive(true);
        this.sel_project_index = -1;
    }

    public void set_save_status_new()
    {
        this.obj_icon_save_status_new.SetActive(true);
        this.is_change_editor = true;
    }

    public void play_sound(int index_sound=0)
    {
        if(this.is_sound) this.sound[index_sound].Play();
    }

    public void btn_change_status_sound_setting()
    {
        if (this.is_sound)
        {
            this.is_sound = false;
            PlayerPrefs.SetInt("is_sound", 1);
        }
        else
        {
            this.is_sound = true;
            PlayerPrefs.SetInt("is_sound", 0);
            this.play_sound();
        }
        this.check_sound_status();
    }

    private void check_sound_status()
    {
        if (this.is_sound)
            this.img_icon_setting_sound.sprite = this.sp_sound_on;
        else
            this.img_icon_setting_sound.sprite = this.sp_sound_off;
    }

    public void check_and_show_ads()
    {
        if (this.is_ads)
        {
            this.count_ads++;
            if (this.count_ads > 16)
            {
#if UNITY_WSA_10_0 || UNITY_WINRT_8_1 || UNITY_METRO
                Vungle.playAd(this.Vungle_ads_placementID);
#else
                Advertisement.Show("Interstitial_Android");
#endif 
                this.count_ads=0;
            }
        }
    }

    public void show_select_color()
    {
        this.play_sound();
        this.Panel_select_color.show_table();
    }

    public int get_index_sel_mode()
    {
        return this.index_sel_mode;
    }

    public void on_success_carrot_pay(string s_id)
    {
        if (s_id == this.carrot.shop.get_id_by_index(0))
        {
            this.carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("remove_ads_success","Remove ads successfully!"));
            this.in_app_removeAds();
        }
    }

    public void on_success_carrot_restore(string[] arr_id)
    {
        for (int i = 0; i < arr_id.Length; i++)
        {
            string s_id_p = arr_id[i];
            if (s_id_p == this.carrot.shop.get_id_by_index(0)) this.in_app_removeAds();
        }
    }

    private void in_app_removeAds()
    {
        this.is_ads = false;
        PlayerPrefs.SetInt("is_ads", 1);
        this.panel_setting_removeads.SetActive(false);
        this.btn_main_removeads.SetActive(false);
    }

    public void buy_success(Product product)
    {
        this.on_success_carrot_pay(product.definition.id);
    }

    public void show_help()
    {
        this.Panel_help.SetActive(true);
        this.play_sound();
    }

    public void close_help()
    {
        this.Panel_help.SetActive(false);
        this.play_sound();
    }

    public void btn_help_next()
    {
        this.sel_index_help++;
        if (this.sel_index_help >= this.sp_help.Length) this.sel_index_help = 0;
        this.img_show_help.sprite = this.sp_help[this.sel_index_help];
        this.play_sound();
    }
    
    public void btn_help_prev()
    {
        this.sel_index_help--;
        if (this.sel_index_help <0) this.sel_index_help = this.sp_help.Length-1;
        this.img_show_help.sprite = this.sp_help[this.sel_index_help];
        this.play_sound();
    }

    public void btn_show_more_app()
    {
        this.carrot.show_list_carrot_app();
        this.play_sound();
    }

    public void btn_show_login()
    {
        this.carrot.show_login();
        this.play_sound();
    }

    public void act_after_login()
    {
        this.check_data_login();
    }

    private void check_data_login()
    {
        if (this.carrot.user.get_id_user_login() != "")
        {
            this.txt_username_login.text = this.carrot.user.get_data_user_login("name");
            this.carrot.img_btn_login.color = Color.white;
        }
        else
        {
            this.txt_username_login.text = PlayerPrefs.GetString("login","Login");
            this.carrot.img_btn_login.color = Color.black;
        }
    }

    public void btn_show_select_lang()
    {
        this.carrot.show_list_lang(act_show_select_lang);
    }

    private void act_show_select_lang(string s_data)
    {
        if(this.sel_project_index==-1) this.txt_save_status.text = PlayerPrefs.GetString("new_file", "New File");
        if(this.list_item_obj[0]!=null) this.list_item_obj[0].GetComponent<js_object>().txt_tip.text= PlayerPrefs.GetString("json_root", "Json Root Object");
    }

    public void btn_show_import()
    {
        this.panel_import.SetActive(true);
        this.play_sound(0);
    }

    public void btn_close_import()
    {
        this.panel_import.SetActive(false);
        this.play_sound(0);
    }

    public void btn_act_import_url()
    {
        string s_inp_url = this.inp_import_url.text;
        if (s_inp_url.Trim() == "") this.carrot.show_msg(PlayerPrefs.GetString("import","Import"),PlayerPrefs.GetString("import_url_error", "Input url cannot be empty"),Carrot.Msg_Icon.Error);
        this.GetComponent<Manager_Project>().import_json_url(s_inp_url);
    }
}
