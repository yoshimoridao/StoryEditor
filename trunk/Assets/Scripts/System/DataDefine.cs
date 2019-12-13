using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefine
{
    public static string pref_path_Panel                = "Prefabs/panel";
    public static string pref_path_originPanel          = "Prefabs/panel_origin";
    public static string pref_path_rowLabel             = "Prefabs/row_label";
    public static string pref_path_label                = "Prefabs/label";
    public static string pref_path_inputLabel           = "Prefabs/label_input";
    public static string pref_path_linkLabel            = "Prefabs/label_link";
    public static string pref_path_highlight_panel      = "Prefabs/highlight_panel";

    public static string defaultLabelVar = "---";

    public static char save_parse_story_seperateCompMark    = '@';

    public static string save_path_storyData = "Assets/Resources/Data/story_data.txt";
    public static string save_path_indexData = "Assets/Resources/Data/index_data.txt";
    public static string save_key_storyData = "save_story_data";
    public static string save_key_indexData = "save_index_data";
    
    public static string tag_board_element      = "rootpanel_attribute";
    public static string tag_board_story        = "rootpanel_sentence";
    public static string tag_board_result       = "rootpanel_result";
    public static string tag_colorbar           = "color_bar";
    public static string tag_colorbar_btn       = "color_bar_btn";
    public static string tag_panel_common       = "panel_common";
    public static string tag_panel_origin       = "panel_origin";
}
