using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextUtil
{
    public static string GetOpenColorTag(ColorBar.ColorType colorType)
    {
        string val = "<color=";
        switch (colorType)
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
                val += colorType.ToString().ToLower();
                break;
            default:
                break;
        }

        val += ">";

        return val;
    }

    public static string GetCloseColorTag()
    {
        return "</color>";
    }
}
