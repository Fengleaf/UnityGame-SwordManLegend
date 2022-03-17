using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeySettingShower : MonoBehaviour
{
    public Image ctrlImage;
    public Image shiftImage;
    public Image aImage;
    public Image dImage;
    public Image xImage;
    public Image rImage;
    public Image endImage;
    public Image pdnImage;
    public Image MaskPrefab;

    public readonly Dictionary<string, Image> images = new Dictionary<string, Image>();

    // Start is called before the first frame update
    void Start()
    {
        images["left ctrl"] = ctrlImage;
        images["left shift"] = shiftImage;
        images["a"] = aImage;
        images["d"] = dImage;
        images["x"] = xImage;
        images["r"] = rImage;
        images["end"] = endImage;
        images["page down"] = pdnImage;
    }

    public void ShowKey(string s, Sprite sprite)
    {
        if (images.ContainsKey(s))
        {
            foreach (string key in images.Keys)
            {
                if (images[key].sprite == sprite)
                    HideKey(key);
            }
            images[s].sprite = sprite;
            images[s].rectTransform.sizeDelta = new Vector2(1, 1);
        }
    }

    public void HideKey(string s)
    {
        if (images.ContainsKey(s))
        {
            images[s].rectTransform.sizeDelta = new Vector2(0, 0);
        }
    }

    public void ShowCoolDownEffect(string s, float time)
    {
        if (images.ContainsKey(s))
            StartCoroutine(RunCoolDown(s, time));
    }

    IEnumerator RunCoolDown(string s, float time)
    {
        Image mask = Instantiate(MaskPrefab, images[s].transform.position, Quaternion.identity, images[s].transform);
        mask.transform.SetAsFirstSibling();
        Text text = images[s].transform.GetChild(1).GetComponent<Text>();
        text.fontSize = 32;
        float timeLeft = time;
        float timer = Time.time;
        while (Time.time - timer < time)
        {
            text.text = (Mathf.Floor(timeLeft)).ToString();
            mask.fillAmount = timeLeft / time;
            yield return null;
            timeLeft -= Time.deltaTime;
        }
        text.fontSize = 100;
        Destroy(mask.gameObject);
    }
}
