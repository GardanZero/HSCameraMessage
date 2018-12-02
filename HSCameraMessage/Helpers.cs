using UnityEngine;

namespace CameraMessage
{
    public static class Helpers
    {
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static void GetCameraStudioNeo()
        {
            //Console.WriteLine("GetCameraStudioNeo() entered" );

            if (CameraMessageBase.studioneocam != null)
            {
                // do nothing
            }
            Studio.CameraControl[] array = UnityEngine.Object.FindObjectsOfType<Studio.CameraControl>();
            int num = 0;

            if (num < array.Length)
            {
                CameraMessageBase.studioneocam = array[num];
                //Console.WriteLine("camera 1 found" + studioneocam);
            }
        }

        public static BaseCameraControl getCameraBase()
        {
            //Console.WriteLine("getCameraBase() entered");

            if (CameraMessageBase.basecam != null)
            {
                return CameraMessageBase.basecam;
            }
            BaseCameraControl[] array = UnityEngine.Object.FindObjectsOfType<BaseCameraControl>();
            int num = 0;

            if (num < array.Length)
            {
                BaseCameraControl baseCameraControl = CameraMessageBase.basecam = array[num];
                //Console.WriteLine("camera 2 found" + studioneocam);
            }
            return CameraMessageBase.basecam;
        }




        public static CameraControl_Ver2 getCameraV2()
        {
            if (CameraMessageBase.camv2 != null)
            {
                return CameraMessageBase.camv2;
            }
            CameraControl_Ver2[] array = UnityEngine.Object.FindObjectsOfType<CameraControl_Ver2>();
            int num = 0;

            if (num < array.Length)
            {
                CameraControl_Ver2 cameraControl_Ver = CameraMessageBase.camv2 = array[num];
            }
            return CameraMessageBase.camv2;
        }


    }
}
