using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Delay;
    public float atkDelay;
    public float stunDelay;
    public float farToPlayer;

    public float speed;
    public EnemyData ed;

    public PlayManager pm;
    public Animator anim;
    public GameObject stun;
    public Transform atkPoint;

    public skillInfo selectSkill;
    public bool superArmor;
    public AudioSource atkSound;
    public void checkNearEnemy()
    {
        farToPlayer = transform.position.x - pm.player.transform.position.x;
        if (pm.player.target == null || pm.player.target == this || farToPlayer < pm.player.nearEnemy)
        {
            pm.player.target = this;
            pm.player.nearEnemy = farToPlayer;
        }
    }
    public void settingEnemy(EnemyData ed, int tear)
    {
        anim.runtimeAnimatorController = ed.enemyAnim;
        this.ed = ed;
        speed = ed.speed;
        GameManager gm = pm.gm;
        Delay = 0;
        superArmor = false;
        stunDelay = 0;
        sua = false;
        stunTime = 2.8f;
        atkSound.clip = ed.aktSound;
        if (tear == 0)
        {
            float add = 0.01f * gm.stageNum;
            if (pm.gm.checkStage("addHpNomalMob")) add += 0.2f;
            add += 0.5f;
            hp = (int)(ed.hp * add);
        }
        else if(tear == 1)
        {
            float add = 0.02f * gm.stageNum;
            if (pm.gm.checkStage("addHpHardMob")) add += 0.1f;
            add += 0.7f;
            hp = (int)(ed.hp * add * 2);
            superArmor = gm.checkStage("superHardMob");
        }
        else
        {
            float add = 0.05f * gm.stageNum;
            if (pm.gm.checkStage("addHpHardMob")) add += 0.2f;
            add += 1;
            hp = (int)(ed.hp * add);
            if (pm.gm.checkStage("superBossMob")) stunTime -= 0.5f;
        }
        maxHp = hp;
        die = false;
    }
    public void giveGold()
    {
        pm.getGold(transform.position, 5);
    }
    public void Death()
    {
        gameObject.SetActive(false);
    }

    public int hp = 1;
    public float maxHp;
    public bool die;
    public bool sua;
    public void Apuda(int gab)
    {
        if(Random.Range(0, 100) < pm.player.critical)
        {
            pm.criCount++;
            gab += (int)(gab *pm.player.criDmg);
            if (pm.gm.selectCharacter == 0 && pm.criCount % 5 == 0) pm.player.mpAdd(1);
        }
        pm.MakeDmgText(gab, false, transform.position);
        if (hp > 0) pm.getNewTarget(this, hp/ maxHp, (hp-gab)/ maxHp);
        hp -= gab;

        /*if (true)
        {
            gameObject.SetActive(false);
            return;
        }*/
        if (!superArmor && !sua)
        {
            anim.SetTrigger("Knock");
            //경직 에니메이션 0.25
            startKnockBack(pm.player.knockBack());
        }
    }
    public void startKnockBack(float gab)
    {
        Delay = Time.time + pm.player.setDelay();
        StartCoroutine(knockBack(gab));
    }
    IEnumerator knockBack(float gab)
    {
        //0.2초간 밀자
        float timer = 0;
        Vector3 go = Vector3.right * gab / 0.2f; 
        while(timer <= 0.2f)
        {
            transform.position += go * Time.deltaTime;
            timer += Time.deltaTime;
            yield return false;
        }
    }
    [System.Serializable]
    public class skillInfo
    {
        public int dmg;
        public float moveDelay;
        public float coolTime;
        public bool skillKind;//false 이면 일반
    }
    public int skillcount;
    public void atk()
    {
        if (atkDelay > Time.time)
        {
            return;
        }
        sua = true;
        selectSkill = ed.mySkills[skillcount];
        if (skillcount == 1) skillcount = 0;
        else skillcount = (Random.Range(0, 3) == 0) ? 1 : 0;
        //skillcount = (skillcount + 1) % ed.mySkills.Length;
        Delay = Time.time + selectSkill.moveDelay;
        atkDelay = Time.time + selectSkill.coolTime;
        if(ed.myPattern > 0)
        {
            anim.SetFloat("Boss", Random.Range(0, pm.gm.checkStage("newPattern") ? ed.myPattern : 2) * 2);
            anim.SetTrigger(selectSkill.skillKind ? "Pattern" : "Atk");
        }
        else
        {
            anim.SetTrigger(selectSkill.skillKind ? "Skill" : "Atk");
        }
    }
    public void endToAtk()
    {
        sua = false;
    }
    public void move()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
    public void moveSpeed(float a)
    {
        transform.position += Vector3.left * a;
        checkNearEnemy();
    }
    public void RangeCheck()
    {
        if(pm.player.transform.localPosition.x > atkPoint.transform.position.x -0.5f)
        {
           if(pm.player.EvasionTime < Time.time)
            {
                //맞음
                pm.player.Apuda(selectSkill.dmg);
            }
            else
            {
                //회피성공
                pm.makeGuardFont(false);
            }
        }
        else
        {
            Debug.Log("사거리밖");
        }
    }
    public void RangeCheckWithDmg(int gub)
    {
        if (pm.player.transform.localPosition.x > atkPoint.transform.position.x - 0.5f)
        {
            if (pm.player.EvasionTime < Time.time)
            {
                //맞음
                pm.player.Apuda(Mathf.Max(1, selectSkill.dmg / gub));
            }
            else
            {
                //회피성공
                pm.makeGuardFont(false);
            }
        }
        else
        {
            Debug.Log("사거리밖");
        }
    }
    public float stunTime;
    public void blockSkill()
    {
        if (pm.player.transform.localPosition.x > atkPoint.transform.position.x - 0.5f)
        {
            if (pm.player.EvasionTime >= Time.time)
            {
                pm.makeGuardFont(true);
                anim.SetTrigger("Knock");
                stunDelay = Time.time + stunTime;
                startKnockBack(1.2f);
            }
        }
        else
        {
            Debug.Log("사거리밖");
        }
    }
}
