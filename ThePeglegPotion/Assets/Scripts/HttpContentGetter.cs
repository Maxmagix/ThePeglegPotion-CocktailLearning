using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class HttpContentGetter : MonoBehaviour
{
    private List<string> LetterCategories = new List<string>();
    public List<string> AllCocktails = new List<string>();

    [SerializeField]
    private GameObject ImageParent;
    [SerializeField]
    private GameObject ImagePrefab;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject loadingScreen;

    static string textResult = "";
    private int pageLoaded = 0;
    private bool called = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void EmptyParent()
    {
        int nbChild = ImageParent.transform.childCount;
        for (int i = nbChild - 1; i >= 0; i--)
        {
            GameObject.Destroy(ImageParent.transform.GetChild(nbChild).gameObject);
        }
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            // ImageComponent.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;

            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            //ImageComponent.GetComponent<Image>().overrideSprite = sprite;
        }
    }

    public async void createImages(string name, int number)
    {
        EmptyParent();
        string prefixSearch = "https://www.google.com/search?q=";
        string tags = "+%2B+cocktail&sxsrf=ALeKk01UHctxsQWoYrNnQQRmJeuH54zuUA:1612613995233&source=lnms&tbm=isch&sa=X&ved=2ahUKEwjOxuGin9XuAhUnE6YKHXZZAlwQ_AUoAXoECAcQAw&biw=1472&bih=1083";
        string url = prefixSearch + name + tags;
        string image_start = "<div id=\"islmp\" jsname=\"ixzLGF\"";
        string image_delimiter = "<img class=\"rg_i Q4LuWd\" src=\"data:image/jpeg;base64,";
        string srcUrl = " src=\"";
        string strEnd = "\"";
        string result = await DownloadPageAsync(url);
        int indexStart = result.IndexOf(image_start);
        if (indexStart != -1)
            result = result.Substring(indexStart + image_start.Length);
        for (int i = 0; i < number; i++)
        {
            int imageStartIndex = result.IndexOf(image_delimiter);
            if (imageStartIndex != -1)
                result = result.Substring(imageStartIndex + image_delimiter.Length);
            int startUrl = result.IndexOf(srcUrl);
            string urlContent = "";
            if (startUrl != -1)
            {
                result = result.Substring(startUrl + srcUrl.Length);
                int endUrl = result.IndexOf(strEnd);
                urlContent = result.Substring(0, endUrl);
                Debug.Log(urlContent);
            }
        }
    }

    private string cleanFromAElement(string rawText)
    {
        string lineJump = "br /";
        string elementStart = "a href";
        string beginTitle = "title=\"";
        string endTitle = "\"";
        string elementEnd = "/a";
        int startIndex = rawText.IndexOf(elementStart);
        int endIndex = rawText.IndexOf(elementEnd);
        int brIndex = rawText.IndexOf(lineJump);
        while ((startIndex != -1 && endIndex != -1) || brIndex != -1)
        {
            if (startIndex != -1 && endIndex != -1)
            {
                string beforeA = rawText.Substring(0, startIndex);
                string afterA = rawText.Substring(endIndex + elementEnd.Length);
                int beginTitleIndex = rawText.IndexOf(beginTitle);
                rawText = rawText.Substring(beginTitleIndex + beginTitle.Length);
                int endTitleIndex = rawText.IndexOf(endTitle);
                string AContent = rawText.Substring(0, endTitleIndex);
                rawText = beforeA + AContent + afterA;
            } else
            {
                string tempText = rawText.Substring(0, brIndex);
                rawText = tempText + rawText.Substring(brIndex + lineJump.Length);
            }
            startIndex = rawText.IndexOf(elementStart);
            endIndex = rawText.IndexOf(elementEnd);
            brIndex = rawText.IndexOf(lineJump);
        }
        return rawText;
    }

    public async Task<string> getCocktailDescription(string name)
    {
        string baseUrl = "https://wiki.webtender.com/wiki/";
        string recipeStart = "<div lang=\"en\" dir=\"ltr\" class=\"mw-content-ltr\">";
        string recipeEnd = "<!-- \nNewPP limit report";


        string ingredientDelimiterStart = "<li>";
        string ingredientDelimiterEnd = "</li>";

        string descriptionStart = "<p>";
        string descriptionEnd = "</p>";

        string AllRecipeEnd = "<span class=\"mw-headline\" id=\"";
        string similarRecipeEnd = "<span class=\"mw-headline\" id=\"Similar_";

        string resultPage = await DownloadPageAsync(baseUrl + name);
        Debug.Log(resultPage);
        resultPage = customSubStr(resultPage, recipeStart, recipeEnd);
        int similarRecipeIndex = resultPage.IndexOf(similarRecipeEnd);
        if (similarRecipeIndex != -1)
           resultPage = resultPage.Substring(0, similarRecipeIndex);
        Debug.Log(resultPage);
        string finalStr = name;
        int isDescription = resultPage.IndexOf(descriptionStart);
        int endDescription = resultPage.IndexOf(descriptionEnd);
        int allRecipeIndex = resultPage.IndexOf(AllRecipeEnd);

        if (isDescription < endDescription && isDescription != -1 && endDescription != -1)
            finalStr += "\nDescription:\n";
        while (isDescription < allRecipeIndex && isDescription < endDescription && isDescription != -1 && endDescription != -1)
        {
            string category = customSubStr(resultPage, descriptionStart, descriptionEnd);
            finalStr += cleanFromAElement(category);
            resultPage = resultPage.Substring(endDescription + descriptionEnd.Length);
            isDescription = resultPage.IndexOf(descriptionStart);
            endDescription = resultPage.IndexOf(descriptionEnd);
        }
        finalStr += "\n\nRecipe:\n\n";
        allRecipeIndex = resultPage.IndexOf(AllRecipeEnd);
        if (allRecipeIndex != -1)
        {
            resultPage = resultPage.Substring(allRecipeIndex + AllRecipeEnd.Length);
        }
        List<string> ingredients = new List<string>();
        while (resultPage.Contains(ingredientDelimiterStart) && resultPage.Contains(ingredientDelimiterEnd))
        {
            int startIndex = resultPage.IndexOf(ingredientDelimiterStart);
            int endIndex = resultPage.IndexOf(ingredientDelimiterEnd);
            isDescription = resultPage.IndexOf(descriptionStart);
            endDescription = resultPage.IndexOf(descriptionEnd);
            while (
                isDescription < endDescription &&
                isDescription != -1 &&
                endDescription != -1 &&
                isDescription < startIndex)
            {
                string category = customSubStr(resultPage, ingredientDelimiterStart, ingredientDelimiterEnd);
                category = cleanFromAElement(category);
                finalStr += category + "\n";
                resultPage = resultPage.Substring(endDescription + descriptionEnd.Length);
                startIndex = resultPage.IndexOf(ingredientDelimiterStart);
                endIndex = resultPage.IndexOf(ingredientDelimiterEnd);
                isDescription = resultPage.IndexOf(descriptionStart);
                endDescription = resultPage.IndexOf(descriptionEnd);
            }
            startIndex = resultPage.IndexOf(ingredientDelimiterStart);
            endIndex = resultPage.IndexOf(ingredientDelimiterEnd);
            if (startIndex < endIndex)
            {
                string category = customSubStr(resultPage, ingredientDelimiterStart, ingredientDelimiterEnd);
                category = cleanFromAElement(category);
                finalStr += "• " + category + "\n";
            }
            if (endIndex != -1)
            {
                resultPage = resultPage.Substring(endIndex + ingredientDelimiterEnd.Length);
            }
            else
            {
                break;
            }
        }
        foreach (string ingredient in ingredients)
        {
            finalStr += "• " + ingredient + "\n";
        }
        finalStr += resultPage;
        return finalStr;
    }

    static void Main(string url)
    {
        Task t = new Task(() => DownloadPageAsync(url));
        t.Start();
        Console.WriteLine("Downloading page...");
        Console.ReadLine();
    }

    static async Task<string> DownloadPageAsync(string url)
    {
        // ... Endpoint
        string page = url;

        string pageResult = "";
        // ... Use HttpClient.
        using (HttpClient client = new HttpClient())
        using (HttpResponseMessage response = await client.GetAsync(page))
        using (HttpContent content = response.Content)
        {
            // ... Read the string.
            pageResult = await content.ReadAsStringAsync();
        }
        return pageResult;
    }

    private string customSubStr(string original, string start, string end)
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

    private void getLetterCategories(string rawText)
    {
        string beginningCategory = "<h3>";
        string separatorElement = "</h3>\n<ul><li>";

        while(rawText.Contains(beginningCategory))
        {
            string category = customSubStr(rawText, beginningCategory, separatorElement);
            Debug.Log(category);
            LetterCategories.Add(category);
            int endIndex = rawText.IndexOf(separatorElement);
            if (endIndex != -1)
            {
                rawText = rawText.Substring(endIndex + separatorElement.Length);
            } else
            {
                break;
            }
        }
    }

    private void getDrinkNames(string rawText)
    {
        string startNameBalise = "<li>";
        string endNameBalise = "</li>";
        string startName = "\">";
        string endName = "</";

        while (rawText.Contains(startNameBalise))
        {
            string category = customSubStr(rawText, startNameBalise, endNameBalise);
            category = customSubStr(category, startName, endName);
            AllCocktails.Add(category);
            Debug.Log(category);
            int endIndex = rawText.IndexOf(endNameBalise);
            if (endIndex != -1)
            {
                rawText = rawText.Substring(endIndex + endNameBalise.Length);
            } else
            {
                break;
            }
        }
    }

    public string getDrinkListOnPage(string rawText)
    {
        string beginningList = "<div lang=\"en\" dir=\"ltr\" class=\"mw-content-ltr\"><table width=\"100%\"><tr valign=\"top\">";
        string endList = "</td></tr></table></div>";
        
        return customSubStr(rawText, beginningList, endList);
    }

    private async void getAllDrinks()
    {
        string[] PagesUrls = {
            "https://wiki.webtender.com/wiki?title=Category:Recipes&pageuntil=Godfather#mw-pages",
            "https://wiki.webtender.com/wiki?title=Category:Recipes&pagefrom=Godfather#mw-pages",
            "https://wiki.webtender.com/wiki?title=Category:Recipes&pagefrom=Santiago+Julep#mw-pages"
        };
        if (pageLoaded >= PagesUrls.Length)
        {
            loadingScreen.SetActive(false);
            menu.SetActive(true);
            pageLoaded = -1;
            return;
        }
        if (textResult == "")
        {
            Debug.Log("Getting url " + PagesUrls[pageLoaded]);
            textResult = await Task.Run(() => DownloadPageAsync(PagesUrls[pageLoaded]));
        }
    }

    private void LoadContentAndParse()
    {
        if (pageLoaded >= 0 && textResult.Length == 0 && called == false)
        {
            called = true;
            Debug.Log("Get all Drinks (" + called + ")");
            getAllDrinks();
        }
        if (textResult.Length > 0 && called == true)
        {
            Debug.Log("Parse Text (" + called + ")");
            string scrapedText = getDrinkListOnPage(textResult);
            textResult = scrapedText;
            getLetterCategories(scrapedText);
            getDrinkNames(scrapedText);
            pageLoaded += 1;
            textResult = "";
            called = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        string totalString = "";
        LoadContentAndParse();
        if (LetterCategories.Count == 0)
            return;
        foreach (string category in LetterCategories)
        {
            totalString += category + ",";
        }
    }
}
