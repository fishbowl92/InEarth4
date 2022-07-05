using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;

public class GameManager : MonoBehaviour
{
    public static GameManager Instans;
    public AudioSource[] btnSound;

    public List<EnemyData> nomalMob;
    public List<EnemyData> bossMob;

    public int selectCharacter;
    public GameObject[] Characters;

    public int stageNum;
    public int[] seed = new int[3];

    public int myGold;
    void Awake()
    {
        if (Instans)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            Instans = this;
        }
    }

    public ItemData[] textWeapon;
    public void changeWeapon(ItemData id)
    {
        if (PlayManager.Instans)
        {
            Player p = PlayManager.Instans.player;
            p.hitObj = id.bombEffect;
            SpriteResolver sr = p.transform.Find("weapon").GetComponent<SpriteResolver>();
            sr.SetCategoryAndLabel(sr.GetCategory(), id.itemCode);
            p.bulletImg = id.weaponEffect;
        }
    }

    public List<skillData> allSkillData;
    public List<skillData> playerHaveSkill;

    // 오브젝트 풀링용 리스트
    private List<GameObject> linesOfPatern = new List<GameObject>();

    // 스킬 정보 UI 표시용
    public GameObject[] skillInfos; // 스킬 정보 담는곳
    public GameObject linePrefab;

    public List<Color> lineColor;

    public void drawSkillPattern(int[] pattern, GameObject patternPanel)
    {
        for (int i = 0; i < pattern.Length - 1; ++i)
        {
            createLineOnPatternPanel(patternPanel, pattern[i], pattern[i + 1]);
        }
    }
    public string levCorrection(int lev)
    {
        switch (lev)
        {
            default:
                return "모험의 시작";
            case 1:
                return "강적이 더 많은 체력을 얻습니다";
            case 2:
                return "무리의 적이 더 많은 체력을 얻습니다";
            case 3:
                return "대장이 더 많은 체력을 얻습니다";
            case 4:
                return "시작시 회피스텍이 없습니다";
            case 5:
                return "회피 쿨타임이 증가합니다";
            case 6:
                return "무리의 적수가 증가합니다";
            case 7:
                return "강적이 경직을 무시합니다";
            case 8:
                return "대장의 강인함이 증가합니다";
            case 9:
                return "무리의 속도가 증가합니다";
            case 10:
                return "대장이 새로운 패턴을 가집니다";
            case 11:
                return "회피의 판정이 짧아집니다";
            case 12:
                return "게임속도가 증가합니다";
        }
    }
    public bool checkStage(string s)
    {
        switch (s)
        {
            case "addHpHardMob":
                return stageNum >= 1;
            case "addHpNomalMob":
                return stageNum >= 2;
            case "addHpBossMob":
                return stageNum >= 3;
            case "noneEvasionStack":
                return stageNum >= 4;
            case "upEvasionCoolTime":
                return stageNum >= 5;
            case "mobVolume":
                return stageNum >= 6;
            case "superHardMob":
                return stageNum >= 7;
            case "superBossMob":
                return stageNum >= 8;
            case "nomalMobSpeedUp":
                return stageNum >= 9;
            case "newPattern":
                return stageNum >= 10;
            case "hardEvasion":
                return stageNum >= 11;
            case "gameSpeed":
                return stageNum >= 12;
            default:
                return false;
        }
    }
    public void getSeed()
    {
        seed = new int[3];
        seed[0] = Random.Range(0, nomalMob.Count);
        do
        {
            seed[1] = Random.Range(0, nomalMob.Count);
        }
        while (seed[0] == seed[1]);
        seed[2] = Random.Range(0, bossMob.Count);
    }
    public void createLineOnPatternPanel(GameObject patternPanel, int id, int nextId)
    {
        // 시작점 그리기
        GameObject line = null;
        foreach (var lineT in linesOfPatern)
        {
            if (!lineT.activeInHierarchy)
            {
                line = lineT;
                line.transform.SetParent(patternPanel.transform, false);
            }
        }
        if (line == null)
        {
            line = GameObject.Instantiate(linePrefab, patternPanel.transform, false);
            linesOfPatern.Add(line);
        }
        Vector3 pos = setPoint(id);
        line.SetActive(true);
        line.GetComponent<Image>().color = Color.black;
        line.transform.localPosition = pos;

        // 라인 완성
        Vector3 nextPos = setPoint(nextId);
        RectTransform lineRcTs = line.GetComponent<RectTransform>();


        lineRcTs.sizeDelta = new Vector2(lineRcTs.sizeDelta.x, Vector3.Distance(pos, nextPos));
        lineRcTs.rotation = Quaternion.FromToRotation(Vector3.up, (nextPos - pos).normalized);
    }
    public void release()
    {
        for (int i = 0; i < linesOfPatern.Count; ++i)
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
                pos = new Vector2(-100, 100);
                break;
            case 1:
                pos = new Vector2(0, 100);
                break;
            case 2:
                pos = new Vector2(100, 100);
                break;
            case 3:
                pos = new Vector2(-100, 0);
                break;
            case 4:
                pos = new Vector2(0, 0);
                break;
            case 5:
                pos = new Vector2(100, 0);
                break;
            case 6:
                pos = new Vector2(-100, -100);
                break;
            case 7:
                pos = new Vector2(0, -100);
                break;
            case 8:
                pos = new Vector2(100, -100);
                break;
        }
        return pos;
    }

    public List<ItemData> allItemData;

    // 0 : 무기, 1 ~ 3 : etc
    public List<ItemData> playerEquipment;
    public List<ItemData> inventory;

}
