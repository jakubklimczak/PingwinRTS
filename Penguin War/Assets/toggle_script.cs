using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class toggle_script : MonoBehaviour
{
    private Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        //toggle.onValueChanged.AddListener( (value) => OnToggleValueChanged(value));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnToggleValueChanged(bool isOn)
    {
        ColorBlock cb = toggle.colors;
        if (isOn)
        {
            cb.normalColor = Color.gray;
            cb.highlightedColor = Color.gray;
        }
        else
        {
            cb.normalColor = Color.blue;
            cb.highlightedColor = Color.blue;
        }
        Debug.Log("here");
        Debug.Log(cb.normalColor);
        //toggle.colors = cb;
    }
}
