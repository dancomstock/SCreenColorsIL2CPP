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
            Melon<SCreenSignCommandsIL2CPP.Core>.Instance.register_command("/color", "Choose Sign Colors", signcolor);
        }

        public IEnumerator signcolor(Il2CppScheduleOne.EntityFramework.LabelledSurfaceItem instance)
        {
            //Melon<Core>.Logger.Msg($"Inside {instance} sign color");
            yield return null;
            string result = "";
            bool signRgbFlag = false;
            bool textrRgbFlag = false;
            var material = instance.gameObject.GetComponentInChildren<MeshRenderer>().material;
            try
            {
                string message = instance.Message;
                var args = SignCommand.getArgs(instance);
                if (args.ContainsKey("signcolor"))
                {
                    string[] arg = args["signcolor"].Split(",");
                    if (arg[0] == "rgb")
                    {
                        signRgbFlag = true;
                    }
                    else 
                    {
                        float r = float.Parse(arg[0]) / 255f;
                        float g = float.Parse(arg[1]) / 255f;
                        float b = float.Parse(arg[2]) / 255f;
                        float a = float.Parse(arg[3]);
                        material.color = new Color(r, g, b, a);
                    }
                }

                if (args.ContainsKey("textcolor"))
                {
                    string[] arg = args["textcolor"].Split(",");
                    if (arg[0] == "rgb")
                    {
                        textrRgbFlag = true;
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
            }
            catch (System.Exception ex)
            {
                Melon<Core>.Logger.Msg($"error: {ex}");
                result = ex.ToString();
            }
            instance.Label.text = result;
            if (signRgbFlag || textrRgbFlag)
            {
                while (instance.Message.Contains("signcolor=rgb") || instance.Message.Contains("textcolor=rgb"))
                {
                    yield return null;
                    //yield return new WaitForSeconds(Melon<Core>.Instance.waittime);
                    var args = SignCommand.getArgs(instance);
                    instance.Label.text = args.GetValueOrDefault("remaining_text", "");

                    if (instance.Message.Contains("signcolor=rgb"))
                    {
                        material.color = rgb_cycle(material.color);
                    }
                    if (instance.Message.Contains("textcolor=rgb"))
                    {
                        instance.Label.color = rgb_cycle(instance.Label.color);
                    }
                }
            }
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