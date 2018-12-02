using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CameraMessage
{
    public class SaveFileManager
    {
        private const string TEXTSPEED = "textspeed:";
        private const string MESSAGEDELAY = "messagedelay:";


        public static void SaveSettingsToFile(PluginUserSettings pluginUserSettings)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TEXTSPEED);
            sb.Append(pluginUserSettings.Textspeed);
            sb.Append(Environment.NewLine);
            sb.Append(MESSAGEDELAY);
            sb.Append(pluginUserSettings.MessageDelay);

            WriteSettingsToFile(sb.ToString());
        }

        public static void WriteSettingsToFile(string textForFile)
        {
            if (!Directory.Exists(UserData.Path + "cameramessage"))
            {
                Directory.CreateDirectory(UserData.Path + "cameramessage");
            }

            string filePathAndName = UserData.Path + "cameramessage/userSettings.txt";
            File.WriteAllText(filePathAndName, textForFile);
        }

        public static PluginUserSettings LoadUserSettingsFromFile()
        {
            PluginUserSettings userSettings = new PluginUserSettings();

            //default values
            userSettings.Textspeed = 0.04f;
            userSettings.MessageDelay = 8;

            if (File.Exists(UserData.Path + "cameramessage/userSettings.txt"))
            {

                string filetext = File.ReadAllText(UserData.Path + "cameramessage/userSettings.txt");
                string[] settingsMessageArray = filetext.Split('\n');

                foreach (string userSetting in settingsMessageArray)
                {
                    if (userSetting.Contains(TEXTSPEED))
                    {
                        userSettings.Textspeed = float.Parse(userSetting.Substring(TEXTSPEED.Length));
                    }
                    else if (userSetting.Contains(MESSAGEDELAY))
                    {
                        userSettings.MessageDelay = int.Parse(userSetting.Substring(MESSAGEDELAY.Length));
                    }
                }

            }
            return userSettings;
        }

        public void SaveCameraDictionaryToFile(Dictionary<string, CameraPositionAndMessage> cameraDictionary, string saveFileName)
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
                sb.Append(cameraDictionary[key].CameraDistance);
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
            WriteCameraDictionaryToFile(sb.ToString(), saveFileName);
        }

        private void WriteCameraDictionaryToFile(string csvText, string savename)
        {
            if (!Directory.Exists(UserData.Path + "cameramessage"))
            {
                Directory.CreateDirectory(UserData.Path + "cameramessage");
            }

            string filePathAndName = UserData.Path + "cameramessage/" + savename + "-" + UnityEngine.Random.Range(123456, 99999999) + ".csv";
            File.WriteAllText(filePathAndName, csvText);
            //Console.WriteLine("Saved a file to: " + filePathAndName);
        }

        public Dictionary<string, CameraPositionAndMessage> LoadCameraDictionaryFile(string fileName)
        {
            Dictionary<string, CameraPositionAndMessage> cameraPositionAndMessageDictionary = new Dictionary<string, CameraPositionAndMessage>();

            string filetext = File.ReadAllText(UserData.Path + "cameramessage/" + fileName + ".csv");
            string[] cameraMessageArray = filetext.Split('\n');

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
                    string decodedText = message.Replace('§', '\n');

                    CameraPositionAndMessage cameraPositionAndMessage = new CameraPositionAndMessage(decodedText, targetPos, cameraDir, cameraAngle, cameraFov);

                    cameraPositionAndMessageDictionary.Add(cameraName, cameraPositionAndMessage);
                }
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
