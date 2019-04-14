using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Wristwatch : MonoBehaviour
    {
        private TextMesh watch;
        private MeshRenderer renderer;
        public Transform head;
        public float viewAngle;

        private void Start()
        {
            watch = GetComponent<TextMesh>();
            renderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            watch.text = DateTime.Now.ToString("HH:mm");

            var watchFacingDir = Quaternion.LookRotation(transform.forward);
            var facingDir = Quaternion.LookRotation(head.forward);

            var angle = Quaternion.Angle(watchFacingDir, facingDir);

            if (angle < 180)
            {
                renderer.enabled = true;
            }
            else
            {
                renderer.enabled = false;
            }
            
            //var color = watch.color;
            //color.a = 1f - (angle / 360f);

            //watch.color = color;

        }
    }
}