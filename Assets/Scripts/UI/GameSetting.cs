using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public CanvasGroup[] others;
    private CanvasGroup canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        StartCoroutine("DectectInput");
        Hide();
    }

    IEnumerator DectectInput()
    {
        while (true)
        {
            if (KeyboardSetting.specialKeyTable.ContainsKey(ValueType.Setting))
            {
                if (Input.GetKeyDown(KeyboardSetting.specialKeyTable[ValueType.Setting].key.key))
                {
                    if (canvas.alpha == 0)
                        Show();
                    else
                        Hide();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (canvas.alpha == 0)
                    {
                        bool canOpen = true; ;
                        for (int i = 0; i < others.Length; i++)
                        {
                            if (others[i].alpha != 0)
                            {
                                canOpen = false;
                                break;
                            }
                        }
                        if (canOpen)
                            Show();
                    }
                    else
                        Hide();
                }
            }
            yield return null;
        }
    }

    public void Show()
    {
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        transform.SetAsLastSibling();
    }

    public void Hide()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
    }

    public void SetPlayerName(Text text)
    {
        GameManager.instance.PlayerName = text.text;
        FindObjectOfType<StateWindow>().Refrech();
    }
}
