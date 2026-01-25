
using HopeTools;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SDH_OutCartFsm : UdonSharpBehaviour
    {
        #region init code

        private bool _is_init = false;

        [SerializeField] public int[] out_card_id_list;
        [SerializeField] public int out_card_num;
        [SerializeField] private int[] _select_card_id_list;
        [SerializeField] private int _select_card_num;

        [SerializeField] private SDH_GameManager sDH_GameManager;
        [SerializeField] private SDH_FaPaiJi sDH_FaPaiJi;
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
            _select_card_id_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            this._select_card_num = 0;
        }

        public HopeTools.HopeUdonFramework hugf;
        public object eventData;

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SelecCardCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.UnselecCardCall), this);

            hugf.udonEvn.RegisterListener(nameof(this.FaPaiCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.StartChuPaiCall), this);

            hugf.udonEvn.RegisterListener(nameof(this.ToggleEvn_OutButCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.ToggleEvn_MaiDiCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.ToggleEvn_TipsButCall), this);

            hugf.udonEvn.RegisterListener(nameof(this.JiaoZhuangFinishCall), this);
        }


        public void HufgIocGet()
        {
            sDH_FaPaiJi = (SDH_FaPaiJi)hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi));
            sDH_GameManager = (SDH_GameManager)hugf.udonIoc.GetServiceUdon(SDH_GameManager.SDH_CONFIG_Singleton_String);
        }

        //public void DemeFunCall()
        //{
        //    this.eventData = data;
        //}
        #endregion end init code



        public void ResetOutCardState()
        {
            this.out_card_num = 0;
            this._select_card_num = 0;
            this._current_player = -1;
        }

        public void ToggleEvn_OutBut(int x)
        {
            if (x == this._current_player)
                OutFun();
        }

        public void ToggleEvn_TipsBut(int x)
        {
            hugf.Log($"ToggleEvn_TipsBut: {x}");
            // test 
        }

        private void DelIdxHandList(int[] _list, int num, int idx)
        {
            sDH_GameManager.SortListByIdxCard(_list, num);
            var _out_list = GetPlayerHandList(idx);
            if (_out_list == null)
            {
                return;
            }
            sDH_GameManager.DelListCard(_out_list, _list, SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX, num);
        }


        private void AddIdxHandlist(int[] _list, int num, int idx)
        {
            var _hand_list = GetPlayerHandList(idx);
            if (_hand_list == null)
            {
                return;
            }

            for (int i = 0; i < num; i++)
            {
                _hand_list[i + this._pre_hand_card_num] = _list[i];
            }
            this._pre_hand_card_num += num;
            sDH_GameManager.SortListByIdxCard(_hand_list, _pre_hand_card_num);
        }

        public void JiaoZhuangFinishCall()
        {
            int x = (int)this.eventData;
            this._select_card_num = 0;
            
            AddIdxHandlist(this._dipai_list, SDH_GameManager.CONST_DIPAI_CARD_NUM, x);
            TrigPlayerHandCardShow(x, this._pre_hand_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.EnCardTileClickCall), this._dipai_list, this._dipai_num);
        }

        public void ToggleEvn_MaiDi(int x)
        {
            //hugf.Log($"ToggleEvn_MaiDi: {x}");
            if (this._select_card_num == SDH_GameManager.CONST_DIPAI_CARD_NUM)
            {
                for (int i = 0; i < this._select_card_num; i++)
                {
                    this._dipai_list[i] = this._select_card_id_list[i];
                }
                DelIdxHandList(this._dipai_list, this._dipai_num, this._current_player);
                this._pre_hand_card_num -= SDH_GameManager.CONST_DIPAI_CARD_NUM;
                TrigPlayerHandCardShow(this._current_player, this._pre_hand_card_num);
                hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.DisCardTileClickCall), this._dipai_list, this._dipai_num);
                hugf.TriggerEventWith2Data(nameof(SDH_DiPaiManager.SetDiCardPositionCall), this._dipai_list, this._dipai_num);
            }
        }

        public bool CardIsInPlayerHand(int card_id, int p)
        {
            var _list = GetPlayerHandList(p);
            if (_list == null)
                return false;
                
            for (int i = 0; i < _pre_hand_card_num; i++)
            {
                if (_list[i] == card_id)
                    return true;
            }
            return false;
        }

        public void SelecCardCall()
        {
            if (_select_card_num >= _select_card_id_list.Length) return;

            var _id = (int)this.eventData;
            if (!CardIsInPlayerHand(_id, this._current_player))
                return;

            for (int i = 0; i < this._select_card_num; i++)
            {
                if (this._select_card_id_list[i] == _id)
                    return;
            }
            this._select_card_id_list[this._select_card_num++] = _id;
            sDH_FaPaiJi.UpdateCardPosition(_id, SDH_CardTile.CARD_POS_SELECT);
        }

        public void UnselecCardCall()
        {
            if (_select_card_num <= 0) return;

            var _id = (int)this.eventData;
            int foundIndex = -1;

            // Find the index of the card to remove
            for (int i = 0; i < this._select_card_num; i++)
            {
                if (this._select_card_id_list[i] == _id)
                {
                    foundIndex = i;
                    break;
                }
            }

            // If card found, shift elements from foundIndex onwards
            if (foundIndex != -1)
            {
                for (int i = foundIndex; i < this._select_card_num - 1; i++)
                {
                    this._select_card_id_list[i] = this._select_card_id_list[i + 1];
                }
            }

            if (this._select_card_num >= this._select_card_id_list.Length)
            {
                hugf.udondebug.LogWarning($"UnselecCardCall: {_id}, select_card_num: {this._select_card_num}");
                return;
            }
            this._select_card_num--;
            sDH_FaPaiJi.UpdateCardPosition(_id, SDH_CardTile.CARD_POS_UNSELEC);
        }

        // start method
        public void ToggleEvn_OutButCall()
        {
            var idx = (int)this.eventData;
            ToggleEvn_OutBut(idx);
        }
        public void ToggleEvn_TipsButCall()
        {
            var idx = (int)this.eventData;
            ToggleEvn_TipsBut(idx);
        }
        public void ToggleEvn_MaiDiCall()
        {
            var idx = (int)this.eventData;
            ToggleEvn_MaiDi(idx);
        }
        // end method

        #region 出牌判定

        private int _current_player;
        private int _first_out_player;

        private int[] _p0_out_list;
        private int[] _p1_out_list;
        private int[] _p2_out_list;
        private int[] _p3_out_list;

        private int[] _p0_hand_list;
        private int[] _p1_hand_list;
        private int[] _p2_hand_list;
        private int[] _p3_hand_list;

        private int[] _dipai_list;
        private int _dipai_num;
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

            this._dipai_list = new int[SDH_GameManager.CONST_DIPAI_CARD_NUM];
            this._dipai_num = SDH_GameManager.CONST_DIPAI_CARD_NUM;


            for (int i = 0; i < SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX; i++)
            {
                _p0_hand_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p1_hand_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p2_hand_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p3_hand_list[i] = SDH_GameManager.CONST_CARD_NULL;

                _p0_out_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p1_out_list[i] = SDH_GameManager.CONST_CARD_NULL;
                _p2_out_list[i] = SDH_GameManager.CONST_CARD_NULL;
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

            for (int i = 0; i < 8; i++)
            {
                this._dipai_list[i] = dat[SDH_GameManager.CONST_SDH_TOTAL_CARD_NUM - i - 1];
            }
            _pre_hand_card_num = SDH_GameManager.CONST_PLAYER_GRAB_CARD_NUM;
            _feng_card_num = 0;
            sDH_GameManager.SortListByIdxCard(this._p0_hand_list, _pre_hand_card_num);
            sDH_GameManager.SortListByIdxCard(this._p1_hand_list, _pre_hand_card_num);
            sDH_GameManager.SortListByIdxCard(this._p2_hand_list, _pre_hand_card_num);
            sDH_GameManager.SortListByIdxCard(this._p3_hand_list, _pre_hand_card_num);

            hugf.TriggerEventWith2Data(nameof(SDH_HandCartP.SetHandCardP0Call), this._p0_hand_list, _pre_hand_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_HandCartP.SetHandCardP1Call), this._p1_hand_list, _pre_hand_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_HandCartP.SetHandCardP2Call), this._p2_hand_list, _pre_hand_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_HandCartP.SetHandCardP3Call), this._p3_hand_list, _pre_hand_card_num);

            hugf.TriggerEventWith2Data(nameof(SDH_DiPaiManager.SetDiCardPositionCall), this._dipai_list, this._dipai_num);

            hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.EnCardTileClickCall), dat, _pre_hand_card_num * 4);
            hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.DisCardTileClickCall), _dipai_list, _dipai_num);
        }

        private int[] GetPlayerHandList(int idx)
        {
            if (idx == SDH_GameManager.CONST_PLAYER_P0)
            {
                return this._p0_hand_list;
            }
            else if (idx == SDH_GameManager.CONST_PLAYER_P1)
            {
                return this._p1_hand_list;
            }
            else if (idx == SDH_GameManager.CONST_PLAYER_P2)
            {
                return this._p2_hand_list;
            }
            else if (idx == SDH_GameManager.CONST_PLAYER_P3)
            {
                return this._p3_hand_list;
            }
            hugf.udondebug.LogWarning($"GetPlayerHandList: {idx} is null");
            return null;
        }

        private void OutFun()
        {
            var _out_en = false;

            if (_select_card_num >= 2)
            {
                sDH_GameManager.SortListByIdxCard(this._select_card_id_list, this._select_card_num);
            }

            if (this._current_player == this._first_out_player)
            {
                _out_en = CheckFirstOutEn();
                // 出牌不合法
                if (!_out_en)
                {
                    hugf.Log($"CheckFirstOutEn CheckOutEn failed, select_card_num: {this._select_card_num}");
                    return;
                }
            }
            else
            {
                _out_en = CheckAfterOutEn();
                if (!_out_en)
                {
                    hugf.Log($"CheckAfterOutEn CheckOutEn failed, select_card_num: {this._select_card_num}");
                    return;
                }
            }


            // clear last times card
            if (_pre_round_card_show_num > 0)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.SetCardTileDisCall), this._p0_out_list, this._pre_round_card_show_num);
                hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.SetCardTileDisCall), this._p1_out_list, this._pre_round_card_show_num);
                hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.SetCardTileDisCall), this._p2_out_list, this._pre_round_card_show_num);
                hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.SetCardTileDisCall), this._p3_out_list, this._pre_round_card_show_num);
                
                _pre_round_card_show_num = 0;
            }

            if (this._current_player == this._first_out_player)
            {
                for (int i = 0; i < this._select_card_num; i++)
                {
                    var _id = this._select_card_id_list[i];
                    this.first_out_card_list[i] = _id;
                    this.fitrt_out_card_type_list[i] = sDH_GameManager.GetTypeById(_id);
                }
                this.first_out_card_num = this._select_card_num;
            }

            for (int i = 0; i < this._select_card_num; i++)
            {
                var _id = this._select_card_id_list[i];
                this.out_card_id_list[i] = _id;
                var _list = GetPlayerHandList(_current_player);
                _list[i] = _id;
            }

            this.out_card_num = this._select_card_num;
            TrigPlayerOutCardShow(255, this.out_card_num);

            hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.DisCardTileClickCall), this.out_card_id_list, this.out_card_num);

            this._current_player++;
            this._current_player %= 4;

            if (this._current_player == this._first_out_player)
            {
                this._select_card_num = 0;
                _pre_round_card_show_num = this.first_out_card_num;
                DelHandOutCard();
                var _max_p = CheckOutMaxPlayer();
                JianFen(_max_p, _max_p + 1);
                StartNewRound(_max_p);
            }
            else
            {
                this._select_card_num = 0;
                hugf.TriggerEventWithData(nameof(SDH_Tips.SetActivePlayerCall), _current_player);
            }
        }

        private void TrigPlayerOutCardShow(int idx, int num)
        {
            if (_current_player == SDH_GameManager.CONST_PLAYER_P0 || idx == 255)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_OutCartP.SetOutCardP0Call), this._p0_out_list, num);
            }
            else if (_current_player == SDH_GameManager.CONST_PLAYER_P1 || idx == 255)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_OutCartP.SetOutCardP1Call), this._p1_out_list, num);
            }
            else if (_current_player == SDH_GameManager.CONST_PLAYER_P2 || idx == 255)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_OutCartP.SetOutCardP2Call), this._p2_out_list, num);
            }
            else if (_current_player == SDH_GameManager.CONST_PLAYER_P3 || idx == 255)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_OutCartP.SetOutCardP3Call), this._p3_out_list, num);
            }
        }

        private void TrigPlayerHandCardShow(int idx, int num)
        {
            if (idx == SDH_GameManager.CONST_PLAYER_P0 || idx == 255)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_HandCartP.SetHandCardP0Call), this._p0_hand_list, num);
            }
            else if (idx == SDH_GameManager.CONST_PLAYER_P1 || idx == 255)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_HandCartP.SetHandCardP1Call), this._p1_hand_list, num);
            }
            else if (idx == SDH_GameManager.CONST_PLAYER_P2 || idx == 255)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_HandCartP.SetHandCardP2Call), this._p2_hand_list, num);
            }
            else if (idx == SDH_GameManager.CONST_PLAYER_P3 || idx == 255)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_HandCartP.SetHandCardP3Call), this._p3_hand_list, num);
            }
        }

        private int _pre_round_card_show_num;
        public void StartNewRound(int p)
        {
            this._current_player = p;
            this._first_out_player = p;
            this.out_card_num = 0;
            this._select_card_num = 0;
            hugf.TriggerEventWithData(nameof(SDH_Tips.SetActivePlayerCall), p);
        }


        private void DelHandOutCard()
        {
            var _num = this._pre_round_card_show_num;
            if (_num <= 0 || _pre_hand_card_num <= 0) return;

            // 使用通用的DelListCard方法删除手牌
            sDH_GameManager.DelListCard(_p0_hand_list, _p0_out_list, _pre_hand_card_num, _num);
            sDH_GameManager.DelListCard(_p1_hand_list, _p1_out_list, _pre_hand_card_num, _num);
            sDH_GameManager.DelListCard(_p2_hand_list, _p2_out_list, _pre_hand_card_num, _num);
            sDH_GameManager.DelListCard(_p3_hand_list, _p3_out_list, _pre_hand_card_num, _num);
            _pre_hand_card_num -= _num;

            TrigPlayerHandCardShow(255, this._pre_hand_card_num);
        }

        private bool CheckEqCheckEq(int[] first_list, int[] _list_1, int num)
        {
            int first_typ = sDH_GameManager.GetTypeById(first_list[0]);
            int select_typ = sDH_GameManager.GetTypeById(_list_1[0]);

            var _en = sDH_GameManager.CheckOutBigEnType(first_typ, select_typ);

            // 16进制打印，检查大小关系
            // hugf.udondebug.LogUdonMsg(this, first_typ.ToString("X") + " " + select_typ.ToString("X") + " " + _en);
            if (_en == false)
            {
                return false;
            }

            if (num == 1)
            {
                return true;
            }

            if (num == 2)
            {
                if ((_list_1[0] / 2) == (_list_1[1] / 2))
                {
                    return true;
                }
                return false;
            }

            int _tuo = sDH_GameManager.GetCardTuoLaJi(_list_1, num, sDH_GameManager.GetTypeById(_list_1[0]));
            return _tuo * 2 == num;
            return false;
        }

        public int CheckOutMaxPlayer()
        {
            var _typ_list = new int[4];
            _typ_list[0] = sDH_GameManager.GetTypeById(_p0_out_list[0]) & SDH_GameManager.CONST_ID_TYP_MASK;
            _typ_list[1] = sDH_GameManager.GetTypeById(_p1_out_list[0]) & SDH_GameManager.CONST_ID_TYP_MASK;
            _typ_list[2] = sDH_GameManager.GetTypeById(_p2_out_list[0]) & SDH_GameManager.CONST_ID_TYP_MASK;
            _typ_list[3] = sDH_GameManager.GetTypeById(_p3_out_list[0]) & SDH_GameManager.CONST_ID_TYP_MASK;

            var max_p = -1;
            var max_typ = -1;
            var en_list = new bool[4];
            en_list[0] = CheckEqCheckEq(this.first_out_card_list, this._p0_out_list, this.first_out_card_num);
            en_list[1] = CheckEqCheckEq(this.first_out_card_list, this._p1_out_list, this.first_out_card_num);
            en_list[2] = CheckEqCheckEq(this.first_out_card_list, this._p2_out_list, this.first_out_card_num);
            en_list[3] = CheckEqCheckEq(this.first_out_card_list, this._p3_out_list, this.first_out_card_num);

            for (int i = 0; i < 4; i++)
            {
                var _p = (i + this._first_out_player) % 4;
                if (en_list[_p] && _typ_list[_p] > max_typ)
                {
                    max_p = _p;
                    max_typ = _typ_list[_p];
                }
            }
            return max_p;
        }

        private bool CheckFirstOutEn()
        {
            if (_select_card_num == 1)
            {
                return true;
            }

            if (_select_card_num == 2)
            {
                var x = _select_card_id_list[0] / 2;
                var y = _select_card_id_list[1] / 2;
                return x == y;
            }

            // 单数， 暂时不支持甩牌, err
            if ((_select_card_num & 0x01) > 0)
            {
                return false;
            }

            var _typ = sDH_GameManager.GetTypeById(this._select_card_id_list[0]);
            var _icon_num = sDH_GameManager.GetIconNumS(this._select_card_id_list, this._select_card_num, _typ);

            if (_icon_num != _select_card_num)
            {
                return false;
            }


            var tuo_la_ji = sDH_GameManager.GetCardTuoLaJi(this._select_card_id_list, this._select_card_num, _typ);
            // hugf.udondebug.LogUdonMsg(this, "tuo_la_ji: " + tuo_la_ji);
            return tuo_la_ji * 2 == this._select_card_num;

            // 最高到五连拖
            if (_select_card_num > 10)
            {
                return false;
            }

            return false;
        }

        private int GetPlayerIconNum(int p, int typ)
        {
            var _list = GetPlayerHandList(p);
            if (_list == null)
            {
                return -1;
            }

            return sDH_GameManager.GetIconNumS(_list, this._pre_hand_card_num, typ);

        }

        private int GetPlayerPairNum(int p, int typ)
        {
            var _list = GetPlayerHandList(p);
            if (_list == null)
            {
                return -1;
            }

            return sDH_GameManager.GetTypePairList(_list, this._pre_hand_card_num, typ, _temp_int_list);

        }

        private int GetPlayerTuoLaJi(int p, int typ)
        {
            var _list = GetPlayerHandList(p);
            if (_list == null)
            {
                return -1;
            }
            return sDH_GameManager.GetCardTuoLaJi(_list, this._pre_hand_card_num, typ);
        }

        [SerializeField] private int[] _temp_int_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];

        private bool CheckAfterOutEn()
        {
            if (this._select_card_num != this.first_out_card_num)
            {
                return false;
            }

            var _first_typ = fitrt_out_card_type_list[0];
            int _icon_num = GetPlayerIconNum(this._current_player, _first_typ);
            int _sele_num = sDH_GameManager.GetIconNumS(this._select_card_id_list, this._select_card_num, _first_typ);

            // 手牌花色不够或刚好，全出
            if (_icon_num <= first_out_card_num)
            {
                return _sele_num == _icon_num;
            }

            // 手牌有多，但不出对应的花色
            if (_sele_num < first_out_card_num)
            {
                return false;
            }

            int select_pair_num = sDH_GameManager.GetTypePairList(this._select_card_id_list, this._select_card_num, _first_typ, this._temp_int_list);
            int player_pair_num = GetPlayerPairNum(this._current_player, _first_typ);
            hugf.udondebug.LogUdonMsg(this, $"CheckAfterOutEn: select_pair_num: {select_pair_num}, player_pair_num: {player_pair_num}");
            // 处理对子情况（2张牌）
            if (this.first_out_card_num == 2)
            {
                if (player_pair_num >= 1) // 手上有对，必须出对
                {
                    return select_pair_num >= 1;
                }
                return true;
            }

            // 处理两连拖情况
            if (this.first_out_card_num == 4)
            {
                int select_tuo = sDH_GameManager.GetCardTuoLaJi(this._select_card_id_list, this._select_card_num, _first_typ);
                int player_tuo = GetPlayerTuoLaJi(this._current_player, _first_typ);

                if (player_tuo >= 2) // 有两连拖，必须出连拖
                {
                    return select_tuo >= 2;
                }

                if (select_pair_num < player_pair_num && player_pair_num >= 2) // 出对子数不能大于出牌数
                {
                    return false;
                }
                return true;
            }
            // 处理三连拖情况
            if (this.first_out_card_num == 6)
            {
                int select_tuo = sDH_GameManager.GetCardTuoLaJi(this._select_card_id_list, this._select_card_num, _first_typ);
                int player_tuo = GetPlayerTuoLaJi(this._current_player, _first_typ);

                if (player_tuo >= 3) // 有三连拖，必须出连拖
                {
                    return select_tuo >= 3;
                }

                if (select_tuo < player_tuo && player_tuo >= 2) // 出对子数不能大于出牌数
                {
                    return false;
                }

                if (select_pair_num < player_pair_num && player_pair_num >= 3) // 出对子数不能大于出牌数
                {
                    return false;
                }
                return true;
            }

            // tuolaji 4
            if (this.first_out_card_num == 8)
            {
                int select_tuo = sDH_GameManager.GetCardTuoLaJi(this._select_card_id_list, this._select_card_num, _first_typ);
                int player_tuo = GetPlayerTuoLaJi(this._current_player, _first_typ);

                if (player_tuo >= 4) // 有三连拖，必须出连拖
                {
                    return select_tuo >= 4;
                }

                if (select_pair_num < player_pair_num && player_pair_num >= 4) // 出对子数不能大于出牌数
                {
                    return false;
                }
                return true;
            }

            return true;
        }

        public void StartChuPaiCall()
        {
            StartNewRound(sDH_GameManager.config_zhuang_player);
            this._pre_round_card_show_num = 0;
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


        [HideInInspector] public int[] _feng_card_list;
        private int _feng_card_num;
        private bool[] _feng_fast_list;
        public void PushToFengListIf(int card_id)
        {

            if (_feng_fast_list == null)
            {
                _feng_fast_list = new bool[0x1f];
                _feng_card_list = new int[0x1f];
                _feng_fast_list[SDH_GameManager.CONST_TYPE_Zheng5] = true;
                _feng_fast_list[SDH_GameManager.CONST_TYPE_Zheng10] = true;
                _feng_fast_list[SDH_GameManager.CONST_TYPE_ZhengK] = true;

                _feng_fast_list[SDH_GameManager.CONST_TYPE_Fu5] = true;
                _feng_fast_list[SDH_GameManager.CONST_TYPE_Fu10] = true;
                _feng_fast_list[SDH_GameManager.CONST_TYPE_FuK] = true;
            }

            var typ = sDH_GameManager.GetTypeById(card_id) & SDH_GameManager.CONST_ID_TYP_MASK;
            // 16 进制打印
            //Debug.Log($"PushToFengListIf: card_id: {card_id},{typ} typ: {typ.ToString("X")}, feng_fast_list: [typ]");
            if (typ == SDH_GameManager.CONST_TYPE_UNKNOWN)
                return;

            Debug.Log($"PushToFengListIf: card_id: {card_id}, typ: {typ}");
            if (_feng_fast_list[typ])
            {
                _feng_card_list[_feng_card_num++] = card_id;
            }
        }

        public void JianFen(int zhuang, int max_p)
        {
            if (zhuang == max_p)
            {
                return;
            }

            var _num = this._feng_card_num;
            for (int i = 0; i < first_out_card_num; i++)
            {
                PushToFengListIf(this._p0_out_list[i]);
                PushToFengListIf(this._p1_out_list[i]);
                PushToFengListIf(this._p2_out_list[i]);
                PushToFengListIf(this._p3_out_list[i]);
            }
            if (this._feng_card_num != _num)
            {
                hugf.TriggerEventWith2Data(nameof(SDH_DiPaiManager.SetFenCardPositionCall), this._feng_card_list, this._feng_card_num);
            }
        }
    }
}




