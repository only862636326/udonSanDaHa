
using HopeTools;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]

    public class SDH_PlayerManager : UdonSharpBehaviour
    {
        [UdonSynced] int[] syn_data_list;

        public int[] hand_card_list;
        public int hand_card_num;

        private int _player_vrc_id;
        private int _player_index;

        [SerializeField] private Transform[] card_tf_list;

        public Transform handGrabCardPrt;
        private Transform _hand_card_positon_prt;
        [SerializeField] private SDH_GameManager sDH_GameManager;

        void Start()
        {
            ;
        }

        private bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            this._player_index = this.transform.GetSiblingIndex();

            var _tile = handGrabCardPrt.GetChild(this._player_index);
            foreach (Transform child in _tile)
            {
                var _n = child.name.ToLower();
                if (_n.Contains("position") && (_n.Contains("prt") || _n.Contains("parent")))
                {
                    this._hand_card_positon_prt = child;
                    break;
                }
            }

            this.hand_card_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            this.syn_data_list = new int[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX + 10];
        }

        private HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1;
        public object eventData2;
        public void HugfInit()
        {
            if (hugf == null)
            {
                hugf = GameObject.Find(SDH_GameManager.CONST_SDH_HUGF_STRING).GetComponent<HopeUdonFramework>();
                if(hugf == null)
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
            hugf.udonEvn.RegisterListener(nameof(this.FaPaiCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetHandCardPositionCall), this);
        }
        public void HufgIocGet()
        {
            card_tf_list = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
            sDH_GameManager = (SDH_GameManager)hugf.udonIoc.GetServiceUdon(SDH_GameManager.SDH_CONFIG_Singleton_String);
        }


        public void FaPaiCall()
        {
            var dat = (int[])this.eventData;
            if (dat == null || dat.Length == 0)
            {
                hugf.udondebug.LogError("SDH_DiPaiManager FaPaiCall data is null or empty!");
                return;
            }

            GrabHandCard(dat);
            sDH_GameManager.SortListByIdxCard(this.hand_card_list, this.hand_card_num);
            hugf.TriggerEventWith2Data(nameof(SDH_FaPaiJi.EnCardTileClickCall), this.hand_card_list, this.hand_card_num);            
            RequestSyn();
        }       

        public void GrabHandCard(int[] cards)
        {
            for (int i = 0; i < SDH_GameManager.CONST_PLAYER_GRAB_CARD_NUM; i++)
            {
                this.hand_card_list[i] = cards[i * 4 + this._player_index];
            }
            for (int i = SDH_GameManager.CONST_PLAYER_GRAB_CARD_NUM; i < SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX; i++)
            {
                this.hand_card_list[i] = SDH_GameManager.CONST_CARD_NULL;
            }
            hand_card_num = SDH_GameManager.CONST_PLAYER_GRAB_CARD_NUM;
        }

        public void SetHandCardPosition(Transform[] tf_cards)
        {
            var rot = this.GetCardRotation(this._hand_card_positon_prt, 0, hand_card_num);
            for (int i = 0; i < hand_card_num; i++)
            {
                var idx = this.hand_card_list[i];
                var pos = GetCardPosition(this._hand_card_positon_prt, i, hand_card_num);
                tf_cards[idx].position = pos;
                tf_cards[idx].rotation = rot;

                tf_cards[idx].GetComponent<SDH_CardTile>().SetCardP_x(0);
            }
        }


        public void SetHandCardPositionCall()
        {
            var _card_id_list = (int[])(this.eventData);
            var _card_num = (int)this.eventData2;

            var _r = GetCardRotation(this._hand_card_positon_prt, 0, _card_num);
            for (int i = 0; i < _card_num; i++)
            {
                var card_id = _card_id_list[i];

                var pos = GetCardPosition(this._hand_card_positon_prt, i, _card_num);
                if (pos == Vector3.zero)
                    continue;

                var tf = this.card_tf_list[card_id];
                if (tf == null)
                    continue;
                tf.position = pos;
                tf.rotation = _r;
                tf.gameObject.SetActive(true);
            }
        }

#if UNITY_EDITOR       
        [Header("测试用，运行时无效")]
        public Transform test_tf_card_prt;
        public int test_card_num;
        public void TestSetCardPosition()
        {
            Debug.Log($"-------------TestSetCardPosition, {test_card_num}");
            if (test_tf_card_prt == null || test_card_num <= 0)
                return;
            hand_card_num = test_card_num;

            for (int i = 0; i < SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX; i++)
            {
                if (i < test_card_num)
                    this.hand_card_list[i] = i;
                else
                    this.hand_card_list[i] = SDH_GameManager.CONST_CARD_NULL;
            }

            var tf_cards = new Transform[test_tf_card_prt.childCount];
            for (int i = 0; i < test_tf_card_prt.childCount; i++)
            {
                tf_cards[i] = test_tf_card_prt.GetChild(i);
                tf_cards[i].localPosition = Vector3.zero;
                tf_cards[i].localRotation = Quaternion.identity;
            }
            SetHandCardPosition(tf_cards);

            if (hand_card_num < SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX)
            {
                test_card_num++;
                this.SendCustomEventDelayedSeconds(nameof(this.TestSetCardPosition), 0.2f);
            }
        }
#endif

        private Quaternion GetCardRotation(Transform tf, int card_index, int card_num)
        {
            if (tf == null)
                return Quaternion.identity;
            return tf.GetChild(0).rotation;
        }

        private Vector3 GetCardPosition(Transform tf, int card_index, int card_num)
        {
            if (card_num <= 0)
                return Vector3.zero;

            if (tf == null)
                return Vector3.zero;

            var first_pos = tf.GetChild(0).position;
            var second_pos = tf.GetChild(1).position;
            var last_pos = tf.GetChild(tf.childCount - 1).position;
            var center_pos = (first_pos + last_pos) / 2;
            var offset = (second_pos - first_pos); // 使用相邻两个位置点的间距作为标准偏移量

            if (card_num <= tf.childCount)
            {
                // 卡牌数量小于等于位置点数量时，以center_pos为中心向两侧分布
                if (card_num % 2 == 0)
                {
                    // 偶数张卡牌时，中心在中间两张之间，向两侧对称分布
                    int half = card_num / 2;
                    // 计算当前卡牌与中心的偏移索引（从-0.5开始）
                    float offsetIndex = card_index - (half - 0.5f);
                    return center_pos + offset * offsetIndex;
                }
                else
                {
                    // 奇数张卡牌时，中间卡牌位于center_pos，向两侧对称分布
                    int half = card_num / 2;
                    int offsetIndex = card_index - half;
                    return center_pos + offset * offsetIndex;
                }
            }
            else
            {
                // 卡牌数量大于位置点数量时，重新计算offset以适应更多卡牌
                var newOffset = (last_pos - first_pos) / (card_num - 1);
                return center_pos + newOffset * (card_index - card_num / 2);
            }

            return Vector3.zero;
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
            for (int i = 0; i < SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX; i++)
            {
                syn_data_list[i] = hand_card_list[i];
            }
            syn_data_list[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX] = this.hand_card_num;
            SetHandCardPosition(this.card_tf_list);
            // DebugSynData();
        }

        public override void OnDeserialization()
        {
            for (int i = 0; i < SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX; i++)
            {
                hand_card_list[i] = syn_data_list[i];
            }
            this.hand_card_num = syn_data_list[SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX];
            SetHandCardPosition(this.card_tf_list);
            // DebugSynData();
        }

        public void DebugSynData()
        {
            string s = "SynData: ";
            for (int i = 0; i < SDH_GameManager.CONST_PLAYER_HAND_CARD_MAX + 1; i++)
            {
                s += syn_data_list[i] + ", ";
            }
            s += $" HandCardNum: {this.hand_card_num}";
            Debug.Log(s);
        }

        #endregion end syn
    }
}