using UnityEngine;
using UnityEngine.UI;

public class Color_Board : MonoBehaviour
{
    public InputField inp_show_result_color;
    public InputField inp_show_properties_color;
    public Image img_show_result_color;
    public Transform[] color_table_col;
    public Color32[] color_col_default;
    public InputField inp_show_color;
    private Color32[] color_col_show = new Color32[11];

    public App app;
    public GameObject Item_color_pick_prefab;

    private byte step_color_r = 0;
    private byte step_color_g = 0;
    private byte step_color_b = 0;

    private int index_sel_model;

    public Image[] img_btn_sel;

    public void show_table()
    {
        this.gameObject.SetActive(true);
        this.index_sel_model = 0;
        this.check_model();
    }

    private void create_color()
    {

        if (this.index_sel_model == 0)
        {
            this.color_col_show = new Color32[this.color_col_default.Length];
            for (int i = 0; i < this.color_col_default.Length; i++) this.color_col_show[i] = this.color_col_default[i];
        }

        if (this.index_sel_model == 1)
        {
            this.color_col_show = new Color32[this.color_col_default.Length];
            for (int i = 0; i < this.color_col_show.Length; i++) this.color_col_show[i] = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }

        for (int i = 0; i < this.color_table_col.Length; i++)
        {
            byte c_r = 0;
            byte c_g = 0;
            byte c_b = 0;

            if (this.index_sel_model == 0 || this.index_sel_model == 1)
            {
                this.step_color_r = (byte)(this.color_col_show[i].r / 6);
                this.step_color_g = (byte)(this.color_col_show[i].g / 6);
                this.step_color_b = (byte)(this.color_col_show[i].b / 6);
            }

            this.app.carrot.clear_contain(this.color_table_col[i]);
            for (int y = 0; y < 6; y++)
            {
                if (this.index_sel_model == 0|| this.index_sel_model ==1)
                {
                    if (this.color_col_show[i].r > 0) c_r = (byte)(this.color_col_show[i].r - (this.step_color_r * y));
                    if (this.color_col_show[i].g > 0) c_g = (byte)(this.color_col_show[i].g - (this.step_color_g * y));
                    if (this.color_col_show[i].b > 0) c_b = (byte)(this.color_col_show[i].b - (this.step_color_b * y));
                }

                if (this.index_sel_model ==2)
                {
                    c_r = (byte)Random.Range(0, 255);
                    c_g = (byte)Random.Range(0, 255);
                    c_b = (byte)Random.Range(0, 255);
                }

                GameObject item_color_pick = Instantiate(this.Item_color_pick_prefab);
                item_color_pick.transform.SetParent(this.color_table_col[i]);
                item_color_pick.transform.localScale = new Vector3(1f, 1f, 1f);
                item_color_pick.GetComponent<Image>().color = new Color32(c_r, c_g, c_b, 255);
            }
        }
    }

    public void close()
    {
        this.gameObject.SetActive(false);
    }

    public void btn_select_model_color(int index_mode)
    {
        this.app.carrot.play_sound_click();
        this.index_sel_model = index_mode;
        this.check_model();
    }

    private void check_model()
    {
        this.img_btn_sel[0].color = this.app.json_editor.color_properties_nomal;
        this.img_btn_sel[1].color = this.app.json_editor.color_properties_nomal;
        this.img_btn_sel[2].color = this.app.json_editor.color_properties_nomal;
        this.img_btn_sel[this.index_sel_model].color = this.app.json_editor.color_properties_select;
        this.create_color();
    }

    public void set_color(Color32 c)
    {
        this.inp_show_color.text=string.Format("#{0:X2}{1:X2}{2:X2}",c.r, c.g,c.b);
        this.inp_show_properties_color.text= string.Format("#{0:X2}{1:X2}{2:X2}", c.r, c.g, c.b);
        this.img_show_result_color.color = c;
        this.app.carrot.play_sound_click();
        this.gameObject.SetActive(false);
    }

    public void set_color_for_edit_properties(string s_color)
    {
        if (s_color == "0"||s_color.ToLower()=="null") s_color = "#000000";
        if (s_color.ToLower()=="true") s_color = "#000000";
        if (s_color.ToLower()=="false") s_color = "#ffffff";
        if (s_color.Length < 6) s_color = s_color + "ffffff";
        this.inp_show_color.text = s_color;
        this.inp_show_properties_color.text = s_color;
        try
        {
            this.img_show_result_color.color = hexToColor(s_color);
        }
        catch
        {
            this.set_color_for_edit_properties("#ffffff");
        }
    }

    public static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");
        byte a = 255;
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
}
