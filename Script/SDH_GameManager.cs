
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SDH_GameManager : UdonSharpBehaviour
    {

        public const int CONST_PLAYER_NUM = 4;
        public const int CONST_PLAYER_NONE = -1;
        public const int CONST_TOTAL_CARD_NUM = 92;
        public const int CONST_PLAYER_HAND_CARD_MAX = ((92 - 8) / 4) + 8;
        public const int CONST_PLAYER_GRAB_CARD_NUM = ((92 - 8) / 4);
        public const int CONST_DIPAI_CARD_NUM = 8;
        public const int CONST_CARD_NULL = -1;
        public const string CONST_SDH_HUGF_STRING = "SDH_hufg";

        public const int CONST_ICON_MEI = 0;
        public const int CONST_ICON_FANG = 1;
        public const int CONST_ICON_HONG = 2;
        public const int CONST_ICON_HEI = 3;
        public const int CONST_ICON_JOKER = 4;

        public int config_zhuang_player = -1;
        public int config_zhuang_icon = -1;
        public int config_zhunag_score = 0;
        public int[] config_player_vrcid_list;

        public int info_game_sta = 0;
        public int info_acitve_layer = 0;
        public int[] info_out_card;

        #region init code
        private bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            config_player_vrcid_list = new int[CONST_PLAYER_NUM];
            for (int i = 0; i < CONST_PLAYER_NUM; i++)
            {
                config_player_vrcid_list[i] = CONST_PLAYER_NONE;
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
            hugf.udonEvn.RegisterListener(nameof(this.SetPlayerVrcIdCall), this);
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


        public void SetPlayerVrcIdCall()
        {
            var _id = (int[])this.eventData;
            hugf.Log("SetPlayerVrcIdCall");
            for (int i = 0; i < CONST_PLAYER_NUM; i++)
            {
                config_player_vrcid_list[i] = _id[i];
            }
        }

        public void StartGameCall()
        {
            ;
        }


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


        // start method

        // end method
    }
}