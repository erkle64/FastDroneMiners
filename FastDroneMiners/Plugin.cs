using C3.ModKit;
using HarmonyLib;
using System;
using Unfoundry;
using UnityEngine;

namespace FastDroneMiners
{
    [UnfoundryMod(GUID)]
    public class Plugin : UnfoundryPlugin
    {
        public const string
            MODNAME = "FastDroneMiners",
            AUTHOR = "erkle64",
            GUID = AUTHOR + "." + MODNAME,
            VERSION = "0.1.0";

        public static LogSource log;

        public static TypedConfigEntry<float> miningSpeedMultiplier;
        public static TypedConfigEntry<float> capacityMultiplier;
        public static TypedConfigEntry<float> chargeMultiplier;

        public Plugin()
        {
            log = new LogSource(MODNAME);

            new Config(GUID)
                .Group("Multipliers")
                    .Entry(out miningSpeedMultiplier, "miningSpeedMultiplier", 4.0f, true, "Multiplies the mining speed of the drones.")
                    .Entry(out capacityMultiplier, "capacityMultiplier", 2.0f, true, "Multiplies the ore capacity of the drones.")
                    .Entry(out chargeMultiplier, "chargeMultiplier", 1.0f, true, "Multiplies the energy required to charge each drones.")
                .EndGroup()
                .Load()
                .Save();
        }

        public override void Load(Mod mod)
        {
            log.Log($"Loading {MODNAME}");
        }

        [HarmonyPatch]
        public static class Patch
        {
            [HarmonyPatch(typeof(BuildableObjectTemplate), nameof(BuildableObjectTemplate.onLoad))]
            [HarmonyPrefix]
            public static void BuildableObjectTemplateOnLoad(BuildableObjectTemplate __instance)
            {
                if (__instance.identifier.StartsWith("_base_drone_miner_"))
                {
                    var miningSpeedMultiplier = Plugin.miningSpeedMultiplier.Get();
                    if (miningSpeedMultiplier > 0.0f && miningSpeedMultiplier != 1.0f)
                    {
                        var oldValue = Convert.ToSingle(__instance.droneMiner_miningSpeed_str, System.Globalization.CultureInfo.InvariantCulture);
                        var newValue = oldValue * miningSpeedMultiplier;
                        log.LogFormat("Changing mining speed of {0} from {1} to {2}", __instance.identifier, oldValue, newValue);
                        __instance.droneMiner_miningSpeed_str = newValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }

                    var chargeMultiplier = Plugin.chargeMultiplier.Get();
                    if (chargeMultiplier > 0.0f && chargeMultiplier != 1.0f)
                    {
                        var oldValue = Convert.ToSingle(__instance.droneMiner_droneCharge_str, System.Globalization.CultureInfo.InvariantCulture);
                        var newValue = oldValue * chargeMultiplier;
                        log.LogFormat("Changing drone charge of {0} from {1} to {2}", __instance.identifier, oldValue, newValue);
                        __instance.droneMiner_droneCharge_str = newValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }

                    var capacityMultiplier = Plugin.capacityMultiplier.Get();
                    if (capacityMultiplier > 0.0f && capacityMultiplier != 1.0f)
                    {
                        var oldValue = __instance.droneMiner_itemCapacityPerDrone;
                        var newValue = Mathf.CeilToInt(oldValue * capacityMultiplier);
                        log.LogFormat("Changing drone capacity of {0} from {1} to {2}", __instance.identifier, oldValue, newValue);
                        __instance.droneMiner_itemCapacityPerDrone = newValue;
                    }
                }
            }
        }
    }
}


