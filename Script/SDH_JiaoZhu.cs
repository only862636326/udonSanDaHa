
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SDH_JiaoZhu : UdonSharpBehaviour
    {
        #region init code
        private bool _is_init = false;
        public GameObject[] _maidi_obj;
        public GameObject[] _outbut_obj;

        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // user code init here
            var n = transform.childCount;
            _maidi_obj =new GameObject[n];
            _outbut_obj = new GameObject[n];
            for (int i = 0; i < n; i++)
            {
                var tf = this.transform.GetChild(i);

                foreach (Transform child in tf)
                {
                    var _low = child.name.ToLower();
                    if (_low.Contains("maidi") && _low.Contains("prt"))
                    {
                        _maidi_obj[i] = child.gameObject;
                    }
                    if (_low.Contains("toggle") && _low.Contains("outbut"))
                    {
                        _outbut_obj[i] = child.gameObject;
                    }
                }
            }
            SDH_GameResetCall();
        }

        [HideInInspector] public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1; // eventData1 is the same as eventData (eventData1 = eventData)
        public object eventData2;

        public void HugfInitAfter()
        {
            // user code after hugf init here
            hugf.udonEvn.RegisterListener(nameof(this.JiaoZhuangFinishCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.MaiDiFinishCall), this);
            hugf.udonEvn.RegisterListener(nameof(this.SDH_GameResetCall), this);
            //hugf.udonIoc.RegisterSingleton(nameof(this.card_tf_list), this, this.card_tf_list);
        }

        public void MaiDiFinishCall()
        {
            var x = (int)this.eventData;
            for (int i = 0; i < _maidi_obj.Length; i++)
            {
                this._maidi_obj[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _outbut_obj.Length; i++)
            {
                this._outbut_obj[i].gameObject.SetActive(true);
            }
        }

        public void JiaoZhuangFinishCall()
        {
            foreach (Transform tf in this.transform)
            {
                tf.gameObject.SetActive(true);
            }

            var x = (int)this.eventData;
            for (int i = 0; i < _maidi_obj.Length; i++)
            {
                this._maidi_obj[i].gameObject.SetActive(i == x);
            }

            for (int i = 0; i < _outbut_obj.Length; i++)
            {
                this._outbut_obj[i].gameObject.SetActive(false);
            }
        }

        public void SDH_GameResetCall()
        {
            foreach (Transform tf in this.transform)
            {
                tf.gameObject.SetActive(false);
            }

            for (int i = 0; i < _maidi_obj.Length; i++)
            {
                this._maidi_obj[i].gameObject.SetActive(false);
            }
        }

        public void HufgIocGet()
        {
            //var p = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }
        #endregion
    }
}




