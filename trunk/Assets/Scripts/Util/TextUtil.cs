using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextUtil
{
    // bold tag
    public static string OpenBoldTag() { return "<b>"; }
    public static string CloseBoldTag() { return "</b>"; }
    public static string AddBoldTag(string _val)
    {
        return OpenBoldTag() + _val + CloseBoldTag();
    }

    // color tag
    public static string OpenColorTag(Color _color)
    {
        string val = "<color=#" + ColorUtility.ToHtmlStringRGBA(_color) + ">";
        //switch (_color)
        //{
        //    case ColorMenu.ColorType.WHITE:
        //        break;
        //    case ColorMenu.ColorType.BLACK:
        //    case ColorMenu.ColorType.RED:
        //    case ColorMenu.ColorType.CYAN:
        //    case ColorMenu.ColorType.GREEN:
        //    case ColorMenu.ColorType.BLUE:
        //    case ColorMenu.ColorType.ORANGE:
        //    case ColorMenu.ColorType.PURPLE:
        //        val += _color.ToString().ToLower();
        //        break;
        //    default:
        //        break;
        //}

        //val += ">";

        return val;
    }
    public static string CloseColorTag() { return "</color>"; }
    public static string AddColorTag(Color _color, string _val)
    {
        return OpenColorTag(_color) + _val + CloseColorTag();
    }

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
