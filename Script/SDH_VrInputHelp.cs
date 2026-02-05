
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.Udon.Common;

namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SDH_VrInputHelp : UdonSharpBehaviour
    {
        public float _front_len = 0.2f;
        LineRenderer _line_renderer;
        SDH_CardTile _udonBehaviour;
        public const int CARD_BOX_LAYER = 13;
        public bool _is_right_hand = false;
        public Transform _hit_icon_tf;
        public BoxCollider _tar_canvas_box;
        void Start()
        {
#if !UNITY_EDITOR
            if (!Networking.LocalPlayer.IsUserInVR())
            {
                this.gameObject.SetActive(false);
            }
#endif
            _line_renderer = this.GetComponentInChildren<LineRenderer>();
        }

        private bool _task_flag = false;
        void Update()
        {
            if (_is_auto_flow)
            {
                if (this._is_right_hand)
                {
#if UNITY_EDITOR
                    var handData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
#else
                    var handData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand);
#endif
                    // 类似子物体的跟踪效果：先应用手部旋转到位置偏移
                    this.transform.position = handData.position + handData.rotation * this._position_offset;
                    // 直接使用手部旋转与偏移旋转的组合
                    this.transform.rotation = handData.rotation * this._rotation_offset;
                }
                else
                {
                    var handData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand);
                    // 类似子物体的跟踪效果：先应用手部旋转到位置偏移
                    this.transform.position = handData.position + handData.rotation * this._position_offset;
                    // 直接使用手部旋转与偏移旋转的组合
                    this.transform.rotation = handData.rotation * this._rotation_offset;
                }
            }
            if (this._task_flag)
            {
                HitCardCheck();
            }
            else
            {
                HitToggleCheck();
            }

            this._task_flag = !this._task_flag;
        }

        public bool _is_hit_card = false;
        public bool _is_hit_toggle = false;
        private Transform _hit_history_tf;
        void HitCardCheck()
        {
            if (_is_hit_toggle)
            {
                return;
            }
            var head_p = this.transform.position;
            var head_r = this.transform.rotation;

            //  向前打shot
            var forward = head_r * Vector3.forward;
            _line_renderer.SetPosition(0, head_p);

            //  检查是否命中
            if (Physics.Raycast(head_p, forward, out RaycastHit hit, this._front_len, 1 << CARD_BOX_LAYER))
            {
                var _name = hit.transform.name;
                if (_name.StartsWith("CardTile Variant"))
                {
                    if (_hit_history_tf != hit.transform)
                    {
                        //Debug.Log("命中了" + hit.transform.name);
                        if (_udonBehaviour != null)
                        {
                            _udonBehaviour.VrInputExit();
                        }
                        _hit_history_tf = hit.transform;
                        _udonBehaviour = hit.transform.GetComponent<SDH_CardTile>();
                        if (_udonBehaviour != null)
                        {
                            _udonBehaviour.VrInputEnter();
                        }
                    }
                    _is_hit_card = true;
                    _hit_icon_tf.position = hit.point;
                    _line_renderer.SetPosition(1, hit.point);

                    _hit_icon_tf.gameObject.SetActive(true);
                    return;
                }
            }
            _line_renderer.SetPosition(1, head_p + forward * _front_len);
            _hit_icon_tf.gameObject.SetActive(false);
            _is_hit_card = false;
            _hit_history_tf = null;
            if (_udonBehaviour != null)
            {
                _udonBehaviour.VrInputExit();
                _udonBehaviour = null;

            }
        }
    

        private Transform _toggle_hit_history_tf = null;
        void HitToggleCheck()
        {
            if (this._is_hit_card)
            {
                return;
            }

            var head_p = this.transform.position;
            var head_r = this.transform.rotation;

            //  向前打shot
            var forward = head_r * Vector3.forward;
            _line_renderer.SetPosition(0, head_p);

            //  检查是否命中
            if (Physics.Raycast(head_p, forward, out RaycastHit hit, this._front_len, 1 << (CARD_BOX_LAYER + 1)))
            {
                if (hit.transform.name.StartsWith("ToggleEvn_"))
                {
                    if (_toggle_hit_history_tf != hit.transform)
                    {
                        _toggle_hit_history_tf = hit.transform;
                    }
                    this._hit_icon_tf.position = hit.point;
                    this._hit_icon_tf.gameObject.SetActive(true);

                    _line_renderer.SetPosition(1, hit.point);
                    _is_hit_toggle = true;
                    return;
                }
            }

            _line_renderer.SetPosition(1, head_p + forward * _front_len);
            this._is_hit_toggle = false;
            this._hit_icon_tf.gameObject.SetActive(false);
        }


        private bool _is_auto_flow = false;
        private bool _is_use_check_task = false;
        private int _interact_times;
        private Vector3 _position_offset;
        private Quaternion _rotation_offset;

        public void DecTimeUser()
        {
            if (_interact_times >= 3)
            {
                _is_auto_flow = true;

                var _pick = this.GetComponent<VRCPickup>();
                if (_pick == null)
                {
                    return;
                }

                _is_right_hand = (_pick.currentHand == VRCPickup.PickupHand.Right);

                Vector3 p;
                Quaternion r;

                if (this._is_right_hand)
                {
                    p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
                    r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation;
                }
                else
                {
                    p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
                    r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).rotation;
                }

#if UNITY_EDITOR
                p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
                r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
#endif
                // 计算相对于手部的本地位置偏移（类似子物体的本地坐标）
                this._position_offset = Quaternion.Inverse(r) * (this.transform.position - p);
                // 计算相对于手部的本地旋转偏移
                this._rotation_offset = Quaternion.Inverse(r) * this.transform.rotation;
                _pick.Drop();
                _pick.pickupable = false;

                if (_tar_canvas_box != null)
                {
                    _tar_canvas_box.enabled = false;
                }
            }

            //Debug.Log($"-------------DecTimeUser,   {_interact_times}");
            _interact_times = 0;
            _is_use_check_task = false;
        }

        public override void OnPickupUseUp()
        {
            //Debug.Log($"-------------Interact {_interact_times}");
            _interact_times += 1;
            if (!_is_use_check_task)
            {
                _is_use_check_task = true;
                this.SendCustomEventDelayedSeconds(nameof(DecTimeUser), 0.5f);
            }
        }

        public override void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {            
            if (!value) 
                return;

            if (this._is_right_hand && (args.handType == HandType.LEFT))
            {
                return;
            }

            if (!this._is_right_hand && (args.handType == HandType.RIGHT))
            {
                return;
            }


            if (_udonBehaviour != null)
            {
                _udonBehaviour.VrInputTrg();
            }

            if (this._is_hit_toggle)
            {
                var toggle = this._toggle_hit_history_tf.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = !toggle.isOn;
                }
            }
        }


        private int _grab_count = 0;
        private bool _is_grab_check_task = false;
        public void DecTimeGrab()
        {
            if (_grab_count > 3)
            {
                Debug.Log($"-------------DecTimeGrab,   {_grab_count}");
                this.GetComponent<VRCPickup>().pickupable = true;
                this._is_auto_flow = false;
                if(_tar_canvas_box != null)
                {
                    _tar_canvas_box.enabled = true;
                }
            }
            _grab_count = 0;
            _is_grab_check_task = false;
        }

        public override void InputGrab(bool value, UdonInputEventArgs args)
        {
            if (this._is_auto_flow)
            {
                _grab_count++;
                if (!_is_grab_check_task)
                    this.SendCustomEventDelayedSeconds(nameof(DecTimeGrab), 0.5f);
            }
        }
    }
}

















