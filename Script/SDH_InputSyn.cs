
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;



namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]

    public class SDH_InputSyn : UdonSharpBehaviour
    {
        public string[] evn_all;
        public const int evn_buff_len = 5;
        [UdonSynced] public string evn_one;
        public string[] evn_buff;

        void Start()
        {
            this.evn_buff = new string[evn_buff_len];
        }

        private void Update()
        {
            ;
        }

        [HideInInspector] public HopeTools.HopeUdonFramework hugf;
        #region syn

        void RequestSyn()
        {
#if !UNITY_EDITOR
            if(!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }
            RequestSerialization();
#else
            OnPreSerialization();
#endif
            ;
        }

        public override void OnPreSerialization()
        {

            //DebugSynData();
        }

        public override void OnDeserialization()
        {

            //DebugSynData();
        }

        public void DebugSynData()
        {

        }
        #endregion end syn
    }
}