using System;
using Unibas.DBIS.VREP;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Wrist watch code by f.spiess@unibas.ch
    /// </summary>
    public class Wristwatch : MonoBehaviour
    {
        private TextMesh watch;
        private MeshRenderer renderer;
        public Transform head;
        public float viewAngle, facingAngle;

        private void Start()
        {
            if (VREPController.Instance.Settings.WristwatchEnabled)
            {
                watch = GetComponent<TextMesh>();
                renderer = GetComponent<MeshRenderer>();                
            }
            else
            {
                gameObject.SetActive(false);
            }
            
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