using UnityEngine.InputSystem;

namespace MikeNspired.UnityXRHandPoser
{
    // InputActionReference를 확장하는 InputActionReferenceExtensions 클래스
    public static class InputActionReferenceExtensions
    {
        // 입력 액션을 활성화하는 확장 메서드
        public static void EnableAction(this InputActionReference actionReference)
        {
            // InputAction을 가져옴
            var action = GetInputAction(actionReference);
            // 액션이 null이 아니고 비활성화되어 있으면 활성화
            if (action != null && !action.enabled)
                action.Enable();
        }

        // 입력 액션을 비활성화하는 확장 메서드
        public static void DisableAction(this InputActionReference actionReference)
        {
            // InputAction을 가져옴
            var action = GetInputAction(actionReference);
            // 액션이 null이 아니고 활성화되어 있으면 비활성화
            if (action != null && action.enabled)
                action.Disable();
        }

        // InputActionReference에서 InputAction을 가져오는 메서드
        public static InputAction GetInputAction(this InputActionReference actionReference)
        {
            // null 전파 사용하여 InputAction을 반환하거나 null 반환
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }
    }
}