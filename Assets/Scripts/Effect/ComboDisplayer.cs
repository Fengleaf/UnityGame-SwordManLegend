using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComboDisplayer : MonoBehaviour
{
    public Sprite[] comboNumberSprites;
    public SpriteRenderer comboBG;
    private SpriteRenderer digit1;
    private SpriteRenderer digit10;
    private SpriteRenderer digit100;

    private static ComboDisplayer instance;

    public static ComboDisplayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ComboDisplayer)) as ComboDisplayer;
                if (instance == null)
                {
                    GameObject go = new GameObject("Combo Displayer");
                    instance = go.AddComponent<ComboDisplayer>();
                }
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != this) Destroy(gameObject);
        digit1 = transform.GetChild(0).GetComponent<SpriteRenderer>();
        digit10 = transform.GetChild(1).GetComponent<SpriteRenderer>();
        digit100 = transform.GetChild(2).GetComponent<SpriteRenderer>();
        UpdateCombo(Player.Instance.playerAttack.Combo);
    }

    public void UpdateCombo(int value)
    {
        int temp = value;
        if (value == 0)
        {
            comboBG.color = new Color(255, 255, 255, 0);
            digit1.sprite = null;
            digit10.sprite = null;
            digit100.sprite = null;
        }
        else
        {
            comboBG.color = new Color(255, 255, 255, 255);
            int index = temp % 10;
            digit1.sprite = comboNumberSprites[index];
            if (value >= 10)
            {
                temp /= 10;
                index = temp % 10;
                digit10.sprite = comboNumberSprites[index];
            }
            if (value >= 100)
            {
                temp /= 10;
                index = temp % 10;
                digit100.sprite = comboNumberSprites[index];
                temp /= 10;
                index = temp % 10;
            }
        }
    }
}
