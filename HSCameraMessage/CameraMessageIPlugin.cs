using System;
using IllusionPlugin;
using UnityEngine;

namespace CameraMessage
{
    public class CameraMessageIPlugin : IPlugin
    {
        string IPlugin.Name => "Camera Message";

        string IPlugin.Version => "1.1.0.0";

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
                CameraMessageBase movieMaker = componentsInChildren[i];
                flag = true;
            }
            if (!flag)
            {
            
                CameraMessageBase cameraMessagePlugin = gameObject.AddComponent<CameraMessageBase>();

                if (CameraMessageBase.studioneocam == null)
                {
                    //getCameraV2();
                    //getCameraBase();
                    cameraMessagePlugin.GetCameraStudioNeo();
                }
            }

        }

        void IPlugin.OnUpdate()
        {
            
        }
    }
}
