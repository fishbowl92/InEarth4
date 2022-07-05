using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object Asset/Item Data")]

public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemCode;
    public int itemKind;    // 0 - 무기, 1 - etc
    public int level;
    public int cost;
    // Get State의 순서로 스탯이 정해진다 (임시) (추가예정)
    // 0 - 공격력, 1 - 방어력, 2 - 최대체력
    public float[] getState;
    [TextArea(10, 20)]
    public string info;
    public Sprite itemImage;

    public Sprite[] weaponEffect;  // 유저 무기 사용시 이펙트
    public GameObject bombEffect;   // 몬스터 피격 이펙트
}
