
using HopeSDH;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;



namespace HopeSDH
{ 
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SDH_OutCartFsm : UdonSharpBehaviour
    {
        #region init code

        public const int CONST_MAX_OUT_CARD = 23;
        private bool _is_init = false;

        private int config_zhuang_icon;
        private int config_zhuang_player;

        private int _active_player;

        public int[] out_card_list;

        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here
            var n = 0;
            for (int i = 0; i < n; i++)
            {
                var tf = this.transform.GetChild(i);

                foreach (Transform child in tf)
                {
                    var _low = child.name.ToLower();
                    if (_low.Contains("tips") && _low.Contains("text"))
                    {
                    }
                }
            }
            out_card_list = new int[CONST_MAX_OUT_CARD];
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

        [UdonSynced] int[] syn_list;

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