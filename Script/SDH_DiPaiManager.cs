
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
        [UdonSynced] private int[] _dipai_list_syn;
        private int _dipai_count;

        [SerializeField] private Transform _dipai_positon_prt;
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
            _dipai_list_syn = new int[_dipai_count];

            foreach (Transform child in this.transform)
            {
                var _n = child.name.ToLower();
                if (_n.Contains("dipai") && (_n.Contains("prt") || _n.Contains("parent")))
                {
                    this._dipai_positon_prt = child;
                    break;
                }
            }
        }

        private HopeTools.HopeUdonFramework hugf;
        public object eventData;
        private bool _is_hugf_init = false;
        public void HugfInit()
        {
            if (hugf == null)
            { 
                hugf = GameObject.Find(SDH_GameManager.CONST_SDH_HUGF_STRING).GetComponent<HopeUdonFramework>();
                if(hugf == null)
                {
                    Debug.LogError("SDH_DiPaiManager HugfInit failed, hugf is null!");
                    return;
                }
                hugf.Init();
                return;
            }    
        }

        public void HugfInitAfter()
        {
            hugf.udonEvn.RegisterListener(nameof(this.FaPaiCall), this);
        }

        public void HufgIocGet()
        {
            card_tf_list = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }

        #endregion init

        public void GrabHandCard(int[] cards)
        {
            var n = cards.Length;
            // cards 最后的作为底牌;
            for (int i = 0; i < _dipai_count; i++)
            {
                _dipai_list[i] = cards[n - 1 - i];
            }
        }

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

        public void FaPaiCall()
        {
            var dat = (int[])this.eventData;
            if (dat == null || dat.Length == 0)
            {
                hugf.udondebug.LogWarning("SDH_DiPaiManager FaPaiCall data is null or empty!");
                return;
            }
            GrabHandCard(dat);
            RequestSyn();
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
            for (int i = 0; i < SDH_GameManager.CONST_DIPAI_CARD_NUM; i++)
            {
                _dipai_list_syn[i] = _dipai_list[i];
            }
            SetDiPaiPosition(card_tf_list);
        }

        public override void OnDeserialization()
        {
            for (int i = 0; i < _dipai_count; i++)
            {
                _dipai_list[i] = _dipai_list_syn[i];
            }
            SetDiPaiPosition(card_tf_list);
        }

        #endregion end syn
    }
}