using System;
using IllusionPlugin;
using UnityEngine;

namespace CameraMessage
{
    public class CameraMessageIPlugin : IPlugin
    {
        string IPlugin.Name => "Camera Message";

        string IPlugin.Version => "1.3.0.0";

        void IPlugin.OnApplicationQuit()
        {
        }

        void IPlugin.OnApplicationStart()
        {

        }

        void IPlugin.OnFixedUpdate()
        {

        }

        void IPlugin.OnLevelWasInitialized(int level)
        {

        }

        void IPlugin.OnLevelWasLoaded(int level)
        {

            //Console.WriteLine("on level was loaded");

            GameObject gameObject = GameObject.Find("CommonSpace");
            
            bool flag = false;
            CameraMessageBase[] componentsInChildren = gameObject.GetComponentsInChildren<CameraMessageBase>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                CameraMessageBase cameraMessageBase = componentsInChildren[i];
                flag = true;
                cameraMessageBase.InitializeCaches();
            }
            if (!flag)
            {
            
                CameraMessageBase cameraMessageBase = gameObject.AddComponent<CameraMessageBase>();

                if (CameraMessageBase.studioneocam == null)
                {
                    //getCameraV2();
                    //getCameraBase();
                    Helpers.GetCameraStudioNeo();
                }


                cameraMessageBase.InitializeCaches();

                PluginUserSettings userSettings = SaveFileManager.LoadUserSettingsFromFile();
                CameraMessageBase.textSpeed = userSettings.Textspeed;
                CameraMessageBase.textSpeedString = CameraMessageBase.textSpeed.ToString().Substring(CameraMessageBase.textSpeed.ToString().Length-1);
                CameraMessageBase.messageDelay = userSettings.MessageDelay.ToString();

                //Console.WriteLine("loaded textSpeed: '" + userSettings.Textspeed + "'");

            }

            

        }

        void IPlugin.OnUpdate()
        {
            
        }
    }
}
