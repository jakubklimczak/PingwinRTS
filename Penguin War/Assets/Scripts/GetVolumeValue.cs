using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetVolumeValue : MonoBehaviour
{
    public Slider volume;
    public Text text;
    public float volumeValue;

    private void Update()
    {
        volumeValue = volume.value;
        text.text = volumeValue.ToString();
    }
}
