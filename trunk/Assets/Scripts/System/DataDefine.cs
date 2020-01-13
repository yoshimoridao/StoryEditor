using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefine
{
    // prefabs
    public static string pref_path_storyPanel           = "Prefabs/Panel/panel_story";
    public static string pref_path_elementPanel         = "Prefabs/Panel/panel_element";

    public static string pref_path_story_label          = "Prefabs/Label/story_label";
    public static string pref_path_element_label        = "Prefabs/Label/element_label";
    public static string pref_path_rowLabel             = "Prefabs/row_label";
    public static string pref_path_result_row           = "Prefabs/result_row";
    
    // save
    public static string save_filename_default          = "DemoText";
    public static string save_filename_suffix_tracery   = "_tracery";
    public static string save_key_last_path             = "pref_last_path";
    public static string save_key_last_save_file        = "pref_last_save_file";

    // tag
    public static string tag_board_element      = "rootpanel_attribute";
    public static string tag_board_story        = "rootpanel_sentence";
    public static string tag_board_result       = "rootpanel_result";
    public static string tag_colorbar           = "color_bar";
    public static string tag_colorbar_btn       = "color_bar_btn";
    public static string tag_panel_common       = "panel_common";
    public static string tag_panel_origin       = "panel_origin";
    public static string tag_label_input        = "label_input";
    public static string tag_button_toolbar     = "button_toolbar";

    // text
    public static string defaultLabelVar = "---";
    public static string default_name_element_panel = "Element";
    public static string default_name_story_panel = "Story";
    public static string error_refer_text = "#error#";

    // notice text
    public static string notice_load_done = "Load Done";
    public static string notice_save_done = "Save Done";

    // popup content
    public static string popup_title_save = "Save";
    public static string popup_content_save = "Do you want to save your change ?";
}
