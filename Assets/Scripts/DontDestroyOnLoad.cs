using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holobalance
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private static DontDestroyOnLoad Instance = null;
        // Use this for initialization
        void Awake()
        {
            //if(Instance == null)
            {
                //Instance = this;
                DontDestroyOnLoad(gameObject);
                //return;
            }
            //Destroy(this.gameObject);
        }
    }
}

