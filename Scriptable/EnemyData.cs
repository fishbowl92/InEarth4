using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Scriptable Object/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public RuntimeAnimatorController enemyAnim;

    public Sprite headImg;
    public int hp;
    public float speed;
    public float distance; //이격거리
    public Enemy.skillInfo[] mySkills;
    public int tear;
    public int myPattern;
    public string enemyText;
    public AudioClip aktSound;
}
