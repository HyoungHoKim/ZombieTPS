using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // 유니티 입력 설정에 기본으로 추가되어 있는 이름
    public string fireButtonName = "Fire1"; // 발사를 위한 입력 버튼 이름
    public string jumpButtonName = "Jump"; // 점프를 위한 입력 버튼 이름
    public string moveHorizontalAxisName = "Horizontal"; // 좌우 회전을 위한 입력축 이름
    public string moveVerticalAxisName = "Vertical"; // 앞뒤 움직임을 위한 입력축 이름

    // 유니티 기본 입력에 없기 때문에 추가 필요
    public string reloadButtonName = "Reload"; // 재장전을 위한 입력 버튼 이름

    // 값 할당은 내부에서만 가능
    public Vector2 moveInput { get; private set; } // Horizontal, Vertical
    public bool fire { get; private set; } // 감지된 발사 입력값
    public bool jump { get; private set; } // 감지된 점프 입력값
    public bool reload { get; private set; } // 감지된 재장전 입력값

    // 매 프레임 사용자 입력을 감지
    private void Update()
    {
        // 게임오버 상태에서는 사용자 입력을 감지하지 않는다.
        if (GameManager.Instance != null
            && GameManager.Instance.isGameover)
        {
            moveInput = Vector2.zero;
            fire = false;
            reload = false;
            jump = false;
            return;
        }

        moveInput = new Vector2(Input.GetAxis(moveHorizontalAxisName), Input.GetAxis(moveVerticalAxisName));

        // 대각선 이동시 벡터 길이가 1보다 커지기 때문에 속도가 빨라지는 현상이 발생한다. 이를 제한
        if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;

        jump = Input.GetButtonDown(jumpButtonName);
        fire = Input.GetButton(fireButtonName);
        reload = Input.GetButtonDown(reloadButtonName);
    }
}