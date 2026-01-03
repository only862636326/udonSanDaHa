
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SDH_GameManager : UdonSharpBehaviour
    {

        public const int CONST_SHOW_CARD_NUM = 108;

        public const int CONST_PLAYER_NUM = 4;
        public const int CONST_PLAYER_NONE = -1;
        public const int CONST_SDH_TOTAL_CARD_NUM = 92;
        public const int CONST_PLAYER_HAND_CARD_MAX = ((92 - 8) / 4) + 8;
        public const int CONST_PLAYER_GRAB_CARD_NUM = ((92 - 8) / 4);
        public const int CONST_DIPAI_CARD_NUM = 8;
        public const int CONST_CARD_NULL = -1;
        public const string CONST_SDH_HUGF_STRING = "SDH_hufg";
        public const string SDH_CONFIG_Singleton_String = "SDH_CONFIG_Singleton_String";
        public const int CONST_MAX_OUT_CARD = 23;

        public const int CONST_ICON_MEI = 0;
        public const int CONST_ICON_FANG = 1;
        public const int CONST_ICON_HONG = 2;
        public const int CONST_ICON_HEI = 3;
        public const int CONST_ICON_JOKER = 4;

        public int config_zhuang_player = -1;
        public int config_zhuang_score = 0;
        public int config_zhu_icon = -1;
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

            this.config_zhu_icon = CONST_ICON_FANG;
            ConfigSortIdList(this.config_zhu_icon);
            ConfigTypeIdList(this.config_zhu_icon);
            this.config_zhuang_player = 0;
            this.config_zhuang_score = 80;
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
            hugf.udonIoc.RegisterSingleton(nameof(SDH_CONFIG_Singleton_String), this, this);
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

        #region sort list
        public int[] sort_id_list = null;
        public int[] id_in_sorted_list = null;
        public int[] split_id_list = { 4, 20, 20 + 18, 20 + 36, 20 + 54, 20 + 72 };
        private bool _is_sorted_init = false;
        public void ConfigSortIdList(int _icon)
        {
            if (_is_sorted_init == false)
            {
                _is_sorted_init = true;
                this.sort_id_list = new int[SDH_GameManager.CONST_SHOW_CARD_NUM];
                this.id_in_sorted_list = new int[SDH_GameManager.CONST_SHOW_CARD_NUM];
            }

            var _all_num = sort_id_list.Length;

            var _sort_idx = 0;
            sort_id_list[_sort_idx++] = _all_num - 1;
            sort_id_list[_sort_idx++] = _all_num - 2;
            sort_id_list[_sort_idx++] = _all_num - 3;
            sort_id_list[_sort_idx++] = _all_num - 4;

            int _num;
            _num = 7;
            if (_icon >= 0 && _icon < 4)
            {
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 2);
            }

            for (var _t = 0; _t < 4; _t++)
            {
                if (_t == _icon)
                    continue;
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 2);
            }

            _num = 2;
            if (_icon >= 0 && _icon < 4)
            {
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 2);
            }

            for (var _t = 0; _t < 4; _t++)
            {
                if (_t == _icon)
                    continue;
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 2);
            }

            // 
            if (_icon >= 0 && _icon < 4)
            {
                _num = 1;
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 2);

                for (int i = 13; i >= 3; i--)
                {
                    if (i == 7)
                        continue;
                    _num = i;
                    sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 1);
                    sort_id_list[_sort_idx++] = _icon * 26 + (_num * 2 - 2);
                }
            }

            for (var _t = 0; _t < 4; _t++)
            {
                if (_t == _icon)
                    continue;

                _num = 1;

                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 1);
                sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 2);

                for (int i = 13; i >= 3; i--)
                {
                    if (i == 7)
                        continue;
                    _num = i;
                    sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 1);
                    sort_id_list[_sort_idx++] = _t * 26 + (_num * 2 - 2);
                }
            }

            for (int i = 0; i < _all_num; i++)
            {
                // 记录每个牌的排序索引
                id_in_sorted_list[sort_id_list[i]] = i;
            }
        }

        #endregion end sortd list
        // zhu;
        public const int CONST_TYPE_Zheng5 = 100;
        public const int CONST_TYPE_Zheng6 = CONST_TYPE_Zheng5 + 1;
        public const int CONST_TYPE_Zheng8 = CONST_TYPE_Zheng6 + 1;
        public const int CONST_TYPE_Zheng9 = CONST_TYPE_Zheng8 + 1;
        public const int CONST_TYPE_Zheng10 = CONST_TYPE_Zheng9 + 1;
        public const int CONST_TYPE_ZhengJ = CONST_TYPE_Zheng10 + 1;
        public const int CONST_TYPE_ZhengQ = CONST_TYPE_ZhengJ + 1;
        public const int CONST_TYPE_ZhengK = CONST_TYPE_ZhengQ + 1;
        public const int CONST_TYPE_ZhengA = CONST_TYPE_ZhengK + 1;
        public const int CONST_TYPE_Fu2 = CONST_TYPE_ZhengA + 1;
        public const int CONST_TYPE_Zheng2 = CONST_TYPE_Fu2 + 1;
        public const int CONST_TYPE_Fu7 = CONST_TYPE_Zheng2 + 1;
        public const int CONST_TYPE_Zheng7 = CONST_TYPE_Fu7 + 1;
        public const int CONST_TYPE_SmallJoker = CONST_TYPE_Zheng7 + 1;
        public const int CONST_TYPE_BigJoker = CONST_TYPE_SmallJoker + 1;

        // fu
        public const int CONST_TYPE_Fu5 = 0;
        public const int CONST_TYPE_Fu6 = CONST_TYPE_Fu5 + 1;
        public const int CONST_TYPE_Fu8 = CONST_TYPE_Fu6 + 1;
        public const int CONST_TYPE_Fu9 = CONST_TYPE_Fu8 + 1;
        public const int CONST_TYPE_Fu10 = CONST_TYPE_Fu9 + 1;
        public const int CONST_TYPE_FuJ = CONST_TYPE_Fu10 + 1;
        public const int CONST_TYPE_FuQ = CONST_TYPE_FuJ + 1;
        public const int CONST_TYPE_FuK = CONST_TYPE_FuQ + 1;
        public const int CONST_TYPE_FuA = CONST_TYPE_FuK + 1;

        public const int CONST_TYPE_UNKNOWN = 999;
        public const int CONST_TYPE_Zheng3 = CONST_TYPE_UNKNOWN;
        public const int CONST_TYPE_Zheng4 = CONST_TYPE_UNKNOWN;
        public const int CONST_TYPE_Fu3 = CONST_TYPE_UNKNOWN;
        public const int CONST_TYPE_Fu4 = CONST_TYPE_UNKNOWN;

        public int[] config_type_id_list;
        private bool _is_config_typ_init = false;
        public void ConfigTypeIdList(int zhu)
        {
            if (_is_config_typ_init == false)
            {
                _is_config_typ_init = true;
                config_type_id_list = new int[CONST_SHOW_CARD_NUM];
            }

            var card_id = 0;
            for (int icon = 0; icon < 4; icon++)
            {
                for (int i = 0; i < 13; i++)
                {
                    card_id += 2;
                    // 0 -> A, 1->2,
                    if (config_zhu_icon == icon)
                    {
                        switch (i)
                        {
                            case 0:
                                config_type_id_list[card_id - 1] = CONST_TYPE_ZhengA;
                                config_type_id_list[card_id - 2] = CONST_TYPE_ZhengA;
                                break;
                            case 1:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng2;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng2;
                                break;
                            case 2:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng3;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng3;
                                break;
                            case 3:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng4;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng4;
                                break;
                            case 4:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng5;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng5;
                                break;
                            case 5:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng6;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng6;
                                break;
                            case 6:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng7;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng7;
                                break;
                            case 7:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng8;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng8;
                                break;
                            case 8:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng9;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng9;
                                break;
                            case 9:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng10;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng10;
                                break;
                            case 10:
                                config_type_id_list[card_id - 1] = CONST_TYPE_ZhengJ;
                                config_type_id_list[card_id - 2] = CONST_TYPE_ZhengJ;
                                break;
                            case 11:
                                config_type_id_list[card_id - 1] = CONST_TYPE_ZhengQ;
                                config_type_id_list[card_id - 2] = CONST_TYPE_ZhengQ;
                                break;
                            case 12:
                                config_type_id_list[card_id - 1] = CONST_TYPE_ZhengK;
                                config_type_id_list[card_id - 2] = CONST_TYPE_ZhengK;
                                break;
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                config_type_id_list[card_id - 1] = CONST_TYPE_FuA;
                                config_type_id_list[card_id - 2] = CONST_TYPE_FuA;
                                break;
                            case 1:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu2;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu2;
                                if (zhu == CONST_ICON_JOKER)
                                {
                                    config_type_id_list[card_id - 1] = CONST_TYPE_Zheng2;
                                    config_type_id_list[card_id - 2] = CONST_TYPE_Zheng2;
                                }
                                break;
                            case 2:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu3;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu3;
                                break;
                            case 3:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu4;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu4;
                                break;
                            case 4:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu5;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu5;
                                break;
                            case 5:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu6;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu6;
                                break;
                            case 6:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu7;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu7;
                                if (zhu == CONST_ICON_JOKER)
                                {
                                    config_type_id_list[card_id - 1] = CONST_TYPE_Zheng7;
                                    config_type_id_list[card_id - 2] = CONST_TYPE_Zheng7;
                                }
                                break;
                            case 7:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu8;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu8;
                                break;
                            case 8:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu9;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu9;
                                break;
                            case 9:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu10;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu10;
                                break;
                            case 10:
                                config_type_id_list[card_id - 1] = CONST_TYPE_FuJ;
                                config_type_id_list[card_id - 2] = CONST_TYPE_FuJ;
                                break;
                            case 11:
                                config_type_id_list[card_id - 1] = CONST_TYPE_FuQ;
                                config_type_id_list[card_id - 2] = CONST_TYPE_FuQ;
                                break;
                            case 12:
                                config_type_id_list[card_id - 1] = CONST_TYPE_FuK;
                                config_type_id_list[card_id - 2] = CONST_TYPE_FuK;
                                break;
                        }
                    }
                }
            }
            config_type_id_list[card_id++] = CONST_TYPE_SmallJoker;
            config_type_id_list[card_id++] = CONST_TYPE_SmallJoker;
            config_type_id_list[card_id++] = CONST_TYPE_BigJoker;
            config_type_id_list[card_id++] = CONST_TYPE_BigJoker;
        }

        public int GetTypeById(int id)
        {
            return config_type_id_list[id];
        }

        [SerializeField] private int[] _sort_temp_list = new int[CONST_SHOW_CARD_NUM];
        public bool CheckIsTuoLaJi(int[] id_list, int num)
        {
            if (num <= 0)
            {
                return false;
            }
            // 单数, 
            if ((num & 0x01) > 0)
            {
                return false;
            }

            // 双数， 检查是否是五连拖
            for (int i = 0; i < num; i++)
            {
                _sort_temp_list[i] = GetTypeById(id_list[i]);
            }

            // 手动排序， 选择排序
            for (int i = 0; i < num; i++)
            {
                for (int j = i + 1; j < num; j++)
                {
                    if (_sort_temp_list[i] > _sort_temp_list[j])
                    {
                        int t = _sort_temp_list[i];
                        _sort_temp_list[i] = _sort_temp_list[j];
                        _sort_temp_list[j] = t;
                    }
                }
            }
            _sort_temp_list[num] = _sort_temp_list[num - 1] + 1;

            // 相等，[i] = [i + 1], [i + 1] + 1 = [i + 2]
            for (int i = 0; i < num; i += 2)
            {
                if (_sort_temp_list[i] != _sort_temp_list[i + 1])
                {
                    return false;
                }
                if (_sort_temp_list[i + 1] + 1 != _sort_temp_list[i + 2])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 排序大小
        /// </summary>
        /// <param name="_id_list"></param>
        /// <param name="num"></param>
        public void SortListByIdxCard(int[] _id_list, int num)
        {
            if (_sort_temp_list == null)
            {
                _sort_temp_list = new int[SDH_GameManager.CONST_SHOW_CARD_NUM];
            }

            for (int i = 0; i < _sort_temp_list.Length; i++)
            {
                _sort_temp_list[i] = -1;
            }

            var max_id = 0;
            for (int i = 0; i < num; i++)
            {
                int card_id = _id_list[i];
                if (card_id < 0)
                    continue;
                _sort_temp_list[card_id] = i;
            }

            int _n = 0;
            for (int i = 0; i < sort_id_list.Length; i++)
            {
                int card_id = this.sort_id_list[i];
                if (card_id >= 0 && _sort_temp_list[card_id] >= 0)
                {
                    _id_list[_n++] = card_id;
                }
            }

            if (_n != num)
            {
                hugf.udondebug.LogWarning($"SortCard failed, _n != this._hand_card_num, _n = {_n}, this._hand_card_num = {num}");
                return;
            }
        }
    }
}