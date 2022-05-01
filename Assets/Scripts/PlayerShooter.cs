using UnityEngine;


public class PlayerShooter : MonoBehaviour
{
    public enum AimState
    {
        Idle, // 무기 사용 x
        HipFire // 발사 (조준 없이 견착 발사)
    }

    public AimState aimState { get; private set; }

    public Gun gun; // 사용할 총
    public LayerMask excludeTarget; // 조준에서 제외할 레이어들을 할당할 레이어 마스크

    private PlayerInput playerInput; // PlayerInput 타입으로 플레이어 입력정보를 전달
    private Animator playerAnimator; // 애니메이터 컴포넌트
    private Camera playerCamera; // 메인 카메라가 할당될 곳

    private float waitingTimeForReleasingAim = 2.5f; // 마지막 발사 입력 시점으로부터 얼마의 기간동안 발사 입력이 없어야 견착 조준 상태 HitFire 상태에서 Idle 상태로 돌아가는지 그 텀이 되는 시간
    private float lastFireInputTime; // 마지막 발사 입력 시점

    private Vector3 aimPoint; // 실제로 조준하고 있는 대상의 위치

    // PlayerCharacter가 바라보는 방향과 playerCamera 카메라가 바라보는 방향 사이 y축 회전값 사이의 각도가 너무 벌어졌는지 벌어지지 않았는지를 bool타입으로 리턴해주는 프로퍼티
    // y축의 각도 차이가 1도 이상이면 false
    // y축의 각도 차이가 1도 이하면 true
    private bool linedUp => !(Mathf.Abs( playerCamera.transform.eulerAngles.y - transform.eulerAngles.y) > 1f);
    // Player Character가 정면에 총을 발사할 수 있을 정도 넉넉한 공간을 확보하고 있는지를 리턴하는 프로퍼티 -> 벽에 붙는 경우 발사가 안되게
    private bool hasEnoughDistance => !Physics.Linecast(transform.position + Vector3.up * gun.fireTransform.position.y, gun.fireTransform.position, ~excludeTarget);

    // excludeTarget에 Player Character의 레이어가 포함되어 있지 않다면 Player Character의 레이어를 excludeTarget에 추가하는 연산을 수행
    void Awake()
    {
        if (excludeTarget != (excludeTarget | (1 << gameObject.layer)))
        {
            excludeTarget |= 1 << gameObject.layer;
        }
    }

    private void Start()
    {
        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        aimState = AimState.Idle;
        gun.gameObject.SetActive(true);
        gun.Setup(this);
    }

    private void OnDisable()
    {
        aimState = AimState.Idle;
        gun.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (playerInput.fire)
        {
            lastFireInputTime = Time.time;
            Shoot();
        }
        else if (playerInput.reload)
        {
            Reload();
        }
    }

    private void Update()
    {
        // aimPoint 계속해서 갱신
        UpdateAimTarget();

        var angle = playerCamera.transform.eulerAngles.x; // x축 회전값
        if (angle > 270f) angle -= 360f; // -90 ~ +90 범위로 만들기. 270도는 -90도는 마찬가지니까 270도 넘는 건 360을 빼주어야 함

        angle = angle / 180f * -1f + 0.5f; // 1~0 범위로 변환

        playerAnimator.SetFloat("Angle", angle);

        if (!playerInput.fire && Time.time >= lastFireInputTime + waitingTimeForReleasingAim)
        {
            aimState = AimState.Idle;
        }

        UpdateUI();
    }

    public void Shoot()
    {
        if (aimState == AimState.Idle)
        {
            if (linedUp) aimState = AimState.HipFire;
        }
        else if (aimState == AimState.HipFire)
        {
            if (hasEnoughDistance)
            {
                if (gun.Fire(aimPoint)) playerAnimator.SetTrigger("Shoot");
            }
            else
            {
                aimState = AimState.Idle;
            }
        }
    }

    public void Reload()
    {
        // 재장전 입력 감지시 재장전
        if (gun.Reload()) playerAnimator.SetTrigger("Reload");
    }

    private void UpdateAimTarget()
    {
        RaycastHit hit;

        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));

        if (Physics.Raycast(ray, out hit, gun.fireDistance, ~excludeTarget))
        {
            aimPoint = hit.point;

            if (Physics.Linecast(gun.fireTransform.position, hit.point, out hit, ~excludeTarget))
            {
                aimPoint = hit.point;
            }
        }
        else
        {
            aimPoint = playerCamera.transform.position + playerCamera.transform.forward * gun.fireDistance;
        }
    }

    // 탄약 UI 갱신
    private void UpdateUI()
    {
        if (gun == null || UIManager.Instance == null) return;

        // UI 매니저의 탄약 텍스트에 탄창의 탄약과 남은 전체 탄약을 표시
        UIManager.Instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);

        // 크로스 헤어 UI 갱신
        UIManager.Instance.SetActiveCrosshair(hasEnoughDistance); // 충분한 공간이 있을 때만 크로스 헤어를 활성화함
        UIManager.Instance.UpdateCrossHairPosition(aimPoint); // 크로스 헤어를 조준점 위치로 업데이트
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex)
    {
        if (gun == null || gun.state == Gun.State.Reloading) return;

        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다.
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, gun.leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, gun.leftHandMount.rotation);
    }
}