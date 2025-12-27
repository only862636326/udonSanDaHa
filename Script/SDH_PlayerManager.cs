
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class SDH_PlayerManager : UdonSharpBehaviour
    {
        void Start()
        {
            Init();
        }

        private bool _is_init = false;
        public void Init()
        {
        }
    }
}