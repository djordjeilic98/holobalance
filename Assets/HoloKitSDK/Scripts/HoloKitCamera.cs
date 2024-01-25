using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloKit
{
    public enum CameraType : int
    {
        AR = 0,
        MR = 1
    }

    public enum EyeSide : int
    {
        Right = 0,
        Left = 1
    }

    public class HoloKitCamera : MonoBehaviour
    {
        private static HoloKitCamera instance;
        public static HoloKitCamera Instance
        {
            get {
                if (instance == null)
                {
                    instance = FindObjectOfType<HoloKitCamera>();
                }
                return instance;
            }
        }
        public HoloKit.CameraType cameraType = CameraType.AR;
        public Camera cameraCenter;
        public Camera cameraRight;
        public Camera cameraLeft;
        public HoloKitDistortionPost postefRight;
        public HoloKitDistortionPost postefLeft;
        public Transform holoKitOffset;
        public Profile.ModelType profileModel;
        public Profile.PhoneType profilePhone;

        private int camCullingMask;
        private CameraClearFlags camClearFlags = CameraClearFlags.Color;
        private Color camColor = Color.black;
        private HoloKit.CameraType oldCameraType;

        public Profile profile;

        public void Awake()
        {
            CreateAll();
            switch (cameraType)
            {
                case CameraType.AR:
                    SwitchToModeAR();
                    break;
                case CameraType.MR:
                    SwitchToModeMR();
                    break;
            }

            ChangeProfile();
            UpdateProfile();
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        private void Update()
        {
            if (oldCameraType != cameraType)
            {
                switch (cameraType)
                {
                    case CameraType.AR:
                        SwitchToModeAR();
                        break;
                    case CameraType.MR:
                        SwitchToModeMR();
                        break;
                }
                oldCameraType = cameraType;
            }
        }

        private void CreateAll()
        {
            camCullingMask = cameraCenter.cullingMask;
        }

        protected void SwitchToModeAR()
        {
            holoKitOffset.gameObject.SetActive(false);
            cameraCenter.cullingMask = camCullingMask;
            cameraCenter.clearFlags = camClearFlags;
            cameraCenter.backgroundColor = camColor;
        }

        protected void SwitchToModeMR()
        {
            holoKitOffset.gameObject.SetActive(true);
            cameraRight.cullingMask = camCullingMask;
            cameraLeft.cullingMask = camCullingMask;

            //cameraCenter.cullingMask = camCullingMask;
            //cameraCenter.clearFlags = camClearFlags;
            //cameraCenter.backgroundColor = camColor;

            //if (Display.displays.Length > 1) {
            //    cameraCenter.cullingMask = camCullingMask;
            //    cameraCenter.clearFlags = camClearFlags;
            //    cameraCenter.backgroundColor = camColor;

            //} else {
            cameraCenter.cullingMask = 0;
            cameraCenter.clearFlags = CameraClearFlags.Color;
            cameraCenter.backgroundColor = Color.black;
            //}
        }

        public void ChangeProfile()
        {
            profile = Profile.GetProfile(profileModel, profilePhone);
        }

        public GameObject Avatar;
        Vector3 AvatarPosition;
        Vector3 AvatarScale;
        float XOffset = 0.0f;
        float YOffset = 0.0f;
        float ZOffset = 0.0f;
        float XScaleFactor = 1.0f;
        float YScaleFactor = 1.0f;
        float ZScaleFactor = 1.0f;

        float MatrixParam1 = 0.037f;
        float MatrixParam2 = 0.037f;
        float MatrixParam3 = -0.015f;
        float MatrixParam4 = 0.2f;

        public void SetParameters(string parameters)
        {
            string[] StringArray = parameters.Split(',');

            if (StringArray.Length == 28)
            {
                float[] FloatArray = new float[StringArray.Length];

                for (int j = 0; j < StringArray.Length; j++)
                {
                    FloatArray[j] = float.Parse(StringArray[j]);
                }

                profile = new Profile();
                int i = 0;
                profile.phone.screenWidth = FloatArray[i++];
                profile.phone.screenHeight = FloatArray[i++];
                profile.phone.screenBottom = FloatArray[i++];
                profile.phone.cameraOffset = new Vector3(FloatArray[i++], FloatArray[i++], FloatArray[i++]);
                profile.model.eyeDistance = FloatArray[i++];
                profile.model.lensLength = FloatArray[i++];
                profile.model.fieldOfView = FloatArray[i++];

                profile.model.distortion = FloatArray[i++];
                profile.model.viewBottom = FloatArray[i++];
                profile.model.toEyeDist = FloatArray[i++];
                profile.model.viewWidth = FloatArray[i++];
                profile.model.viewHeight = FloatArray[i++];
                profile.model.mrOffset = new Vector3(FloatArray[i++], FloatArray[i++], FloatArray[i++]);
                profile.model.toScreenDist = FloatArray[i++];

                XOffset = FloatArray[i++];
                YOffset = FloatArray[i++];
                ZOffset = FloatArray[i++];
                XScaleFactor = FloatArray[i++];
                YScaleFactor = FloatArray[i++];
                ZScaleFactor = FloatArray[i++];
                //AvatarPosition = Avatar.transform.position;
                //AvatarPosition.x += XOffset;
                //AvatarPosition.y += YOffset;
                //AvatarPosition.z += ZOffset;
                //Avatar.transform.position = AvatarPosition;
                //AvatarScale = Avatar.transform.localScale;
                //AvatarScale.x *= XScaleFactor;
                //AvatarScale.y *= YScaleFactor;
                //AvatarScale.z *= ZScaleFactor;
                //Avatar.transform.localScale = AvatarScale;

                MatrixParam1 = FloatArray[i++];
                MatrixParam2 = FloatArray[i++];
                MatrixParam3 = FloatArray[i++];
                MatrixParam4 = FloatArray[i++];

                UpdateProfile();
            }
        }

        public void UpdateProfile()
        {
            holoKitOffset.localPosition = profile.model.mrOffset + profile.phone.cameraOffset;

            cameraLeft.rect = profile.GetViewportRect(EyeSide.Left);
            cameraRight.rect = profile.GetViewportRect(EyeSide.Right);

            cameraLeft.transform.localPosition = new Vector3(-profile.model.eyeDistance / 2f, 0f, 0f);
            cameraRight.transform.localPosition = new Vector3(profile.model.eyeDistance / 2f, 0f, 0f);

            float near = cameraLeft.nearClipPlane = cameraRight.nearClipPlane = profile.model.toEyeDist;
            float far = cameraLeft.farClipPlane = cameraRight.farClipPlane = 100f;
            float ipd = profile.model.eyeDistance;

            Rect viewportLeftInMeter = profile.GetViewportRectInMeter(EyeSide.Left);
            Rect viewportRightInMeter = profile.GetViewportRectInMeter(EyeSide.Right);

            //Calc and update projection matrix
            Matrix4x4 leftEyeProjectionMatrix = Matrix4x4.zero;
            Matrix4x4 rightEyeProjectionMatrix = Matrix4x4.zero;
            // Old matrix system

            leftEyeProjectionMatrix[0, 0] = 2.0f * near / MatrixParam1;// viewportLeftInMeter.width;
            leftEyeProjectionMatrix[1, 1] = 2.0f * near / MatrixParam2;// viewportLeftInMeter.height;
            leftEyeProjectionMatrix[0, 2] = MatrixParam3; //(ipd - viewportLeftInMeter.width) / viewportLeftInMeter.width;
            leftEyeProjectionMatrix[1, 2] = MatrixParam4;
            leftEyeProjectionMatrix[2, 2] = -(far + near) / (far - near);
            leftEyeProjectionMatrix[2, 3] = -(2.0f * far * near) / (far - near);
            leftEyeProjectionMatrix[3, 2] = -1.0f;
            rightEyeProjectionMatrix = leftEyeProjectionMatrix;
            rightEyeProjectionMatrix[0, 2] = -MatrixParam3; // (viewportRightInMeter.width - ipd) / viewportRightInMeter.width;

            cameraLeft.projectionMatrix = leftEyeProjectionMatrix;
            cameraRight.projectionMatrix = rightEyeProjectionMatrix;

            postefRight.BarrelDistortionFactor = profile.model.distortion;
            postefLeft.BarrelDistortionFactor = profile.model.distortion;
            //postefLeft.HorizontalOffsetFactor = 0.5f - ipd / 2.0f / viewportLeftInMeter.width;
            postefLeft.HorizontalOffsetFactor = 0.5f / 2.0f / viewportLeftInMeter.width;
            //  postefLeft.HorizontalOffsetFactor = 0f;
            postefLeft.VerticalOffsetFactor = 0f;
            //postefRight.HorizontalOffsetFactor = ipd / 2.0f / viewportRightInMeter.width - 0.5f;
            postefRight.HorizontalOffsetFactor = 0.0f / 2.0f / viewportRightInMeter.width - 0.5f;
            // postefRight.HorizontalOffsetFactor = 0f;
            postefRight.VerticalOffsetFactor = 0f;
        }

        private Matrix4x4 MakeProjection(float left, float top, float right, float bottom, float near, float far)
        {
            Matrix4x4 m = Matrix4x4.zero;
            m[0, 0] = 2f * near / (right - left);
            m[1, 1] = 2f * near / (top - bottom);
            m[0, 2] = (right + left) / (right - left);
            m[1, 2] = (top + bottom) / (top - bottom);
            m[2, 2] = (near + far) / (near - far);
            m[2, 3] = 2f * near * far / (near - far);
            m[3, 2] = -1f;
            return m;
        }
    }
}
