using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Holobalance
{
    public class HolokitCamera : HoloKit.HoloKitCamera
    {
        public void SetToModeAR()
        {
            cameraType = HoloKit.CameraType.AR;
            base.SwitchToModeAR();
        }

        public void SetToModeMR()
        {
            cameraType = HoloKit.CameraType.MR;
            base.SwitchToModeMR();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // initializes holokit camera when new scene loaded
            base.Awake();
        }
    }
}