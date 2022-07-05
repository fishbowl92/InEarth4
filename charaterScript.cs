using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Object Asset/Character Data")]
public class charaterScript : ScriptableObject
{
    public string CharacterName;
    public int hp, mp, atk, def;
    public GameObject ImgSet;
}
