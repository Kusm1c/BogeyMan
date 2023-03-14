using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerArms : MonoBehaviour, IGrabable
{
    [SerializeField] private float summonerReleaseTime;
    [SerializeField] private float spammingFactor = 0.02f;
    public SummonerArms OtherArm;
    [HideInInspector] public bool Grabbed;

    private Player grabbingPlayer;
    private bool isSpamming = false;
    private bool cancel = false;
    private float spammingStartingTime = 0;

    public void CancelBecauseDegrab()
    {
        if(isSpamming == true)
        {
            cancel = true;
        }
    }

    private IEnumerator Spamming()
    {
        float gaugeValue = 0;
        spammingStartingTime = Time.time;
        Vector2 player1LastAimDirection = GameManager.Instance.Players[0].playerController.aimDirection;
        Vector2 player2LastAimDirection = GameManager.Instance.Players[1].playerController.aimDirection;
        while (cancel == false && gaugeValue < 1f)
        {
            gaugeValue += Mathf.Abs(Vector2.Distance(GameManager.Instance.Players[0].playerController.aimDirection, player1LastAimDirection) * spammingFactor); 
            gaugeValue += Mathf.Abs(Vector2.Distance(GameManager.Instance.Players[1].playerController.aimDirection, player2LastAimDirection) * spammingFactor);/*
            print(11111);
            print(Vector2.Distance(GameManager.Instance.Players[0].playerController.aimDirection, player1LastAimDirection));
            print(gaugeValue);
            print(Vector2.Distance(GameManager.Instance.Players[0].playerController.aimDirection, player1LastAimDirection));
            print(GameManager.Instance.Players[0].playerController.aimDirection);
            print(player1LastAimDirection);*/
            print(gaugeValue);

            player1LastAimDirection = GameManager.Instance.Players[0].playerController.aimDirection;
            player2LastAimDirection = GameManager.Instance.Players[1].playerController.aimDirection;
            if (Time.time > spammingStartingTime + summonerReleaseTime)
            {
                cancel = true;
            }
            yield return 0;

        }

        if(cancel == true)
        {
            cancel = false;
            print("Cancelled");

        }
        else
        {
            print("Summoner dechired");
        }
    }








    public Collider GetCollider()
    {
        return GetComponent<Collider>();
    }

    public float GetThrowDuration()
    {
        return 0; // no throw duration since it is not throwable
    }

    public float GetThrowSpeed()
    {
        return 0; // no throw speed since it is not throwable
    }

    public bool IsThrowable()
    {
        return false;
    }

    public void OnGrab(Player a_grabbingPlayer)
    {
        foreach(Player p in GameManager.Instance.Players)
        {
            p.playerController.grab.GrabableObjects.Remove(this);
        }
        Grabbed = true;
        grabbingPlayer = a_grabbingPlayer;
        grabbingPlayer.playerState.isGrabbingSummoner = true;
        print("StartedGrab");

        if (OtherArm.Grabbed == true)
        {
            print("StartedSpam");
            StartCoroutine(Spamming());
        }

        return;
    }

    public void OnImpact()
    {
        return; // no throw impact since it is not throwable

    }

    public void OnRelease()
    {
        Grabbed = false;
        grabbingPlayer.playerState.isGrabbingSummoner = false;
        OtherArm.CancelBecauseDegrab();
        print("release");

        return;

    }

    public void OnThrow()
    {
        return;

    }

	public int GetThrowDamage()
	{
        return 0;
    }

	public float GetThrowForce()
	{
        return 0;
	}
}
