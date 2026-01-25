
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

        public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1;
        public object eventData2;

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
            ;
        }
        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.ToggleEvn_BuJiaoCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.ToggleEvn_JiaoZhuangCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.ToggleEvn_ScoreCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.StartJiaoCall), this);
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

        private bool _first_jiao_zhuang = false;
        public void StartJiaoCall()
        {
            _first_jiao_zhuang = true;

            for (int i = 0; i < PLAYER_NUM; i++)
            {
                _jiao_zhuang_idx_list[i] = i;
            }
            _active_player = 0;
            _current_score = 80;
            _select_score = 0;
            _zhung_player = -1;
            StartJiaoShow();
        }

        private void StartJiaoShow()
        {
            hugf.TriggerEventWithData(nameof(SDH_Tips.SetActivePlayerCall), this._active_player);

            foreach (Transform tf in this.transform)
            {
                tf.gameObject.SetActive(true);
            }

            var _scro_num = _tf_score_prt_list[0].childCount;            
            foreach (Transform tf in _tf_score_prt_list)
            {
                for (int i = 0; i < _scro_num; i++)
                {
                    tf.GetChild(i).gameObject.SetActive(true);
                }
            }

            for (int i = 0; i < PLAYER_NUM; i++)
            {
                _obj_jiao_zhuang_list[i].SetActive(i == _active_player);
                _obj_bu_jiao_li_list[i].SetActive(false);
                _tf_score_prt_list[i].gameObject.SetActive(true);
                this._text_tips[i].text = _current_score.ToString();
            }
        }

        public void SetZhuangShow()
        {
            if (_zhung_player < 0)
            {
                hugf.udondebug.LogWarning("SetZhuangShow is not a player");
                return;
            }
            foreach (Transform tf in this.transform)
            {
                tf.gameObject.SetActive(false);
            }

            hugf.TriggerEventWithData(nameof(SDH_Tips.SetActivePlayerCall), this._active_player);
            hugf.TriggerEventWithData(nameof(SDH_Tips.SetZhuangPlayerCall), this._active_player);
            hugf.TriggerEventWithData(nameof(SDH_Tips.SetJiaoFenShowCall), this._current_score);
            hugf.TriggerEventWithData(nameof(SDH_OutCartFsm.JiaoZhuangFinishCall), this._zhung_player);
        }

        public void NextPlayerShow()
        {
            var n = _current_score / 5;      // 5 1
            var _score_num = _tf_score_prt_list[0].childCount; // 16
            var _ = _score_num - n;
            hugf.TriggerEventWithData(nameof(SDH_Tips.SetActivePlayerCall), this._active_player);

            foreach (Transform tf in _tf_score_prt_list)
            {
                for (int i = 0; i < _score_num; i++)
                    tf.GetChild(i).gameObject.SetActive(i > _);
            }
            this._select_score = 0;
            for (int i = 0; i < PLAYER_NUM; i++)
            {
                _obj_jiao_zhuang_list[i].SetActive(i == _active_player);
                _obj_bu_jiao_li_list[i].SetActive(i == _active_player);
                _tf_score_prt_list[i].gameObject.SetActive(true);
                this._text_tips[i].text = _current_score.ToString();
            }
        }

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
            if(this._select_score == 0)
            {
                return;
            }

            if (this._jiao_zhuang_idx_list[idx] >= -1)
            {
                this._current_score = this._select_score;
                if (this._current_score == 5)
                {
                    _zhung_player = this._active_player;
                    SetZhuangShow();
                    return;
                }
            }

            do
            {
                this._active_player++;
                this._active_player %= this.PLAYER_NUM;
            } while (this._jiao_zhuang_idx_list[this._active_player] < 0);
            NextPlayerShow();
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
                SetZhuangShow();
            }
            else
            {
                this._active_player = _his;
                NextPlayerShow();
            }
        }

        public void ToggleEvn_ScoreCall()
        {
            int idx = (int) this.eventData;
            int score = (int)this.eventData2;
            ToggleEvn_Score(score, idx);
        }

        public void ToggleEvn_JiaoZhuangCall()
        {
            int idx = (int)this.eventData;
            ToggleEvn_JiaoZhuang(idx);
        }

        public void ToggleEvn_BuJiaoCall()
        {
            int idx = (int)this.eventData;
            ToggleEvn_BuJiao(idx); ;
        }
    }
}
