
using Cysharp.Threading.Tasks.Triggers;
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

        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // 自己在prt 的child位置作为自己的id
            card_id = transform.GetSiblingIndex();
            // Debug.Log($"SDH_CardTile: Init: 自己的id为{card_id}, {transform.name}");
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

        private int _card_p1;

        private Transform _child_tf;
        private Vector3 _org_p;

        public void SetCardP_x(int x)
        {
            this._card_p1 = x;
        }

        private void OnMouseDown()
        {
            if (_card_p1 == 0)
            {
                _card_p1 = 2;
                hugf.TriggerEventWithData(nameof(SDH_OutCartFsm.SelecCardCall), this.card_id);
            }
            else if (_card_p1 == 2)
            {
                _card_p1 = 0;
                hugf.TriggerEventWithData(nameof(SDH_OutCartFsm.UnselecCardCall), this.card_id);
            }
            UpdateCardPosition(_card_p1);
        }

        private void OnMouseDrag()
        {
            
        }

        private void UpdateCardPosition(int _p)
        {
            if (_child_tf == null)
            {
                _child_tf = transform.GetChild(0);
            }
            var p = this._child_tf.up * 0.005f * _p;
            _child_tf.position = this._org_p + p;
        }

        private void OnMouseEnter()
        {
            if (_child_tf == null)
            {
                _child_tf = transform.GetChild(0);
            }

            if (_card_p1 == 0)
            {
                this._org_p = this._child_tf.position;
                UpdateCardPosition(1);
            }
        }
        public void OnMouseExit()
        {
            UpdateCardPosition(_card_p1);
        }
    }
}