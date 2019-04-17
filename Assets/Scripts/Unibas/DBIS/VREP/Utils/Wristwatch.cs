using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Wristwatch : MonoBehaviour
    {
        private TextMesh watch;
        private MeshRenderer renderer;
        public Transform head;
        public float viewAngle, facingAngle;

        private void Start()
        {
            watch = GetComponent<TextMesh>();
            renderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            watch.text = DateTime.Now.ToString("HH:mm");

            var watchFacingDir = Quaternion.LookRotation(transform.forward);
            var watchUpDir = Quaternion.LookRotation(transform.up);
            var facingDir = Quaternion.LookRotation(head.forward);
            var facingUp = Quaternion.LookRotation(head.up);

            var angle = Quaternion.Angle(watchFacingDir, facingDir);
            var upAngle = Quaternion.Angle(watchUpDir, facingUp);
            
            if (angle < viewAngle && upAngle < facingAngle)
            {
                if (!renderer.enabled)
                {
                    renderer.enabled = true;
                }
            }
            else
            {
                if (renderer.enabled)
                {
                    renderer.enabled = false;
                }
            }
            
            //var color = watch.color;
            //color.a = 1f - (angle / 360f);

            //watch.color = color;

        }
    }
}