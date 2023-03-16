using System;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class SummonerArms : MonoBehaviour, IGrabable
    {
        [SerializeField] public float summonerReleaseTime;
        [SerializeField] private SummonerArms otherArm;
        [SerializeField] private Summoner summoner;
        [SerializeField] private GameObject uiGrabFeedback;
        [SerializeField] private GameObject uiSpamFeedback;
        [SerializeField] private SummonerUI summonerUI;
        [HideInInspector] public bool Grabbed;

        private Player grabbingPlayer;
        private bool isSpamming = false;
        private bool cancel = false;
        private float spammingStartingTime = 0;

        private void Start()
        {
            uiSpamFeedback.SetActive(false);
        }

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
            summonerUI.FillGauge(0);
            summonerUI.SetGaugeVisible(true);
            spammingStartingTime = Time.time;
            Player player1 = GameManager.Instance.Players[0];
            Player player2 = GameManager.Instance.Players[1];
            Vector2 player1LastAimDirection = player1.playerController.aimDirection;
            Vector2 player2LastAimDirection = player2.playerController.aimDirection;
            while (cancel == false && gaugeValue < 1f)
            {
                gaugeValue += Mathf.Abs(Vector2.Distance(player1.playerController.aimDirection, player1LastAimDirection)
                                        * player1.settings.summonerDismembermentSpammingFactor); 
                gaugeValue += Mathf.Abs(Vector2.Distance(player2.playerController.aimDirection, player2LastAimDirection)
                                        * player2.settings.summonerDismembermentSpammingFactor);

                summonerUI.FillGauge(gaugeValue);

                player1LastAimDirection = GameManager.Instance.Players[0].playerController.aimDirection;
                player2LastAimDirection = GameManager.Instance.Players[1].playerController.aimDirection;
                if (Time.time > spammingStartingTime + summonerReleaseTime) // à remplacer par isstunned = false;
                {
                    cancel = true;
                }
                yield return 0;
            }

            if(cancel == true)
            {
                cancel = false;
            }
            else
            {
                summoner.DieForReal();

                // Degrab players
                Cancel();
                otherArm.Cancel();
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
            Grabbed = true;
            grabbingPlayer = a_grabbingPlayer;
            grabbingPlayer.playerState.isGrabbingSummoner = true;
            grabbingPlayer.playerController.characterAnimator.SetBool("IsGrabbingSummoner", true);
            cancel = false;
            uiSpamFeedback.SetActive(true);
            uiGrabFeedback.SetActive(false);

            if (otherArm.Grabbed == true)
            {
                StartCoroutine(Spamming());
            }

            return;
        }

        public void Cancel()
        {
            if (Grabbed)
            {
                cancel = true;

                grabbingPlayer.playerController.grab.Release();
            }
        }

        public void OnImpact()
        {
            return; // no throw impact since it is not throwable

        }

        public void OnRelease()
        {
            Grabbed = false;
            grabbingPlayer.playerState.isGrabbingSummoner = false;
            grabbingPlayer.playerController.characterAnimator.SetBool("IsGrabbingSummoner", false);
            otherArm.CancelBecauseDegrab();
            uiSpamFeedback.SetActive(false);
            uiGrabFeedback.SetActive(true);
            summonerUI.FillGauge(0);
            summonerUI.SetGaugeVisible(false);
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
}
