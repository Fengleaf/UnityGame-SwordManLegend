using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageDisplayer : MonoBehaviour
{
    public Canvas canvas;
    public Text damageText;
    public float existTime;
    public float numberSpace;
    public float offsetLX;
    public float offsetRX;
    public float offsetDY;
    public float offsetUY;

    public void DisplayDamage(int damage, Vector2 position, bool isCritical)
    {
        float x = Random.Range(offsetLX, offsetRX);
        float y = Random.Range(offsetDY, offsetUY);
        Text d = Instantiate(damageText, position + new Vector2(x, y), Quaternion.identity, canvas.transform);
        if (isCritical)
            d.color = Color.red;
        d.text = damage.ToString();
        StartCoroutine("MoveDamage", d);
    }

    IEnumerator MoveDamage(Text number)
    {
        for (int i = 0; i < 7; i++)
        {
            number.rectTransform.position += new Vector3(0, 0.1f);
            yield return null;
        }

        yield return new WaitForSeconds(existTime);
        Destroy(number.gameObject);
    }
}
