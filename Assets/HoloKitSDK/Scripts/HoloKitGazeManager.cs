using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HoloKit
{
    [RequireComponent(typeof(Collider))]
    public partial class HoloKitGazeTarget : MonoBehaviour
    {
        [Header("Gaze Callbacks")]
        public UnityEvent GazeEnter;
        public UnityEvent GazeExit;
    }

    public class HoloKitGazeManager : MonoBehaviour
    {
        public Image gazingBar;
        public float timeToSelect;

        private bool gazing;
        private float timeGazing;

        private float overridenTimeToSelectl;

        private static HoloKitGazeManager instance;

        public static HoloKitGazeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<HoloKitGazeManager>();
                }

                return instance;
            }
        }

        public Transform GazeCursor;
        public float RaycastDistance = 10;

        public LayerMask RaycastMask;

        private HoloKitGazeTarget currentTarget;

        public HoloKitGazeTarget CurrentTarget 
        {
            get { return currentTarget; }
        }

        private Vector3 initialScale;

        void Start()
        {
            initialScale = GazeCursor.localScale;
        }

        void Update()
        {
            Transform eyeCenter = HoloKitCamera.Instance.cameraType == CameraType.AR ?
                                  HoloKitCamera.Instance.cameraCenter.transform :
                                  HoloKitCamera.Instance.holoKitOffset.transform;
           
            Ray ray = new Ray(
                eyeCenter.position, 
                eyeCenter.forward
            );

            RaycastHit hitInfo;
            bool hit = Physics.Raycast(ray, out hitInfo, RaycastDistance, RaycastMask);
            /*if (hit) 
            {
                //Debug.Log("Hit: " + hitInfo.collider.gameObject.name);
                GazeCursor.position = hitInfo.point + hitInfo.normal.normalized * hitInfo.distance * 0.1f;
                GazeCursor.localScale = initialScale / RaycastDistance * hitInfo.distance;
                GazeCursor.forward = -hitInfo.normal;
            }
            else
            {
                GazeCursor.position = eyeCenter.position + eyeCenter.forward.normalized * RaycastDistance;
                GazeCursor.forward = eyeCenter.forward;
                GazeCursor.localScale = initialScale;
            }*/
            GazeCursor.position = eyeCenter.position + eyeCenter.forward.normalized * RaycastDistance;
            //GazeCursor.transform.LookAt(HoloKitCamera.Instance.cameraCenter.transform.position);
            GazeCursor.forward = eyeCenter.forward;

            // Invoke gaze events on gaze targets
            HoloKitGazeTarget newTarget = hit ? hitInfo.collider.GetComponent<HoloKitGazeTarget>() : null;
            if (newTarget != currentTarget)  {
                if (currentTarget != null && currentTarget.GazeExit != null)
                {
                    currentTarget.GazeExit.Invoke();
                    gazing = false;
                    timeGazing = 0;
                }

                if (newTarget != null && newTarget.GazeEnter != null)
                {
                    newTarget.GazeEnter.Invoke();

                    timeGazing = 0;
                    gazing = true;
                }

                currentTarget = newTarget;
                //Debug.Log("New target: " + currentTarget.name);
            }

            if (gazing && currentTarget.enabled && currentTarget.GetComponent<GazeButton>().enabled)
            {
                //Debug.Log("Gazing: " + timeGazing);
                if (currentTarget.GetComponent<GazeButton>().gazeTime > 0)
                {
                    overridenTimeToSelectl = currentTarget.GetComponent<GazeButton>().gazeTime;
                } else
                {
                    overridenTimeToSelectl = timeToSelect;
                }

                timeGazing += Time.deltaTime;
                gazingBar.fillAmount = timeGazing / overridenTimeToSelectl;
                if (timeGazing > overridenTimeToSelectl)
                {
                    currentTarget.GetComponent<GazeButton>().OnAction.Invoke();
                    gazing = false;
                }
            } else
            {
                if (gazingBar)
                {
                    gazingBar.fillAmount = 0;
                }
            }
        }

        void OnDestroy()
        {
            instance = null;
        }
    }
}