using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActorType {
    Player,
    Creature,
    Item
}

public enum CombatActionType {
    Block,
    Parry,
    WindUp,
    SwitchStyle,
    Swing,
    Recover
}

public class CombatAction {
    public CombatActionType type;
    public HeldEquipment prop;
    public WeaponStyle weaponStyle;
    public List<Vector2> targetArea;
    public int windUpDepth;

    public int damage {
        get {
            float total = (float)((Weapon)prop).damage;
            total *= (windUpDepth + 1);
            if (weaponStyle != null)
                total *= (float)weaponStyle.damageModifier / 100f;

            return Mathf.RoundToInt(total);
        }
    }

    public int damageReduction {
        get {
            return prop.damageReduction;
        }
    }

    public CombatAction(CombatActionType _type, HeldEquipment _prop, WeaponStyle _weaponStyle = null, List<Vector2> _targetArea = null, int _windUpDepth = 0) {
        type = _type;
        prop = _prop;
        weaponStyle = _weaponStyle;
        targetArea = _targetArea;
        windUpDepth = _windUpDepth;
    }
}

public class Actor {
    public GameObject objectReference;
    public int health;
    public ActorType type;
    public Weapon equippedWeapon;
    public Shield equippedShield;
    public CombatAction previousAction;

    public Actor() {
        HeldEquipment blocker = equippedShield;
        if (blocker == null) // Use defensive stats of weapon if no shield equipped.
            blocker = equippedWeapon;

        previousAction = new CombatAction(CombatActionType.Block, equippedShield);
    }

    public List<CombatAction> getAvailableCombatActions() {
        return new List<CombatAction>();
    }
}
