using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "text", menuName = "Scriptable Object Asset/Boss Script")]

public class BossScripts : ScriptableObject
{
    [TextArea(5, 10)]
    public string[] lotOfText;
}
