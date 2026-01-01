
using HopeSDH;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SDH_OutCartP : UdonSharpBehaviour
    {
        #region init code
        private bool _is_init = false;

        private Transform[] _out_card_prt_list;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here
            var n = this.transform.childCount;
            for (int i = 0; i < n; i++)
            {
                var tf = this.transform.GetChild(i);

                foreach (Transform child in tf)
                {
                    this._out_card_prt_list[i] = child;
                }
            }
        }

        private HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public void HugfInit()
        {
            if (hugf == null)
            {
                hugf = GameObject.Find(SDH_GameManager.CONST_SDH_HUGF_STRING).GetComponent<HopeTools.HopeUdonFramework>();
                if (hugf == null)
                {
                    Debug.LogError("HugfInit failed, hugf is null!");
                    return;
                }

                hugf.Init();
                return;
            }
        }

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
        }


        public void HufgIocGet()
        {
            //var p = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }

        //public void DemeFunCall()
        //{
        //    this.eventData = data;
        //}
        #endregion end init code

        #region syn


        private int _out_card_player;
        public void SetOutCardPlayerCall()
        {
            this._out_card_player = (int)this.eventData;
        }


        public void SetOutCardP(int idx)
        {

        }

        public void SetOutCardP0Call() { SetOutCardP(0); }
        public void SetOutCardP1Call() { SetOutCardP(1); }
        public void SetOutCardP2Call() { SetOutCardP(2); }
        public void SetOutCardP3Call() { SetOutCardP(3); }


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


        // start method

        // end method
    }
}