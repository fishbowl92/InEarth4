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

    // ������Ʈ Ǯ���� ����Ʈ
    private List<GameObject> linesOfPatern = new List<GameObject>();

    // ��ų ���� UI ǥ�ÿ�
    public GameObject[] skillInfos; // ��ų ���� ��°�
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
                return "������ ����";
            case 1:
                return "������ �� ���� ü���� ����ϴ�";
            case 2:
                return "������ ���� �� ���� ü���� ����ϴ�";
            case 3:
                return "������ �� ���� ü���� ����ϴ�";
            case 4:
                return "���۽� ȸ�ǽ����� �����ϴ�";
            case 5:
                return "ȸ�� ��Ÿ���� �����մϴ�";
            case 6:
                return "������ ������ �����մϴ�";
            case 7:
                return "������ ������ �����մϴ�";
            case 8:
                return "������ �������� �����մϴ�";
            case 9:
                return "������ �ӵ��� �����մϴ�";
            case 10:
                return "������ ���ο� ������ �����ϴ�";
            case 11:
                return "ȸ���� ������ ª�����ϴ�";
            case 12:
                return "���Ӽӵ��� �����մϴ�";
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
        // ������ �׸���
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

        // ���� �ϼ�
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

    // 0 : ����, 1 ~ 3 : etc
    public List<ItemData> playerEquipment;
    public List<ItemData> inventory;

}
