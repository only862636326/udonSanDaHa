
using HopeTools;
using SGS;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SDH_DiPaiManager : UdonSharpBehaviour
    {
        private int[] _dipai_list;
        private int _dipai_count;

        [SerializeField] private Transform _dipai_positon_prt;
        [SerializeField] private Transform _fenpai_position_prt;
        private Transform[] card_tf_list;

        #region init 
        private bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            _dipai_count = SDH_GameManager.CONST_DIPAI_CARD_NUM;
            _dipai_list = new int[_dipai_count];

            foreach (Transform child in this.transform)
            {
                var _n = child.name.ToLower();
                if (_n.Contains("dipai") && (_n.Contains("prt") || _n.Contains("parent")))
                {
                    this._dipai_positon_prt = child;
                    this._dipai_positon_prt.gameObject.SetActive(false);
                }
                if (_n.Contains("fen") && (_n.Contains("prt") || _n.Contains("parent")))
                {
                    this._fenpai_position_prt = child;
                    this._fenpai_position_prt.gameObject.SetActive(false);
                }
            }
        }

        public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1;
        public object eventData2;

        public void HugfInitAfter()
        {
            hugf.udonEvn.RegisterListener(nameof(this.SetFenCardPositionCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SetDiCardPositionCall), this);
        }

        public void HufgIocGet()
        {
            card_tf_list = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }

        #endregion init


        public void SetDiPaiPosition(Transform[] tf_list)
        {
            for (int i = 0; i < _dipai_count; i++)
            {
                int card_index = _dipai_list[i];
                Transform card_tf = tf_list[card_index];
                Transform dipai_pos_tf = _dipai_positon_prt.GetChild(i);
                card_tf.position = dipai_pos_tf.position;
                card_tf.rotation = dipai_pos_tf.rotation;
            }
        }


        private int[] _fend_id_list;

        public void SetFenCardPositionCall()
        {
            if (this._fend_id_list == null)
            {
                this._fend_id_list = new int[SDH_GameManager.CONST_MAX_FENG_NUM];
            }

            var dat = (int[])this.eventData;
            var num = (int)this.eventData2;
            if (dat == null || dat.Length == 0)
            {
                hugf.udondebug.LogWarning("SDH_DiPaiManager SetFenCardCall data is null or empty!");
                return;
            }
            for (int i = 0; i < num; i++)
            {
                var _id = dat[i];
                _fend_id_list[i] = _id;
                this.card_tf_list[_id].gameObject.SetActive(true);
                this.card_tf_list[_id].position = this._fenpai_position_prt.GetChild(i).position;
                this.card_tf_list[_id].rotation = this._fenpai_position_prt.GetChild(i).rotation;
            }
        }

        public void SetDiCardPositionCall()
        {
            if (this._dipai_list == null)
            {
                _dipai_list = new int[_dipai_count];
            }
            var dat = (int[])this.eventData;
            var num = (int)this.eventData2;

            if (dat == null || dat.Length == 0)
            {
                hugf.udondebug.LogWarning("SDH_DiPaiManager SetFenCardCall data is null or empty!");
                return;
            }
            for (int i = 0; i < num; i++)
            {
                var _id = dat[i];
                _dipai_list[i] = _id;
                this.card_tf_list[_id].gameObject.SetActive(true);
            }
            SetDiPaiPosition(this.card_tf_list);
        }
    }
}



