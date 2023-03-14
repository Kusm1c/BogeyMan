using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [HideInInspector] public bool isKnockedBack = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isGrabbing = false;
    [HideInInspector] public bool isGrabbingSummoner = false;
    [HideInInspector] public bool isOnDeadAlly = false;
    [HideInInspector] public bool isRevivingAlly = false;
    [HideInInspector] public bool isReflecting = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isInvulnerable = false;

    public bool canMove => isKnockedBack == false
                        && isGrabbingSummoner == false 
                        && isDead == false
                        && isRevivingAlly == false;

    public bool canAttack => isAttacking == false
                        && isGrabbingSummoner == false
                        && isDead == false;

    public bool canThrow => isAttacking == false
                        && isOnDeadAlly == false
                        && isDead == false
                        && isGrabbing == true;

    public bool canReflect => canAttack == true
                        && isDead == false;

    public bool canGrab => isAttacking == false
                        && isOnDeadAlly == false
                        && isDead == false;

    public bool canAim => isAttacking == false
                        && isDead == false
                        && isKnockedBack == false;
}
