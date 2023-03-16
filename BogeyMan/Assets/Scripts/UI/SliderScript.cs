using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float colorMultiplier;
    public void OnSelect(BaseEventData eventData)
    {
        var colorBlock = gameObject.GetComponent<Slider>().colors;
        colorBlock.colorMultiplier = 5f;
        colorMultiplier = 5f;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        var colorBlock = gameObject.GetComponent<Slider>().colors;
        colorBlock.colorMultiplier = 1f;
        colorMultiplier = 1f;
    }
}
