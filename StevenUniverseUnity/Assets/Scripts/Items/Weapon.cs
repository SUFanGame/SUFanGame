using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour 
{
    [SerializeField]
    int range_ = 0;
    public int Range_ { get { return range_; } }

    [SerializeField]
    int damage_ = 0;
    public int Damage_ { get { return damage_; } }

    [SerializeField]
    int accuracy_ = 0;
    public int Accuracy_ { get { return accuracy_; } }

    [SerializeField]
    int critChance_ = 5;
    public int CritChance_ { get { return critChance_; } }
}
