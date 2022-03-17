using DigitalRuby.WeatherMaker;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerRecord
{
    public string finishTime;
    public int endType;
    public string totalCostTime;
}

public class GameManager : MonoBehaviour
{
    public Player player;
    public Dialog dialog;
    public Vector2 playerLoadedPosition;
    public SpriteRenderer fadeInImage;
    public GameObject balloon;

    public AudioClip demonEndBGM;
    public AudioClip kingEndBGM;
    public AudioClip forestBGM;
    public AudioClip castleBGM;
    public AudioClip finnalBattle;
    public AudioClip dieBGM;

    private string playerName = "玩家";
    public string PlayerName
    {
        get { return playerName; }
        set
        {
            playerName = value;
            StateWindow stateWindow = FindObjectOfType<StateWindow>();
            if (stateWindow != null)
            {
                stateWindow.playerName.text = playerName;
                stateWindow.sName.text = playerName;
            }
        }
    }

    public AudioSource audioSource;

    public WWW www;
    //public DownloadHandlerAssetBundle handler;
    //public bool AllStop { get; private set; }

    private static bool[] passScene = new bool[0];

    public static GameManager instance;

    //public static GameManager instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindObjectOfType(typeof(GameManager)) as GameManager;
    //            if (instance == null)
    //            {
    //                GameObject go = new GameObject("GameManager");
    //                instance = go.AddComponent<GameManager>();
    //            }
    //        }
    //        return instance;
    //    }
    //}

    public static bool defeatedDemon = false;

    public static List<PlayerRecord> playerRecords;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        string json = File.ReadAllText(Application.dataPath + "/Resources/record.json");
        if (json.Length == 0)
            playerRecords = new List<PlayerRecord>();
        else
            playerRecords = new List<PlayerRecord>(JsonHelper.FromJson<PlayerRecord>(json));
        if (passScene.Length == 0)
        {
            passScene = new bool[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < passScene.Length; i++)
                passScene[i] = false;
        }
        StartCoroutine("LoadAssetBundle");
        SceneManager.sceneLoaded += ScenePreview;
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (instance != this) Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
        //SceneManager.sceneLoaded += ScenePreview;
        //ScenePreview(SceneManager.GetActiveScene(), LoadSceneMode.Additive);
    }

    public void Pause(bool pause)
    {
        if (pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    private void ScenePreview(Scene scene, LoadSceneMode arg1)
    {
        
        if (fadeInImage != null)
        {
            StartCoroutine("FadeOutScene");
        }
        Player.Instance.transform.position = instance.playerLoadedPosition;
        // Main
        if (scene.buildIndex == 0)
        {
            if (FindObjectOfType<Player>() != null)
                Destroy(FindObjectOfType<Player>().gameObject);
        }
        // Boss Forest
        if (scene.buildIndex == 1)
        {
            StartCoroutine("FadeOutScene");
            if (defeatedDemon)
            {
                FindObjectOfType<CameraMove>().UpdateCameraMove();
                Destroy(FindObjectOfType<DemonBoss>().gameObject);
                Destroy(FindObjectOfType<WeatherMakerScript>().gameObject);
            }
            else
            {
                audioSource.clip = forestBGM;
                audioSource.Play();
                instance.StartCoroutine("ShowBossForestStory");
            }
        }
        // Plain
        else if (scene.buildIndex == 3 && !passScene[scene.buildIndex - 1])
        {
            if (audioSource.clip != forestBGM)
            {
                audioSource.clip = forestBGM;
                audioSource.Play();
            }
            FindObjectOfType<CameraMove>().UpdateCameraMove();
            instance.StartCoroutine("ShowPlainStory1");
        }
        // Castle 1
        else if (scene.buildIndex == 4 && !passScene[scene.buildIndex - 1])
        {
            if (audioSource.clip != castleBGM)
            {
                audioSource.clip = castleBGM;
                audioSource.Play();
            }
            FindObjectOfType<CameraMove>().UpdateCameraMove();
            instance.StartCoroutine("ShowCastleStory1");
        }
        else if (scene.buildIndex == 5)
        {
            FindObjectOfType<CameraMove>().UpdateCameraMove();
            if (!passScene[scene.buildIndex - 1])
            {
                instance.StartCoroutine("ShowCastleStory2");
            }
            else
            {
                Destroy(GameObject.Find("Queen"));
                GameObject[] soliders = GameObject.FindGameObjectsWithTag("Enemy");
                for (int i = 0; i < soliders.Length; i++)
                {
                    soliders[i].GetComponent<Monster>().enabled = true;
                    soliders[i].GetComponent<Soldier>().enabled = true;
                    soliders[i].transform.localPosition = new Vector2(soliders[i].transform.localPosition.x, 0.811834f);
                }
            }
        }
        else if (scene.buildIndex == 6)
        {
            FindObjectOfType<CameraMove>().UpdateCameraMove();
            instance.StartCoroutine("ShowFinalCastleStory");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(LoadAssetBundle());
        //}
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    AssetBundle asset = www.assetBundle;
        //    UnityEngine.Object o = asset.LoadAsset("mx.png");
        //    Instantiate(o);
        //}
    }

    public void StartGame()
    {
        if (passScene.Length == 0)
        {
            passScene = new bool[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < passScene.Length; i++)
                passScene[i] = false;
        }
        defeatedDemon = false;
        KeyboardSetting.isSetting = false;
        KeyboardSetting.skillKeyTable.Clear();
        KeyboardSetting.itemKeyTable.Clear();
        KeyboardSetting.specialKeyTable.Clear();
        SkillTree.skills.Clear();
        StartCoroutine(FadeInScene(1));
    }

    IEnumerator ShowBossForestStory()
    {
        if (!passScene[SceneManager.GetActiveScene().buildIndex])
        {
            Pause(true);
            Player.Instance.transform.position = new Vector2(-6.601418f, -4.184577f);
            string[] stories =
            {
                playerName + ":\n終於走到這一步了，這場風雨也將隨著你生命的消逝而終止！",
                "魔王:\n沒想到會有這一天的到來，看來不得不與你一決勝負了。",
                playerName + ":\n我要消滅你，然後為世界帶來和平！",
                "魔王:\n哼，那就好好讓我盡興吧！",
                "提示:\n現在有4點技能點可以使用，關閉技能視窗後將開始戰鬥！！"
            };
            for (int i = 0; i < stories.Length; i++)
            {
                dialog.ShowDialog(stories[i]);
                yield return null;
                while (!Input.GetKeyDown(KeyCode.Space))
                    yield return null;
            }
            SkillTree skillTree = FindObjectOfType<SkillTree>();
            skillTree.ShowSKillTree();
            KeyboardSetting keyboard = FindObjectOfType<KeyboardSetting>();
            keyboard.Show();
            while (skillTree.IsOpen)
                yield return null;
            skillTree.HideSkillTree();
            keyboard.Hide();
            dialog.HideDialog();
            FindObjectOfType<CameraMove>().UpdateCameraMove();
            Pause(false);
        }
    }

    public IEnumerator ShowBossStory2()
    {
        Pause(true);
        string[] stories =
        {
                "魔王:\n真不愧是你，竟然能和我戰鬥到如此地步。",
                "魔王:\n我就稍微認真起來吧！",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.HideDialog();
        Pause(false);
    }

    public IEnumerator ShowBossStory3()
    {
        Pause(true);
        string[] stories =
        {
                playerName + ":\n這是最後了，你將敗倒在我的劍之下並讓世界變為和平！",
                "魔王:\n嘖，你的力量已經成長到這個地步了嗎...",
                playerName + ":\n為你的罪過付出代價吧！",
                "魔王:\n還沒呢，在風雨未停之際，我的力量就不會消失！",
                "魔王:\n回應我的呼喚吧！緋紅的魔王之雷！！",
                playerName + ":\n什麼！",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.HideDialog();
        Pause(false);
    }

    public IEnumerator ShowBossStory4()
    {
        Pause(true);
        string[] stories =
        {
                "魔王:\n嗚...",
                playerName + ":\n永別了，勝利是屬於我的，屬於世界的和平！",
                "魔王:\n在我死前，告訴你一個真相吧。",
                playerName + ":\n有什麼遺言想說的嗎？",
                "魔王:\n在那之前，先問你一個問題吧，你是受國王所託來打倒我的嗎？",
                playerName + ":\n我是流浪的劍客，用我自己的意志決定消滅邪惡的。",
                "魔王:\n是嗎，那告訴你吧，真正的惡魔，是那個國王啊！",
                playerName + ":\n作為你的遺言還滿可笑的。",
                "魔王:\n像你這樣的強者卻感受不到嗎，森林裡的怪物們是這麼的弱小，越靠近王城的怪物卻有著更邪惡的氣息。",
                playerName + ":\n.........",
                "魔王:\n睜大自己的雙眼看清楚真正的黑暗是誰吧！你們所居住的，所保護的國家，才是真正黑暗的起源啊！",
                playerName + ":\n那這場風雨呢？你所散發的魔王的氣息呢？",
                "魔王:\n你看起來很想要打一場，於是我就配合你了。",
                playerName + ":\n.........",
                "魔王:\n現在回頭還來的及，作為魔王的我是不會這麼輕易死掉的！去做自己真正該做的事吧。",
                playerName + ":\n我知道了......。",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.HideDialog();
        defeatedDemon = true;
        StartCoroutine(FindObjectOfType<DemonBoss>().Die());
        Destroy(FindObjectOfType<WeatherMakerScript>().gameObject);
        passScene[SceneManager.GetActiveScene().buildIndex - 1] = true;
        Pause(false);
    }

    public IEnumerator ShowPlainStory1()
    {
        Pause(true);
        string[] stories =
        {
                playerName + ":\n再往前走就抵達城堡了，如果真如魔王所說，國王是邪惡的話，前方的怪物肯定會變得更加兇暴。",
                playerName + ":\n城裡的警備應該也相當森嚴，絕對不能貿然闖入。",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.HideDialog();
        passScene[SceneManager.GetActiveScene().buildIndex - 1] = true;
        Pause(false);
    }

    public IEnumerator ShowCastleStory1()
    {
        Pause(true);
        string[] stories =
        {
                playerName + ":\n一邊消滅邪惡的士兵，一邊前進吧！",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.HideDialog();
        passScene[SceneManager.GetActiveScene().buildIndex - 1] = true;
        Pause(false);
    }

    public IEnumerator ShowCastleStory2()
    {
        Pause(true);
        FindObjectOfType<CameraMove>().stop = true;
        string[] stories =
        {
                "???:\n歡迎回來。",
                "皇后:\n聽說你獨自一人消滅了森林裡的魔王呢，國王感到非常的開心哦。",
                playerName + ":\n是嗎...",
                "皇后:\n如此一來就沒人有能夠阻止國王了呢，趕緊進來讓國王慶祝你的大功勞吧！",
                playerName + ":\n少裝蒜了！你們的陰謀我已經全部都知道了！",
                "皇后:\n陰謀？你指的是什麼呢？",
                playerName + ":\n魔王已經全都告訴我了，我也自己感受到了，你們所散發出的邪惡氣息！！",
                "皇后:\n你怎麼會相信魔王所說的話呢？我和國王都是為了世界的美好而拚命努力的呀！請你一定要好好想清楚，究竟誰才是你的夥伴。",
                "皇后:\n你所感受到的邪惡氣息，是魔王臨死前為了陷害王國而留下的呀，你感覺不出來嗎？",
                playerName + ":\n那為什麼史萊姆和士兵們變得這麼兇暴？",
                "皇后:\n肯定也是受魔王的影響。",
                playerName + ":\n......",
                "皇后:\n好好想想吧，是要與我和國王為敵，還是相信我們並一起保護這個世界呢？",
            };
        dialog.ShowDialog(stories[0]);
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        dialog.HideDialog();
        GameObject s = Instantiate(balloon, FindObjectOfType<Player>().transform);
        float t = 0;
        // Wait for balloon.
        while (true)
        {
            if (t == 0.75f)
                break;
            t += Time.unscaledDeltaTime;
            t = Mathf.Min(t, 0.75f);
            yield return null;
        }
        Destroy(s);
        // Camera move to queen.
        GameObject queen = GameObject.Find("Queen");
        queen.transform.localScale = new Vector2(1, 1);
        float x = Camera.main.transform.position.x;
        for (int i = 0; i < 80; i++)
        {
            Camera.main.transform.Translate((-0.13f - x) / 80, 0, 0);
            yield return null;
        }
        while (true)
        {
            if (t == 1.0f)
                break;
            t += Time.unscaledDeltaTime;
            t = Mathf.Min(t, 1.0f);
            yield return null;
        }
        // Queen moveforward.
        queen.transform.GetChild(0).GetComponent<Animator>().SetBool("Run", true);
        while (queen.transform.position.x > -18.5f)
        {
            queen.transform.Translate(-0.1f, 0, 0);
            Camera.main.transform.Translate(-0.1f, 0, 0);
            yield return null;
        }
        queen.transform.GetChild(0).GetComponent<Animator>().SetBool("Run", false);
        while (Camera.main.transform.position.x > -26.49f)
        {
            Camera.main.transform.Translate(-0.1f, 0, 0);
            yield return null;
        }
        // Story continue.
        for (int i = 1; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        // Show selection.
        dialog.ShowTwoSelection("相信國王和皇后", "相信魔王");
        while (dialog.SelectionResult == -1)
            yield return null;
        // Choose King and queen, get ending1.
        if (dialog.SelectionResult == 1)
        {
            StartCoroutine("EndStory1");
        }
        // Choose demon, get ending2, go to defeat king and queen.
        else if (dialog.SelectionResult == 2)
        {
            StartCoroutine("ShowCastleStory3");
        }
    }

    public IEnumerator EndStory1()
    {
        Pause(true);
        Character queen = GameObject.Find("Queen").GetComponent<Character>();
        queen.enabled = false;
        Character player = Player.Instance;
        string[] stories =
        {
                playerName + ":\n有道理。果然一切都是魔王造成的，消滅他之後世界肯定會漸漸變美好的。",
                "皇后:\n你這麼聰明真是太好了，現在立刻隨我進王宮慶祝吧！",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        // Player move forward
        player.GetComponentInChildren<Animator>().SetBool("Run", true);
        dialog.HideDialog();
        while (player.transform.position.x < -26.53007f)
        {
            player.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        // Player move forward and camera follow.
        while (player.transform.position.x < -20.96f)
        {
            player.transform.Translate(0.1f, 0, 0);
            Camera.main.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        // Plater, queen, camera move and play scene fade in.
        StartCoroutine("FadeInScene", -1);
        player.GetComponentInChildren<Animator>().SetBool("Run", true);
        queen.GetComponentInChildren<Animator>().SetBool("Run", true);
        while (player.transform.position.x < -8.07f)
        {
            player.transform.Translate(0.1f, 0, 0);
            queen.transform.localScale = new Vector2(-1, 1);
            queen.transform.Translate(0.1f, 0, 0);
            Camera.main.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        audioSource.clip = demonEndBGM;
        audioSource.Play();

        stories = new string[]
        {
            playerName + ":\n就這樣，我選擇相信了皇后的話，認為這個魔王已經被消滅的世界將會變得和平美麗。",
            playerName + ":\n國王舉辦了相當大的派對祝賀我的成功，也給了我數不盡的財富。",
            playerName + ":\n然而我錯了，這一切都是國王的陰謀，他利用魔王掩蓋了自己的邪惡，祕密的籌畫著統治世界的計畫。",
            playerName + ":\n發現了一切真相的我，除了服從國王的命令以外，唯有死亡能夠逃離這悲慘的命運。國王的計畫成功了，我也已經無法回頭了。",
            playerName + ":\n一切都結束了...。",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        StartCoroutine("ShowEnd", 0);
        // Call Game over.
        passScene[SceneManager.GetActiveScene().buildIndex - 1] = true;
    }

    IEnumerator FadeInScene(int sceneIndex = -1)
    {
        fadeInImage.color = new Color(0, 0, 0, 0);
        while (fadeInImage.color.a < 1)
        {
            fadeInImage.color += new Color(0, 0, 0, 0.01f);
            yield return null;
        }
        if (sceneIndex != -1)
            SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator FadeOutScene()
    {
        while (fadeInImage.color.a > 0)
        {
            fadeInImage.color -= new Color(0, 0, 0, 0.02f);
            yield return null;
        }
    }

    IEnumerator ShowCastleStory3()
    {
        Pause(true);
        string[] stories =
        {
                playerName + ":\n我不會被你們騙的！親自和魔王交過手的我才是最清楚的，真正的邪惡就是你們！",
                "皇后:\n沒想到特地給你留一條活路你卻不領情呢。",
                playerName + ":\n我現在就要討伐你和國王！",
                "皇后:\n哈哈哈哈哈哈哈...討伐我和國王？先看看你自己的處境再說吧！",
                playerName + ":\n什麼？",
                "皇后:\n難道你以為我會什麼都不做就出來迎接你嗎？別傻了，我早就做好當你背叛的那個瞬間就要除掉你的準備了！",
                playerName + ":\n嘖...",
                "皇后:\n那我就先走了，我會好好向國王報備，讓他給你準備一場盛大的葬禮的。",
                "皇后:\n哇哈哈哈哈哈...哇哇哈哈哈...咳咳...哈哈...",
                playerName + ":\n站住！"
            };
        // Soldiers jump out.
        for (int i = 0; i < 4; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.HideDialog();
        GameObject[] soliders = GameObject.FindGameObjectsWithTag("Enemy");
        while (soliders[0].transform.localPosition.y > 0.811834f)
        {
            for (int i = 0; i < soliders.Length; i++)
                soliders[i].transform.Translate(0, -9.81f * Time.unscaledDeltaTime * 2.5f, 0, Space.Self);
            yield return null;
        }
        // Ballon.
        GameObject s = Instantiate(balloon, FindObjectOfType<Player>().transform);
        float t = 0;
        // Wait for balloon.
        while (true)
        {
            if (t == 0.75f)
                break;
            t += Time.unscaledDeltaTime;
            t = Mathf.Min(t, 0.75f);
            yield return null;
        }
        Destroy(s);
        for (int i = 5; i < 6; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.ShowDialog(stories[7]);
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        StartCoroutine("MoveQueen");
        dialog.ShowDialog(stories[8]);
        yield return null;
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        dialog.ShowDialog(stories[9]);
        yield return null;
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        dialog.HideDialog();
        Player.Instance.playerMove.animator.SetBool("Run", true);
        while (Player.Instance.transform.position.x < -27.23f)
        {
            Player.Instance.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        soliders[2].GetComponentInChildren<Animator>().SetTrigger("Attack");
        t = 0;
        while (true)
        {
            if (t == 0.1f)
                break;
            t += Time.unscaledDeltaTime;
            t = Mathf.Min(t, 0.1f);
            yield return null;
        }
        Player.Instance.playerMove.animator.SetBool("Run", false);
        Player.Instance.transform.Translate(-4.0f, 0, 0);
        dialog.ShowDialog(playerName + "\n嗚...");
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        yield return null;
        dialog.ShowDialog(playerName + "\n沒辦法了，想盡辦法闖入王宮吧！必須和國王與皇后決一了斷！");
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        dialog.HideDialog();
        for (int i = 0; i < soliders.Length; i++)
        {
            soliders[i].GetComponent<Monster>().enabled = true;
            soliders[i].GetComponent<Soldier>().enabled = true;
        }
        FindObjectOfType<CameraMove>().stop = false;
        passScene[SceneManager.GetActiveScene().buildIndex - 1] = true;
        Pause(false);
    }

    IEnumerator ShowFinalCastleStory()
    {
        Pause(true);
        Player.Instance.playerMove.animator.SetBool("Run", true);
        while (Player.Instance.transform.position.x < -6.6f)
        {
            Player.Instance.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        Player.Instance.playerMove.animator.SetBool("Run", false);
        FindObjectOfType<CameraMove>().stop = true;
        while (Camera.main.transform.position.x < -1.9f)
        {
            Camera.main.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        string[] stories =
        {
                "國王:\n呦，歡迎來到我的王宮，消滅魔王的勇士啊。",
                playerName +  "\n國王！",
                "國王:\n呵呵，雖然魔王會說出我的秘密的確讓我感到驚訝，但你這麼輕易就相信魔王也是讓我很頭疼呢。",
                playerName +  "\n今天我就會在這裡了結你，為世界帶來真正的和平！",
                "國王:\n和平？你所謂的和平究竟是什麼？你真的是為了和平而戰嗎？",
                playerName +  "\n......",
                "國王:\n像你這樣的偽善者我看得太多了，戰鬥到最後你也只會淪落到死亡的下場而已。",
                "國王:\n在你消滅魔王的那個瞬間，你早就已經與世界為敵了！",
                playerName +  "\n即使如此，我仍要為了自己所相信的事物而戰！我不會再被你欺騙了！",
                "國王:\n只是你自己太過愚蠢罷了，一個人流浪至今卻還會受他人影響，你根本擺脫不了自己對自己的束縛！拋開自己毫無價值的自尊吧，現在反悔還來的及。",
                playerName +  "\n愚蠢也好束縛也好，現在乘載著魔王意念的我，要和你決一死戰！",
                "皇后:\n國王，不必和他廢話了，動手處刑吧，徹底破壞掉這個王國的叛徒吧！",
                "國王:\n那麼，我現在宣布，將由我國王本人親自對" + playerName + "處以死刑，現在開始！",
            };
        for (int i = 0; i < stories.Length - 1; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        StartCoroutine("MoveQueen2");
        yield return null;
        dialog.ShowDialog(stories[stories.Length - 1]);
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        dialog.HideDialog();
        FindObjectOfType<CameraMove>().stop = false ;
        audioSource.clip = finnalBattle;
        audioSource.Play();
        Pause(false);
    }

    IEnumerator ShowKingBattleStory()
    {
        Pause(true);
        string[] stories =
        {
                "國王:\n沒想到你的力量竟如此強大...",
                "皇后:\n國王，再這樣下去我們都會被他幹掉的！",
                "國王:\n果然不能小看足以打倒魔王的人呢。",
                playerName +  "\n廢話少說，納命來！",
                "國王:\n皇后，大砲預備！",
                "皇后:\n遵命！！",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.HideDialog();
        Pause(false);
    }

    IEnumerator ShowKingBattleStory2()
    {
        Pause(true);
        string[] stories =
        {
                "皇后:\n國王啊啊啊...",
                "國王:\n嗚...",
                playerName +  "\n受死吧，用我手上的寶劍肅清邪惡！",
                playerName +  "\n輪到你了，少了國王你就什麼都做不了了！",
                "皇后:\n可惡啊啊...究竟誰才是真正的邪惡啊啊...假借正義之名的你，屠殺生命的你才是真正的邪惡啊啊...",
                playerName +  "\n...",
                playerName +  "\n我只做了我應該做的事而已。",
            };
        for (int i = 0; i < 3; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        Player.Instance.playerMove.animator.SetBool("Run", true);
        while (Player.Instance.transform.position.x < 2.28f)
        {
            Player.Instance.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        Player.Instance.playerMove.animator.SetBool("Run", false);
        FindObjectOfType<CameraMove>().stop = true;
        for (int i = 3; i < 7; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        Player.Instance.playerMove.animator.SetBool("Run", true);
        while (Player.Instance.transform.position.x < 4.96f)
        {
            Player.Instance.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        Player.Instance.playerMove.animator.SetBool("Run", false);
        Player.Instance.playerMove.animator.SetInteger("Attack", 1);
        float t = 0;
        // Wait for balloon.
        while (true)
        {
            if (t == 0.35f)
                break;
            t += Time.unscaledDeltaTime;
            t = Mathf.Min(t, 0.35f);
            yield return null;
        }
        dialog.HideDialog();
        Player.Instance.playerMove.animator.SetInteger("Attack", 0);
        audioSource.clip = kingEndBGM;
        audioSource.Play();
        StartCoroutine("FadeInScene", -1);
        while (true)
        {
            if (t == 2f)
                break;
            t += Time.unscaledDeltaTime;
            t = Mathf.Min(t, 2f);
            yield return null;
        }
        stories = new string[]
        {
            playerName + ":\n就這樣，我選擇相信了魔王，也成功消滅了偷偷策畫著邪惡計畫的國王與皇后。",
            playerName + ":\n在城堡中發現了他們計畫的痕跡，國王也利用了魔王的氣息隱藏了自己的陰謀。",
            playerName + ":\n如果再晚一些的話，這個世界就會完全被國王統治。",
            playerName + ":\n而我，完成了自己的任務，再一次踏上一個人的旅途。",
            playerName + ":\n等待著下一次的開始。",
            };
        for (int i = 0; i < stories.Length; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        StartCoroutine("ShowEnd", 1);
        Destroy(FindObjectOfType<Queen>().gameObject);
        Destroy(FindObjectOfType<King>().gameObject);
    }

    IEnumerator MoveQueen()
    {
        GameObject queen = GameObject.Find("Queen");
        queen.transform.localScale = new Vector2(-1, 1);
        queen.transform.GetChild(0).GetComponent<Animator>().SetBool("Run", true);
        while (queen.transform.position.x < -8.57f)
        {
            queen.transform.Translate(+0.1f, 0, 0);
            yield return null;
        }
        Destroy(queen);
    }

    IEnumerator MoveQueen2()
    {
        GameObject queen = GameObject.Find("Queen");
        queen.transform.localScale = new Vector2(-1, 1);
        queen.transform.GetChild(0).GetComponent<Animator>().SetBool("Run", true);
        while (queen.transform.position.x < 5.97f)
        {
            queen.transform.Translate(0.1f, 0, 0);
            yield return null;
        }
        queen.transform.localScale = new Vector2(1, 1);
        queen.transform.GetChild(0).GetComponent<Animator>().SetBool("Run", false);
    }

    IEnumerator ShowEnd(int type)
    {
        string s = "劍士傳說 The End";
        string ss = "";
        for (int i = 0; i < s.Length; i++)
        {
            ss += s[i];
            dialog.ShowDialog(ss);
            float t = 0;
            while (true)
            {
                if (t == 0.2f)
                    break;
                t += Time.unscaledDeltaTime;
                t = Mathf.Min(t, 0.2f);
                yield return null;
            }
        }
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        dialog.ShowDialog(s + "\n要儲存遊玩記錄嗎？");
        dialog.ShowTwoSelection("儲存", "回到標題");
        while (dialog.SelectionResult == -1)
            yield return null;
        if (dialog.SelectionResult == 1)
        {
            PlayerRecord playerRecord = new PlayerRecord();
            playerRecord.finishTime = DateTime.Now.ToString();
            playerRecord.endType = type;
            playerRecord.totalCostTime = ((int)Time.realtimeSinceStartup / 60).ToString();
            playerRecord.totalCostTime += " : ";
            playerRecord.totalCostTime += (((int)(Time.realtimeSinceStartup * 100) % 60) / 100.0f).ToString();
            playerRecord.totalCostTime += "s";
            string json = File.ReadAllText(Application.dataPath + "/Resources/record.json");
            if (json.Length == 0)
                playerRecords = new List<PlayerRecord>();
            else
                playerRecords = new List<PlayerRecord>(JsonHelper.FromJson<PlayerRecord>(json));
            playerRecords.Add(playerRecord);
            json = JsonHelper.ToJson(playerRecords.ToArray(), true);
            File.WriteAllText(Application.dataPath + "/Resources/record.json", json);
        }
        Destroy(Camera.main.gameObject);
        Destroy(Player.Instance.gameObject);
        dialog.HideDialog();
        Pause(false);
        audioSource.Stop();
        SceneManager.LoadScene(0);
    }

    IEnumerator HandlePlayerDie()
    {
        Pause(true);
        AudioClip clip = audioSource.clip;
        audioSource.clip = demonEndBGM;
        audioSource.Play();
        StartCoroutine("FadeInScene", -1);
        float t = 0;
        // Wait for balloon.
        while (true)
        {
            if (t == 1.5f)
                break;
            t += Time.unscaledDeltaTime;
            t = Mathf.Min(t, 1.5f);
            yield return null;
        }
        string[] stories =
        {
                playerName +  "\n...太大意了...",
                playerName +  "\n這麼快就被這個世界給淘汰了嗎...",
            };
        for (int i = 0; i < 2; i++)
        {
            dialog.ShowDialog(stories[i]);
            yield return null;
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;
        }
        dialog.ShowDialog("\n要重新開始嗎？");
        dialog.ShowTwoSelection("恢復HP、MP100%後從死掉的地方繼續", "回到標題");
        while (dialog.SelectionResult == -1)
            yield return null;
        if (dialog.SelectionResult == 1)
        {
            Player.Instance.Damage(-Player.Instance.MaxHP);
            Player.Instance.CostMp(-Player.Instance.MaxMP);
            dialog.HideDialog();
            StartCoroutine("FadeOutScene", -1);
            t = 0;
            // Wait for balloon.
            while (true)
            {
                if (t == 1.5f)
                    break;
                t += Time.unscaledDeltaTime;
                t = Mathf.Min(t, 1.5f);
                yield return null;
            }
            audioSource.clip = clip;
            audioSource.Play();
            Pause(false);
        }
        else if (dialog.SelectionResult == 2)
        {
            Destroy(Camera.main.gameObject);
            Destroy(Player.Instance.gameObject);
            dialog.HideDialog();
            Pause(false);
            audioSource.Stop();
            Pause(false);
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator LoadAssetBundle()
    {
        if (www == null)
        {
            Debug.Log("Path = " + Application.dataPath + "/AssetBundles/Test/hspecial");
            www = new WWW(Application.dataPath + "/Resources/items");
            //AssetBundle.load
            yield return www;
            //handler = new DownloadHandlerAssetBundle(www.url, uint.MaxValue);
            //www.downloadHandler = handler;
            //yield return www.Send();
        }
    }
}
