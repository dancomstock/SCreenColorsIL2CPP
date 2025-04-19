using System.Collections;
using MelonLoader;
using SCreenSignCommandsIL2CPP;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(SCreenColorsIL2CPP.Core), "SCreenColorsIL2CPP", "1.0.0", "animandan", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SCreenColorsIL2CPP
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            //SignCommand signColors = new SignCommand("/colors", "sets colors for signs", signcolor);
            //MelonMod.GetModInstance<SCreenSignCommandsIL2CPP.Core>();
            Melon<SCreenSignCommandsIL2CPP.Core>.Instance.register_command("/color", "Choose Sign Colors", signcolor);
        }

        public async Task signcolor(Il2CppScheduleOne.EntityFramework.LabelledSurfaceItem instance)
        {
            Melon<Core>.Logger.Msg($"Inside {instance} sign color");
            //Melon<SCreenSignCommandsIL2CPP.Core>.Instance.runningLabels[instance.GetInstanceID()] = "locked";
            string result = "";
            //try
            //{
                var args = SignCommand.getArgs(instance);
                if (args.ContainsKey("signcolor"))
                {
                    var material = instance.transform.parent.gameObject.GetComponentInChildren<MeshRenderer>().material;
                    string[] arg = args["signcolor"].Split(",");
                    if (arg[0] == "rgb")
                    {
                        instance.Label.text = args.GetValueOrDefault("remaining_text", "");
                        //Color color = instance.transform.parent.gameObject.GetComponentInChildren<MeshRenderer>().material.color;
                        //instance.transform.parent.gameObject.GetComponentInChildren<MeshRenderer>().material.color = rgb_cycle(instance.transform.parent.gameObject.GetComponentInChildren<MeshRenderer>().material.color);
                        while (instance.Message != "stop")
                        {
                            //await Task.Yield();
                            await Task.Delay(1000);
                            material.color = rgb_cycle(material.color);
                        }
                        
                    }
                    else 
                    {
                        float r = float.Parse(arg[0]) / 255f;
                        float g = float.Parse(arg[1]) / 255f;
                        float b = float.Parse(arg[2]) / 255f;
                        float a = float.Parse(arg[3]);
                        //instance.transform.parent.gameObject.GetComponentInChildren<MeshRenderer>().material.color = new Color(r, g, b, a);
                        material.color = new Color(r, g, b, a);
                    }
                }

                if (args.ContainsKey("textcolor"))
                {
                    string[] arg = args["textcolor"].Split(",");
                    if (arg[0] == "rgb")
                    {
                        instance.Label.color = rgb_cycle(instance.Label.color);
                    }
                    else
                    {
                        float r = float.Parse(arg[0]) / 255f;
                        float g = float.Parse(arg[1]) / 255f;
                        float b = float.Parse(arg[2]) / 255f;
                        float a = float.Parse(arg[3]);
                        instance.Label.color = new Color(r, g, b, a);
                    }

                }
                result = args.GetValueOrDefault("remaining_text", "");
            //}
            //catch (System.Exception ex)
            //{
            //    Melon<Core>.Logger.Msg($"error: {ex}");
            //    result = ex.ToString();
            //}
            instance.Label.text = result;
            //Melon<SCreenSignCommandsIL2CPP.Core>.Instance.runningLabels[instance.GetInstanceID()] = "unlocked";
            //yield return result;
        }

        public Color rgb_cycle(Color currentColor)
        {
            //adapted from Madgvox's post here: https://discussions.unity.com/t/color-cycle-script-need-help/832829/4
            float H, S, V;
            Color.RGBToHSV(currentColor, out H, out S, out V);
            S = 0.5f;
            V = 0.5f;
            H += 1f + Time.deltaTime;
            if (H > 1f)
            {
                // we subtract 1 instead of setting to zero to preserve the tiny value that overshoots 1
                // for eg. if currentHue is 1.014, this will make it 0.014. This makes more of a difference
                //   than you might think!
                H -= 1f;
            }
            return Color.HSVToRGB(H, S, V);
        }
    }
}