using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player Character의 오브젝트의 Collider 처리
    private CharacterController characterController;
    // 입력 감지
    private PlayerInput playerInput;
    private PlayerShooter playerShooter;
    // Player Character 오브젝트의 애니메이션을 제어
    private Animator animator;

    // 플레이어가 움직일 때 카메라의 방향을 기준으로 움직이게 되므로 현재의 카메라 방향을 알기 위해서 필요
    private Camera followCam;

    public float speed = 6f;
    public float jumpVelocity = 8f;
    // 점프 시 플레이어가 공중에 떠 있는동안 원래 속도의 몇 퍼센트의 속도를 통제할 수 있을지에 대한 퍼센트
    // 0 : 플레이어가 공중에 있을 때 조작을 할 수 없다.
    // 1 : 플레이어가 공중에 있어도 원래 속도로 조작할 수 있다.
    [Range(0.01f, 1f)] public float airControlPercent;

    // 이동하는데 있어 스무스하게 댐핑하는 지연시간
    public float speedSmoothTime = 0.1f;
    // 회전하는데 있어 스무스하게 댐핑하는 지연시간
    public float turnSmoothTime = 0.1f;

    // Mathf의 SmoothDamp함수를 사용할 때 스무스하게 변화하는 값을 계속해서 리턴하기 때문에 그 값을 받기 위해 선언
    private float speedSmoothVelocity;
    private float turnSmoothVelocity;

    // 플레이어의 Y방향 속도 -> Rigidbody와 달리 Character Controller 컴포넌트는 외부 물리적인 힘을 받지 않으므로
    // 중력을 받아 떨어지지 않기 때문에 개발자가 직접 Y방향 속도를 조정해야한다.
    private float currentVelocityY;

    // Y를 제외한 X,Z방향에서의 속도. 즉, 플레이어의 지면 상에서의 현재 속도
    // get 프로퍼티를 람다식으로 =>을 사용하여 간단하게 표현
    public float currentSpeed =>
        new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerShooter = GetComponent<PlayerShooter>();
        followCam = Camera.main;
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // 플레이어가 조금이라도 움직이거나 공격할 때 카메라와 플레이어의 방향을 일치시켜줌
        if (currentSpeed > 0.2f || playerInput.fire) Rotate();

        Move(playerInput.moveInput);

        if (playerInput.jump) Jump();
    }

    private void Update()
    {
        UpdateAnimation(playerInput.moveInput);
    }

    public void Move(Vector2 moveInput)
    {
        var targetSpeed = speed * moveInput.magnitude;
        var moveDirection = Vector3.Normalize(transform.forward * moveInput.y + transform.right * moveInput.x);

        var smoothTime = characterController.isGrounded ? speedSmoothTime : speedSmoothTime / airControlPercent;

        targetSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
        currentVelocityY += Time.deltaTime * Physics.gravity.y;

        var velocity = moveDirection * targetSpeed + Vector3.up * currentVelocityY;

        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded) currentVelocityY = 0;
    }

    public void Rotate()
    {
        var targetRotation = followCam.transform.eulerAngles.y;

        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
    }

    public void Jump()
    {
        if (!characterController.isGrounded) return;
        currentVelocityY = jumpVelocity;
    }

    private void UpdateAnimation(Vector2 moveInput)
    {
        var animationSpeedPercent = currentSpeed / speed;

        animator.SetFloat("Horizontal Move", moveInput.x * animationSpeedPercent, 0.05f, Time.deltaTime);
        animator.SetFloat("Vertical Move", moveInput.y * animationSpeedPercent, 0.05f, Time.deltaTime);
    }
}