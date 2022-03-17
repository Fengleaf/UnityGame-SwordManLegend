using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dialog : MonoBehaviour
{
    public static bool isShowDialog;
    public Image dialogBG;
    public Text dialogText;

    public Image selectionBG;
    public Text selection1;
    public Text selection2;
    public int SelectionResult { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        dialogBG.rectTransform.localScale = new Vector2(0, 0);
        selectionBG.rectTransform.localScale = new Vector2(0, 0);
    }
    
    public void ShowDialog(string text)
    {
        isShowDialog = true;
        dialogBG.rectTransform.localScale = new Vector2(1, 1);
        dialogText.text = text;
    }

    public void HideDialog()
    {
        isShowDialog = false;
        dialogBG.rectTransform.localScale = new Vector2(0, 0);
    }

    public void ShowTwoSelection(string s1, string s2)
    {
        SelectionResult = -1;
        selection1.text = s1;
        selection2.text = s2;
        selectionBG.rectTransform.localScale = new Vector2(1, 1);
    }

    public void SettingSelection(int value)
    {
        SelectionResult = value;
        selectionBG.rectTransform.localScale = new Vector2(0, 0);
    }
}
