using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SelectionType {
    All,
    Vertical,
    Horizontal,
    Point,
    Square
}

public class WeaponStyle {
    public string name;
    public int windUpDepth;
    public int damageModifier; // in percent
    public SelectionType targetSelectionType;
}

public class HeldEquipment {
    public string name;
    public int baseDamageReduction; // in percent
    public int quality; // in percent

    public int damageReduction {
        get {
            float modifier = (float)quality / 100f;
            return Mathf.RoundToInt((float)baseDamageReduction * modifier);
        }
    }

    public HeldEquipment(string _name, int _baseDamageReduction, int _quality) {
        name = _name;
        baseDamageReduction = _baseDamageReduction;
        quality = _quality;
    }
}

public class Weapon : HeldEquipment {
    public int baseDamage;

    public int damage {
        get {
            float modifier = (float) quality / 100f;
            return Mathf.RoundToInt((float)baseDamage * modifier);
        }
    }

    public List<WeaponStyle> styleList;

    public Weapon(string _name, int _baseDamage, int _baseDamageReduction, int _quality)
        : base(_name, _baseDamageReduction, _quality) {
        
        baseDamage = _baseDamage;
        styleList = new List<WeaponStyle>();
    }
}

public class Shield : HeldEquipment{
    public Shield(string _name, int _baseDamageReduction, int _quality)
        : base(_name, _baseDamageReduction, _quality) { }
}