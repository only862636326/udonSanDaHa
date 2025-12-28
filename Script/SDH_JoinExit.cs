
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeSDH
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SDH_JoinExit : UdonSharpBehaviour
    {
        [HideInInspector] public int MAX_PLAYER = 4;
        public const int PLAYER_NONE = -1;
        [UdonSynced] public int[] player_list_syn;
        private int[] player_list_loc;

        private GameObject[] but_join_list;
        private GameObject[] but_exit_list;
        private Text[] text_name_list;

        void Start()
        {
            Init();
        }

        private bool _is_init = false;
        public void Init()
        {
            // 如果已经初始化，则直接返回
            if (this._is_init)
                return;
            this._is_init = true;

            MAX_PLAYER = this.transform.childCount;

            player_list_syn = new int[MAX_PLAYER];
            player_list_loc = new int[MAX_PLAYER];

            for (int i = 0; i < MAX_PLAYER; i++)
            {
                player_list_syn[i] = PLAYER_NONE;
                player_list_loc[i] = PLAYER_NONE;
            }

            but_join_list = new GameObject[MAX_PLAYER];
            but_exit_list = new GameObject[MAX_PLAYER];
            text_name_list = new Text[MAX_PLAYER];

            var n = this.transform.childCount;
            for (int i = 0; i < n; i++)
            {
                var c = this.transform.GetChild(i);
                var toggles = c.GetComponentsInChildren<Toggle>();

                foreach (var g in toggles)
                {
                    if (g.name.Contains("JoinBut") && g.name.Contains("Toggle"))
                    {
                        but_join_list[i] = g.gameObject;
                    }
                    else if (g.name.Contains("ExitBut") && g.name.Contains("Toggle"))
                    {
                        but_exit_list[i] = g.gameObject;
                    }
                }

                var texts = c.GetComponentsInChildren<Text>();
                foreach (var t in texts)
                {
                    if (t.name.Contains("Name") && t.name.Contains("Text"))
                    {
                        text_name_list[i] = t;
                    }
                }
            }
            UiCtr();
        }


        private void Update()
        {
            ;
        }

        public void ToggleEvn_JoinBut(int idx)
        {
            if(idx < 0 || idx >= MAX_PLAYER)
            {
                return;
            }
            var p = Networking.LocalPlayer.playerId;
            if (this.player_list_loc[idx] == PLAYER_NONE)
            {
                this.player_list_loc[idx] = p;
                RequestSyn();
            }
        }
        public void ToggleEvn_ExitBut(int idx)
        {
            if (idx < 0 || idx >= MAX_PLAYER)
            {
                return;
            }
            var p = Networking.LocalPlayer.playerId;
            if (this.player_list_loc[idx] == p)
            {
                this.player_list_loc[idx] = PLAYER_NONE;
                RequestSyn();
            }
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
            for (int i = 0; i < MAX_PLAYER; i++)
            {
                player_list_syn[i] = player_list_loc[i];
            }
            UiCtr();
        }

        public override void OnDeserialization()
        {
            for (int i = 0; i < MAX_PLAYER; i++)
            {
                player_list_loc[i] = player_list_syn[i];
            }
            UiCtr();
        }

        public void UiCtr()
        {
            var n = this.transform.childCount;
            var loc_id = Networking.LocalPlayer.playerId;
            var loc_name = Networking.LocalPlayer.displayName;

            for (int i = 0; i < n; i++)
            {
                if (but_join_list[i] == null || but_exit_list[i] == null || text_name_list[i] == null)
                {
                    continue;
                }
                if (player_list_loc[i] == loc_id)
                {

                    but_join_list[i].SetActive(false);
                    but_exit_list[i].SetActive(true);
                    text_name_list[i].text = loc_name;
                }
                else if (player_list_loc[i] == PLAYER_NONE)
                {
                    but_join_list[i].SetActive(true);
                    but_exit_list[i].SetActive(false);
                    text_name_list[i].text = "";
                }
                else
                {
                    but_join_list[i].SetActive(false);
                    but_exit_list[i].SetActive(false);
                    // get other player name
                    var _name = VRCPlayerApi.GetPlayerById(player_list_loc[i]).displayName;
                    text_name_list[i].text = _name;
                }
            }
        }


        public void ToggleEvn_JoinBut_0() { ToggleEvn_JoinBut(0); }
        public void ToggleEvn_ExitBut_0() { ToggleEvn_ExitBut(0); }
        public void ToggleEvn_JoinBut_1() { ToggleEvn_JoinBut(1); }
        public void ToggleEvn_ExitBut_1() { ToggleEvn_ExitBut(1); }
        public void ToggleEvn_JoinBut_2() { ToggleEvn_JoinBut(2); }
        public void ToggleEvn_ExitBut_2() { ToggleEvn_ExitBut(2); }
        public void ToggleEvn_JoinBut_3() { ToggleEvn_JoinBut(3); }
        public void ToggleEvn_ExitBut_3() { ToggleEvn_ExitBut(3); }
        // end method
    }
}