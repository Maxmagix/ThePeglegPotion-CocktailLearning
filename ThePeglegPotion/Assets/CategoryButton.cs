using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    [SerializeField]
    public GameObject listCocktails;
    [SerializeField]
    public Text nameCategory;
    [SerializeField]
    public Text numberCocktails;

    private bool state = false;

    private void Start()
    {
        ForceSetState(false);
    }

    private void DeactivateAll()
    {
        GameObject[] categories = GameObject.FindGameObjectsWithTag("Category");
        foreach(GameObject category in categories)
        {
            category.gameObject.transform.GetComponent<CategoryButton>().ForceSetState(false);
        }
    }

    public void SwitchState()
    {
        if (!state)
        {
            DeactivateAll();
        }
        state = !state;
        listCocktails.SetActive(state);
    }

    public void ForceSetState(bool value)
    {
        state = value;
        listCocktails.SetActive(state);
    }
}
