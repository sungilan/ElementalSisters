using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    // 수류탄을 제어하는 클래스
    public class Grenade : MonoBehaviour, IDamageable
    {
        // 상호작용 가능한 오브젝트와 폭발 이펙트, 활성화 사운드, 메쉬 라이트 활성화를 위한 변수
        [SerializeField] private XRGrabInteractable interactable = null;
        [SerializeField] private GameObject Explosion = null;
        [SerializeField] private AudioSource activationSound = null;
        [SerializeField] private GameObject meshLightActivation = null;

        // 폭발까지의 시간 및 활성화 후 타이머 시작 여부
        [SerializeField] private float detonationTime = 3;
        [SerializeField] private bool startTimerAfterActivation = false;

        private bool canActivate;
        private XRInteractionManager interactionManager;

        // Start is called before the first frame update
        void Start()
        {
            OnValidate();
            interactable = GetComponent<XRGrabInteractable>();
            interactable.onActivate.AddListener(TurnOnGrenade);
            interactable.onSelectExited.AddListener(Activate);
            if (meshLightActivation)
                meshLightActivation.SetActive(false);
        }

        private void OnValidate()
        {
            if (!interactable)
                interactable = GetComponent<XRGrabInteractable>();
            if (!interactionManager)
                interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        // 수류탄 활성화
        private void TurnOnGrenade(XRBaseInteractor interactor)
        {
            canActivate = true;
            meshLightActivation.SetActive(true);
            activationSound.Play();

            if (startTimerAfterActivation)
                Invoke(nameof(TriggerGrenade), detonationTime);
        }

        // 수류탄 활성화
        private void Activate(XRBaseInteractor interactor)
        {
            if (canActivate && !startTimerAfterActivation)
                Invoke(nameof(TriggerGrenade), detonationTime);
        }

        // 수류탄 폭발
        private void TriggerGrenade()
        {
            Explosion.SetActive(true);
            Explosion.transform.parent = null;
            Explosion.transform.localEulerAngles = Vector3.zero;

            if (interactable.selectingInteractor)
                interactionManager.SelectExit(interactable.selectingInteractor, interactable);

            StartCoroutine(MoveAndDisableCollider());
        }

        // 오브젝트 이동 및 충돌체 비활성화
        private IEnumerator MoveAndDisableCollider()
        {
            transform.position += Vector3.one * 9999;
            yield return new WaitForSeconds(Time.fixedDeltaTime * 2);
            Destroy(gameObject);
        }

        // 수류탄이 피해를 받았을 때
        public void TakeDamage(float damage, GameObject damager)
        {
            TriggerGrenade();
        }
    }
} 