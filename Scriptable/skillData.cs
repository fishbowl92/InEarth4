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

    public int level;    // 스킬 현재 레벨
    public int[] needExpForLevUp;   // 스킬 레벨업당 요구 경험치

    public Vector3Int[] skillVal; //        // x 타입 // 0- 플레이어 버프 1- 플레이어가 공격, 2- 적이 디버프걸림
                                            // y  시간 
                                            // z 반복횟수
    public UnityEvent<arg>[] skillEvent;
    [TextArea(10, 20)]
    public string info;
    public Sprite skillImage;
    public Sprite[] be;
    [ContextMenu("skillCode셋팅")]
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
