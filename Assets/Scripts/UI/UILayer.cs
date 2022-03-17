using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILayer : MonoBehaviour
{
    public void ToTop(RectTransform rectTransform)
    {
        rectTransform.SetAsLastSibling();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0)
        {
            if (KeyboardSetting.Instance.nowSelected != null)
                return;
            string name = transform.GetChild(transform.childCount - 1).name;
            if (name == "Key Setting Shower" || name == "PlayerState" || (!KeyboardSetting.specialKeyTable.ContainsKey(ValueType.Setting) && name == "Setting"))
                return;
            transform.GetChild(transform.childCount - 1).GetComponent<CanvasGroup>().alpha = 0;
            transform.GetChild(transform.childCount - 1).GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>().SetAsFirstSibling();
        }
    }
}
