using System.Collections;
using System.Collections.Generic;
using FC;
using UnityEngine;

public class AlertChecker : MonoBehaviour
{
    [Range(0f, 50f)] public float alertRadius; // 경고를 뿌릴 범위
    public int extraWaves = 1;

    public LayerMask alertMask = TagAndLayer.LayerMasking.Enemy;
    private Vector3 current;
    private bool alert;

    private void Start()
    {
        InvokeRepeating("PingAlert", 1, 1); // PingAlert를 1초주기로 1초동안 반복
    }

    private void AlertNearBy(Vector3 origin, Vector3 target, int wave = 0)
    {
        if (wave > extraWaves)
        {
            return;
        }

        Collider[] targetsInViewRadius = Physics.OverlapSphere(origin, alertRadius, alertMask);

        foreach (Collider obj in targetsInViewRadius)
        {
            obj.SendMessageUpwards("AlertCallback", target, SendMessageOptions.DontRequireReceiver);

            AlertNearBy(obj.transform.position, target, wave + 1);
        }
    }

    public void RootAlertNearBy(Vector3 origin)
    {
        current = origin;
        alert = true;
    }

    void PingAlert()
    {
        if (alert)
        {
            alert = false;
            AlertNearBy(current, current);
        }
    }
}
