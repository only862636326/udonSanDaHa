
using HopeSDH;
using HopeTools;
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

        private bool _is_init = false;

        private int config_zhuang_icon;
        private int config_zhuang_player;

        private int _active_player;

        [SerializeField] public int[] out_card_id_list;
        [SerializeField] public int out_card_num;
        [SerializeField] private int[] select_card_id_list;
        [SerializeField] private int select_card_num;

        [SerializeField]  private SDH_GameManager sDH_GameManager;
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
            _init_chupai_1412();

            out_card_id_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            select_card_id_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            this.select_card_num = 0;
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
            hugf.udonEvn.RegisterListener(nameof(this.SelecCardCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.UnselecCardCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.StartChuPaiCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.FaPaiCall), this);
        }


        public void HufgIocGet()
        {
            //var p = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
            sDH_GameManager = (SDH_GameManager)hugf.udonIoc.GetServiceUdon(SDH_GameManager.SDH_CONFIG_Singleton_String);
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


        public void ResetOutCardState()
        {
            this.out_card_num = 0;
            this.select_card_num = 0;
            this._active_player = -1;
        }

        public void ToggleEvn_OutBut(int x)
        {

            FirstOutFun();
            AfterOutFun();
        }

        public void ToggleEvn_TipsBut(int x)
        {
            hugf.Log($"ToggleEvn_TipsBut: {x}");
            // test 
            StartChuPaiCall();
        }

        public void ToggleEvn_MaiDi(int x)
        {
            hugf.Log($"ToggleEvn_MaiDi: {x}");
        }
        public void SelecCardCall()
        {
            if (select_card_num >= select_card_id_list.Length) return;
            var _id = (int)this.eventData;
            for (int i = 0; i < this.select_card_num; i++)
            {
                if (this.select_card_id_list[i] == _id)
                    return;
            }
            this.select_card_id_list[this.select_card_num++] = _id;
        }

        public void UnselecCardCall()
        {
            if (select_card_num <= 0) return;

            var _id = (int)this.eventData;

            var _has = false;
            for (int i = 0; i < this.select_card_num; i++)
            {
                if (this.select_card_id_list[i] == _id)
                {
                    _has = true;
                }
                if (_has)
                {
                    this.select_card_id_list[i] = this.select_card_id_list[this.select_card_num - 1];
                }
            }
            if(this.select_card_num >= this.select_card_id_list.Length)
            {
                hugf.udondebug.LogWarning($"UnselecCardCall: {_id}, select_card_num: {this.select_card_num}");
                return;
            }
            this.select_card_num--;
        }
        // start method

        public void ToggleEvn_OutBut_0() { ToggleEvn_OutBut(0); }
        public void ToggleEvn_TipsBut_0() { ToggleEvn_TipsBut(0); }
        public void ToggleEvn_MaiDi_0() { ToggleEvn_MaiDi(0); }
        // end method


        #region 出牌判定

        private int _current_player;
        private int _first_out_player;
        private int _zhu_icon;


        private int[] _p0_out_list;
        private int[] _p1_out_list;
        private int[] _p2_out_list;
        private int[] _p3_out_list;

        private int[] _p0_hand_list;
        private int[] _p1_hand_list;
        private int[] _p2_hand_list;
        private int[] _p3_hand_list;
        
        private int _pre_hand_card_num;
        private int[] _jipaiqi_list;

        private int[] fitrt_out_card_type_list;
        private int[] first_out_card_list;
        private int first_out_card_num;
        private bool _is_init_chupai_1412 = false;
        private void _init_chupai_1412()
        {
            if (_is_init_chupai_1412)
                return;
            _is_init_chupai_1412 = true;

            _p0_out_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            _p1_out_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            _p2_out_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            _p3_out_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];

            _p0_hand_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            _p1_hand_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            _p2_hand_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            _p3_hand_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];


            fitrt_out_card_type_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            first_out_card_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];

            for (int i = 0; i < SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX; i++)
            {
                _p0_hand_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p1_hand_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p2_hand_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p3_hand_list[i] = SDH_GameManager.CONST_CARD_NULL;

                _p0_out_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p1_out_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p2_out_list[i] = SDH_GameManager.CONST_CARD_NULL   ;
                _p3_out_list[i] = SDH_GameManager.CONST_CARD_NULL;  
            }
        }

        public void FaPaiCall()
        {
            var dat = (int[])this.eventData;
            if (dat == null || dat.Length == 0)
            {
                hugf.udondebug.LogWarning("SDH_DiPaiManager FaPaiCall data is null or empty!");
                return;
            }

            for (int i = 0; i < SDH_GameManager.CONST_PLAYER_GRAB_CARD_NUM; i++)
            {
                this._p0_hand_list[i] = dat[i * 4 + 0];
                this._p1_hand_list[i] = dat[i * 4 + 1];
                this._p2_hand_list[i] = dat[i * 4 + 2];
                this._p3_hand_list[i] = dat[i * 4 + 3];
            }
            _pre_hand_card_num = SDH_GameManager.CONST_PLAYER_GRAB_CARD_NUM;

            sDH_GameManager.SortListByIdxCard(this._p0_hand_list, _pre_hand_card_num);
            sDH_GameManager.SortListByIdxCard(this._p1_hand_list, _pre_hand_card_num);
            sDH_GameManager.SortListByIdxCard(this._p2_hand_list, _pre_hand_card_num);
            sDH_GameManager.SortListByIdxCard(this._p3_hand_list, _pre_hand_card_num);

            hugf.TriggerEventWith2Data(nameof(SDH_PlayerManager.SetHandCardP0Call), this._p0_hand_list, _pre_hand_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_PlayerManager.SetHandCardP1Call), this._p1_hand_list, _pre_hand_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_PlayerManager.SetHandCardP2Call), this._p2_hand_list, _pre_hand_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_PlayerManager.SetHandCardP3Call), this._p3_hand_list, _pre_hand_card_num);

            hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.EnCardTileClickCall), dat, _pre_hand_card_num * 4);
        }

        private bool CheckIsOutUser(int idx)
        {
            return idx == _current_player;
        }

        private int[] _sort_temp_list;


        private void FirstOutFun()
        {
            if (this._current_player != this._first_out_player)
            {
                return;
            }
            var b = CheckFirstOutEn();
            if (!b)
            {
                hugf.Log($"CheckOutEn failed, select_card_num: {this.select_card_num}");
                return;
            }

            for (int i = 0; i < this.select_card_num; i++)
            {
                this.out_card_id_list[i] = this.select_card_id_list[i];
                this.first_out_card_list[i] = this.select_card_id_list[i];
                this.fitrt_out_card_type_list[i] = sDH_GameManager.GetTypeById(this.select_card_id_list[i]);
            }

            this.out_card_num = this.select_card_num;
            this.first_out_card_num = this.select_card_num;
            
            sDH_GameManager.SortListByIdxCard(this.out_card_id_list, this.out_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_OutCartP.SetOutCardP0Call), this.out_card_id_list, this.out_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.DisCardTileClickCall), this.out_card_id_list, this.out_card_num);
        }


        private void AfterOutFun()
        {
            ;
        }

        private bool CheckFirstOutEn()
        {            
            if (select_card_num == 1)
            {
                return true;
            }

            if (select_card_num == 2)
            {
                var x = select_card_id_list[0] / 2;
                var y = select_card_id_list[1] / 2;
                return x == y;
            }

            // 单数， 暂时不支持甩牌, err
            if ((select_card_num & 0x01) > 0)
            {
                return false;
            }
            // 最高到五连拖
            if (select_card_num > 10)
            {
                return false;
            }
             
            var b = sDH_GameManager.CheckIsTuoLaJi(this.select_card_id_list, this.select_card_num);
            return b;
        }



        private bool CheckAfterOutEn()
        {
            if (this.select_card_num != this.first_out_card_num)
            {
                return false;
            }

            if(this.first_out_card_num == 1)
            {
                var typ = sDH_GameManager.GetTypeById(this.select_card_id_list[0]);
                
            }


            return false;
        }

        public void StartChuPaiCall()
        {
            this._current_player = sDH_GameManager.config_zhuang_player;
            this._zhu_icon = sDH_GameManager.config_zhu_icon;
            this.select_card_num = 0;
        }
        
        public void ChuPaiFirst()
        {
            ;
        }

        public void ChuPaiNext()
        {
            ;
        }

        #endregion   出牌判定
    }
}