using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CameraMessage
{
    public class SaveFileManager
    {
        public void SaveToFile(Dictionary<string, CameraPositionAndMessage> cameraDictionary, string saveFileName)
        {
            // gather all info from plugin and create csv

            /// csv structure:
            ///         string CameraName (C1, C2 etc.)
            ///         Vector3 TargetPos  
            ///         Vector3 CameraDir  
            ///         Vector3 CameraAngle  
            ///         float CameraFov  
            ///         string Text  

            StringBuilder sb = new StringBuilder();

            foreach (string key in cameraDictionary.Keys)
            {
                sb.Append(key);
                sb.Append(';');
                sb.Append(cameraDictionary[key].TargetPos);
                sb.Append(';');
                sb.Append(cameraDictionary[key].CameraDir);
                sb.Append(';');
                sb.Append(cameraDictionary[key].CameraAngle);
                sb.Append(';');
                sb.Append(cameraDictionary[key].CameraFov);
                sb.Append(';');

                // we have to encode newline, otherwise it messes with the CSV
                string encodedText = cameraDictionary[key].Text.Replace('\n', '§');

                sb.Append(encodedText);
                sb.Append(';');
                sb.Append(Environment.NewLine);
            }

            // then call WriteToFile with csv text
            WriteFile(sb.ToString(), saveFileName);
        }

        private void WriteFile(string csvText, string savename)
        {
            if (!Directory.Exists(UserData.Path + "cameramessage"))
            {
                Directory.CreateDirectory(UserData.Path + "cameramessage");
            }

            string filePathAndName = UserData.Path + "cameramessage/" + savename + "-" + UnityEngine.Random.Range(123456, 99999999) + ".csv";
            File.WriteAllText(filePathAndName, csvText);
            //Console.WriteLine("Saved a file to: " + filePathAndName);
        }

        public Dictionary<string, CameraPositionAndMessage> LoadFile(string fileName)
        {
            

            Dictionary<string, CameraPositionAndMessage> cameraPositionAndMessageDictionary = new Dictionary<string, CameraPositionAndMessage>();

            string filetext = File.ReadAllText(UserData.Path + "cameramessage/" + fileName + ".csv");
            string[] cameraMessageArray = filetext.Split('\n');

            try
            {

                foreach (string cameraMessage in cameraMessageArray)
            {
                string[] cameraSetting = cameraMessage.Trim().Split(';');

                    /// csv structure:
                    ///         string CameraName (C1, C2 etc.)
                    ///         Vector3 TargetPos  
                    ///         Vector3 CameraDir  
                    ///         Vector3 CameraAngle  
                    ///         float CameraFov  
                    ///         string Text  

                    if (cameraSetting.Length >= 6)
                    {
                        string cameraName = cameraSetting[0];
                        Vector3 targetPos = StringToVector3(cameraSetting[1]);
                        Vector3 cameraDir = StringToVector3(cameraSetting[2]);
                        Vector3 cameraAngle = StringToVector3(cameraSetting[3]);
                        float cameraFov = 0f;
                        float.TryParse(cameraSetting[4], out cameraFov);
                        string message = cameraSetting[5];

                        // we have to encode newline, otherwise it messes with the CSV
                        string decodedText = message.Replace('§','\n');

                        CameraPositionAndMessage cameraPositionAndMessage = new CameraPositionAndMessage(decodedText, targetPos, cameraDir, cameraAngle, cameraFov);

                        cameraPositionAndMessageDictionary.Add(cameraName, cameraPositionAndMessage);
                    }
            }

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message + " " + e.StackTrace);
            }

            return cameraPositionAndMessageDictionary;
        }

        private static Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }

    }
}
