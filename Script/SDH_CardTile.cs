
using Cysharp.Threading.Tasks.Triggers;
using SGS;
using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class SDH_CardTile : UdonSharpBehaviour
    {

        public int card_id;
        public int owner_id = -1;
        private int hand_idx = -1;
        private bool _is_init = false;

        private bool is_clickable = false;

        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // 自己在prt 的child位置作为自己的id
            card_id = transform.GetSiblingIndex();
            // Debug.Log($"SDH_CardTile: Init: 自己的id为{card_id}, {transform.name}");
        }

        public HopeTools.HopeUdonFramework hugf;
        public object eventData;

        private BoxCollider _box;
        public bool IsSelectable
        {
            get { return is_clickable; }
            set
            {
                is_clickable = value;
                _card_p1 = 0;
                UpdateCardPosition(0);
                if (_box == null)
                {
                    _box = GetComponent<BoxCollider>();
                }
                _box.enabled = is_clickable;
            }
        }

        private int _card_p1;

        private Transform _child_tf;
        private Vector3 _org_p;

        public void SetCardP_x(int x)
        {
            this._card_p1 = x;
        }

        SDH_Input sdh_input;
        public void HufgIocGet()
        {
            //var p = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
            sdh_input = (SDH_Input)hugf.udonIoc.GetServiceUdon(nameof(SDH_Input));
        }

        private void OnMouseDown()
        {
            if (!IsSelectable)
                return;

            if (_card_p1 == CARD_POS_SELECT)
            {
                sdh_input.ToggleEvn_UnselecCard(this.card_id);
                //    _card_p1 = 0;
            }
            else if (_card_p1 == CARD_POS_UNSELEC)
            {
                sdh_input.ToggleEvn_SelecCard(this.card_id);
                //    _card_p1 = 2;
            }
        }

        public const int CARD_POS_UNSELEC = 0;
        public const int CARD_POS_HOVER = 1;
        public const int CARD_POS_SELECT = 2;

        public void UpdateCardPosition(int _p)
        {
            if (_child_tf == null)
            {
                _child_tf = transform.GetChild(0);
            }
            var p = this.transform.position + this.transform.up * 0.005f * _p;
            _child_tf.position = p;
            if (_p != CARD_POS_HOVER)
            {
                this._card_p1 = _p;
            }
        }

        private void OnMouseEnter()
        {
            if (!IsSelectable)
                return;

            if (_card_p1 == CARD_POS_UNSELEC)
            {
                UpdateCardPosition(CARD_POS_HOVER);
            }
        }
        public void OnMouseExit()
        {
            if (!IsSelectable)
                return;
            UpdateCardPosition(_card_p1);
        }
    }
}