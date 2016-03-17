using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActorType
{
    Player,
    Creature,
    Item
}

public class Actor
{
    public class CombatActorState
    {
        public CombatAction prevAction;
        public WeaponStyle weaponStyle;
        public List<Vector2> targetArea;
        public int windUpLevel;
        public int offBalanceLevel;

        public CombatActorState()
        {
            prevAction = null;
            weaponStyle = null;
            targetArea = null;
            windUpLevel = 0;
            offBalanceLevel = 0;
        }

        public void Clear()
        {
            prevAction = null;
            weaponStyle = null;
            targetArea = null;
            windUpLevel = 0;
            offBalanceLevel = 0;
        }
    }

    public GameObject objectReference;
    public int health;
    public ActorType type;
    public Weapon weapon;
    public Shield shield;
    public CombatActorState combatState;

    public Actor()
    {
        combatState = new CombatActorState();
    }

    public List<CombatAction> getAvailableCombatActions()
    {
        List<CombatAction> actionList = new List<CombatAction>();
        List<CombatActionType> potentialActions = CombatActionGraph.GetAvailableActions(combatState.prevAction);

        foreach (CombatActionType actionType in potentialActions)
        {
            CombatAction action = new CombatAction();
            switch (actionType)
            {
                case CombatActionType.Block:
                    if (shield != null)
                    {
                        action.InitBlock(shield, combatState.offBalanceLevel);
                    }
                    break;
                case CombatActionType.Parry:
                    action.InitParry(weapon, combatState.weaponStyle, combatState.windUpLevel, combatState.offBalanceLevel);
                    break;
                case CombatActionType.WindUp:
                    if (combatState.windUpLevel < combatState.weaponStyle.maxWindUpDepth)
                        action.InitWindUp(weapon, combatState.weaponStyle, combatState.windUpLevel);
                    break;
                case CombatActionType.SwitchStyle:
                    action.InitSwitchStyle(weapon, combatState.windUpLevel);
                    break;
                case CombatActionType.Swing:
                    action.InitSwing(weapon, combatState.weaponStyle, combatState.targetArea, combatState.windUpLevel);
                    break;
                case CombatActionType.Recover:
                    action.InitRecover(combatState.offBalanceLevel);
                    break;
                default:
                    break;
            }
        }

        return new List<CombatAction>();
    }
}
