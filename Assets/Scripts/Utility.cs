using UnityEngine;
using UnityEngine.AI;

public static class Utility
{
    // NavMesh 위에서 어떤 위치와 반경을 기준으로한 랜덤한 위치를 리턴
    // args : center(중심 위치), distance(반경 거리), areaMask(검색할 Area)
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask)
    {
        // center를 중점으로 하여 반지름(반경) distance 내에 랜덤한 위치 리턴.
        // *Random.insideUnitSphere*은 반지름 1 짜리의 구 내에서 랜덤한 위치를 리턴해주는 프로퍼티다.
        var randomPos = Random.insideUnitSphere * distance + center;

        // NavMesh 샘플링의 결과를 담을 컨테이너. Raycast hit 과 비슷
        NavMeshHit hit;

        // areaMask에 해당하는 NavMesh 중에서 randomPos로부터 distance 반경 내에서 randomPos에 *가장 가까운* 위치를 하나 찾아서 그 결과를 hit에 담음.
        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);

        // 샘플링 결과 위치인 hit.position 리턴
        return hit.position;
    }

    public static float GetRandomNormalDistribution(float mean, float standard)
    {
        var x1 = Random.Range(0f, 1f);
        var x2 = Random.Range(0f, 1f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }
}