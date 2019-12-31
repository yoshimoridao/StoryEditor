using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextUtil
{
    public static string OpenBoldTag() { return "<b>"; }
    public static string CloseBoldTag() { return "</b>"; }
    public static string AddBoldTag(string _val)
    {
        return OpenBoldTag() + _val + CloseBoldTag();
    }

    public static string OpenColorTag(ColorBar.ColorType _colorType)
    {
        string val = "<color=";
        switch (_colorType)
        {
            case ColorBar.ColorType.WHITE:
                break;
            case ColorBar.ColorType.BLACK:
            case ColorBar.ColorType.RED:
            case ColorBar.ColorType.CYAN:
            case ColorBar.ColorType.GREEN:
            case ColorBar.ColorType.BLUE:
            case ColorBar.ColorType.ORANGE:
            case ColorBar.ColorType.PURPLE:
                val += _colorType.ToString().ToLower();
                break;
            default:
                break;
        }

        val += ">";

        return val;
    }

    public static string CloseColorTag() { return "</color>"; }
    public static string AddColorTag(ColorBar.ColorType _colorType, string _val)
    {
        return OpenColorTag(_colorType) + _val + CloseColorTag();
    }

    public static string AddBoldColorTag(ColorBar.ColorType _colorType, string _val)
    {
        string result = AddBoldTag(_val);
        if (_colorType != ColorBar.ColorType.WHITE)
            result = AddColorTag(_colorType, _val);

        return result;
    }
}
