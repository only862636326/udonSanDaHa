
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
        RaycastHit _hit_history;
        public float _front_len = 0.2f;
        LineRenderer _line_renderer;
        SDH_CardTile _udonBehaviour;
        public const int CARD_BOX_LAYER = 13;
        public bool _is_right_hand = false;
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
            HitCheck();
        }

        void HitCheck()
        {
            var head_p = this.transform.position;
            var head_r = this.transform.rotation;

            //  向前打shot
            var forward = head_r * Vector3.forward;
            _line_renderer.SetPosition(0, head_p);
            _line_renderer.SetPosition(1, head_p + forward * _front_len);

            //  检查是否命中
            if (Physics.Raycast(head_p, forward, out RaycastHit hit, this._front_len, 1 << CARD_BOX_LAYER))
            {
                if (_hit_history.collider != hit.collider)
                {
                    //Debug.Log("命中了" + hit.transform.name);
                    if (_udonBehaviour != null)
                    {
                        _udonBehaviour.VrInputExit();
                    }
                    _hit_history = hit;
                    _udonBehaviour = hit.transform.GetComponent<SDH_CardTile>();
                    if (_udonBehaviour != null)
                    {
                        _udonBehaviour.VrInputEnter();
                    }
                }
            }
            else
            {
                _hit_history = default;
                if (_udonBehaviour != null)
                {
                    _udonBehaviour.VrInputExit();
                    _udonBehaviour = null;
                }
            }
        }

        void HitToggle()
        {

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

                if (this._is_right_hand)
                {
#if UNITY_EDITOR
                    var p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
                    var r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
#else
                    var p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
                    var r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation;
#endif

                    // 计算相对于手部的本地位置偏移（类似子物体的本地坐标）
                    this._position_offset = Quaternion.Inverse(r) * (this.transform.position - p);
                    // 计算相对于手部的本地旋转偏移
                    this._rotation_offset = Quaternion.Inverse(r) * this.transform.rotation;
                }
                else
                {
                    var p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
                    var r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).rotation;

                    // 计算相对于手部的本地位置偏移（类似子物体的本地坐标）
                    this._position_offset = Quaternion.Inverse(r) * (this.transform.position - p);
                    // 计算相对于手部的本地旋转偏移
                    this._rotation_offset = Quaternion.Inverse(r) * this.transform.rotation;
                }
                this.GetComponent<VRCPickup>().Drop();
                this.GetComponent<VRCPickup>().pickupable = false;
            }
            Debug.Log($"-------------DecTimeUser,   {_interact_times}");
            _interact_times = 0;
            _is_use_check_task = false;
        }

        public override void OnPickupUseUp()
        {
            _interact_times += 1;
            //Debug.Log($"-------------Interact {_interact_times}");
            if (!_is_use_check_task)
            {
                _is_use_check_task = true;
                this.SendCustomEventDelayedSeconds(nameof(DecTimeUser), 1.0f);
            }
        }
        private Transform _toggle_hit;
        public override void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (_udonBehaviour != null)
            {
                if (!value)
                {
                    return;
                }

                if (this._is_right_hand && (args.handType == HandType.RIGHT))
                {
                    _udonBehaviour.VrInputTrg();
                    return;
                }

                if (!this._is_right_hand && (args.handType == HandType.LEFT))
                {
                    _udonBehaviour.VrInputTrg();
                    return;
                }
                return;
            }

            var head_p = this.transform.position;
            var head_r = this.transform.rotation;

            //  向前打shot
            var forward = head_r * Vector3.forward;

            //  检查是否命中
            if (Physics.Raycast(head_p, forward, out RaycastHit hit, this._front_len, 1 << (CARD_BOX_LAYER + 1)))
            {
                if (hit.transform.name.StartsWith("ToggleEvn_"))
                {
                    var toggle = hit.transform.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = !toggle.isOn;
                        _toggle_hit = toggle.transform;
                    }
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
                    this.SendCustomEventDelayedSeconds(nameof(DecTimeGrab), 1.0f);
            }
        }
    }
}

















