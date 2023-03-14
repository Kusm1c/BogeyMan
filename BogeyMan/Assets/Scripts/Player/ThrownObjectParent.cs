using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownObjectParent : MonoBehaviour
{
    [HideInInspector] public float Speed;
    [HideInInspector] public float Duration;
    [HideInInspector] public int Damage;
    private float flyStartTime = 0;
    [HideInInspector] public Player LastHitingPlayer = null;
    private IGrabable thrownObject = null;
    private Rigidbody rb = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    public void Throw(float speed, float duration, Player throwingPlayer, Vector3 direction, int damage, IGrabable thrownObjectRef = null)
    {
        if(thrownObjectRef != null)
        {            
            thrownObject = thrownObjectRef;
            thrownObject.transform.SetParent(transform);
            thrownObject.transform.localPosition = Vector3.zero;
        }
        Damage = damage;
        flyStartTime = Time.time;
        Speed = speed;
        Duration = duration;
        LastHitingPlayer = throwingPlayer;
        rb.velocity = direction.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        IGrabable collidedGrabable = other.GetComponent<IGrabable>();
        if(collidedGrabable == thrownObject || other.GetComponentInParent<Player>() == LastHitingPlayer || other.isTrigger == true)
        {
            return;
        }
        
        if(collidedGrabable == null || collidedGrabable.IsThrowable() == false)
        {
            Enemies.Enemy enemy = other.GetComponent<Enemies.Enemy>();
            if(enemy != null)
            {
                enemy.TakeHit(0, Vector3.zero, Damage);
            }
            FinishLaunch();
        }
        else
        {
            Enemies.Enemy enemy = other.GetComponent<Enemies.Enemy>();
            if(enemy != null)
            {
                Vector3 direction;

                Vector3 EnemyrelativePos = enemy.transform.position - transform.position;
                Vector2 flatEnemyRelativePos = new Vector2(EnemyrelativePos.x, EnemyrelativePos.z);
                Vector2 OrthogonalFlatDirection = new Vector2(rb.velocity.z, -rb.velocity.x);

                direction =new Vector3(OrthogonalFlatDirection.x, 0, OrthogonalFlatDirection.y).normalized;
                if (Vector2.Dot(OrthogonalFlatDirection, flatEnemyRelativePos) < 0)
                    direction *= -1f;

                enemy.TakeHit(thrownObject.GetThrowForce(), direction, Damage);
            }
        }
    }

    private void FixedUpdate()
    {
        bool InRangeForSlowmotion = false;
        Player p = GameManager.Instance.Players[LastHitingPlayer.playerIndex == 1 ? 0 : 1];
        if (p != null)
        {
            Vector2 flatPlayerPos = new Vector2(p.transform.position.x, p.transform.position.z);
            Vector2 flatTrownObjectPos = new Vector2(transform.position.x, transform.position.z);

            float distance = Vector2.Distance(flatTrownObjectPos, flatPlayerPos);
            if (distance <= p.settings.slowMotionReflectRange)
            {
                InRangeForSlowmotion = true;
            }
        }

        if(InRangeForSlowmotion == true)
        { 
            if(GameManager.Instance.SlowMotionIsActive == false)
            {
                GameManager.Instance.SlowMotionIsActive = true;
                Time.timeScale = LastHitingPlayer.settings.slowmotionTimeScale;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                p.playerController.SetSlowMotionLightAttackAcceleration(p.settings.slowmotionTimeScale);
            }
        }
        else
        {
            if(GameManager.Instance.SlowMotionIsActive == true)
            {
                GameManager.Instance.SlowMotionIsActive = false;
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                p.playerController.SetSlowMotionLightAttackAcceleration(1);
            }
            if (Time.time > flyStartTime + Duration)
            {
                FinishLaunch();
            }
        }

    }

    private void FinishLaunch()
    {
        GetComponent<Collider>().enabled = false;
        //fx
        Grab.ResetGrabbedObject(thrownObject);
        thrownObject.OnImpact();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance.SlowMotionIsActive == true)
        {
            GameManager.Instance.SlowMotionIsActive = false;
            Time.timeScale = 1;
        }
    }
}
