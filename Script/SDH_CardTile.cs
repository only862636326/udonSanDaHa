
using UdonSharp;
using UnityEngine;
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

        private void OnMouseEnter()
        {
            // hugf.Log($"SDH_CardTile: OnMouseEnter: 鼠标进入{transform.name}");
        }

        public void OnMouseExit()
        {
            // hugf.Log($"SDH_CardTile: OnMouseExit: 鼠标退出{transform.name}");
        }
    }
}