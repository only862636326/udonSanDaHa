
using HopeTools;
using System.Drawing;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace HopeSDH
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SDH_FaPaiJi : UdonSharpBehaviour
    {
        private int CHILD_CARD_NUM;
        //public const int SYN_EVN_FAPAI = 1;
        //private int _syn_evn;
        [UdonSynced] private int[] card_list_syn;
        public int[] card_id_list;

        public Transform player_manager_prt;

        [HideInInspector] public Transform[] card_tf_list;

        public string hufg_init_string = "Init|";
        void Start()
        {
            InitRender();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                FaPai();
            }
        }

        private bool _is_init = false;
        public void Init()
        {
            // 如果已经初始化，则直接返回
            if (this._is_init)
                return;
            this._is_init = true;

            this.CHILD_CARD_NUM = this.transform.childCount;
            this.card_tf_list = new Transform[this.CHILD_CARD_NUM];
            for (int i = 0; i < card_tf_list.Length; i++)
            {
                this.card_tf_list[i] = this.transform.GetChild(i);
            }


            card_list_syn = new int[SDH_GameManager.CONST_SDH_TOTAL_CARD_NUM + 10];

            InitCardIdList();
        }


        private void InitCardIdList()
        {
            this.card_id_list = new int[SDH_GameManager.CONST_SDH_TOTAL_CARD_NUM];
            int _idx = 0;
            int card_id = 0;

            for (int icon = 0; icon < 4; icon++)
            {
                for (int i = 0; i < 13; i++)
                {
                    card_id += 2;
                    if (i == 2 || i == 3)
                    {
                        this.card_tf_list[card_id - 1].gameObject.SetActive(false);
                        this.card_tf_list[card_id - 2].gameObject.SetActive(false);
                        continue;
                    }
                    card_id_list[_idx++] = card_id - 1;
                    card_id_list[_idx++] = card_id - 2;
                }
            }
            card_id_list[_idx++] = card_id++;
            card_id_list[_idx++] = card_id++;
            card_id_list[_idx++] = card_id++;
            card_id_list[_idx++] = card_id++;
        }


        private HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public void HugfInit()
        {
            if (hugf == null)
            {
                hugf = GameObject.Find(SDH_GameManager.CONST_SDH_HUGF_STRING).GetComponent<HopeUdonFramework>();
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
            if (hugf == null)
            {
                Debug.LogError("HugfInitAfter failed, hugf is null!");
                return;
            }
            // user code after hugf init here
            hugf.udonEvn.RegisterListener(nameof(SDH_DiPaiManager.FaPaiCall), this);
            hugf.udonIoc.RegisterSingleton(nameof(this.card_tf_list), this, this.card_tf_list);
        }

        // 发牌
        public void FaPai()
        {
            int seed = System.DateTime.Now.Ticks.GetHashCode();
            FisherYatesShuffle(seed);
            hugf.TriggerEventWithData(nameof(SDH_DiPaiManager.FaPaiCall), this.card_id_list);
            RequestSyn();
        }

        private void ClearCardSelect()
        {
            foreach (var card in card_tf_list)
            {
                var udon = card.GetComponent<SDH_CardTile>();
                udon.SetCardP_x(0);
            }
        }

        public void ResetCardList()
        {
            for (int i = 0; i < card_id_list.Length; i++)
            {
                card_id_list[i] = i;
            }
        }

        /// <summary>
        /// Fisher-Yates 洗牌算法
        /// </summary>
        public void FisherYatesShuffle(int seed)
        {
            // 设置随机数种子
            Random.InitState(seed);

            // Fisher-Yates 洗牌算法
            for (int i = 0; i < card_id_list.Length; i++)
            {
                int r = Random.Range(0, card_id_list.Length);
                int temp = card_id_list[i];
                card_id_list[i] = card_id_list[r];
                card_id_list[r] = temp;
            }
        }


        #region syn_code
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
            for (int i = 0; i < card_id_list.Length; i++)
            {
                card_list_syn[i] = card_id_list[i];
            }
        }

        public override void OnDeserialization()
        {
            for (int i = 0; i < card_id_list.Length; i++)
            {
                card_id_list[i] = card_list_syn[i];
            }
        }

        #endregion end syn_code


        #region reander tool


        public const int ICON_MEI = 0;
        public const int ICON_FANG = 1;
        public const int ICON_HONG = 2;
        public const int ICON_HEI = 3;
        public const int ICON_JOKER = 4;
        private bool _is_render_init = false;
        public void InitRender()
        {
            if (this._is_render_init)
                return;
            this._is_render_init = true;

            var _card_id = 0;

            for (int icon = 0; icon < 4; icon++)
            {
                for (int i = 0; i < 13; i++)
                {
                    SetNullShow(_card_id++, icon, i, false);
                    SetNullShow(_card_id++, icon, i, false);
                }
            }

            SetNullShow(_card_id++, ICON_JOKER, 0, false);
            SetNullShow(_card_id++, ICON_JOKER, 0, false);
            SetNullShow(_card_id++, ICON_JOKER, 1, false);
            SetNullShow(_card_id++, ICON_JOKER, 1, false);
        }

        private Renderer _renderer;
        private MaterialPropertyBlock _block;
        public void SetNullShow(int _chird, int icon, int num, bool is_null)
        {
            {
                _renderer = this.transform.GetChild(_chird).GetChild(0).GetComponent<Renderer>();
                this._block = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(_block);
            }

            if (!is_null)
            {
                this._block.SetVector("_MainParams", new Vector4(4 - icon, num, 5, 13));
                this._renderer.SetPropertyBlock(this._block);
            }
            else
            {
                this._block.SetVector("_MainParams", new Vector4(5, 0, 5, 3));
                this._renderer.SetPropertyBlock(this._block);
            }
        }
        #endregion end render tool
    }
}