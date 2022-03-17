using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoShower : MonoBehaviour
{
    public Text monsterNmae;
    public Image hpStripe;
    public Image hpStripeBg;

    public bool showName;

    public void UpdateHpStripe(float targetValue, float maxValue)
    {
        if (targetValue < 0)
            targetValue = 0;
        hpStripe.rectTransform.sizeDelta = new Vector2(targetValue / maxValue * hpStripeBg.rectTransform.sizeDelta.x, hpStripe.rectTransform.sizeDelta.y);
    }

    public void Flip(bool positive)
    {
        Vector2 scale = GetComponent<RectTransform>().localScale;
        if (positive)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -1 * Mathf.Abs(scale.x);
        GetComponent<RectTransform>().localScale = scale;
    }
}
