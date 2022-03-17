using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public Image infoBG;
    public Text skillName;
    public Text skillLevel;
    public Text skillMpCost;
    public Text skillCoolTime;
    public Text skillDescription;

    public Image nextInfoBG;
    public Text skillNextLevel;
    public Text skillNextMpCost;
    public Text skillNextCoolTime;
    public Text skillNextDescription;

    public Text skillPoint;

    public bool IsOpen { get; private set; }

    private CanvasGroup canvas;
    // To store the information of skill that the player learned.
    public static Dictionary<int, Skill> skills = new Dictionary<int, Skill>();

    private void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        HideSkillTree();
        infoBG.rectTransform.localScale = new Vector3(0, 0, 0);
        skillPoint.text = FindObjectOfType<Player>().SkillPoint.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (KeyboardSetting.specialKeyTable.ContainsKey(ValueType.SkillTree))
        {
            if (Input.GetKeyDown(KeyboardSetting.specialKeyTable[ValueType.SkillTree].key.key))
            {
                if (canvas.alpha == 0)
                    ShowSKillTree();
                else
                    HideSkillTree();
            }
        }
    }

    public void ShowSKillTree()
    {
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        IsOpen = true;
        transform.SetAsLastSibling();
    }

    public void HideSkillTree()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        IsOpen = false;
    }

    public void ShowSkillInfo(bool show)
    {
        if (show)
        {
            infoBG.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1);
        }
        else
        {
            infoBG.rectTransform.localScale = new Vector3(0, 0, 0);
        }
    }

    public void UpdateSkillInfo(Skill skill)
    {
        Skill s;
        if (skills.ContainsKey(skill.id))
            s = skills[skill.id];
        else
        {
            if (skill.id == 4)
                skill.level = 0;
            s = skill;
        }
        ShowSkillInfo(true);
        skillName.text = s.skillName;
        skillLevel.text = s.level.ToString();
        if (s.mpCost.Length == 0)
            skillMpCost.text = "0";
        else if (s.level > s.mpCost.Length)
            skillMpCost.text = s.mpCost[s.mpCost.Length - 1].ToString();
        else if (s.level == 0)
            skillMpCost.text = s.mpCost[0].ToString();
        else
            skillMpCost.text = s.mpCost[s.level - 1].ToString();

        if (s.coolTime.Length == 0)
            skillCoolTime.text = "0";
        else if (s.level > s.coolTime.Length)
            skillCoolTime.text = s.coolTime[s.coolTime.Length - 1].ToString();
        else if (s.level == 0)
            skillCoolTime.text = s.coolTime[0].ToString();
        else
            skillCoolTime.text = s.coolTime[s.level - 1].ToString();

        string s1 = "", s2 = "";

        Skill.GetSkillDescription(s, ref s1, ref s2);
        skillDescription.text = s1;

        if (s2 != "")
        {
            nextInfoBG.rectTransform.localScale = new Vector3(1, 1, 1);
            skillNextLevel.text = (s.level + 1).ToString();
            if (s.mpCost.Length == 0)
                skillNextMpCost.text = "0";
            else if (s.level > s.mpCost.Length)
                skillNextMpCost.text = s.mpCost[s.mpCost.Length - 1].ToString();
            else
                skillNextMpCost.text = s.mpCost[s.level].ToString();

            if (s.coolTime.Length == 0)
                skillNextCoolTime.text = "0";
            else if (s.level > s.coolTime.Length)
                skillNextCoolTime.text = s.coolTime[s.coolTime.Length - 1].ToString();
            else
                skillNextCoolTime.text = s.coolTime[s.level].ToString();
            skillNextDescription.text = s2;
        }
        else
            nextInfoBG.rectTransform.localScale = new Vector3(0, 0, 0);
        skillPoint.text = FindObjectOfType<Player>().SkillPoint.ToString();
    }

    public void UpgradeSkill(Skill skill)
    {
        Player player = FindObjectOfType<Player>();
        if (CanUpgradeSkill(player, skill))
        {
            player.SkillPoint -= skill.levelUpSp[skill.level];
            if (!skills.ContainsKey(skill.id) && skill.level == 0)
            {
                skills[skill.id] = Instantiate(skill, player.transform);
                skills[skill.id].gameObject.SetActive(false);
                skills[skill.id].level = 1;
                player.LearnSkill(skills[skill.id]);
            }
            else
            {
                skills[skill.id].level++;
                player.LearnSkill(skills[skill.id]);
            }
            UpdateSkillInfo(skills[skill.id]);
        }
    }

    private bool CanUpgradeSkill(Player player, Skill skill)
    {
        int sp = player.SkillPoint;
        // Already max level, can't upgrade.
        if (skills.ContainsKey(skill.id) && skills[skill.id].level == skill.maxLevel)
            return false;
        // Skill point isn't enough, can't upgrade. 
        if (sp < skill.levelUpSp[skill.level])
            return false;
        bool hasPreSkill = false;
        // Check for pre-need skill. Only need one of them.
        if (skill.preSkill.Length > 0)
        {
            for (int i = 0; i < skill.preSkill.Length; i++)
            {
                if (skills.ContainsKey(skill.preSkill[i].id))
                {
                    hasPreSkill = true;
                    break;
                }
            }
        }
        else
            hasPreSkill = true;
        return hasPreSkill;
    }
}
