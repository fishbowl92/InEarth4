using UnityEngine;
using UnityEngine.Events;
using arg = PlayManager.buffArg;
[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Object Asset/Skill Data")]
public class skillData : ScriptableObject
{
    public string buffName;
    public string skillCode;
    public int cost;
    public int duration;
    public int[] pattern;

    public int level;    // ��ų ���� ����
    public int[] needExpForLevUp;   // ��ų �������� �䱸 ����ġ

    public Vector3Int[] skillVal; //        // x Ÿ�� // 0- �÷��̾� ���� 1- �÷��̾ ����, 2- ���� ������ɸ�
                                            // y  �ð� 
                                            // z �ݺ�Ƚ��
    public UnityEvent<arg>[] skillEvent;
    [TextArea(10, 20)]
    public string info;
    public Sprite skillImage;
    public Sprite[] be;
    [ContextMenu("skillCode����")]
    public void settingSkillCode()
    {
        skillCode = string.Empty;
        foreach (int gab in pattern) skillCode += gab.ToString(); 
    }

    public void abilityUp(arg a)
    {
        Vector3Int vTemp = skillVal[a.num];
        PlayManager.Instans.player.getAbilityBuff(vTemp.x, a.revers ? -vTemp.y :vTemp.y);
    }
}
