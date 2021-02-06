using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    [SerializeField]
    private Sprite[] stateTextures;
    [SerializeField]
    private Image Cross;
    [SerializeField]
    private Image Eye;
    [SerializeField]
    private float interval;
    private float elapsedTime;

    public bool visible;

    // Start is called before the first frame update
    void Start()
    {
        setStateVisible(true);
    }

    public void setStateVisible(bool state)
    {
        visible = !state;
        changeValueVisible();
    }

    public void changeValueVisible()
    {
        visible = !visible;
        Cross.gameObject.SetActive(!visible);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > interval)
        {
            elapsedTime = 0;
            Eye.sprite = stateTextures[Random.Range(0, stateTextures.Length)];
        }
    }
}
