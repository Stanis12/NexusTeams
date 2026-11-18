using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NexusTeams
{
    internal sealed class TeamRolePanel : MonoBehaviour
    {
        private GUIStyle title = null!;
        private GUIStyle item = null!;
        private GUIStyle hint = null!;
        private Texture2D panel = null!;
        private bool stylesReady;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            panel = new Texture2D(1, 1);
            panel.SetPixel(0, 0, new Color(0.025f, 0.035f, 0.05f, 0.82f)); panel.Apply();
        }

        private void Update()
        {
            if (ChatManager.instance != null && ChatManager.instance.chatActive) return;
            if (Input.GetKeyDown(NexusTeamsPlugin.ToggleKey.Value)) NexusTeamsPlugin.Visible = !NexusTeamsPlugin.Visible;
        }

        private void OnGUI()
        {
            if (!NexusTeamsPlugin.Visible || !SemiFunc.RunIsLevel() || SemiFunc.RunIsShop() || SceneManager.GetActiveScene().name.ToLowerInvariant().Contains("menu")) return;
            EnsureStyles();
            float s = NexusTeamsPlugin.PanelScale.Value, width = 238f * s;
            var rows = NexusTeamsPlugin.Roles.Values.Where(r => NexusTeamsPlugin.ShowLocalPlayer.Value || PlayerAvatar.instance == null || r.Id != SemiFunc.PlayerGetSteamID(PlayerAvatar.instance)).OrderBy(r => r.Name).ToArray();
            float height = (60 + rows.Length * 25) * s;
            Rect rct = new Rect(Screen.width - width - 18 * s, Screen.height * 0.34f, width, height);
            GUI.DrawTexture(rct, panel);
            GUI.Label(new Rect(rct.x + 12*s, rct.y + 7*s, width - 20*s, 25*s), "NEXUS TEAMS", title);
            if (rows.Length == 0) GUI.Label(new Rect(rct.x + 12*s, rct.y + 34*s, width - 20*s, 20*s), "Waiting for roles…", hint);
            for (int i = 0; i < rows.Length; i++) GUI.Label(new Rect(rct.x + 12*s, rct.y + (32 + i*25)*s, width - 20*s, 23*s), rows[i].Name + "  ·  " + rows[i].Role, item);
            GUI.Label(new Rect(rct.x + 12*s, rct.yMax - 19*s, width - 20*s, 16*s), "Toggle: " + NexusTeamsPlugin.ToggleKey.Value, hint);
        }

        private void EnsureStyles()
        {
            if (stylesReady) return;
            title = new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold, normal = { textColor = new Color(0.48f, 0.84f, 1f) } };
            item = new GUIStyle(GUI.skin.label) { fontSize = 14, normal = { textColor = Color.white } };
            hint = new GUIStyle(GUI.skin.label) { fontSize = 11, normal = { textColor = new Color(0.7f, 0.74f, 0.8f) } };
            stylesReady = true;
        }
    }
}
