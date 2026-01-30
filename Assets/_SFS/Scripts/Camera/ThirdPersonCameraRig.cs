using UnityEngine;
using SFS.Core;

namespace SFS.Camera
{
    public class ThirdPersonCameraRig : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;

        [Header("Follow")]
        public Vector3 offset = new Vector3(0, 2.2f, -4.2f);
        public float followSmooth = 12f;

        [Header("Look")]
        public float lookSmooth = 14f;

        [Header("Reduced Motion Settings")]
        public float reducedFollowSmooth = 30f;
        public float reducedLookSmooth = 30f;

        void OnEnable()
        {
            GameEvents.OnSettingsChanged += ApplySettings;
        }

        void OnDisable()
        {
            GameEvents.OnSettingsChanged -= ApplySettings;
        }

        void LateUpdate()
        {
            if (!target) return;

            bool reduced = SettingsManager.Instance && SettingsManager.Instance.Data.reducedMotion;
            float fSmooth = reduced ? reducedFollowSmooth : followSmooth;
            float lSmooth = reduced ? reducedLookSmooth : lookSmooth;

            Vector3 desiredPos = target.position + target.TransformDirection(offset);
            transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-fSmooth * Time.deltaTime));

            Quaternion desiredRot = Quaternion.LookRotation((target.position + Vector3.up * 1.4f) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-lSmooth * Time.deltaTime));
        }

        void ApplySettings() { /* values read during LateUpdate */ }
    }
}
