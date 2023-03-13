using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public bool isKnockedBack = false;
    public bool isAttacking = false;
    public bool isGrabbing = false;
    public bool isGrabbingSummoner = false;
    public bool isOnDeadAlly = false;
    public bool isReflecting = false;
    public bool isDead = false;
    public bool isInvulnerable = false;

    public bool canMove => isKnockedBack == false
                        && isGrabbingSummoner == false 
                        && isDead == false;

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
