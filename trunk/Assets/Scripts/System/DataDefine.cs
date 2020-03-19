using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefine
{
    // prefabs path
    public static string pref_path_storyPanel           = "Prefabs/Panel/panel_story";
    public static string pref_path_elementPanel         = "Prefabs/Panel/panel_element";

    public static string pref_path_story_label          = "Prefabs/Label/story_label";
    public static string pref_path_element_label        = "Prefabs/Label/element_label";
    public static string pref_path_rowLabel             = "Prefabs/row_label";
    public static string pref_path_result_row           = "Prefabs/result_row";
    public static string pref_path_element_space        = "Prefabs/element_space";
    public static string pref_path_panel_space          = "Prefabs/panel_space";

    public static string pref_tag_editor_field          = "Prefabs/TagEditors/TagEditorField";

    // save
    public static string save_filename_default          = "DemoText";
    public static string save_filename_suffix_tracery   = "_tracery";
    public static string save_filename_suffix_game      = "_game";
    public static string save_key_last_path             = "pref_last_path";
    public static string save_key_last_load_file        = "pref_last_save_file";

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
    public static string tag_colormenu          = "color_menu";
    public static string tag_colormenu_topbar   = "color_menu_top_bar";
    public static string tag_window_event_tag   = "window_event_tag";

    public static string tag_tageditor_workspace_view = "tageditor_workspace_view";

    // text
    public static string defaultLabelVar = "---";
    public static string default_name_element_panel = "Element";
    public static string default_name_story_panel = "Story";
    public static string default_event_tag_value = "new event tag";
    public static string error_refer_text = "#error#";
    public static string default_new_group_tag = "new group";
    public static string default_new_flow_tag = "new flow";

    // notice text
    public static string notice_load_done = "Load Success";
    public static string notice_save_done = "Save Success";

    // popup content
    public static string popup_title_save = "Save";
    public static string popup_content_save = "Do you want to save your change ?";

    // highlight color
    public static Color highlight_drop_zone_color = new Color(33 / 255.0f, 40 / 255.0f, 144 / 255.0f, 1.0f);
    public static Color highlight_drag_obj_color = new Color(214 / 255.0f, 219 / 255.0f, 32 / 255.0f, 1.0f);
    public static Color highlight_select_obj_color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f, 130 / 255.0f);
}
