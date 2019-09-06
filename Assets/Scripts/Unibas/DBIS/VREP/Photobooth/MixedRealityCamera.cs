using UnityEngine;
using Valve.VR;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class MixedRealityCamera : MonoBehaviour
    {
        public Material material;
        public Transform target;
        public bool undistorted = false;

        private void OnEnable()
        {
            // The video stream must be symmetrically acquired and released in
            // order to properly disable the stream once there are no consumers.
            SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted);
            source.Acquire();

            // Auto-disable if no camera is present.
            if (!source.hasCamera)
                enabled = false;
        }

        private void OnDisable()
        {
            // Clear the texture when no longer active.
            material.mainTexture = null;

            // The video stream must be symmetrically acquired and released in
            // order to properly disable the stream once there are no consumers.
            SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted);
            source.Release();
        }

        private void Update()
        {
            SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted);
            Texture2D texture = source.texture;
            if (texture == null)
            {
                return;
            }

            // Apply the latest texture to the material.  This must be performed
            // every frame since the underlying texture is actually part of a ring
            // buffer which is updated in lock-step with its associated pose.
            // (You actually really only need to call any of the accessors which
            // internally call Update on the SteamVR_TrackedCamera.VideoStreamTexture).
            material.mainTexture = texture;

            // Adjust the height of the quad based on the aspect to keep the texels square.
            float aspect = (float)texture.width / (texture.height / 2f);

            
            material.mainTextureOffset = Vector2.zero;
            material.mainTextureScale = new Vector2(1, -0.5f);

            target.localScale = new Vector3(1, 1.0f / aspect, 1);

            // Apply the pose that this frame was recorded at.
            /*if (source.hasTracking)
            {
                SteamVR_Utils.RigidTransform rigidTransform = source.transform;
                target.localPosition = rigidTransform.pos;
                target.localRotation = rigidTransform.rot;
            }*/
        }
        
    }
}