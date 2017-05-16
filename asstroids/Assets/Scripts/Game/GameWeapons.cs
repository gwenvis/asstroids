using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Game/Weapons", order = 1)]
public class GameWeapons : ScriptableObject
{
    public Weapon[] UsableWeapons;   
}
