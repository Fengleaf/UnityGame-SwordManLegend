using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarEffect : MonoBehaviour
{
    public Image healthBar;
    public Image healthBarEffect;

    // Update is called once per frame
    void Update()
    {
        if (healthBarEffect.rectTransform.sizeDelta.x > healthBar.rectTransform.sizeDelta.x * healthBar.fillAmount)
        {
            Vector2 size = healthBarEffect.rectTransform.sizeDelta;
            size.x -= 0.1f;
            healthBarEffect.rectTransform.sizeDelta = size;
        }
        else
        {
            Vector2 size = healthBar.rectTransform.sizeDelta;
            size.x *= healthBar.fillAmount;
            healthBarEffect.rectTransform.sizeDelta = size;
        }
    }
}
