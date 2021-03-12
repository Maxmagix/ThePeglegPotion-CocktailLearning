using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hideIfOutOfScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject divisionsContainer;
    [SerializeField]
    private ScrollRect scrollrect;

    [SerializeField]
    private int numberOfCocktailsActive;

    private int topIndex = 0;
    private int bottomIndex = 0;

    private int nbMaxDivs = 0;

    private bool lastScrollBottom = false;
    private bool readyToLoad = false;

    private bool scrolling = false;

    void Start()
    {
        nbMaxDivs = divisionsContainer.transform.childCount;
        bottomIndex = getBottomFromTop();
        setValue(false);
    }

    IEnumerator scaleUpdater()
    {
        yield return new WaitForSeconds(0.001f);
        divisionsContainer.SetActive(false);
        yield return new WaitForSeconds(0.001f);
        divisionsContainer.SetActive(true);
    }

    public void resetToTop()
    {
        topIndex = 0;
        bottomIndex = getBottomFromTop();
        scrolling = false;
        setValue(false);
    }

    public void updateScale()
    {
        StartCoroutine(scaleUpdater());
    }

    private int getBottomFromTop()
    {
        int nbDisplayed = 0;
        for (int i = topIndex; i < nbMaxDivs; i++)
        {
            Transform child = divisionsContainer.transform.GetChild(i);
            for (int index = 0; index < child.childCount; index++)
            {
                if (child.GetChild(index).gameObject.activeSelf)
                    nbDisplayed++;
            }
            if (nbDisplayed >= numberOfCocktailsActive)
                return i;
        }
        return nbMaxDivs - 1;
    }

    private int getTopFromBottom()
    {
        int nbDisplayed = 0;
        for (int i = bottomIndex; i > 0; i--)
        {
            Transform child = divisionsContainer.transform.GetChild(i);
            for (int index = 0; index < child.childCount; index++)
            {
                if (child.GetChild(index).gameObject.activeSelf)
                    nbDisplayed++;
            }
            if (nbDisplayed >= numberOfCocktailsActive)
                return i;
        }
        return nbMaxDivs - 1;
    }

    private void setValue(bool value)
    {
        bool valToPut = value;
        for (int i = 0; i < nbMaxDivs; i++)
        {
            valToPut = value;
            if (i >= topIndex && i <= bottomIndex)
                valToPut = true;
            int nbCocktailActive = 0;
            Transform child = divisionsContainer.transform.GetChild(i);
            for (int index = 0; index < child.childCount; index++)
            {
                if (child.GetChild(index).gameObject.activeSelf)
                {
                    nbCocktailActive++;
                }
            }
            if (nbCocktailActive == 0)
                valToPut = false;
            divisionsContainer.transform.GetChild(i).gameObject.SetActive(valToPut);
        }
        updateScale();
    }

    // Update is called once per frame
    void Update()
    {
        if (((scrollrect.verticalNormalizedPosition >= -0.05f && scrollrect.verticalNormalizedPosition <= 0.05f ) ||
            (scrollrect.verticalNormalizedPosition >= 0.95f && scrollrect.verticalNormalizedPosition <= 1.05f))
            && scrollrect.velocity.y == 0)
            readyToLoad = true;
        if (scrollrect.verticalNormalizedPosition < -0.1f)
        {
            if ((!scrolling || !lastScrollBottom) && bottomIndex < nbMaxDivs - 1 && readyToLoad)
            {
                bottomIndex++;
                setValue(false);
                lastScrollBottom = true;
                scrolling = true;
                readyToLoad = false;
            }
        }
        if (scrollrect.verticalNormalizedPosition > 1.1f)
        {
            if ((!scrolling || lastScrollBottom) && topIndex > 0 && readyToLoad)
            {
                topIndex--;
                setValue(false);
                lastScrollBottom = false;
                scrolling = true;
                readyToLoad = false;
            }
        }
        if (scrollrect.velocity.y > -0.02 && scrollrect.velocity.y < 0.02 && scrolling)
        {
            topIndex = lastScrollBottom ? getTopFromBottom() : topIndex;
            bottomIndex = !lastScrollBottom ? getBottomFromTop() : bottomIndex;
            scrolling = false;
            setValue(false);
            scrollrect.verticalNormalizedPosition = 0.5f;
            scrollrect.velocity = new Vector2(0, 0);
        }
    }
}
