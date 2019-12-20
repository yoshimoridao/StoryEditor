using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefine
{
    public static string pref_path_storyPanel           = "Prefabs/Panel/panel_story";
    public static string pref_path_elementPanel         = "Prefabs/Panel/panel_element";
    public static string pref_path_originPanel          = "Prefabs/Panel/panel_origin";

    public static string pref_path_label                = "Prefabs/Label/label";
    public static string pref_path_inputLabel           = "Prefabs/Label/label_input";
    public static string pref_path_linkLabel            = "Prefabs/Label/label_link";

    public static string pref_path_rowLabel             = "Prefabs/row_label";
    public static string pref_path_result_row           = "Prefabs/result_row";

    public static string defaultLabelVar    = "---";

    public static char save_parse_story_seperateCompMark    = '@';
    
    public static string save_path_dataFolder       = "Assets/Resources/Data/";
    public static string save_path_outputFolder     = "SaveInfo/";
    public static string save_fileName_storyData    = "story_data.txt";
    public static string save_fileName_indexData    = "index_data.txt";
    // player pref
    public static string save_key_storyData     = "save_story_data";
    public static string save_key_indexData     = "save_index_data";
    
    public static string tag_board_element      = "rootpanel_attribute";
    public static string tag_board_story        = "rootpanel_sentence";
    public static string tag_board_result       = "rootpanel_result";
    public static string tag_colorbar           = "color_bar";
    public static string tag_colorbar_btn       = "color_bar_btn";
    public static string tag_panel_common       = "panel_common";
    public static string tag_panel_origin       = "panel_origin";
    public static string tag_label_input        = "label_input";
}
