using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
namespace ColorEnemies
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private static float hue;
        private static Color color = Color.black;

        private static readonly int shader_GlowColorRed = Shader.PropertyToID("_GlowColorRed");
        private static readonly int shader_GlowColor = Shader.PropertyToID("_GlowColor");

        public void Awake()
        {
            Logger = base.Logger;

            Harmony harmony = new(PluginInfo.GUID);
            harmony.PatchAll();
        }

        public void FixedUpdate()
        {
            hue = (float)(Time.fixedTime * 0.5 % 1f);
            color = Color.HSVToRGB(hue, 1, 1);
        }

        private static void ColorEnemy(EnemyMaterial material, Color glowColorRed, Color glowColor)
        {
            material.SetColor(shader_GlowColorRed, glowColorRed);
            material.SetColor(shader_GlowColor, glowColor);
        }

        [HarmonyPatch]
        public class BaseEnemyUpdatePatch
        {
            public static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(BaseEnemy), "FixedUpdate");
            }

            public static void Prefix(BaseEnemy __instance)
            {
                ColorEnemy(__instance.mat, color, color);
            }
        }

        [HarmonyPatch(typeof(Body), "Update")]
        public class BodyUpdatePatch
        {
            public static void Prefix(Body __instance)
            {
                ColorEnemy(__instance.mat, color, color);
            }
        }
    }
}
