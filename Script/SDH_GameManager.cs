
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
        public const int CONST_TOTAL_CARD_NUM = 92;
        public const int CONST_PLAYER_HAND_CARD_MAX = ((92 - 8) / 4) + 8;
        public const int CONST_PLAYER_GRAB_CARD_NUM = ((92 - 8) / 4);
        public const int CONST_DIPAI_CARD_NUM = 8;
        public const int CONST_CARD_NULL = -1;
        public const string CONST_SDH_HUGF_STRING = "SDH_hufg";

        void Start()
        {
            Init();
        }

        private bool _is_init = false;
        public void Init()
        {
            // 如果已经初始化，则直接返回
            if (this._is_init)
                return;
            this._is_init = true;
        }

        void Update()
        {
            ;
        }      
    }
}