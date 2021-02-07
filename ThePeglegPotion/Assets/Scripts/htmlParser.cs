using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class htmlParser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string htmlExtract(string htmlText)
    {
        string recipeStart = "<div lang=\"en\" dir=\"ltr\" class=\"mw-content-ltr\">";
        string recipeEnd = "<!-- \nNewPP limit report";
        string AllRecipeEnd = "<span class=\"mw-headline\" id=\"";
        string similarRecipeEnd = "<span class=\"mw-headline\" id=\"Similar_";

        htmlText = customSubStr(htmlText, recipeStart, recipeEnd);
        //htmlText = extractElementContent(htmlText, AllRecipeEnd, similarRecipeEnd, "", "");
        htmlText = extractElementContent(htmlText, "<a", "</a>", "", "");
        htmlText = extractElementContent(htmlText, "<p", "</p>", "", "");
        htmlText = extractElementContent(htmlText, "<h2", "</h2>", "", "");
        htmlText = extractElementContent(htmlText, "<h3", "</h3>", "", "");
        htmlText = extractElementContent(htmlText, "<span", "</span>", "", "\n");
        htmlText = extractElementContent(htmlText, "<table", "</table>", "", "");
        htmlText = extractElementContent(htmlText, "<div", "</div>", "", "");
        htmlText = extractElementContent(htmlText, "<ul", "</ul>", "", "");
        htmlText = extractElementContent(htmlText, "<li", "</li>", " • ", "");
        htmlText = extractElementContent(htmlText, "<b", "</b>", "", "");
        htmlText = extractElementContent(htmlText, "<i", "</i>", "", "");
        htmlText = removeElement(htmlText, "&amp;", "");
        htmlText = removeElement(htmlText, "<ul>", "");
        htmlText = removeElement(htmlText, "</ul>", "");
        htmlText = removeElement(htmlText, "<li", " • ");
        htmlText = removeElement(htmlText, "</li>", "");
        htmlText = removeElement(htmlText, "<h2", "");
        htmlText = removeElement(htmlText, "</h2>", "");
        htmlText = removeElement(htmlText, "<span", "");
        htmlText = removeElement(htmlText, "</span>", "");
        htmlText = removeElement(htmlText, "<tr>", "");
        htmlText = removeElement(htmlText, "</tr>", "");
        htmlText = removeElement(htmlText, "<td>", "");
        htmlText = removeElement(htmlText, "</td>", "");
        htmlText = removeElement(htmlText, "<br/>", "");
        htmlText = removeElement(htmlText, "<br />", "");
        htmlText = removeElement(htmlText, "<hr/>", "");
        htmlText = removeElement(htmlText, "<hr />", "");
        htmlText = removeElement(htmlText, "<dl>", "");
        htmlText = removeElement(htmlText, "</dl>", "");
        htmlText = removeElement(htmlText, "<dt>", "");
        htmlText = removeElement(htmlText, "</dt>", "");
        return htmlText;
    }

    public string customSubStr(string original, string start, string end)
    {
        if (original == "")
            return "";
        int startIndex = original.IndexOf(start);
        int endIndex = original.IndexOf(end);
        string subresult = "";
        if (startIndex != -1 && endIndex != -1)
        {
            startIndex += start.Length;
            subresult = original.Substring(startIndex, endIndex - startIndex);
        }
        return subresult;
    }

    public string removeElement(string text, string elementToAvoid, string charaBefore)
    {
        string finalStr = "";
        string elmentEnd = ">";
        string nextStart = "<";

        int elemIndex = text.IndexOf(elementToAvoid);
        while (elemIndex != -1)
        {
            finalStr += text.Substring(0, elemIndex) + charaBefore;
            text = text.Substring(elemIndex + elementToAvoid.Length);
            int nextStartIndex = text.IndexOf(nextStart);
            int posEndElem = text.IndexOf(elmentEnd);
            if (posEndElem != -1 && (nextStartIndex == -1 || posEndElem < nextStartIndex))
            {
                text = text.Substring(posEndElem + elmentEnd.Length);
            }
            elemIndex = text.IndexOf(elementToAvoid);
        }
        finalStr += text;
        return finalStr;
    }


    private string extractElementContent(string text, string baliseStart, string baliseEnd, string addCharacterBefore, string addCharacterAfter)
    {
        string finalStr = "";
        string elmentEnd = ">";
        int startPos = text.IndexOf(baliseStart);
        int endPos = text.IndexOf(baliseEnd);

        while (startPos != -1 && endPos != -1 && startPos < endPos)
        {
            finalStr += text.Substring(0, startPos);
            text = text.Substring(startPos + baliseStart.Length);
            endPos = text.IndexOf(baliseEnd);
            int posEndElem = text.IndexOf(elmentEnd);
            if (posEndElem != -1 && posEndElem < endPos && endPos != -1)
            {
                finalStr += addCharacterBefore + customSubStr(text, elmentEnd, baliseEnd);
            }
            text = text.Substring(endPos + baliseEnd.Length);
            finalStr += addCharacterAfter;
            startPos = text.IndexOf(baliseStart);
            endPos = text.IndexOf(baliseEnd);
        }
        finalStr += text;
        return finalStr;
    }
}
