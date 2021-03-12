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
    private GameObject drinkCard;
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private Scrollbar scrollbarImages;
    [SerializeField]
    private GameObject CancelLoadCross;

    [SerializeField]
    private htmlParser html;

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
            GameObject.Destroy(ImageParent.transform.GetChild(i).gameObject);
        }
    }

    IEnumerator createPreviewImages(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            GameObject instImg = Instantiate(ImagePrefab);
            instImg.transform.SetParent(ImageParent.transform);
            instImg.transform.GetComponent<Image>().sprite = sprite;
            instImg.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public async void imageTest(string _name, int number)
    {
        var query = "http://images.google.com/images?q=" + _name + "&hl=en&imgsz=Medium";
        string returnStr = await DownloadPageAsync(query);
        string urlDelimiter = "<img class=\"t0fcAb\" alt=\"\" src=\"";
        string endUrl = "\"/></div>";

        EmptyParent();
        for (int i = 0; i < number; i++)
        {
            var startIndex = returnStr.IndexOf(urlDelimiter);
            if (startIndex != -1)
                returnStr = returnStr.Substring(startIndex + urlDelimiter.Length);
            var endIndex = returnStr.IndexOf(endUrl);
            if (endIndex != -1)
            {
                string tempurl = returnStr.Substring(0, endIndex);
                StartCoroutine(createPreviewImages(tempurl));
            }
            returnStr = returnStr.Substring(endIndex + endUrl.Length);
        }
        loadingScreen.SetActive(false);
        scrollbarImages.value = 0;
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

        string resultPage = await DownloadPageAsync(baseUrl + name);
        resultPage = html.htmlExtract(resultPage);
        return resultPage;
    }

    static void Main(string url)
    {
        Task t = new Task(() => DownloadPageAsync(url));
        t.Start();
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

    private void getLetterCategories(string rawText)
    {
        string beginningCategory = "<h3>";
        string separatorElement = "</h3>\n<ul><li>";

        while(rawText.Contains(beginningCategory))
        {
            string category = html.customSubStr(rawText, beginningCategory, separatorElement);
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
            string category = html.customSubStr(rawText, startNameBalise, endNameBalise);
            category = html.customSubStr(category, startName, endName);
            category = html.removeElement(category, "&amp;", "");

            AllCocktails.Add(category);
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
        
        return html.customSubStr(rawText, beginningList, endList);
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
            CancelLoadCross.SetActive(true);
            pageLoaded = -1;
            return;
        }
        if (textResult == "")
        {
            textResult = await Task.Run(() => DownloadPageAsync(PagesUrls[pageLoaded]));
        }
    }

    private void LoadContentAndParse()
    {
        if (pageLoaded >= 0 && textResult.Length == 0 && called == false)
        {
            called = true;
            getAllDrinks();
        }
        if (textResult.Length > 0 && called == true)
        {
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
