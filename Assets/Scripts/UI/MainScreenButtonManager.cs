using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainScreenButtonManager : MonoBehaviour
{
    public CanvasGroup main;
    public CanvasGroup setting;
    public CanvasGroup record;
    public RectTransform recordContent;
    public Image recordDetail;
    public Text finishTime;
    public Text end;
    public Text costTime;
    public Text recordText;

    public void StartGame()
    {
        GameObject.Find("Start Button").GetComponent<Coffee.UIExtensions.UIDissolve>().Play();
        GameObject.Find("Setting Button").GetComponent<Coffee.UIExtensions.UIDissolve>().Play();
        GameObject.Find("Record Button").GetComponent<Coffee.UIExtensions.UIDissolve>().Play();
        GameObject.Find("Exit Button").GetComponent<Coffee.UIExtensions.UIDissolve>().Play();
        GameManager.instance.StartGame();
    }

    public void EnterSetting()
    {
        main.alpha = 0;
        main.blocksRaycasts = false;
        setting.alpha = 1;
        setting.blocksRaycasts = true;
        record.alpha = 0;
        record.blocksRaycasts = false;
    }

    public void ExitSetting()
    {
        main.alpha = 1;
        main.blocksRaycasts = true;
        setting.alpha = 0;
        setting.blocksRaycasts = false;
        record.alpha = 0;
        record.blocksRaycasts = false;
    }

    public void SetPlayerName(Text name)
    {
        GameManager.instance.PlayerName = name.text;
    }

    public void ShowRecord()
    {
        if (record.alpha == 1)
            return;
        recordDetail.rectTransform.localScale = new Vector2(0, 0);
        record.alpha = 1;
        record.blocksRaycasts = true;
        float n = GameManager.playerRecords.Count - 9;
        recordContent.offsetMin = new Vector2(recordContent.offsetMin.x, -n * 41);
        for (int i = 0; i < GameManager.playerRecords.Count; i++)
        {
            Text text = Instantiate(recordText, recordContent);
            text.text = GameManager.playerRecords[i].finishTime;
            text.transform.localPosition += new Vector3(0, 164 + 19 * n - i * 41, 0);
            text.GetComponent<SettingKey>().key = i.ToString();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener(delegate { ShowRecordDetail(text.GetComponent<SettingKey>().key); });
            text.GetComponent<EventTrigger>().triggers.Add(entry);
            EventTrigger.Entry entry2 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entry2.callback.AddListener(delegate { HideRecordDetail(); });
            text.GetComponent<EventTrigger>().triggers.Add(entry2);
        }
    }

    public void HideRecord()
    {
        record.alpha = 0;
        record.blocksRaycasts = false;
        for (int i = 0; i < recordContent.transform.childCount; i++) 
        {
            Destroy(recordContent.transform.GetChild(i).gameObject);
        }
    }

    public void ShowRecordDetail(string index)
    {
        int i = int.Parse(index);
        PlayerRecord record = GameManager.playerRecords[i];
        recordDetail.rectTransform.localScale = new Vector3(38.4f, 40.5f);
        finishTime.text = record.finishTime;
        end.text = record.endType == 0 ? "結局1: 相信國王和皇后" : "結局2: 相信魔王";
        costTime.text = record.totalCostTime;
    }

    public void HideRecordDetail()
    {
        recordDetail.rectTransform.localScale = new Vector2(0, 0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
