using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextUtil
{
    // --- bold tag
    public static string OpenBoldTag() { return "<b>"; }
    public static string CloseBoldTag() { return "</b>"; }
    public static string AddBoldTag(string _val) { return OpenBoldTag() + _val + CloseBoldTag(); }

    // --- color tag
    public static string OpenColorTag(Color _color) { return "<color=#" + ColorUtility.ToHtmlStringRGBA(_color) + ">"; }
    public static string CloseColorTag() { return "</color>"; }
    public static string AddColorTag(Color _color, string _val) { return OpenColorTag(_color) + _val + CloseColorTag(); }

    // --- size tag
    public static string OpenSizeTag(int _size) { return "<size=" + _size + ">"; }
    public static string CloseSizeTag() { return "</size>"; }
    public static string AddSizeTag(int _size, string _val) { return OpenSizeTag(_size) + _val + CloseSizeTag(); }

    // bold + color tag
    public static string AddBoldColorTag(Color _color, string _val)
    {
        string result = AddBoldTag(_val);
        if (_color != Color.white)
            result = AddColorTag(_color, _val);

        return result;
    }

    // escape character
    public static string ReplaceEscapeCharacter(string _val)
    {
        string val = _val;
        val = val.Replace("\\n", "\n");
        val = val.Replace("\\t", "\t");

        return val;
    }
}
