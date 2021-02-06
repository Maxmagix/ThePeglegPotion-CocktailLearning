using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;

public class DrinkSelector : MonoBehaviour
{
    [SerializeField]
    private HttpContentGetter interneter;
    [SerializeField]
    private Text title;
    [SerializeField]
    private Text description;
    [SerializeField]
    private GameObject Menu;
    [SerializeField]
    private GameObject Card;

    public void selectRandomDrink()
    {
        int nbCocktails = interneter.AllCocktails.Count;
        if (nbCocktails == 0)
            return;
        string selectedName = interneter.AllCocktails[Random.Range(0, nbCocktails)];
        goToDrink(selectedName);
    }

    public async void goToDrink(string name)
    {
        Menu.SetActive(false);
        Card.SetActive(true);
        Debug.Log(name);
        description.text = await Task.Run(() => interneter.getCocktailDescription(name));
        title.text = name;
        //interneter.createImages(name, 10);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
