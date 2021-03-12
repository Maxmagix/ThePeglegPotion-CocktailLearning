using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayAllDrinks : MonoBehaviour
{
    [SerializeField]
    private HttpContentGetter interneter;
    [SerializeField]
    private GameObject parentDrinkList;
    [SerializeField]
    private GameObject DrinkPrefab;
    [SerializeField]
    private GameObject DivisionPrefab;
    [SerializeField]
    private InputField searchbar;
    [SerializeField]
    private DrinkSelector selector;
    [SerializeField]
    private Scrollbar scroll;
    [SerializeField]
    private Text nbDrinks;

    [SerializeField]
    private int numberInDivision = 9;

    [SerializeField]
    private hideIfOutOfScreen hide;

    private GameObject lastParentDivision = null;

    private string searchingText = "";
    public int numberOfDrinks = 0;
    private Dictionary<string, GameObject> drinks = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        AddToList();
        scroll.SetValueWithoutNotify(1);
        setAllActive();
    }

    public void UpdateVisibility()
    {
        if (searchbar.text != searchingText)
        {
            searchingText = searchbar.text;
            if (searchingText.Equals(""))
            {
                setAllActive();
                hide.resetToTop();
            }
            else
            {
                emitOutOfSearch();
                hide.resetToTop();
            }
            scroll.SetValueWithoutNotify(1);
        }
    }

    public void AddToList()
    {
        foreach (string name in interneter.AllCocktails)
        {
            if (!drinks.ContainsKey(name))
            {
                if (lastParentDivision == null || numberOfDrinks % numberInDivision == 0)
                {
                    lastParentDivision = createNewDivision();
                }
                drinks.Add(name, createDrinkButton(name, lastParentDivision));
                numberOfDrinks++;
            }
        }
    }

    public void setAllInactive()
    {
        foreach (string name in interneter.AllCocktails)
        {
            if (drinks.ContainsKey(name) && drinks[name].gameObject.activeSelf)
            {
                drinks[name].gameObject.SetActive(false);
            }
        }
    }

    public void setAllActive()
    {
        int nb = 0;
        foreach (string name in interneter.AllCocktails)
        {
            if (drinks.ContainsKey(name) && !drinks[name].gameObject.activeSelf)
            {
                drinks[name].gameObject.SetActive(true);
            }
            nb++;
        }
        nbDrinks.text = "(" + nb + ")";
    }

    public void emitOutOfSearch()
    {
        int nb = 0;
        foreach (string name in interneter.AllCocktails)
        {
            if (drinks.ContainsKey(name))
            {
                bool state = name.ToLower().Contains(searchingText.ToLower());
                drinks[name].gameObject.SetActive(state);
                if (state) nb++;
            }
        }
        nbDrinks.text = "(" + nb + ")";
    }

    private GameObject createNewDivision()
    {
        GameObject obj = Instantiate(DivisionPrefab);
        obj.transform.SetParent(parentDrinkList.transform);
        obj.transform.localScale = new Vector3(1, 1, 1);
        return obj;
    }

    private GameObject createDrinkButton(string name, GameObject parentDivision)
    {
        GameObject obj = Instantiate(DrinkPrefab);
        obj.gameObject.name = name;
        obj.transform.SetParent(parentDivision.transform);
        obj.transform.localScale = new Vector3(1, 1, 1);
        Button objButton = obj.gameObject.GetComponent<Button>();
        if (!objButton)
        {
            objButton = obj.gameObject.AddComponent<Button>();
        }
        objButton.onClick.AddListener(() => selector.goToDrink(name));
        obj.transform.GetChild(1).gameObject.transform.GetComponent<Text>().text = name;
        return obj;
    }
}
