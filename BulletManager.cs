using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletManager : MonoBehaviour
{
    public PlayManager pm;
    public class Bullet
    {
        public int number; // 총알 넘버 (이미지 변경 버프걸릴경우 사용
        public int dmg; // 데미지
        public float range; // 타격범위
        public int count; // 뚫는 수
        public float speed; // 이동 속도
        public float endPoint; // 최종 도착지
        public int lifeTiem; // 생존한 기간
        public Transform t; // 움직일 총알
        public SpriteRenderer sr;
        public List<Enemy> hit; // 타격한 대상
        public Bullet(Transform t)
        {
            number = -1;
            this.t = t;
            hit = new List<Enemy>();
        }
    }
    public void Start()
    {
        StartCoroutine(checkBullet());
    }
    public class EffectObject
    {
        public float spawnTime; //생성된 시간
        public GameObject img; //관리되는 대상
        public EffectObject(GameObject _g)
        {
            spawnTime = Time.time;
            img = _g;
        }
    }
    WaitForSeconds ws = new WaitForSeconds(0.0083f);
    public List<Bullet> myBullets = new List<Bullet>();
    public List<Bullet> usedBullets = new List<Bullet>();
    public AudioSource[] hitSound;
    public int hsCount;
    public List<EffectObject> hitObject = new List<EffectObject>();
    public int hoCount = 0;
    IEnumerator checkBullet()
    {
        while (true)
        {
            if (!pm.Semaphore)
            {
                for (int i = myBullets.Count - 1; i >= 0; --i)
                {
                    Bullet bTemp = myBullets[i];
                    float start = bTemp.t.position.x;
                    if (bTemp.lifeTiem++ % 3 == 0)
                    {
                        Sprite sTemp = pm.player.bulletImg[(bTemp.lifeTiem / 3) % pm.player.bulletImg.Length];
                        if(sTemp != bTemp.sr.sprite)bTemp.sr.sprite = sTemp;
                    }
                    if (bTemp.speed > 0)
                    {
                        bTemp.t.position += Vector3.right * bTemp.speed * 0.0083f;
                    }
                    if (bTemp.lifeTiem / 3 >= pm.player.bulletImg.Length && bTemp.t.position.x >= bTemp.endPoint)
                    {
                        //투사체 끝
                        bTemp.t.gameObject.SetActive(false);
                        usedBullets.Add(bTemp);
                        myBullets.RemoveAt(i);
                    }
                    else if (bTemp.count >= 0)
                    {
                        for (int j = pm.Enemys.Count - 1; j >= 0; --j)
                        {
                            Enemy e = pm.Enemys[j];
                            if (bTemp.hit.Contains(e)) continue; // 이미때림
                            float point = e.transform.position.x;
                            if (point <= start + bTemp.range && point >= start)
                            {
                                //범위 안, 적공격 구현
                                bTemp.hit.Add(e);
                                e.Apuda(bTemp.dmg);
                                hitSound[hsCount++].Play();
                                hsCount %= 5;
                                if (bTemp.speed > 0)
                                {
                                    GameObject gTemp = hit();
                                    gTemp.transform.position = Vector3.Lerp(bTemp.t.position, e.transform.position + Vector3.down, UnityEngine.Random.Range(0.5f, 0.7f));
                                    gTemp.SetActive(true);

                                }

                                if (--bTemp.count < 0)
                                {
                                    break; // 때릴꺼 다떄림
                                }
                            }
                        }
                    }
                    else if (bTemp.lifeTiem / 2 >= pm.player.bulletImg.Length)
                    {
                        //투사체 끝
                        bTemp.t.gameObject.SetActive(false);
                        usedBullets.Add(bTemp);
                        myBullets.RemoveAt(i);
                    }
                }
            }
            yield return ws;
        }
    }
    public GameObject bullet;
    public GameObject hit()
    {
        if (hitObject.Count > hoCount)
        {
            EffectObject eTemp = hitObject[hoCount];
            if (eTemp.spawnTime < Time.time)
            {
                eTemp.img.SetActive(false);
                eTemp.spawnTime = Time.time + 2;
                hoCount = (hoCount + 1) % hitObject.Count;
                return eTemp.img;
            }
        }

        GameObject gTemp = Instantiate(pm.player.hitObj);
        hitObject.Add(new EffectObject(gTemp));
        return gTemp;
    }
    public void shot()
    {
        Bullet bTemp;
        if (usedBullets.Count > 0)
        {
            bTemp = usedBullets[0];
            usedBullets.RemoveAt(0);
            bTemp.hit.Clear();
            bTemp.t.gameObject.SetActive(true);
        }
        else
        {
            bTemp = new Bullet(Instantiate(bullet).transform);
        }
        bTemp.sr = bTemp.t.GetComponent<SpriteRenderer>();
        bTemp.sr.sprite = null;
        bTemp.range = pm.player.Range();
        bTemp.t.position = pm.player.transform.position + new Vector3(0.2f, 5.9f);
        bTemp.dmg = pm.player.getDmg();
        bTemp.speed = pm.player.bulletSpeed();
        bTemp.lifeTiem = 0;
        bTemp.count = pm.player.atkCount();
        if (bTemp.number != pm.player.bulletImgChanger)
        {
            bTemp.number = pm.player.bulletImgChanger;
            /*Array.Resize(ref bTemp.img, pm.player.bulletImg.Length);
            for(int i = 0; i < pm.player.bulletImg.Length; ++i)
            {
                bTemp.img[i] = pm.player.bulletImg[i];
            }*/
        }
        bTemp.endPoint = pm.player.transform.position.x + pm.player.atkRange();
        myBullets.Add(bTemp);
    }
}
