using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 총 4단계에 걸쳐 사격
/// /// 1. 조준중이고, 조준 유효 각도 안에 타겟이 있거나 가깝다면
/// /// 2. 딜레이가 충분히 진행되었다면 애니메이션 재생
/// /// 3. 충돌 검출 하는데, 사격 시 약간의 충격파 더해준다.
/// /// 4. 총구 이펙트 및 총알 이펙트 생성 
/// </summary>
[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : Action
{
    private readonly float startShootDelay = 0.2f;
    private readonly float aimAngleGap = 30f;

    public override void OnReadyAction(StateController controller)
    {
        controller.variables.shotsInRounds = Random.Range(controller.maximumBurst / 2, controller.maximumBurst);
        controller.variables.currentShots = 0;
        controller.variables.startShootTimer = 0f;
        controller.enemyAnimation.anim.ResetTrigger("Shooting");
        controller.enemyAnimation.anim.SetBool("Crouch", false);
        controller.variables.waitInCoverTime = 0;
        controller.enemyAnimation.AbortPendingAim(); // 조준 대기, 시야에 들어오면 조준 가능
    }

    private void DoShot(StateController controller, Vector3 direction, Vector3 hitPoint,
        Vector3 hitNomal = default, bool organic = false, Transform target = null )
    {
        GameObject muzzleFlash = EffectManager.Instance.EffectOneShot((int)EffectList.flash, Vector3.zero);
        muzzleFlash.transform.SetParent(controller.enemyAnimation.gunMuzzle);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.localEulerAngles = Vector3.left * 90f;
        DestroyDelayed destroyDelayed = muzzleFlash.AddComponent<DestroyDelayed>();
        destroyDelayed.delayTime = 0.5f; //auto Delay

        GameObject shotTracer = EffectManager.Instance.EffectOneShot((int)EffectList.tracer, Vector3.zero);
        shotTracer.transform.SetParent(controller.enemyAnimation.gunMuzzle);
        Vector3 origin = controller.enemyAnimation.gunMuzzle.position;
        shotTracer.transform.position = origin;
        shotTracer.transform.rotation = Quaternion.LookRotation(direction);

        if(target && !organic)
        {
            GameObject bulletHole = EffectManager.Instance.EffectOneShot((int)EffectList.bulletHole, 
                hitPoint + 0.01f * hitNomal);
            bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNomal);

            GameObject instantSpark = EffectManager.Instance.EffectOneShot((int)EffectList.sparks,
                hitPoint);
        } 
        else if (target && organic) //플레이어를 맞춘 경우
        {
            HealthBase targetHealth = target.GetComponent<HealthBase>(); // 플레이어 체력
            if(targetHealth)
            {
                targetHealth.TakeDamage(hitPoint, direction, controller.classStats.BulletDamage,
                    target.GetComponent<Collider>(), controller.gameObject);
            }
        }

        SoundManager.Instance.PlayOneShotEffect((int)SoundList.pistol,
            controller.enemyAnimation.gunMuzzle.position, 2f);
    }



    public override void Act(StateController controller)
    {
        throw new System.NotImplementedException();
    }
}
