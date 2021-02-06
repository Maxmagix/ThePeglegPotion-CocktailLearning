using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealHidden : MonoBehaviour
{
    [SerializeField]
    private GameObject images;
    [SerializeField]
    private GameObject text;
    [SerializeField]
    private Image scroll;
    [SerializeField]
    private Color[] colorState;

    [SerializeField]
    private VisibilityChecker visibility;
    private bool stateChecked = false;

    public void initStateChecked()
    {
        setStateChecked(visibility.visible);
    }

    public void setStateChecked(bool state)
    {
        stateChecked = state;
        images.SetActive(stateChecked);
        text.SetActive(stateChecked);
        scroll.color = colorState[(stateChecked ? 0 : 1)];
    }

    public void changeStateChecked()
    {
        setStateChecked(!stateChecked);
    }
}
