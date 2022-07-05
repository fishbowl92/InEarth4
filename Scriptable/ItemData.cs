using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object Asset/Item Data")]

public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemCode;
    public int itemKind;    // 0 - ����, 1 - etc
    public int level;
    public int cost;
    // Get State�� ������ ������ �������� (�ӽ�) (�߰�����)
    // 0 - ���ݷ�, 1 - ����, 2 - �ִ�ü��
    public float[] getState;
    [TextArea(10, 20)]
    public string info;
    public Sprite itemImage;

    public Sprite[] weaponEffect;  // ���� ���� ���� ����Ʈ
    public GameObject bombEffect;   // ���� �ǰ� ����Ʈ
}
