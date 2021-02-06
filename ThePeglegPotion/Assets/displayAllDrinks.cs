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
    private InputField searchbar;
    [SerializeField]
    private DrinkSelector selector;
    [SerializeField]
    private Scrollbar scroll;

    private string searchingText = "";
    private Dictionary<string, GameObject> drinks = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        AddToList();
        scroll.SetValueWithoutNotify(1);
    }

    private void Update()
    {
        if (searchbar.text != searchingText)
        {
            searchingText = searchbar.text;
            scroll.SetValueWithoutNotify(1);
            emitOutOfSearch();
        }
    }

    public void AddToList()
    {
        foreach (string name in interneter.AllCocktails)
        {
            if (!drinks.ContainsKey(name))
            {
                drinks.Add(name, createDrinkButton(name));
            }
        }
    }

    public void emitOutOfSearch()
    {
        foreach (string name in interneter.AllCocktails)
        {
            if (drinks.ContainsKey(name))
            {
                drinks[name].gameObject.SetActive(name.ToLower().Contains(searchingText.ToLower()));
            }
        }
    }

    private GameObject createDrinkButton(string name)
    {
        GameObject obj = Instantiate(DrinkPrefab);
        obj.gameObject.name = name;
        obj.transform.SetParent(parentDrinkList.transform);
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
