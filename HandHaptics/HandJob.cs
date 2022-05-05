using MelonLoader;
using PlagueButtonAPI;
using PlagueButtonAPI.External_Libraries;
using PlagueButtonAPI.Misc;
using PlagueButtonAPI.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.SDKBase;

[assembly: MelonInfo(typeof(HandHaptics.HandJob), "HandHaptics", "1.0", "Iris")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace HandHaptics
{
    public class HandJob : MelonMod
    {
        public static ConfigLib<Configuration> Config = new ConfigLib<Configuration>(Environment.CurrentDirectory + "\\HandJob.json");

        public class Configuration
        {
            public bool Touchies { get; set; } = true;
            public bool CanITouchMyselfMistress { get; set; } = true;
        }

        private float FPS = 0;

        public override void OnUpdate()
        {
            FPS = (int)(1f / Time.smoothDeltaTime);

            if (!Config.InternalConfig.Touchies) // To Do: Headpat Vibin'
            {
                return;
            }

            var players = PlayerManager.prop_PlayerManager_0?.GetPlayers().ToList();

            if (players == null || players.Count == 0 || Player.prop_Player_0 == null)
            {
                return;
            }

            var animator = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCAvatarManager_0.field_Private_Animator_0;

            if (animator == null)
            {
                return;
            }

            var lefthand = animator.GetBoneTransform(UnityEngine.HumanBodyBones.LeftMiddleProximal);
            var righthand = animator.GetBoneTransform(UnityEngine.HumanBodyBones.RightMiddleProximal);

            if (lefthand == null || righthand == null)
            {
                return;
            }

            foreach (var p in players)
            {
                if (p == null || (!Config.InternalConfig.CanITouchMyselfMistress && p == Player.prop_Player_0))
                {
                    continue;
                }

                var playeranim = p._vrcplayer.prop_VRCAvatarManager_0.field_Private_Animator_0;

                if (playeranim == null)
                {
                    continue;
                }

                var playerlefthand = playeranim.GetBoneTransform(UnityEngine.HumanBodyBones.LeftMiddleProximal);
                var playerrighthand = playeranim.GetBoneTransform(UnityEngine.HumanBodyBones.RightMiddleProximal);

                if (playerlefthand == null || playerrighthand == null)
                {
                    continue;
                }

                var DistanceLeftToLeft = Vector3.Distance(playerlefthand.transform.position, lefthand.transform.position);
                var DistanceRightToLeft = Vector3.Distance(playerrighthand.transform.position, lefthand.transform.position);

                var IsVibin = false;

                if ((p != Player.prop_Player_0 && DistanceLeftToLeft <= 0.05f) || DistanceRightToLeft <= 0.05f)
                {
                    IsVibin = true;
                    VRCPlayer.field_Internal_Static_VRCPlayer_0?.field_Private_VRCPlayerApi_0?.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, FPS / 1000f, 0.05f, 0.5f);
                }

                var DistanceLeftToRight = Vector3.Distance(playerlefthand.transform.position, righthand.transform.position);
                var DistanceRightToRight = Vector3.Distance(playerrighthand.transform.position, righthand.transform.position);

                if (DistanceLeftToRight <= 0.05f || (p != Player.prop_Player_0 && DistanceRightToRight <= 0.05f))
                {
                    IsVibin = true;
                    VRCPlayer.field_Internal_Static_VRCPlayer_0?.field_Private_VRCPlayerApi_0?.PlayHapticEventInHand(VRC_Pickup.PickupHand.Right, FPS / 1000f, 0.05f, 0.5f);
                }

                if (IsVibin)
                {
                    break;
                }
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "ui")
            {
                ButtonAPI.OnInit += () =>
                {
                    var page = MenuPage.CreatePage(PlagueButtonAPI.Controls.WingSingleButton.Wing.Both, null, "HandJob", "HandHaptics", Gridify: true);

                    page.Item1.AddToggleButton("Touchies", "Enable Touchies", "Disable Touchies", (val) =>
                    {
                        Config.InternalConfig.Touchies = val;
                    }, Config.InternalConfig.Touchies);

                    page.Item1.AddToggleButton("Self Touchies", "Enable Self Touchies", "Disable Self Touchies", (val) =>
                    {
                        Config.InternalConfig.CanITouchMyselfMistress = val;
                    }, Config.InternalConfig.CanITouchMyselfMistress);
                };
            }
        }
    }
}
