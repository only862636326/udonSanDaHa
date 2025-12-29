
using HopeTools;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeSDH
{
    public class SDH_JiaoZhuang : UdonSharpBehaviour
    {
        private int PLAYER_NUM;
        #region init code
        private bool _is_init = false;


        public const int SYN_DATA_IDX_ACTIVE_PLAYER = 0;
        public const int SYN_DATA_IDX_CURRENT_SCORE = 1;
        public const int SYN_DATA_IDX_ZHUNG_PLAYER = 2;
        public const int SYN_LIST_LEN = 10;
        [UdonSynced] private int[] syn_data_list;
        private int _active_player;
        private int _current_score;
        private int _select_score;
        public int _zhung_player;

        private Text[] _text_tips;
        private GameObject[] _obj_jiao_zhuang_list;
        private GameObject[] _obj_bu_jiao_li_list;
        private Transform[] _tf_score_prt_list;
        private int[] _jiao_zhuang_idx_list;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here
            syn_data_list = new int[SYN_LIST_LEN];

            var _n = this.transform.childCount;
            PLAYER_NUM = this.transform.childCount;

            _text_tips = new Text[_n];
            _obj_jiao_zhuang_list = new GameObject[PLAYER_NUM];
            _obj_bu_jiao_li_list = new GameObject[PLAYER_NUM];
            _tf_score_prt_list = new Transform[PLAYER_NUM];
            
            for (int i = 0; i < PLAYER_NUM; i++)
            {
                var tf = this.transform.GetChild(i);

                foreach (Transform child in tf)
                {
                    var _low = child.name.ToLower();
                    if (_low.Contains("tips") && _low.Contains("text"))
                    {
                        _text_tips[i] = child.GetComponent<Text>();
                    }
                    else if (_low.Contains("jiao") && _low.Contains("zhuang") && _low.Contains("toggleevn"))
                    {
                        _obj_jiao_zhuang_list[i] = child.gameObject;
                    }
                    else if (_low.Contains("bu") && _low.Contains("jiao") && _low.Contains("toggleevn"))
                    {
                        _obj_bu_jiao_li_list[i] = child.gameObject;
                    }
                    else if(_low.Contains("score") && _low.Contains("prt"))
                    {
                        _tf_score_prt_list[i] = child;
                    }
                }
            }
            _jiao_zhuang_idx_list = new int[_n];          
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                StartJiaoCall();
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


        public void StartJiaoCall()
        {
            for (int i = 0; i < PLAYER_NUM; i++)
            {
                _jiao_zhuang_idx_list[i] = i;                
            }
            _active_player = 0;
            _current_score = 85;
            _zhung_player = -1;
            RequestSyn();
        }

        private void SetShow()
        {
            var n = _current_score / 5;      // 5 1
            var _l = _tf_score_prt_list[0].childCount; // 16
            var _ = _l - n;
            hugf.TriggerEventWithData(nameof(SDH_Tips.SetActivePlayerCall), this._active_player);

            if (_zhung_player > -1)
            {
                for (int i = 0; i < PLAYER_NUM; i++)
                {
                    _obj_jiao_zhuang_list[i].SetActive(false);
                    _obj_bu_jiao_li_list[i].SetActive(false);
                    _tf_score_prt_list[i].gameObject.SetActive(false);
                    this._text_tips[i].text = $"庄{_zhung_player}:{_current_score}";
                }
                return;
            }

            else
            {
                for (int i = 0; i < _l; i++)
                {
                    foreach (Transform tf in _tf_score_prt_list)
                    {
                        tf.GetChild(i).gameObject.SetActive(i > _);
                    }
                }

                for (int i = 0; i < PLAYER_NUM; i++)
                {
                    _obj_jiao_zhuang_list[i].SetActive(i == _active_player);
                    _obj_bu_jiao_li_list[i].SetActive(i == _active_player);
                    _tf_score_prt_list[i].gameObject.SetActive(true);
                    this._text_tips[i].text = _select_score.ToString();
                }
            }
        }

        #region syn

        void RequestSyn()
        {
#if !UNITY_EDITOR
            if (!Networking.IsOwner(this.gameObject))
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

            EnCodeGameInfo();
            SetShow();
        }

        public override void OnDeserialization()
        {
            DecodeGameInfo();
            SetShow();
        }

        private void EnCodeGameInfo()
        {
            for (int i = 0; i < PLAYER_NUM; i++)
            {
                syn_data_list[i + 5] = this._jiao_zhuang_idx_list[i];
            }
            syn_data_list[SYN_DATA_IDX_ACTIVE_PLAYER] = _active_player;
            syn_data_list[SYN_DATA_IDX_CURRENT_SCORE] = _current_score;
            syn_data_list[SYN_DATA_IDX_ZHUNG_PLAYER] = _zhung_player;
        }

        public void DecodeGameInfo()
        {
            for (int i = 0; i < PLAYER_NUM; i++)
            {
                this._jiao_zhuang_idx_list[i] = syn_data_list[i + 5];
            }
            _active_player = syn_data_list[SYN_DATA_IDX_ACTIVE_PLAYER];
            _current_score = syn_data_list[SYN_DATA_IDX_CURRENT_SCORE];
            _zhung_player = syn_data_list[SYN_DATA_IDX_ZHUNG_PLAYER];
        }

        public void DebugSynData()
        {

        }
        #endregion end syn


        public void ToggleEvn_Score(int score_idx, int idx)
        {
            //Debug.Log($"ToggleEvn_Score called with score: {score}, idx: {idx}");
            // implement your logic here
            var srore = 80 - score_idx * 5;
            _select_score = srore;
            if (_current_score != 0)
            {
                _text_tips[idx].text = $"{_current_score}>>>{_select_score}";
            }
            else
            {
                _text_tips[idx].text = $"{_select_score}";
            }
        }
        public void ToggleEvn_JiaoZhuang(int idx)
        {
            //Debug.Log($"ToggleEvn_JiaoZhuang called with idx: {idx}");
            if (idx != _active_player)
            {
                return;
            }

            if (this._jiao_zhuang_idx_list[idx] >= -1)
            {
                this._current_score = this._select_score;
                if (this._current_score == 5)
                {
                    _zhung_player = this._active_player;
                    RequestSyn();
                    return;
                }
            }

            do
            {
                this._active_player++;
                this._active_player %= this.PLAYER_NUM;
            } while (this._jiao_zhuang_idx_list[this._active_player] < 0);
            RequestSyn();
        }



        public void ToggleEvn_BuJiao(int idx)
        {
            //Debug.Log($"ToggleEvn_BuJiao called with idx: {idx}");
            if (idx != this._active_player)
            {
                return;
            }
            this._jiao_zhuang_idx_list[this._active_player] = -1;

            do
            {
                this._active_player++;
                this._active_player %= this.PLAYER_NUM;
            } while (this._jiao_zhuang_idx_list[this._active_player] < 0); // next 1 player

            var _his = this._active_player;
            do
            {
                this._active_player++;
                this._active_player %= this.PLAYER_NUM;
            } while (this._jiao_zhuang_idx_list[this._active_player] < 0); // next 2 player


            if (_his == this._active_player) // next 1 == next2 , only one player
            {
                _zhung_player = this._active_player;
            }
            else
            {
                this._active_player = _his;
            }
            RequestSyn();
        }

        #region start method
        // start method
        public void ToggleEvn_Score0_0() { ToggleEvn_Score(0, 0); }
		public void ToggleEvn_Score1_0() { ToggleEvn_Score(1, 0); }
		public void ToggleEvn_Score2_0() { ToggleEvn_Score(2, 0); }
		public void ToggleEvn_Score3_0() { ToggleEvn_Score(3, 0); }
		public void ToggleEvn_Score4_0() { ToggleEvn_Score(4, 0); }
		public void ToggleEvn_Score5_0() { ToggleEvn_Score(5, 0); }
		public void ToggleEvn_Score6_0() { ToggleEvn_Score(6, 0); }
		public void ToggleEvn_Score7_0() { ToggleEvn_Score(7, 0); }
		public void ToggleEvn_Score8_0() { ToggleEvn_Score(8, 0); }
		public void ToggleEvn_Score9_0() { ToggleEvn_Score(9, 0); }
		public void ToggleEvn_Score10_0() { ToggleEvn_Score(10, 0); }
		public void ToggleEvn_Score11_0() { ToggleEvn_Score(11, 0); }
		public void ToggleEvn_Score12_0() { ToggleEvn_Score(12, 0); }
		public void ToggleEvn_Score13_0() { ToggleEvn_Score(13, 0); }
		public void ToggleEvn_Score14_0() { ToggleEvn_Score(14, 0); }
		public void ToggleEvn_Score15_0() { ToggleEvn_Score(15, 0); }
		public void ToggleEvn_JiaoZhuang_0() { ToggleEvn_JiaoZhuang(0); }
		public void ToggleEvn_BuJiao_0() { ToggleEvn_BuJiao(0); }
		public void ToggleEvn_Score0_1() { ToggleEvn_Score(0, 1); }
		public void ToggleEvn_Score1_1() { ToggleEvn_Score(1, 1); }
		public void ToggleEvn_Score2_1() { ToggleEvn_Score(2, 1); }
		public void ToggleEvn_Score3_1() { ToggleEvn_Score(3, 1); }
		public void ToggleEvn_Score4_1() { ToggleEvn_Score(4, 1); }
		public void ToggleEvn_Score5_1() { ToggleEvn_Score(5, 1); }
		public void ToggleEvn_Score6_1() { ToggleEvn_Score(6, 1); }
		public void ToggleEvn_Score7_1() { ToggleEvn_Score(7, 1); }
		public void ToggleEvn_Score8_1() { ToggleEvn_Score(8, 1); }
		public void ToggleEvn_Score9_1() { ToggleEvn_Score(9, 1); }
		public void ToggleEvn_Score10_1() { ToggleEvn_Score(10, 1); }
		public void ToggleEvn_Score11_1() { ToggleEvn_Score(11, 1); }
		public void ToggleEvn_Score12_1() { ToggleEvn_Score(12, 1); }
		public void ToggleEvn_Score13_1() { ToggleEvn_Score(13, 1); }
		public void ToggleEvn_Score14_1() { ToggleEvn_Score(14, 1); }
		public void ToggleEvn_Score15_1() { ToggleEvn_Score(15, 1); }
		public void ToggleEvn_JiaoZhuang_1() { ToggleEvn_JiaoZhuang(1); }
		public void ToggleEvn_BuJiao_1() { ToggleEvn_BuJiao(1); }
		public void ToggleEvn_Score0_2() { ToggleEvn_Score(0, 2); }
		public void ToggleEvn_Score1_2() { ToggleEvn_Score(1, 2); }
		public void ToggleEvn_Score2_2() { ToggleEvn_Score(2, 2); }
		public void ToggleEvn_Score3_2() { ToggleEvn_Score(3, 2); }
		public void ToggleEvn_Score4_2() { ToggleEvn_Score(4, 2); }
		public void ToggleEvn_Score5_2() { ToggleEvn_Score(5, 2); }
		public void ToggleEvn_Score6_2() { ToggleEvn_Score(6, 2); }
		public void ToggleEvn_Score7_2() { ToggleEvn_Score(7, 2); }
		public void ToggleEvn_Score8_2() { ToggleEvn_Score(8, 2); }
		public void ToggleEvn_Score9_2() { ToggleEvn_Score(9, 2); }
		public void ToggleEvn_Score10_2() { ToggleEvn_Score(10, 2); }
		public void ToggleEvn_Score11_2() { ToggleEvn_Score(11, 2); }
		public void ToggleEvn_Score12_2() { ToggleEvn_Score(12, 2); }
		public void ToggleEvn_Score13_2() { ToggleEvn_Score(13, 2); }
		public void ToggleEvn_Score14_2() { ToggleEvn_Score(14, 2); }
		public void ToggleEvn_Score15_2() { ToggleEvn_Score(15, 2); }
		public void ToggleEvn_JiaoZhuang_2() { ToggleEvn_JiaoZhuang(2); }
		public void ToggleEvn_BuJiao_2() { ToggleEvn_BuJiao(2); }
		public void ToggleEvn_Score0_3() { ToggleEvn_Score(0, 3); }
		public void ToggleEvn_Score1_3() { ToggleEvn_Score(1, 3); }
		public void ToggleEvn_Score2_3() { ToggleEvn_Score(2, 3); }
		public void ToggleEvn_Score3_3() { ToggleEvn_Score(3, 3); }
		public void ToggleEvn_Score4_3() { ToggleEvn_Score(4, 3); }
		public void ToggleEvn_Score5_3() { ToggleEvn_Score(5, 3); }
		public void ToggleEvn_Score6_3() { ToggleEvn_Score(6, 3); }
		public void ToggleEvn_Score7_3() { ToggleEvn_Score(7, 3); }
		public void ToggleEvn_Score8_3() { ToggleEvn_Score(8, 3); }
		public void ToggleEvn_Score9_3() { ToggleEvn_Score(9, 3); }
		public void ToggleEvn_Score10_3() { ToggleEvn_Score(10, 3); }
		public void ToggleEvn_Score11_3() { ToggleEvn_Score(11, 3); }
		public void ToggleEvn_Score12_3() { ToggleEvn_Score(12, 3); }
		public void ToggleEvn_Score13_3() { ToggleEvn_Score(13, 3); }
		public void ToggleEvn_Score14_3() { ToggleEvn_Score(14, 3); }
		public void ToggleEvn_Score15_3() { ToggleEvn_Score(15, 3); }
		public void ToggleEvn_JiaoZhuang_3() { ToggleEvn_JiaoZhuang(3); }
		public void ToggleEvn_BuJiao_3() { ToggleEvn_BuJiao(3); }
        // end method
        #endregion end method
    }
}
