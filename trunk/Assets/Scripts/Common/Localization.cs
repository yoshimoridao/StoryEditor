using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization : MonoBehaviour
{
    public enum Language
    {
        Ar, De, En, Es, Fr, He, Id, It, Ko, Pl, Pt, Ro, Ru, Th, Tr, Zh, COUNT
    }

    //    English,
    //    Arabic,
    //    French,
    //    German,
    //    Indonesian,
    //    Italian,
    //    Polish,
    //    Korean,
    //    Brazilian_Portuguese,
    //    Romanian,
    //    Russian,
    //    Simplified_Chinese,
    //    Spanish,
    //    Thai,
    //    Hebrew,
    //    Turkish

    public static string GetFullLanguageCode(Language lan)
    {
        string result = lan.ToString();
        string lanCode = "";
        switch (lan)
        {
            case Language.En:
                lanCode = "English";
                break;
            case Language.Ar:
                lanCode = "Arabic";
                break;
            case Language.Fr:
                lanCode = "French";
                break;
            case Language.De:
                lanCode = "German"; break;
            case Language.Id:
                lanCode = "Indonesian"; break;
            case Language.It:
                lanCode = "Italian"; break;
            case Language.Pl:
                lanCode = "Polish"; break;
            case Language.Ko:
                lanCode = "Korean"; break;
            case Language.Pt:
                lanCode = "Brazilian_Portuguese"; break;
            case Language.Ro:
                lanCode = "Romanian"; break;
            case Language.Ru:
                lanCode = "Russian"; break;
            case Language.Zh:
                lanCode = "Simplified_Chinese"; break;
            case Language.Es:
                lanCode = "Spanish"; break;
            case Language.Th:
                lanCode = "Thai"; break;
            case Language.He:
                lanCode = "Hebrew"; break;
            case Language.Tr:
                lanCode = "Turkish"; break;
            default:
                lanCode = "English"; break;
        }
        result += " (" + lanCode + ")";

        return result;
    }

    public static bool IsLanguageCode(string code)
    {
        for (int i = 0; i < (int)Language.COUNT; i++)
        {
            if (code == ((Language)i).ToString())
            {
                return true;
            }
        }

        return false;
    }
}
