
using HopeSDH;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SDH_OutCartP : UdonSharpBehaviour
    {
        #region init code
        private bool _is_init = false;

        private Transform[] _out_card_prt_list;
        private Transform[] card_tf_list;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here
            this._out_card_prt_list = new Transform[transform.childCount];
            for (int i = 0; i < _out_card_prt_list.Length; i++)
            {
                var tf = this.transform.GetChild(i);
                this._out_card_prt_list[i] = tf;
                this._out_card_prt_list[i].gameObject.SetActive(false);
            }
        }

        private HopeTools.HopeUdonFramework hugf;
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

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetOutCardP0Call), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetOutCardP1Call), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetOutCardP2Call), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetOutCardP3Call), this);
        }


        public void HufgIocGet()
        {
            card_tf_list = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }

        //public void DemeFunCall()
        //{
        //    this.eventData = data;
        //}
        #endregion end init code

        #region syn


        private int _out_card_player;
        public void SetOutCardPlayerCall()
        {
            this._out_card_player = (int)this.eventData;
        }


        public void SetOutCardP(int idx)
        {
            var _card_id_list = (int[])(this.eventData);
            var _card_num = (int)this.eventData2;
            var _r = GetCardRotation(this._out_card_prt_list[idx], idx, _card_num);
            for (int i = 0; i < _card_num; i++)
            {
                var card_id = _card_id_list[i];

                var pos = GetCardPosition(this._out_card_prt_list[idx], i, _card_num);
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

        public void SetOutCardP0Call() { SetOutCardP(0); }
        public void SetOutCardP1Call() { SetOutCardP(1); }
        public void SetOutCardP2Call() { SetOutCardP(2); }
        public void SetOutCardP3Call() { SetOutCardP(3); }

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

        private Quaternion GetCardRotation(Transform tf, int card_index, int card_num)
        {
            if (tf == null)
                return Quaternion.identity;
            return tf.GetChild(0).rotation;
        }

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