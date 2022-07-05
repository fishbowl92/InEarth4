using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim;
    public PlayManager pm;
    public BulletManager bm;

    public int maxHp, curHp;
    public int maxMp, curMp;

    public int atk, def;
    public int atkBuff;
    public float atkSpeed;//공격속도
    public int defaultCount; //관통횟수
    public float atkToFar; // 투사채 진행거리
    public float bulletAtkRange; //투사체 공격범위
    public float defaultBulletSpeed; //투사체 전진속도
    public float DefaultEvasionTime; // 회피지속시간
    public float knockBackPower; // 적이 밀려나가는 거리
    public float knockBackTime; // 적이 경직 시간
    public int DefaultEvasionStack; //회피 스택최대치
    public float EvasionCoolTime; // 회피지속
    public float critical; //치명타 확률
    public float criDmg; // 치명타 배율
    public int prbBtn;

    public bool skillCheck;
    public bool atkDel;

    public float farRange; // 이격거리
    public float nearEnemy;
    public Enemy target;

    public float speed;
    public Sprite[] bulletImg;

    public AudioSource shotSound;
    public AudioClip hitSound;
    public AudioSource aBtnSound;
    public GameObject hitObj;

    public void Start()
    {
        initPlayerStat();
        for(int i = 0; i < 5; ++i)
        {
            bm.hitSound[i].clip = hitSound;
        }
    }
    public void getAbilityBuff(int kind, int gab)
    {
        switch (kind)
        {
            case 0: //공격력
                atkBuff += gab;
                break;
            case 2: //공격속도
                atkSpeed += gab;
                break;
        }
    }
    public void startGame()
    {
        nearEnemy = 300;
        skillCheck = false;
    }
    public void GetAButton()
    {
        if (skillCheck || pm.evasionCharge <= 0) return;
        skillCheck = true;
        pm.evasionCharge--;
        pm.evasionImg.fillAmount = (pm.evasionCharge > 0) ? 1 : pm.evasionTimer / getEvasionCoolTime();
        pm.evasionCount.text = pm.evasionCharge.ToString();
        prbBtn = 1;
        aBtnSound.Play();
        anim.SetTrigger("Abtn");
    }
    public void GetBButton()
    {
        if (skillCheck && (prbBtn == 1 || atkDel)) return;
        skillCheck = true;
        atkDel = true;
        anim.SetFloat("AtkSpeed", 0.5f * getAtkSpeed());
        prbBtn = 2;
        anim.SetTrigger("Bbtn");
    }
    public void shot()
    {
        shotSound.Play();
        bm.shot();
    }
    public void animEnd()
    {
        skillCheck = false;
    }
    public void atkEnd()
    {
        atkDel = false;
    }
    public void movePosition(float a)
    {
        pm.movePlayerToSKill(a);
    }

    public void initPlayerStat()
    {
        curHp = maxHp;
        curMp = maxMp;
        pm.playerStateCheck();
    }

    public void hpAdd(int val)
    {
        curHp = Mathf.Clamp(curHp + val, 0, maxHp);
        pm.playerStateCheck();
    }
    public void mpAdd(int val)
    {
        curMp = Mathf.Clamp(curMp + val, 0, maxMp);
        GameObject gTemp = Instantiate(pm.manaEffect);
        gTemp.transform.position = transform.position + new Vector3(-0.6f, +7);
        Destroy(gTemp, 2);
        pm.playerStateCheck();
    }
    public void Apuda(int gab)
    {
        pm.guardSound[pm.goCount].clip = pm.guardClup[Random.Range(0, 2)];
        pm.guardSound[pm.goCount++].Play();
        pm.goCount %= 5;
        pm.hitCount++;
        hpAdd(-gab);
        pm.MakeDmgText(gab, true, transform.position + Vector3.up * 6);
    }
    public int bulletImgChanger;
    public int getDmg()
    {
        return atk + atkBuff;
    }
    public float getAtkSpeed()
    {
        return atkSpeed;
    }
    public int atkCount()
    {
        return defaultCount;
    }

    public float atkRange()
    {
        return atkToFar;
    }
    public float Range()
    {
        return bulletAtkRange;
    }
    public float bulletSpeed()
    {
        return defaultBulletSpeed;
    }
    public float evasion()
    {
        return pm.gm.checkStage("hardEvasion") ? DefaultEvasionTime * 0.9f : DefaultEvasionTime;
    }
    public float knockBack()
    {
        return knockBackPower;
    }
    public float setDelay()
    {
        return knockBackTime;
    }
    public float EvasionTime;
    public void getEvasion(float a = -1)
    {
        float nextGab = (a > 0) ? Time.time + a : Time.time + evasion();
        if (nextGab > EvasionTime) EvasionTime = nextGab;
    }
    public int getEvasionMaxStack()
    {
        return DefaultEvasionStack;
    }

    public float getEvasionCoolTime()
    {
        return pm.gm.checkStage("upEvasionCoolTime") ? EvasionCoolTime : EvasionCoolTime * 0.9f;
    }
}
