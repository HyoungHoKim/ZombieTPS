using UnityEngine;

public class LateUpdateFollow : MonoBehaviour
{
    // 따라갈 대상 Transform
    public Transform targetToFollow;

    // Update()를 통해 캐릭터 이동을 끝낸 후, LateUpdate를 통해 타겟(targetToFollow)을 따라가도록
    private void LateUpdate()
    {
        // 매 프레임마다 덮어 씌운다.
        transform.position = targetToFollow.position;
        transform.rotation = targetToFollow.rotation;
    }
}