using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]

public class SkillCard : ScriptableObject
{
    public string Skillname;
    public string SkillMain;
    public int SkillValue;
    public int SkillFuc;

}
