using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using NexusRoles.API;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NexusTeams
{
    [BepInPlugin("com.stanis12.nexusteams", "NexusTeams", "1.0.0")]
    [BepInDependency("com.theohay.nuageroles")]
    public sealed class NexusTeamsPlugin : BaseUnityPlugin
    {
        internal static readonly Dictionary<string, TeamRole> Roles = new Dictionary<string, TeamRole>();
        internal static ConfigEntry<KeyCode> ToggleKey = null!;
        internal static ConfigEntry<bool> StartVisible = null!;
        internal static ConfigEntry<bool> ShowLocalPlayer = null!;
        internal static ConfigEntry<float> PanelScale = null!;
        internal static bool Visible;
        private Harmony harmony = null!;

        private void Awake()
        {
            ToggleKey = Config.Bind("NexusTeams", "Toggle key", KeyCode.F8, "Key to show or hide the teammate roles panel.");
            StartVisible = Config.Bind("NexusTeams", "Show at level start", true, "Show the panel when a level begins.");
            ShowLocalPlayer = Config.Bind("NexusTeams", "Show local player", false, "Include your own role in the panel.");
            PanelScale = Config.Bind("NexusTeams", "Panel scale", 1f, new ConfigDescription("Panel size multiplier.", new AcceptableValueRange<float>(0.75f, 1.5f)));
            harmony = new Harmony("com.stanis12.nexusteams");
            harmony.PatchAll();
            NexusRolesAPI.RoleAssigned += OnRoleAssigned;
            SceneManager.sceneLoaded += OnSceneLoaded;
            gameObject.AddComponent<TeamRolePanel>();
            Logger.LogInfo("NexusTeams loaded (Thunderstore team: nexi).");
        }

        private void OnRoleAssigned(object? sender, RoleAssignedEventArgs args)
        {
            if (!SemiFunc.RunIsLevel() || SemiFunc.RunIsShop()) return;
            PlayerAvatar? avatar = PlayerAvatar.instance;
            if (avatar == null) return;
            string id = SemiFunc.PlayerGetSteamID(avatar);
            string name = SemiFunc.PlayerGetName(avatar);
            ApplyRole(id, name, args.RoleId);
            if (PhotonNetwork.InRoom && avatar.photonView != null)
                avatar.photonView.RPC(nameof(RoleRelay.ReceiveRole), RpcTarget.AllBuffered, id, name, args.RoleId);
        }

        internal static void ApplyRole(string id, string name, int roleId)
        {
            string[] names = { "", "Runner", "Tank", "Gambler", "Strongman", "Ranged Looter", "Athletic", "Mage", "Reaper", "Scout", "Regular", "Engineer", "Acrobat", "Savior" };
            if (!string.IsNullOrEmpty(id) && roleId > 0 && roleId < names.Length)
                Roles[id] = new TeamRole(id, name, names[roleId]);
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            Roles.Clear();
            Visible = StartVisible.Value;
        }

        private void OnDestroy()
        {
            NexusRolesAPI.RoleAssigned -= OnRoleAssigned;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            harmony?.UnpatchSelf();
        }
    }

    internal sealed class TeamRole
    {
        internal readonly string Id, Name, Role;
        internal TeamRole(string id, string name, string role) { Id = id; Name = name; Role = role; }
    }

    internal sealed class RoleRelay : MonoBehaviour
    {
        [PunRPC] public void ReceiveRole(string id, string name, int roleId) => NexusTeamsPlugin.ApplyRole(id, name, roleId);
    }

    [HarmonyPatch(typeof(PlayerAvatar), "Awake")]
    internal static class AvatarRelayPatch
    {
        private static void Postfix(PlayerAvatar __instance)
        {
            if (__instance != null && __instance.gameObject.GetComponent<RoleRelay>() == null)
                __instance.gameObject.AddComponent<RoleRelay>();
        }
    }
}
