using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;

    public void setMaxHealth(float hp)
    {
        slider.maxValue = hp;
        slider.value = hp;

        fill.color = gradient.Evaluate(1f);
    }

    public void setHealth(float hp)
    {
        slider.value = hp;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
