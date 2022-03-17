using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpDisplayer : MonoBehaviour
{
    public GameObject target;
    public float startY;
    public float endY;
    public float riseSpeed;
    public float riseAcceleration;
    public float stayTime;
    public float disappearingSpeed;
    public Text text;

    private void Start()
    {
        text.GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    public void StartShow()
    {
        text.GetComponent<CanvasRenderer>().SetAlpha(0);
        Vector2 pos = target.transform.position;
        pos.y += startY;
        text.rectTransform.localPosition = pos;
        StartCoroutine("Show");
    }

    IEnumerator Show()
    {
        yield return null;
        text.GetComponent<CanvasRenderer>().SetAlpha(1);
        while (text.rectTransform.localPosition.y < target.transform.position.y + endY)
        {
            text.rectTransform.Translate(0, riseSpeed * Time.deltaTime, 0);
            riseSpeed += riseAcceleration;
            if (riseSpeed < 0)
                break;
            yield return null;
        }
        yield return new WaitForSeconds(stayTime);
        while (text.GetComponent<CanvasRenderer>().GetAlpha() > 0)
        {
            float a = text.GetComponent<CanvasRenderer>().GetAlpha();
            text.GetComponent<CanvasRenderer>().SetAlpha(a - disappearingSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
