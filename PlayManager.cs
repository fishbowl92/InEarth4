using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public Transform[] backGrounds;
    public int[] backGroundsPivot;
    public Player player;
    public Transform myCamera;

    public List<Enemy> Enemys = new List<Enemy>();
    public List<Enemy> offEnemys = new List<Enemy>();
    static public PlayManager Instans;
    public GameManager gm;

    public int[] seed = new int[3];
    public void Awake()
    {
        Instans = this;
        if(GameManager.Instans) gm = GameManager.Instans;
        if (!myCamera) myCamera = Camera.main.transform;
        gm.getSeed();

        GameObject gTemp = Instantiate(gm.Characters[gm.selectCharacter], this.transform);
        player = gTemp.transform.GetComponent<Player>();
        player.pm = this;
        player.bm = transform.GetComponent<BulletManager>();
        buffEffect = player.transform.Find("buffEffect").GetComponent<SpriteRenderer>();

        if (gm.checkStage("noneEvasionStack"))
        {
            evasionCharge = 0;
            evasionImg.fillAmount = 0;
        }
        else
        {
            evasionCharge = player.getEvasionMaxStack();
            evasionImg.fillAmount = 1;
        }
        evasionCount.text = evasionCharge.ToString();
        dmgText.GetComponent<MeshRenderer>().sortingLayerName = "Effect";
        dmgText.GetComponent<MeshRenderer>().sortingOrder = 2;
        guardEffect.transform.GetChild(1).GetComponent<MeshRenderer>().sortingLayerName = "Effect";
        guardEffect.transform.GetChild(1).GetComponent<MeshRenderer>().sortingOrder = 2;
        textBox.transform.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Effect";
        textBox.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 5;

        pregoressSetting();
    }
    public AudioSource[] guardSound;
    public AudioClip[] guardClup;
    public int goCount;
    public void makeGuardFont(bool parfact)
    {
        if (parfact)
        {
            perfactGuardCount++;
            if(gm.selectCharacter == 1) player.mpAdd(1);
        }
        else if (!parfact && gm.selectCharacter == 2)
        {
            evasionSuccessCount++;
            if(gm.selectCharacter == 2 && evasionSuccessCount % 3 == 0) player.mpAdd(1);
        }
        guardSound[goCount].clip = guardClup[(parfact) ? 3 : 2];
        guardSound[goCount++].Play();
        goCount %= 5;
        GameObject gTemp = Instantiate(guardEffect);
        gTemp.transform.GetChild(1).GetComponent<TextMesh>().text = (parfact) ? "PERFECT!" : "GUARD";
        gTemp.transform.position = player.transform.position + new Vector3(-0.6f, +7);
        Destroy(gTemp, 2);
        gTemp.SetActive(true);
    }
    public void Start()
    {
        lines = new List<LockNode>();
        Time.timeScale = gm.checkStage("gameSpeed") ? 1.2f : 1;
        release();
        StartCoroutine(buffManager());
        StartCoroutine(EnemyHpCheck());
    }
    public GameObject pause;
    public void stopGame()
    {
        pause.SetActive(true);
        Semaphore = true;
        Time.timeScale = 0;
    }
    public void resume()
    {
        Time.timeScale = gm.checkStage("gameSpeed") ? 1.2f : 1;
        Semaphore = false;
        pause.SetActive(false);
    }
    public void exitGame()
    {
        Time.timeScale = 1;
        LoadingSceneManager.LoadScene("Main");
    }
    public void reStartGame()
    {
        Time.timeScale = 1;
        gm.myGold = 0;
        LoadingSceneManager.LoadScene("Play");
    }
    public void startGame(Transform t)
    {
        Destroy(t.gameObject);
        player.anim.SetTrigger("Jump");
    }
    public GameObject gameEnd;
    public void oepnGameEndPenal()
    {
        StartCoroutine(calculateScore(false));
    }
    public int criCount;
    public int overKill;
    public int evasionSuccessCount;
    public int perfactGuardCount;
    public int hitCount;
    IEnumerator calculateScore(bool die)
    {
        if (die) yield return new WaitForSeconds(2.5f);
        gameEnd.SetActive(true);
        Semaphore = true;
        Time.timeScale = 1;
        WaitForSeconds ws = new WaitForSeconds(0.03f);
        int score = 500 * gm.stageNum + 5 * criCount + 50 * overKill
            + 10 * evasionSuccessCount + 50 * perfactGuardCount - 2 * hitCount;
        string sTemp =
              "난이도 점수 :  500 x" + (die ? 0 : gm.stageNum) +
            "\n치명타 점수 :    5 x" + criCount +
            "\n오버   킬   :   50 x" + overKill +
            "\n회피   점수 :   10 x" + evasionSuccessCount +
            "\n퍼펙트 가드 :   50 x" + perfactGuardCount +
            "\n피격   수   :   -2 x" + hitCount +
            "\n추가   보상 : " + score  + " / 1000 = "+ score/1000;
        TextMeshProUGUI text = gameEnd.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        text.text = "";

        for (int i = 0; i < sTemp.Length; ++i)
        {
            text.text += sTemp[i];
            if (sTemp[i] != ' ' && sTemp[i] != '\n') gm.btnSound[2].Play();
            else yield return ws;
            yield return ws;
        }
        Transform tTemp = gameEnd.transform.GetChild(1).GetChild(0).GetChild(6);
        for (int i = 0; i < (die ? 1 : Mathf.Clamp(score /1000 + 2, 0, 7)); ++i)
        {
            yield return new WaitForSeconds(0.5f); 
            gm.btnSound[4].Play();
            Transform t = tTemp.GetChild(i);
            t.gameObject.SetActive(true);
            int gab;
            switch (i)
            {
                case 0:
                    //무조건 돈
                    t.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x" + gm.myGold;
                    continue;
                case 1:
                    //무조건 스킬
                    gab = Random.Range(1, gm.allSkillData.Count);
                    break;
                default:
                    if(Random.Range(0, 5) == 0)
                    {
                        //스킬
                        gab = Random.Range(1, gm.allSkillData.Count);
                    }
                    else
                    {
                        //장비
                        gab = Random.Range(0, gm.allItemData.Count);
                        gab = -gab - 1;
                    }
                    break;
            }
            if(gab < 0)
            {
                ItemData id = gm.allItemData[-gab - 1];
                int lev = 1;
                for (int j = 0; j < gm.stageNum / 5; ++j) if (Random.Range(0, 5) == 0) lev++;
                t.GetChild(0).GetComponent<Image>().sprite = id.itemImage;
                t.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + lev;
            }
            else
            {
                skillData sd = gm.allSkillData[gab];
                if (gm.playerHaveSkill.Contains(sd))
                {
                    t.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x" + 30;
                    gm.myGold += 30;
                }
                else
                {
                    t.GetChild(0).GetComponent<Image>().sprite = sd.skillImage;
                }
            }
            Vector3 vTemp = t.position;
            vTemp.z = gab;
            t.position = vTemp;
        }
    }
    public void showItemInfo(Transform t)
    {
        Transform tTemp = gameEnd.transform.GetChild(1).GetChild(0).GetChild(6);
        for (int i = 0; i < tTemp.childCount; ++i)
        {
            Transform p = tTemp.GetChild(i);
            if(p == t)
            {
                if (p.GetChild(2).gameObject.activeSelf)
                {
                    gameEnd.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    p.GetChild(2).gameObject.SetActive(true);
                    gameEnd.transform.GetChild(2).gameObject.SetActive(true);
                    if (t.position.z < -0.5f)
                    {
                        int gab = (int)(-t.position.z - 0.7f);
                        ItemData id = gm.allItemData[gab];
                        gameEnd.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = id.itemName;
                        gameEnd.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = id.info;
                    }
                    else
                    {
                        int gab = (int)(t.position.z + 0.3f);
                        skillData sd = gm.allSkillData[gab];
                        gameEnd.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = sd.buffName;
                        gameEnd.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = sd.info;
                    }
                }
            }
            else
            {
                p.GetChild(2).gameObject.SetActive(false);
            }
        }

    }
    public int testNum = 1;
    public void Update()
    {
        movePlayer();
        checkBuffTime();
        Vector3 cemPoint = Vector2.Lerp(myCamera.position, player.transform.localPosition, Time.deltaTime * 5);
        cemPoint.z = -90;
        myCamera.position = cemPoint;
        if (Input.GetKeyUp(KeyCode.J))
        {
            //A버튼 테스트
            player.GetAButton();
        }
        EvasionCoolTimeCheck();
        if (Input.GetKeyUp(KeyCode.K))
        {
            //B버튼 테스트
            player.GetBButton();
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            //몹생성 테스트
            instantsEnemy(1);
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            //몹생성 테스트
            gm.changeWeapon(gm.textWeapon[testNum]);
            testNum = testNum + 1;
            testNum %= 4;
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            oepnGameEndPenal();
        }
        /*
        secTimer += Time.deltaTime;
        if(secTimer > 1f)
        {
            secTimer = 0;
            for(int j = 0; j < 2; ++j)
            {
                for (int i = 0; i < buffList[j].Count; ++i)
                {
                    buffList[j][i].time -= 1f;
                    buffList[j][i].myUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = buffList[j][i].time.ToString("F0");
                    if (buffList[j][i].time <= 0f)
                    {
                        buffList[j][i].myUI.parent = buffInvGrid[j];
                        buffList[j].RemoveAt(i);
                        buffApplyEvent.Invoke(buffList[j][i].buffSort, 0, -buffList[j][i].val);
                    }
                }
            }
        }*/

        for (int i = Enemys.Count - 1; i >= 0; --i)
        {
            Enemy e = Enemys[i];
            if(e.hp <= 0)
            {
                if (!e.gameObject.activeSelf)
                {
                    Enemys.Remove(e);
                    e.moveSpeed(-500);
                    offEnemys.Add(e);
                    if (target == e)
                    {
                        showEnemyHp.SetActive(false);
                        target = null;
                    }
                }
                else if (!e.die)
                {
                    if (-e.hp > player.atk) overKill++;
                    e.die = true;
                    e.superArmor = true;
                    e.Delay = Time.time + 8;
                    e.anim.SetTrigger("Die");
                }else if(e.Delay < Time.time)
                {
                    e.Delay = Time.time + 8;
                    e.anim.SetTrigger("Die");
                }
            }
            else
            {
                bool stunState = e.stunDelay > Time.time;
                e.stun.SetActive(stunState);
                if (stunState) e.sua = false;
                e.checkNearEnemy();
                if (!stunState && e.Delay < Time.time && !e.sua)
                {
                    if (e.farToPlayer > e.ed.distance)
                    {
                        e.move();
                    }
                    else
                    {
                        e.atk();
                    }
                }
            }
        }

        // 라인 그리기 수정
        if (unlocking)
        {
            Vector3 mousePos = canvas.transform.InverseTransformPoint(Input.mousePosition);

            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, Vector3.Distance(mousePos, canvas.transform.InverseTransformPoint(nodeOnEdit.transform.position)));

            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, (mousePos - canvas.transform.InverseTransformPoint(nodeOnEdit.transform.position)).normalized);
        }
    }
    public GameObject showEnemyHp;
    public Enemy target;
    public Image enemyHeadImg;
    public Image[] enemyHpFill;
    public bool changeTarget;
    public float startGab, endGab;

    public void getNewTarget(Enemy _e, float _start, float _end)
    {
        _end = Mathf.Clamp01(_end);
        if(target != null && target == _e)
        {
            startGab = enemyHpFill[0].fillAmount;
        }
        else
        {
            target = _e;
            startGab = _start;
        }
        endGab = _end;
        changeTarget = true;
    }
    IEnumerator EnemyHpCheck()
    {
        while (true)
        {
            yield return new WaitUntil(() => changeTarget);
            showEnemyHp.SetActive(true);
            changeTarget = false;
            if(startGab > endGab)
            {
                enemyHeadImg.sprite = target.ed.headImg;
                enemyHpFill[0].fillAmount = startGab;
                enemyHpFill[1].fillAmount = startGab;
                float gab = 0;
                while (!changeTarget && gab < 1)
                {
                    gab += 4 * Time.deltaTime;
                    enemyHpFill[0].fillAmount = Mathf.Lerp(startGab, endGab, gab);
                    yield return null;
                }
                enemyHpFill[0].fillAmount = endGab;
                yield return fixWs;
                if(!changeTarget) yield return fixWs;
                enemyHpFill[1].fillAmount = endGab;
            }
            if (endGab <= 0) showEnemyHp.SetActive(false);
        }
    }

    public GameObject textBox;
    public void startTextBox(Transform t, string text)
    {
        textBox.transform.parent = t;
        textBox.transform.localPosition = Vector3.zero;
        TextMesh tm = textBox.transform.GetChild(0).GetComponent<TextMesh>();
        tm.text = "";
        textBox.SetActive(true);
        StartCoroutine(textBoxShow(text, tm));
    }
    IEnumerator textBoxShow(string t, TextMesh tm)
    {
        WaitForSeconds ws = new WaitForSeconds(0.07f);
        for (int i = 0; i < t.Length; ++i)
        {
            tm.text += t[i];
            if (t[i] != ' ' && t[i] != '\n') gm.btnSound[5].Play();
            else yield return null;
            yield return ws;
        }
    }

    public GameObject newGold;
    public List<GameObject> getGolds = new List<GameObject>();
    public TextMeshProUGUI goldCount;
    public void getGold(Vector3 point, int banbuk)
    {
        for(int i = 0; i < banbuk; ++i)
        {
            GameObject gTemp = null;
            if(getGolds.Count > 0)
            {
                gTemp = getGolds[0];
                getGolds.RemoveAt(0);
            }
            if (gTemp == null) gTemp = Instantiate(newGold);
            gTemp.transform.position = point;
            gTemp.SetActive(true);
            gTemp.GetComponent<GetGold>().StartCoroutine("startGetGold");
        }
    }
    public void goldPlus(int a)
    {
        gm.myGold += a;
        goldCount.text = "x" + gm.myGold;
    }

    public struct spwanData
    {
        public int point;
        public int tear;
        public Transform pin;
        public spwanData(int p, int t, Transform pp)
        {
            point = p;
            tear = t;
            pin = pp;
        }
    }
    public List<spwanData> spd;
    public Sprite[] pPointImg;
    public void pregoressSetting()
    {
        spd = new List<spwanData>();
        Transform tTemp = myProgressPoint.GetChild(0);
        for (int i = 0; i < 15;)
        {
            int gab = Random.Range(0, 44) * 5 + 15;
            if (spd.Find(item => item.point == gab).point > 0)
            {
                //이미 있음
                continue;
            }
            else
            {
                ++i;
                GameObject newBar = Instantiate(tTemp.gameObject, myProgressPoint.transform);
                spd.Add(new spwanData(gab
                    , i < 10 ? 0 : 1
                    , newBar.transform
                    ));
                newBar.GetComponent<RectTransform>().anchoredPosition = 
                    Vector3.Lerp(progressBuoy[0].anchoredPosition, progressBuoy[1].anchoredPosition,
                    ((float)gab) / 250);
                newBar.GetComponent<Image>().sprite = pPointImg[0];
                newBar.transform.GetChild(0).GetComponent<Image>().sprite = pPointImg[i < 10 ? 1 : 2];
            }
        }
        tTemp.GetChild(0).GetComponent<Image>().sprite = player.transform.Find("head").GetComponent<SpriteRenderer>().sprite;
        myProgressPoint = tTemp.GetComponent<RectTransform>();
        spd.Sort(delegate (spwanData x, spwanData y)
        {
            if (x.point < y.point) return -1;
            else return 1;
        });
    }
    public RectTransform[] progressBuoy;
    public RectTransform myProgressPoint;
    public Image progressFill;
    public void progressCheck()
    {
        float nowProgress = Mathf.Clamp01(player.transform.position.x / 250);
        Vector3 nextPoint = Vector3.Lerp(progressBuoy[0].anchoredPosition, progressBuoy[1].anchoredPosition, nowProgress);
        myProgressPoint.anchoredPosition = nextPoint;
        progressFill.fillAmount = nowProgress;
        if(spd.Count > 0 && spd[0].point <= player.transform.position.x)
        {
            Destroy(spd[0].pin.gameObject);
            instantsEnemy(spd[0].tear);
            spd.RemoveAt(0);
        }
    }
    public TextMeshProUGUI evasionCount;
    public Image evasionImg;
    public int evasionCharge;
    public float evasionTimer;
    public void EvasionCoolTimeCheck()
    {
        if (evasionCharge < player.getEvasionMaxStack())
        {
            evasionTimer += Time.deltaTime;
            evasionImg.fillAmount = (evasionCharge > 0) ? 1 : evasionTimer / player.getEvasionCoolTime();
            if (evasionTimer >= player.getEvasionCoolTime())
            {
                evasionCharge++;
                evasionTimer = 0;
                evasionCount.text = evasionCharge.ToString();
            }
        }
    }

    public GameObject enemyPrefab;
    public int count;
    public void instantsEnemy(int t)
    {
        if(t < 2)
        {
            t = Mathf.Clamp(t, 0, 1);
            if(t == 0)
            {
                int gab = Random.Range((gm.checkStage("mobVolume") ? 2 : 1) , 4);
                for(int i = 0; i < gab; ++i)
                {
                    Enemy e = returnNewEnemy();
                    e.anim.speed = gm.checkStage("nomalMobSpeedUp") ? 0.85f :0.7f;
                    e.transform.position = player.transform.localPosition + new Vector3(15 + i * 3, -2.5f);
                    e.settingEnemy(gm.nomalMob[gm.seed[t]], t);
                    Enemys.Add(e);
                }
            }
            else
            {
                Enemy e = returnNewEnemy();
                e.anim.speed = 1;
                e.transform.position = player.transform.localPosition + new Vector3(18, -2.5f);
                e.settingEnemy(gm.nomalMob[gm.seed[t]], t);
                Enemys.Add(e);
                startTextBox(e.transform, "안녕?\n나는 적이야!");
            }
        }
    }
    public Enemy returnNewEnemy()
    {
        Enemy e;
        if (offEnemys.Count > 0)
        {
            e = offEnemys[0];
            offEnemys.RemoveAt(0);
            e.gameObject.SetActive(true);
        }
        else
        {
            e = Instantiate(enemyPrefab).GetComponent<Enemy>();
            e.pm = this;
            e.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (++count) + 15;
        }
        e.transform.GetChild(0).transform.localPosition = (Enemys.Count % 2 == 0) ? Vector3.right * ((Enemys.Count + 1)/2) * 0.25f : Vector3.left * ((Enemys.Count + 1) / 2) * 0.25f;
        return e;
    }
    public Transform buffPenal;
    public Animator bookAnim;
    public bool Semaphore;
    public void openBuffPenal()
    {
        if (buffPenal.gameObject.activeSelf)
        {
            //이미 열려있으면
            bookAnim.SetTrigger("Close");
            buffPenal.gameObject.SetActive(false);
            Time.timeScale = gm.checkStage("gameSpeed")? 1.2f: 1;
            Semaphore = false;
        }
        else
        {
            bookAnim.SetTrigger("Open");
            buffPenal.gameObject.SetActive(true);
            Time.timeScale = 0;
            Semaphore = true;
        }

        int maxPage = gm.playerHaveSkill.Count / 3 + 1;
        pageNum.text = (nowPage + 1) + "/" + maxPage;
        initSkillinfo();
    }
    public Animator pageFlip;
    public TextMeshProUGUI pageNum;
    public int nowPage=0;

    public void changePage(int a)
    {
        Debug.Log("sdasd");
        int gab = nowPage + a;
        int maxPage = gm.playerHaveSkill.Count / 3 + 1;

        if (gab < 0 || gab >= maxPage) return;

        releasePatternLines();
        if (a > 0)
        {
            checkSkillIndex += 3;
        }
        else if (a < 0)
        {
            checkSkillIndex -= 3;
        }

        nowPage = gab;
        pageNum.text = (nowPage + 1) + "/" + maxPage;
        pageFlip.SetTrigger((a < 0) ? "Right" : "Left");

        // 페이지 넘어가고 해당 페이지에 나타날 모든 정보들
        if (a < 0)
        {
            for (int j = 0; j < 3; ++j)
            {
                skillInfos[j].SetActive(true);
            }
        }
        initSkillinfo(); // 패턴 라인 그리기
    }


    public void movePlayer()
    {
        if (player.skillCheck) return;
        if (player.nearEnemy <= player.farRange)
        {
            player.anim.SetFloat("Speed", 0);
        }
        else
        {
            player.anim.SetFloat("Speed", 2);
            movePlayerToSKill(player.speed * Time.deltaTime);
            progressCheck();
        }
    }
    public void movePlayerToSKill(float gab)
    {
        //player.nearEnemy -= gab;
        Vector3 endPoint = Vector3.right * gab;
        player.transform.localPosition += endPoint;
        for (int i = 1; i < backGrounds.Length; ++i)
        {
            backGrounds[i].position += endPoint * (10.0f / (backGrounds.Length - i + 9));
        }
        checkBackGroundPivot(player.transform.localPosition.x, gab > 0);
    }
    public void checkBackGroundPivot(float x, bool right)
    {
        for (int i = 0; i < backGrounds.Length; ++i)
        {
            Transform tTemp = backGrounds[i];
            if (right)
            {
                int p = (backGroundsPivot[i] + 1) % 3;
                if (tTemp.GetChild(p).position.x - 5 < x)
                {
                    tTemp.GetChild((p + 1) % 3).localPosition += Vector3.right * 3;
                    backGroundsPivot[i] = p;
                }
            }
            else
            {
                int p = (backGroundsPivot[i] + 2) % 3;
                if (tTemp.GetChild(p).position.x + 5 > x)
                {
                    tTemp.GetChild((p + 2) % 3).localPosition -= Vector3.right * 3;
                    backGroundsPivot[i] = p;
                }
            }
        }
    }
    public Image[] stateBar;
    public TextMeshProUGUI[] stateText;
    public void playerStateCheck()
    {
        stateBar[0].fillAmount = (float)(player.curHp) / player.maxHp;
        stateText[0].text = player.curHp + "/" + player.maxHp;

        stateBar[1].fillAmount = (float)(player.curMp) / player.maxMp;
        stateText[1].text = player.curMp + "/" + player.maxMp;
    }

    public GameObject dmgText;
    public GameObject guardEffect;
    public GameObject manaEffect;
    public Color criColor;
    public List<Transform> dmgTextPool = new List<Transform>();

    public void MakeDmgText(int gab, bool player, Vector3 startPoint)
    {
        Transform t;
        if(dmgTextPool.Count > 0 && !dmgTextPool[0].gameObject.activeSelf)
        {
            t = dmgTextPool[0];
            t.gameObject.SetActive(true);
            dmgTextPool.RemoveAt(0);
        }
        else
        {
            t = Instantiate(dmgText).transform;
        }
        t.position = startPoint;
        Vector3 vTemp = (player) ? Vector3.up : Vector3.right;
        TextMesh tTemp = t.GetComponent<TextMesh>();
        tTemp.text = gab.ToString();
        if (player)
        {
            tTemp.fontSize = 77;
            tTemp.color = Color.red;
        }
        else
        {
            float critical = gab / this.player.atk - 1;
            critical = Mathf.Clamp01(critical * 0.286f);
            tTemp.fontSize = 77 + (int)(22 * critical);
            tTemp.color = Color.Lerp(Color.yellow, criColor, critical);
        }
        float rot = Random.Range(0, 0.5f * Mathf.PI);
        vTemp = new Vector3(vTemp.x * Mathf.Cos(rot) - vTemp.y * Mathf.Sin(rot)
            , vTemp.x * Mathf.Sin(rot) + vTemp.y * Mathf.Cos(rot));
        vTemp *= 3;
        StartCoroutine(startMoveDmgText(t, vTemp));
    }
    public WaitForSeconds fixWs = new WaitForSeconds(0.0166f);
    IEnumerator startMoveDmgText(Transform t, Vector3 goFor)
    {
        goFor += t.position;
        for(int i = 0; i < 60; ++i)
        {
            t.position = Vector3.Lerp(t.position, goFor, 0.1f);
            yield return fixWs;
        }
        dmgTextPool.Add(t);
        t.gameObject.SetActive(false);
    }

    /* < 버프 >
     * 1. 상태에 관여하는 버프(ex. 체력, 마나...)는 1)적용, 2)해제, 3)갱신 시에만 관련 상태에 대하여 업데이트한다.
     * 2. 공격시 발동 or 피해 or 회피 관련 버프는 해당 동작 실행시 계산된 값을 한번에 넘겨줘서 사용이 용이하게 설계한다.
     */
    public class BuffInfo
    {
        public string buffCode;     // DJ - 버프 대조용 (이미 있는 버프라면 값 갱신용)
        public float time;          // 버프 지속시간
        public skillData val;             // 버프 수치
        public Transform myUI;
    }
    public List<BuffInfo> buffList = new List<BuffInfo>(); //start 에서 초기화 하자  //0 플레이어 1 적
    public Transform[] buffStatePenal;
    public Transform buffUIsetting()
    {
        Transform tTemp;
        if (buffStatePenal[0].childCount > 0)
        {
            tTemp = buffStatePenal[0].GetChild(0);
            tTemp.parent = buffStatePenal[1];
        }
        else
        {
            tTemp = Instantiate(buffStatePenal[1].GetChild(0), buffStatePenal[1]).transform;
        }
        return tTemp;
    }
    public void getBuff(skillData sd)
    {
        BuffInfo bi = buffList.Find(item => item.buffCode == sd.skillCode);
        if(bi == null)
        {
            Transform ui = buffUIsetting();
            bi = new BuffInfo
            {
                buffCode = sd.skillCode,
                time = sd.duration,
                val = sd,
                myUI = ui
            };
            ui.GetChild(0).GetComponent<Image>().sprite = sd.skillImage;
            ui.GetChild(1).GetComponent<TextMeshProUGUI>().text = bi.time.ToString("F0");
            buffList.Add(bi);
        }
        else
        {
            bi.time = sd.duration;
        }
    }
    public void checkBuffTime()
    {
        for(int i = buffList.Count - 1; i >= 0; --i)
        {
            buffList[i].time -= Time.deltaTime;
            if(buffList[i].time > 0)
            {
                buffList[i].myUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = buffList[i].time.ToString("F0");
            }
            else
            {
                buffList[i].myUI.parent = buffStatePenal[0];
                for(int j = 0; j < buffList[i].val.skillEvent.Length; ++j)
                {
                    buffList[i].val.skillEvent[j].Invoke(new buffArg(j, 1, true));
                }
                buffList.RemoveAt(i);
            }
        }
    }


    private List<int> visitedNode = new List<int>();    // 패턴 기록
    public List<LockNode> circles;

    public GameObject linePrefab;
    public Canvas canvas;

    private bool unlocking;

    private List<LockNode> lines = new List<LockNode>();
    private int checkVisit = -1;

    private GameObject lineOnEdit;
    private RectTransform lineOnEditRcTs;
    private LockNode nodeOnEdit;  // 어느 노드에서 선이 이어지고있는지 체크용

    // 오브젝트 풀링용 리스트
    private List<GameObject> linesOfPatern = new List<GameObject>();

    // 스킬 정보 UI 표시용
    public GameObject[] patternPanels = new GameObject[3];  // 스킬 패턴
    public GameObject[] skillInfos; // 스킬 정보 담는곳

    // 임시 스킬 보여주기용
    public int checkSkillIndex;

    #region 스킬 정보/패턴 보여주기
    public void initSkillinfo()
    {
        int cnt = -1;
        for(int i = checkSkillIndex; i < checkSkillIndex + 3; ++i)
        {
            ++cnt;
            skillInfos[cnt].SetActive(true);
            if (i >= gm.playerHaveSkill.Count)
            {
                for (int j = cnt; j < 3; ++j)
                {
                    skillInfos[j].SetActive(false);
                }
                break;
            }
            // 스킬 패턴 그리기
            drawSkillPattern(gm.playerHaveSkill[i].pattern, patternPanels[cnt]);
            // 스킬 정보 세팅
            skillInfos[cnt].transform.Find("img").GetComponent<Image>().sprite = gm.playerHaveSkill[i].skillImage;
            skillInfos[cnt].transform.Find("name").GetComponent<TextMeshProUGUI>().text = gm.playerHaveSkill[i].buffName;
            skillInfos[cnt].transform.Find("cost").GetComponent<TextMeshProUGUI>().text = "cost : " + gm.playerHaveSkill[i].cost;
        }
    }
    public void drawSkillPattern(int[] pattern, GameObject patternPanel)
    {
        for(int i =0;i < pattern.Length-1; ++i)
        {
            createLineOnPatternPanel(patternPanel, pattern[i], pattern[i + 1], i);
        }
    }
    public void createLineOnPatternPanel(GameObject patternPanel, int id, int nextId, int num)
    {
        // 시작점 그리기
        GameObject line = null;
        foreach (var lineT in linesOfPatern)
        {
            if (!lineT.activeInHierarchy)
            {
                line = lineT;
                line.transform.SetParent( patternPanel.transform,false);
            }
        }
        if (line == null)
        {
            line = GameObject.Instantiate(linePrefab, patternPanel.transform, false);
            linesOfPatern.Add(line);
        }
        Vector3 pos = setPoint(id);
        line.SetActive(true);
        line.GetComponent<Image>().color = gm.lineColor[num];
        line.transform.localPosition = pos;

        // 라인 완성
        Vector3 nextPos = setPoint(nextId);
        RectTransform lineRcTs = line.GetComponent<RectTransform>();

        
        lineRcTs.sizeDelta = new Vector2(lineRcTs.sizeDelta.x, Vector3.Distance(pos, nextPos));
        lineRcTs.rotation = Quaternion.FromToRotation(Vector3.up, (nextPos - pos).normalized);
    }
    public void releasePatternLines()
    {
        for(int i=0;i< linesOfPatern.Count; ++i)
        {
            linesOfPatern[i].SetActive(false);
        }
    }
    public Vector2 setPoint(int id)
    {
        Vector2 pos = Vector2.zero;
        switch (id)
        {
            case 0:
                pos = new Vector2(-35, 35);
                break;
            case 1:
                pos = new Vector2(0, 35);
                break;
            case 2:
                pos = new Vector2(35, 35);
                break;
            case 3:
                pos = new Vector2(-35, 0);
                break;
            case 4:
                pos = new Vector2(0, 0);
                break;
            case 5:
                pos = new Vector2(35, 0);
                break;
            case 6:
                pos = new Vector2(-35, -35);
                break;
            case 7:
                pos = new Vector2(0, -35);
                break;
            case 8:
                pos = new Vector2(35, -35);
                break;
        }
        return pos;
    }
    #endregion

    #region 패턴 그리기
    public List<skillData> buffData = new List<skillData>();
    public SpriteRenderer buffEffect;
    public struct buffArg
    {
        public int num;
        public int lev;
        public bool revers;
        public buffArg(int n, int l, bool revers)
        {
            num = n;
            lev = l;
            this.revers = revers;
        }
    }

    IEnumerator buffManager()
    {
        WaitForSeconds fws = new WaitForSeconds(0.05f);
        while (true)
        {
            yield return fws;
            yield return new WaitUntil(() => buffData.Count > 0);
            if (Semaphore) continue;
            skillData sdTemp = buffData[0];
            buffData.RemoveAt(0);

            for(int i = 0; i < sdTemp.skillEvent.Length; ++i)
            {
                sdTemp.skillEvent[i].Invoke(new buffArg(i,1, false));
            }
            getBuff(sdTemp);
            for (int i = 0; i < sdTemp.be.Length; ++i)
            {
                buffEffect.sprite = sdTemp.be[i];
                yield return fws;
            }
            buffEffect.sprite = null;
        }
    }


    public void onMouseEnterNode(LockNode ln)
    {
        if (unlocking)
        {
            #region 예외처리
            if (ln.id == 0)
            {
                if(checkVisit == 2)
                {
                    onMouseEnterNode(circles[1]);
                }
                else if(checkVisit == 8)
                {
                    onMouseEnterNode(circles[4]);
                }
                else if(checkVisit == 6)
                {
                    onMouseEnterNode(circles[3]);
                }
            }
            else if(ln.id == 2)
            {
                if (checkVisit == 0)
                {
                    onMouseEnterNode(circles[1]);
                }
                else if (checkVisit == 6)
                {
                    onMouseEnterNode(circles[4]);
                }
                else if (checkVisit == 8)
                {
                    onMouseEnterNode(circles[5]);
                }
            }
            else if (ln.id == 6)
            {
                if (checkVisit == 0)
                {
                    onMouseEnterNode(circles[3]);
                }
                else if (checkVisit ==2)
                {
                    onMouseEnterNode(circles[4]);
                }
                else if (checkVisit == 8)
                {
                    onMouseEnterNode(circles[7]);
                }
            }
            else if (ln.id == 8)
            {
                if (checkVisit == 2)
                {
                    onMouseEnterNode(circles[5]);
                }
                else if (checkVisit == 0)
                {
                    onMouseEnterNode(circles[4]);
                }
                else if (checkVisit == 6)
                {
                    onMouseEnterNode(circles[7]);
                }
            }
            else if (ln.id == 1)
            {
                if (checkVisit == 7)
                {
                    onMouseEnterNode(circles[4]);
                }
            }
            else if (ln.id == 5)
            {
                if (checkVisit == 3)
                {
                    onMouseEnterNode(circles[4]);
                }
            }
            else if (ln.id == 7)
            {
                if (checkVisit == 1)
                {
                    onMouseEnterNode(circles[4]);
                }
            }
            else if (ln.id == 3)
            {
                if (checkVisit == 5)
                {
                    onMouseEnterNode(circles[4]);
                }
            }
            #endregion
            Vector3 temp = canvas.transform.InverseTransformPoint(nodeOnEdit.transform.position);
            Vector3 temp1 = canvas.transform.InverseTransformPoint(ln.transform.position);
            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, Vector3.Distance(temp, temp1));
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, (temp1 - temp).normalized);
            trySetLineEdit(ln);

            if (checkVisit != ln.id)
            {
                visitedNode.Add(ln.id);
                checkVisit = ln.id;
            }
        }
    }
    public void onMouseExitNode(LockNode ln)
    {

    }
    public void onMouseDownNode(LockNode ln)
    {
        if (lines.Count > 0)
        {
            release();
        }
        unlocking = true;
        trySetLineEdit(ln);
        if (checkVisit != ln.id)
        {
            visitedNode.Add(ln.id);
            checkVisit = ln.id;
            Debug.Log(checkVisit);
        }
    }
    public void onMouseUpNode(LockNode ln)
    {
        if (unlocking)
        {
            lineOnEdit.gameObject.SetActive(false);
        }
        string sTemp = string.Empty;
        foreach (int node in visitedNode) sTemp += node.ToString();
        skillData sd = gm.playerHaveSkill.Find(item => item.skillCode == sTemp);

        if(sd == null)
        {
            //잘못된 패턴
        }
        else
        {
            if (buffData.Contains(sd))
            {
                //이미 대기열에 있음
            }
            else if (useManaCost(sd.cost))
            {
                //코스트 사용
                buffData.Add(sd);
            }
            else
            {
                //코스트 부족
            }
        }

        visitedNode.Clear();
        release();
        unlocking = false;
    }
    public bool useManaCost(int a)
    {
        if(player.curMp >= a)
        {
            player.curMp -= a;
            playerStateCheck();
            return true;
        }
        return false;
    }
    public void trySetLineEdit(LockNode node)
    {
        foreach(LockNode line in lines)
        {
            if(line.id == node.id)
            {
                if (line.gameObject.activeInHierarchy)
                {
                    return;
                }
                line.gameObject.SetActive(true);
                lineOnEdit = line.gameObject;
                lineOnEditRcTs = lineOnEdit.GetComponent<RectTransform>();
                nodeOnEdit = node;
                return;
            }
        }
        lineOnEdit = createLine(canvas.transform.InverseTransformPoint(node.transform.position), node.id);
        lineOnEditRcTs = lineOnEdit.GetComponent<RectTransform>();
        nodeOnEdit = node;
    }

    public GameObject createLine(Vector3 pos, int id)
    {
        GameObject line = GameObject.Instantiate(linePrefab, canvas.transform, false);
        line.SetActive(true);
        line.transform.localPosition = pos;
        LockNode lineNode = line.AddComponent<LockNode>();
        lineNode.id = id;
        lines.Add(lineNode);
        return line;
    }

    public void release()
    {
        foreach (LockNode line in lines)
        {
            line.gameObject.SetActive(false);
            line.gameObject.GetComponent<Image>().color = Color.white;
        }
        lineOnEdit = null;
        lineOnEditRcTs = null;
        nodeOnEdit = null;
        visitedNode.Clear();
        checkVisit = -1;
    }

    #endregion
}
