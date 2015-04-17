using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public int windUpLevel;
    public int offBalance;

    public int damage {
        get {
            // damage is weapon base damage * style modifier % * wind up level
            float total = (float)((Weapon)prop).damage;
            total *= (windUpLevel + 1);
            if (weaponStyle != null)
                total *= (float)weaponStyle.damageModifier / 100f;

            return Mathf.RoundToInt(total);
        }
    }

    public int damageReduction {
        get {
            // damage redutcion is halved for each level off balance
            float modifier = Mathf.Pow(.5f, float(offBalance));
            float total = prop.damageReduction * modifier;
            return Mathf.RoundToInt(total);
        }
    }

    public void InitBlock(HeldEquipment _prop, int _offBalance) {
        type = CombatActionType.Block;
        prop = _prop;
        offBalance = _offBalance;
    }

    public void InitParry(HeldEquipment _prop, WeaponStyle _weaponStyle, int _windUpDepth, int _offBalance) {
        type = CombatActionType.Parry;
        prop = _prop;
        weaponStyle = _weaponStyle;
        windUpLevel = _windUpDepth;
        offBalance = _offBalance;
    }

    public void InitWindUp(HeldEquipment _prop, WeaponStyle _weaponStyle, int _windUpDepth) {
        type = CombatActionType.WindUp;
        prop = _prop;
        weaponStyle = _weaponStyle;
        windUpLevel = _windUpDepth;
    }

    public void InitSwitchStyle(HeldEquipment _prop, int _windUpDepth) {
        type = CombatActionType.SwitchStyle;
        prop = _prop;
        windUpLevel = _windUpDepth;
    }

    public void InitSwing(HeldEquipment _prop, WeaponStyle _weaponStyle, List<Vector2> _targetArea,
        int _windUpDepth) {

        type = CombatActionType.Swing;
        prop = _prop;
        weaponStyle = _weaponStyle;
        targetArea = _targetArea;
        windUpLevel = _windUpDepth;
    }

    public void InitRecover(int _offBalance) {
        type = CombatActionType.Recover;
        offBalance = _offBalance;
    }
}

public static class CombatActionGraph {
    //private Dictionary<CombatActionType, List<CombatActionType>> graph;

    // Eventually, I'll implement this as an actual graph and possibly even read the values
    // of it from a text file. For now, a giant hardcoded switch statement will give the same
    // functionality until I start doing some serious design modifications.
    public static void GenerateGraph() {
        //// This can eventually involve reading from a text file or script,
        //// but I'll hardcode the graph for now.

        //graph = new Dictionary<CombatActionType, List<CombatActionType>>();

        //// Block
        //graph.Add(CombatActionType.Block, new List<CombatActionType>() {
        //    CombatActionType.Block,
        //    CombatActionType.Swing,
        //    CombatActionType.WindUp,
        //    CombatActionType.Parry
        //});

        //// Parry
        //graph.Add(CombatActionType.Parry, new List<CombatActionType>() {
        //    CombatActionType.Swing,
        //    CombatActionType.WindUp,
        //    CombatActionType.SwitchStyle,
        //    CombatActionType.Parry
        //});

        //// Wind Up
        //graph.Add(CombatActionType.WindUp, new List<CombatActionType>() {
        //    CombatActionType.Swing,
        //    CombatActionType.WindUp,
        //    CombatActionType.SwitchStyle,
        //    CombatActionType.Parry
        //});

        //// Switch Style

        //// Wind Up
        //graph.Add(CombatActionType.SwitchStyle, new List<CombatActionType>() {
        //    CombatActionType.Swing,
        //    CombatActionType.WindUp,
        //    CombatActionType.SwitchStyle,
        //    CombatActionType.Parry
        //});

        //// Recover
    }

    public static List<CombatActionType> GetAvailableActions(CombatAction prevAction) {
        List<CombatActionType> actionTypeList = new List<CombatActionType>();

        if (prevAction == null) {
            actionTypeList.Add(CombatActionType.Block);
            actionTypeList.Add(CombatActionType.Parry);
            actionTypeList.Add(CombatActionType.WindUp);
            actionTypeList.Add(CombatActionType.Swing);
            return actionTypeList;
        }

        switch (prevAction.type) {
            case CombatActionType.Block:
                actionTypeList.Add(CombatActionType.Block);
                actionTypeList.Add(CombatActionType.Parry);
                actionTypeList.Add(CombatActionType.WindUp);
                actionTypeList.Add(CombatActionType.Swing);
                break;
            case CombatActionType.Parry:
                actionTypeList.Add(CombatActionType.WindUp);
                actionTypeList.Add(CombatActionType.SwitchStyle);
                actionTypeList.Add(CombatActionType.Swing);
                actionTypeList.Add(CombatActionType.Recover);
                break;
            case CombatActionType.WindUp:
                actionTypeList.Add(CombatActionType.WindUp);
                actionTypeList.Add(CombatActionType.SwitchStyle);
                actionTypeList.Add(CombatActionType.Swing);
                actionTypeList.Add(CombatActionType.Recover);
                break;
            case CombatActionType.SwitchStyle:
                actionTypeList.Add(CombatActionType.WindUp);
                actionTypeList.Add(CombatActionType.SwitchStyle);
                actionTypeList.Add(CombatActionType.Swing);
                actionTypeList.Add(CombatActionType.Recover);
                break;
            case CombatActionType.Swing:
                actionTypeList.Add(CombatActionType.Block);
                actionTypeList.Add(CombatActionType.Parry);
                actionTypeList.Add(CombatActionType.WindUp);
                actionTypeList.Add(CombatActionType.Swing);
                actionTypeList.Add(CombatActionType.Recover);
                break;
            case CombatActionType.Recover:
                actionTypeList.Add(CombatActionType.Block);
                actionTypeList.Add(CombatActionType.Parry);
                actionTypeList.Add(CombatActionType.WindUp);
                actionTypeList.Add(CombatActionType.Swing);
                actionTypeList.Add(CombatActionType.Recover);
                break;
            default:
                break;
        }

        return actionTypeList;
    }
}
