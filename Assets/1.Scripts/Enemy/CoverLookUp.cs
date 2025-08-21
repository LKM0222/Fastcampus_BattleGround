using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 장애물 찾기
/// 단순히 찾기만 하는게 아니라 최적의 장애물을 찾음.
/// 숨을 만한 곳을 찾아주는 컴포넌트
/// 플레이어보다 멀리 있는건 제외.
/// </summary>

public class CoverLookUp : MonoBehaviour
{
    private List<Vector3[]> allCoverSpots; // 모든 커버 스팟
    private GameObject[] covers;
    private List<int> coverHashCodes; // Cover unity Id;
    private Dictionary<float, Vector3> filteredSpots; // npc로 부터 특정 오브젝트에서 멀어지거나 하는것들 (제외할 스팟)

    // 현재 씬에서 특정 레이어를 가진 오브젝트 전체를 가져오는 함수.
    private GameObject[] GetObjectsInLayerMask(int layerMask)
    {
        List<GameObject> ret = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.activeInHierarchy && layerMask == (layerMask | (1 << go.layer)))
            {
                ret.Add(go);
            }
        }
        return ret.ToArray();
    }

    private void ProcessPoint(List<Vector3> vector3s, Vector3 nativePoint, float range)
    {
        // 특정 커버가 meshCollider안에 있다면, 유효한곳인가 확인 (갈 수 있는 오브젝트인지)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(nativePoint, out hit, range, NavMesh.AllAreas))
        {
            vector3s.Add(hit.position);
        }
    }

    private Vector3[] GetSpots(GameObject go, LayerMask obstacleMask)
    {
        List<Vector3> bounds = new List<Vector3>();
        foreach (Collider col in go.GetComponents<Collider>())
        {
            float baseHeight = (col.bounds.center - col.bounds.extents).y;
            float range = 2 * col.bounds.extents.y;

            Vector3 deslocalForward = go.transform.forward * go.transform.localScale.z * 0.5f;
            Vector3 deslocalRight = go.transform.right * go.transform.localScale.x * 0.5f;

            if (go.GetComponent<MeshCollider>())
            {
                float maxBounds = go.GetComponent<MeshCollider>().bounds.extents.z + go.GetComponent<MeshCollider>().bounds.extents.x;
                Vector3 originForward = col.bounds.center + go.transform.forward * maxBounds;
                Vector3 originRight = col.bounds.center + go.transform.right * maxBounds;
                if (Physics.Raycast(originForward, col.bounds.center - originForward, out RaycastHit hit, maxBounds, obstacleMask))
                {
                    deslocalForward = hit.point - col.bounds.center;
                }

                if (Physics.Raycast(originRight, col.bounds.center - originRight, out hit, maxBounds, obstacleMask))
                {
                    deslocalRight = hit.point - col.bounds.center;
                }
            }
            else if (Vector3.Equals(go.transform.localScale, Vector3.one))
            {
                deslocalForward = go.transform.forward * col.bounds.extents.z;
                deslocalRight = go.transform.right * col.bounds.extents.x;
            }

            // 갈 수 있는 방향 탐색
            float edgeFactor = 0.75f;
            ProcessPoint(bounds, col.bounds.center + deslocalRight + deslocalForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center + deslocalForward + deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center + deslocalForward, range);
            ProcessPoint(bounds, col.bounds.center + deslocalForward - deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight + deslocalForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center + deslocalRight, range);
            ProcessPoint(bounds, col.bounds.center + deslocalRight - deslocalForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalForward + deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalForward, range);
            ProcessPoint(bounds, col.bounds.center - deslocalForward - deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight - deslocalForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight, range);
        }
        return bounds.ToArray();
    }
}
