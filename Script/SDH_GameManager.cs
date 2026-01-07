
using System;
using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.HID;
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

        public const int CONST_ICON_TYPE_MAST = 0x0f00;
        public const int CONST_ID_TYP_MASK = 0x00ff;

        public const int CONST_PLAYER_P0 = 0;
        public const int CONST_PLAYER_P1 = 1;
        public const int CONST_PLAYER_P2 = 2;
        public const int CONST_PLAYER_P3 = 3;

        public int config_zhuang_player = -1;
        public int config_zhuang_score = 0;
        public int config_zhu_icon = -1;
        public int[] config_player_vrcid_list;

        public const int GAME_STA_IDLE = 0;
        public const int GAME_STA_JOIN_EXIT = 1;
        public const int GAME_STA_JIAO_ZHUANG = 2;
        public const int GAME_STA_JIAO_ZHU = 3;
        public const int GAME_STA_PLAY = 4;
        public const int GAME_STA_OVER = 5;

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
            hugf.udonIoc.RegisterSingleton(nameof(SDH_CONFIG_Singleton_String), this, this);

            hugf.udonEvn.RegisterListener(nameof(this.SetPlayerVrcIdCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.ToggleEvn_StartOneCall), this);
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

        /// <summary>
        /// 开一居
        /// </summary>
        public void ToggleEvn_StartOneCall()
        {
            var seed = (int)this.eventData;
            hugf.TriggerEventWithData(nameof(SDH_FaPaiJi.StartShuffleCall), seed);
        }

        public void JiaoZhuangFinishCall()
        {
            var zhuang = (int)this.eventData;
            this.config_zhuang_player = zhuang;
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

        #region fun for others
        #region sort list
        public int[] sort_id_list = null;
        public int[] id_in_sorted_list = null;

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

        public const int CONST_TYPE_ZHU_BASE = 0x0010;
        // zhu;
        public const int CONST_TYPE_Zheng5 = CONST_TYPE_ZHU_BASE; //
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
        public const int CONST_TYPE_Fu5 = 0x0000;
        public const int CONST_TYPE_Fu6 = CONST_TYPE_Fu5 + 1;
        public const int CONST_TYPE_Fu8 = CONST_TYPE_Fu6 + 1;
        public const int CONST_TYPE_Fu9 = CONST_TYPE_Fu8 + 1;
        public const int CONST_TYPE_Fu10 = CONST_TYPE_Fu9 + 1;
        public const int CONST_TYPE_FuJ = CONST_TYPE_Fu10 + 1;
        public const int CONST_TYPE_FuQ = CONST_TYPE_FuJ + 1;
        public const int CONST_TYPE_FuK = CONST_TYPE_FuQ + 1;
        public const int CONST_TYPE_FuA = CONST_TYPE_FuK + 1;

        public const int CONST_TYPE_UNKNOWN = -1;
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
                        var _base = icon << 8;

                        switch (i)
                        {
                            case 0:
                                config_type_id_list[card_id - 1] = CONST_TYPE_ZhengA | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_ZhengA | _base;
                                break;
                            case 1:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng2 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng2 | _base;
                                break;
                            case 2:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng3 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng3 | _base;
                                break;
                            case 3:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng4 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng4 | _base;
                                break;
                            case 4:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng5 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng5 | _base;
                                break;
                            case 5:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng6 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng6 | _base;
                                break;
                            case 6:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng7 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng7 | _base;
                                break;
                            case 7:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng8 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng8 | _base;
                                break;
                            case 8:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng9 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng9 | _base;
                                break;
                            case 9:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Zheng10 | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Zheng10 | _base;
                                break;
                            case 10:
                                config_type_id_list[card_id - 1] = CONST_TYPE_ZhengJ | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_ZhengJ | _base;
                                break;
                            case 11:
                                config_type_id_list[card_id - 1] = CONST_TYPE_ZhengQ | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_ZhengQ | _base;
                                break;
                            case 12:
                                config_type_id_list[card_id - 1] = CONST_TYPE_ZhengK | _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_ZhengK | _base;
                                break;
                        }
                    }

                    else
                    {
                        var _base = icon << 8;
                        switch (i)
                        {
                            case 0:
                                config_type_id_list[card_id - 1] = CONST_TYPE_FuA + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_FuA + _base;
                                break;
                            case 1:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu2 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu2 + _base;
                                if (zhu == CONST_ICON_JOKER)
                                {
                                    config_type_id_list[card_id - 1] = CONST_TYPE_Zheng2 + _base;
                                    config_type_id_list[card_id - 2] = CONST_TYPE_Zheng2 + _base;
                                }
                                break;
                            case 2:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu3 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu3 + _base;
                                break;
                            case 3:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu4 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu4 + _base;
                                break;
                            case 4:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu5 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu5 + _base;
                                break;
                            case 5:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu6 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu6 + _base;
                                break;
                            case 6:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu7 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu7 + _base;
                                if (zhu == CONST_ICON_JOKER)
                                {
                                    config_type_id_list[card_id - 1] = CONST_TYPE_Zheng7 + _base;
                                    config_type_id_list[card_id - 2] = CONST_TYPE_Zheng7 + _base;
                                }
                                break;
                            case 7:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu8 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu8 + _base;
                                break;
                            case 8:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu9 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu9 + _base;
                                break;
                            case 9:
                                config_type_id_list[card_id - 1] = CONST_TYPE_Fu10 + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_Fu10 + _base;
                                break;
                            case 10:
                                config_type_id_list[card_id - 1] = CONST_TYPE_FuJ + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_FuJ + _base;
                                break;
                            case 11:
                                config_type_id_list[card_id - 1] = CONST_TYPE_FuQ + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_FuQ + _base;
                                break;
                            case 12:
                                config_type_id_list[card_id - 1] = CONST_TYPE_FuK + _base;
                                config_type_id_list[card_id - 2] = CONST_TYPE_FuK + _base;
                                break;
                        }
                    }
                }
            }

            config_type_id_list[card_id++] = CONST_TYPE_SmallJoker | 0x400;
            config_type_id_list[card_id++] = CONST_TYPE_SmallJoker | 0x400;
            config_type_id_list[card_id++] = CONST_TYPE_BigJoker | 0x400;
            config_type_id_list[card_id++] = CONST_TYPE_BigJoker | 0x400;
        }

        public void ConfigTuolajLIst()
        {
            ;
        }
        /// <summary>
        /// 0x0f00 icon mask  0
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetTypeById(int id)
        {
            if (id >= 0 && id < 108)
                return config_type_id_list[id];
            hugf.udondebug.LogWarning("id is CONST_TYPE_UNKNOWN");
            return CONST_TYPE_UNKNOWN;
        }

        [SerializeField] private int[] _sort_temp_list = new int[CONST_SHOW_CARD_NUM];

        // 无需排序
        public int CheckIconNum(int[] id_list, int num, int icon)
        {
            var icon_num = 0;
            for (int i = 0; i < num; i++)
            {
                var typ = GetTypeById(id_list[i]);
                if ((typ & CONST_ICON_TYPE_MAST) == (icon << 8))
                {
                    icon_num++;
                }
            }
            return icon_num;
        }

        public int GetPaiList(int[] id_list, int[] out_pair_list, int num)
        {
            var _num = 0;
            for (int i = 0; i < num - 1; i++)
            {
                if (id_list[i] / 2 == id_list[i + 1] / 2)
                {
                    out_pair_list[i] = id_list[i];
                    _num++;
                }
            }
            out_pair_list[_num] = -1;
            return _num;
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

        public int DelListCard(int[] list1, int[] list2, int _list_num, int _list_num2)
        {
            if (_list_num <= 0 || _list_num2 <= 0) return _list_num;

            // 双指针遍历，直接在原数组上操作
            int newLength = 0;
            int index2 = 0;

            for (int index1 = 0; index1 < _list_num; index1++)
            {
                // 如果还有需要删除的元素，并且当前元素匹配
                if (index2 < _list_num2 && list1[index1] == list2[index2])
                {
                    // 跳过当前元素（删除），并移动list2的指针
                    index2++;
                }
                else
                {
                    // 保留当前元素，将其移到新位置
                    list1[newLength++] = list1[index1];
                }
            }

            // 返回新的元素数量
            return newLength;
        }

        public bool CheckSameIconType(int typ1, int typ2)
        {
            if ((typ1 & CONST_ICON_TYPE_MAST) == (typ2 & CONST_ICON_TYPE_MAST))
            {
                return true;
            }

            var typ1_is_zhu = (typ1 & CONST_ID_TYP_MASK) >= 0x10;
            var typ2_is_zhu = (typ2 & CONST_ID_TYP_MASK) >= 0x10;

            // 都是主牌， 花色相同
            if (typ1_is_zhu && typ2_is_zhu)
            {
                return true;
            }

            // 一个主牌， 一个不是主牌， 花色不同
            else if (typ1_is_zhu != typ2_is_zhu)
            {
                return false;
            }

            var _icon_typ1 = (typ1 & CONST_ICON_TYPE_MAST) >> 8;
            var _icon_typ2 = (typ2 & CONST_ICON_TYPE_MAST) >> 8;
            // 副牌， 花色相同
            if (_icon_typ1 == _icon_typ2)
            {
                return true;
            }
            return false;
        }

        public int GetIconNumS(int[] card_id, int num, int base_typ)
        {
            var icon_s = new int[4];
            icon_s[0] = 0;
            icon_s[1] = 0;
            icon_s[2] = 0;
            icon_s[3] = 0;
            var zhu = 0;
            for (int i = 0; i < num; i++)
            {
                var typ = GetTypeById(card_id[i]);
                if(typ == CONST_TYPE_UNKNOWN)
                    continue;

                if ((typ & CONST_ID_TYP_MASK) >= 0x10)
                {
                    zhu++;
                }
                else
                {
                    icon_s[(typ & CONST_ICON_TYPE_MAST) >> 8]++;
                }
            }

            if ((base_typ & CONST_ID_TYP_MASK) >= 0x0f)
            {
                return zhu;
            }
            else
            {
                return icon_s[(base_typ & CONST_ICON_TYPE_MAST) >> 8];
            }
        }
        #endregion function for others
    }
}