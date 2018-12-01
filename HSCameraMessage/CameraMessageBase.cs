using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Studio;
using System.Collections;

namespace CameraMessage
{
    public class CameraMessageBase : MonoBehaviour
    {
        public const int TEXTBOXHEIGHT = 200;
        public const int TEXTBOXWIDTH = 1000;

        public static bool showMenu = false;
        public static bool pluginActive = false;
        public static bool showTextEditor = false;
        public static bool saveDialogVisible = false;
        public static bool loadDialogVisible = false;
        public static bool playButtonPressed = false;
        public static bool reachedEnd = false;
        public static DateTime lastplayedTimeStamp;
        public static bool isPlaying = false;

        public static string messageDelay = "8";
        public static string saveFileName = "Enter Filename here";
        public static string lastButtonPressed = "C1";
        public static string lastPlayedButton = "C0";
        public string messageTextToEdit = "";
        public string currentMessageText = "";
        public static Studio.CameraControl studioneocam = null;
        public static BaseCameraControl basecam = null;
        public static CameraControl_Ver2 camv2 = null;

        public static Dictionary<string, CameraPositionAndMessage> cameraDictionary = new Dictionary<string, CameraPositionAndMessage>();

        public bool showDisplayBox { get; private set; }

        public void TogglePlugin()
        {
            if (pluginActive)
            {
                pluginActive = false;
            }
            else
            {
                showDisplayBox = true;
                pluginActive = true;
            }
        }

        public void ToggleMainMenu()
        {
            if (showMenu)
            {
                showMenu = false;
            }
            else
            {
                showMenu = true;
            }

        }

        public void ToggleTextEditor()
        {
            if (showTextEditor)
            {
                showTextEditor = false;
            }
            else
            {
                showTextEditor = true;
            }
        }

        public bool SetCameraAndMessage(string cameraButtonName)
        {
            //Console.WriteLine("set camera for " + cameraButtonName + " and isplaying: " +playButtonPressed);

            if (cameraDictionary.ContainsKey(cameraButtonName))
            {

                // //Console.WriteLine("Running setcameraAndamessage for "+cameraButtonName);
                currentMessageText = cameraDictionary[cameraButtonName].Text;

                if (studioneocam != null)
                {
                    studioneocam.targetPos = cameraDictionary[cameraButtonName].TargetPos;
                    studioneocam.fieldOfView = cameraDictionary[cameraButtonName].CameraFov;
                    studioneocam.cameraAngle = cameraDictionary[cameraButtonName].CameraAngle;

                    // to set the distance we need this export and import stuff
                    Studio.CameraControl.CameraData cameraData = new Studio.CameraControl.CameraData();
                    cameraData.pos = studioneocam.Export().pos;
                    cameraData.rotate = studioneocam.Export().rotate;
                    cameraData.parse = studioneocam.Export().parse;
                    cameraData.distance = cameraDictionary[cameraButtonName].CameraDistance;
                    studioneocam.Import(cameraData);

                }

                lastButtonPressed = cameraButtonName;
                lastPlayedButton = cameraButtonName;

                return true;
            }
            else
            {
                // the requested camera position was not found
                return false;
            }
        }

        public void OnGUI()
        {

            // show dialog box
            GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.3), TEXTBOXWIDTH + 100, TEXTBOXHEIGHT + 100));

            /// Todo future: make text appear one char at a time

            GUIStyle largeTextStyle = new GUIStyle();
            largeTextStyle.fontSize = 30;
            largeTextStyle.normal.textColor = Color.white;
            largeTextStyle.wordWrap = true;
            largeTextStyle.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 0.0f, 0.1f));

            GUI.skin.textArea.wordWrap = true;

            // this is the display box of the messages
            if (showDisplayBox)
            { 
                GUILayout.TextArea(currentMessageText, largeTextStyle, GUILayout.Height(TEXTBOXHEIGHT), GUILayout.Width(TEXTBOXWIDTH));
            }

            GUILayout.EndArea();

            if (showDisplayBox)
            {

                GUILayout.BeginArea(new Rect((float)(Screen.width / 3)+TEXTBOXWIDTH, (float)(Screen.height / 1.3)+TEXTBOXHEIGHT, 40, 60));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Hide"))
                {
                    showDisplayBox = false;
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();


            }
            if (showTextEditor)
            {
                GUILayout.BeginArea(new Rect((float)(Screen.width / 4), (float)(Screen.height / 3.5), TEXTBOXWIDTH + 100, TEXTBOXHEIGHT + 100));

                GUILayout.BeginVertical(new GUILayoutOption[0]);

                // this is the editor text box
                messageTextToEdit = GUILayout.TextArea(messageTextToEdit, largeTextStyle, GUILayout.Height(TEXTBOXHEIGHT), GUILayout.Width(TEXTBOXWIDTH));

                if (GUILayout.Button("Save"))
                {
                    ToggleTextEditor();

                    if (studioneocam != null)
                    {
                        if (cameraDictionary.ContainsKey(lastButtonPressed))
                        {
                            cameraDictionary.Remove(lastButtonPressed);
                        }

                        Vector3 cameraDistance = new Vector3(studioneocam.Export().distance.x, studioneocam.Export().distance.y, studioneocam.Export().distance.z);
                        cameraDictionary.Add(lastButtonPressed, new CameraPositionAndMessage(messageTextToEdit, studioneocam.targetPos, cameraDistance, studioneocam.cameraAngle, studioneocam.fieldOfView));
                    }
                    else
                    {
                        Console.WriteLine("Error: StudioNeo Camera not found. (null)");
                        // cameraDictionary.Add(lastButtonPressed, new CameraPositionAndMessage(messageTextToEdit, new Vector3(), new Vector3(), new Vector3(), 110));
                    }

                    // debug
                    //Console.WriteLine("Text was saved to " + lastButtonPressed + " and it was " + cameraDictionary[lastButtonPressed].Text);

                }


                if (GUILayout.Button("Cancel"))
                {
                    ToggleTextEditor();
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

            // Menu shown to do stuff
            if (showMenu)
            {
                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 3), (float)(Screen.width / 3), (float)(Screen.height / 2)));

                GUILayout.BeginVertical(new GUILayoutOption[0]);

                if (GUILayout.Button("Edit Text and save camera position"))
                {
                    //Console.WriteLine("Edit Text of " + lastButtonPressed + " was requested.");
                    ToggleMainMenu();
                    if (cameraDictionary.ContainsKey(lastButtonPressed))
                    {
                        messageTextToEdit = cameraDictionary[lastButtonPressed].Text;
                    }
                    ToggleTextEditor();
                }

                if (GUILayout.Button("Save Camera position"))
                {
                    //Console.WriteLine("Save Camera position of " + lastButtonPressed + " was requested.");
                    showMenu = false;

                    if (cameraDictionary.ContainsKey(lastButtonPressed))
                    {
                        messageTextToEdit = cameraDictionary[lastButtonPressed].Text;
                        cameraDictionary.Remove(lastButtonPressed);
                    }
                    else
                    {
                        messageTextToEdit = string.Empty;
                    }

                    Vector3 cameraDistance = new Vector3(studioneocam.Export().distance.x, studioneocam.Export().distance.y, studioneocam.Export().distance.z);
                    cameraDictionary.Add(lastButtonPressed, new CameraPositionAndMessage(messageTextToEdit, studioneocam.targetPos, cameraDistance, studioneocam.cameraAngle, studioneocam.fieldOfView));
                }

                if (GUILayout.Button("Cancel"))
                {
                    //Console.WriteLine("Cancelled menu.");
                    ToggleMainMenu();
                }

                GUILayout.EndVertical();

                GUILayout.EndArea();
            }

            if (playButtonPressed)
            {
                //Console.WriteLine("isplaying?" + isPlaying);

                if (!isPlaying)
                { 
                    StartCoroutine("PlayCamerAsCoroutine");
                }
            }

            if (pluginActive)
            {
                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.9), (float)(Screen.width / 4), (float)(Screen.height / 2)));

                // Menubuttons
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);

                if (GUILayout.Button("Play"))
                {
                    isPlaying = false;

                    if (!playButtonPressed)
                    { 
                        reachedEnd = false;
                        playButtonPressed = true;
                    }
                }

                if (GUILayout.Button("Stop"))
                {
                    playButtonPressed = false;
                    isPlaying = false;
                    reachedEnd = true;
                    lastPlayedButton = "C0";
                    lastButtonPressed = "C1";
                    currentMessageText = "";
                }

                if (GUILayout.Button("Back"))
                {
                    playButtonPressed = false;
                    ShowPreviousCamera();
                }

                if (GUILayout.Button("Next"))
                {
                    playButtonPressed = false;
                    ShowNextCamera();
                }

                if (GUILayout.Button("Save"))
                {
                    saveDialogVisible = true;
                }

                if (GUILayout.Button("Load"))
                {
                    loadDialogVisible = true;
                }

                GUILayout.Label("  Delay between Messages: ");

                messageDelay = GUILayout.TextField(messageDelay, new GUILayoutOption[0]);
                
                GUILayout.EndHorizontal();

                GUILayout.EndArea();


                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75), (float)(Screen.width / 3), (float)(Screen.height / 2)));

                // Camera Buttons below
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);

                AddCamera("C1");
                AddCamera("C2");
                AddCamera("C3");
                AddCamera("C4");
                AddCamera("C5");
                AddCamera("C6");
                AddCamera("C7");
                AddCamera("C8");
                AddCamera("C9");
                AddCamera("C10");
                AddCamera("C11");
                AddCamera("C12");
                AddCamera("C13");
                AddCamera("C14");
                AddCamera("C15");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 20, (float)(Screen.width / 3), (float)(Screen.height / 2)));
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);

                AddCamera("C16");
                AddCamera("C17");
                AddCamera("C18");
                AddCamera("C19");
                AddCamera("C20");
                AddCamera("C21");
                AddCamera("C22");
                AddCamera("C23");
                AddCamera("C24");
                AddCamera("C25");
                AddCamera("C26");
                AddCamera("C27");
                AddCamera("C28");
                AddCamera("C29");
                AddCamera("C30");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 40, (float)(Screen.width / 3), (float)(Screen.height / 2)));
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);

                AddCamera("C31");
                AddCamera("C32");
                AddCamera("C33");
                AddCamera("C34");
                AddCamera("C35");
                AddCamera("C36");
                AddCamera("C37");
                AddCamera("C38");
                AddCamera("C39");
                AddCamera("C40");
                AddCamera("C41");
                AddCamera("C42");
                AddCamera("C43");
                AddCamera("C44");
                AddCamera("C45");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 60, (float)(Screen.width / 3), (float)(Screen.height / 2)));
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);

                AddCamera("C46");
                AddCamera("C47");
                AddCamera("C48");
                AddCamera("C49");
                AddCamera("C50");
                AddCamera("C51");
                AddCamera("C52");
                AddCamera("C53");
                AddCamera("C54");
                AddCamera("C55");
                AddCamera("C56");
                AddCamera("C57");
                AddCamera("C58");
                AddCamera("C59");
                AddCamera("C60");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 80, (float)(Screen.width / 3), (float)(Screen.height / 2)));
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);

                AddCamera("C61");
                AddCamera("C62");
                AddCamera("C63");
                AddCamera("C64");
                AddCamera("C65");
                AddCamera("C66");
                AddCamera("C67");
                AddCamera("C68");
                AddCamera("C69");
                AddCamera("C70");
                AddCamera("C71");
                AddCamera("C72");
                AddCamera("C73");
                AddCamera("C74");
                AddCamera("C75");
            
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 100, (float)(Screen.width / 3), (float)(Screen.height / 2)));
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);

                AddCamera("C76");
                AddCamera("C77");
                AddCamera("C78");
                AddCamera("C79");
                AddCamera("C80");
                AddCamera("C81");
                AddCamera("C82");
                AddCamera("C83");
                AddCamera("C84");
                AddCamera("C85");
                AddCamera("C86");
                AddCamera("C87");
                AddCamera("C88");
                AddCamera("C89");
                AddCamera("C90");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();


            }

            if (saveDialogVisible)
            {
                loadDialogVisible = false;
                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 3), (float)(Screen.width / 3), 120f));
                GUILayout.BeginVertical(new GUIStyle("Box"), (GUILayoutOption[])new GUILayoutOption[0]);
                Vector2 vector = GUILayout.BeginScrollView(default(Vector2), (GUILayoutOption[])new GUILayoutOption[1]
                {
                GUILayout.Height((float)(Screen.height / 3))
                });
                GUILayout.Label("Save Camera and Message Settings", (GUILayoutOption[])new GUILayoutOption[0]);
                string a = GUILayout.TextField(saveFileName, 100, (GUILayoutOption[])new GUILayoutOption[0]);
                if (a != saveFileName)
                {
                    saveFileName = a;
                }
                if (GUILayout.Button("Save", (GUILayoutOption[])new GUILayoutOption[0]) && saveFileName != "Enter Filename here")
                {
                    SaveFileManager sfm = new SaveFileManager();
                    sfm.SaveToFile(cameraDictionary, saveFileName);
                    saveDialogVisible = false;
                }
                if (GUILayout.Button("Cancel", (GUILayoutOption[])new GUILayoutOption[0]))
                {
                    saveDialogVisible = false;
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
            if (loadDialogVisible)
            {
                saveDialogVisible = false;
                GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 3), (float)(Screen.width / 3), (float)(Screen.height / 3)));
                GUILayout.BeginVertical(new GUIStyle("Box"), (GUILayoutOption[])new GUILayoutOption[0]);
                Vector2 vector2 = GUILayout.BeginScrollView(default(Vector2), (GUILayoutOption[])new GUILayoutOption[1]
                {
                GUILayout.Height((float)(Screen.height / 3))
                });
                GUILayout.Label("Load Camera Message Settings ", (GUILayoutOption[])new GUILayoutOption[0]);
                FolderAssist folderAssist = new FolderAssist();
                folderAssist.CreateFolderInfo(UserData.Path + "cameramessage/", "*.csv", true, true);
                foreach (FolderAssist.FileInfo item6 in folderAssist.lstFile)
                {
                    if (GUILayout.Button(item6.FileName, (GUILayoutOption[])new GUILayoutOption[0]))
                    {
                        SaveFileManager sfm = new SaveFileManager();
                        cameraDictionary = sfm.LoadFile(item6.FileName);
                        loadDialogVisible = false;
                    }
                }
                if (GUILayout.Button("Cancel", (GUILayoutOption[])new GUILayoutOption[0]))
                {
                    loadDialogVisible = false;
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

        }

        private bool ShowPreviousCamera()
        {
            string cameraButtonNext = "C" + (int.Parse(lastPlayedButton.Substring(1)) - 1);
            return SetCameraAndMessage(cameraButtonNext);
        }

        IEnumerator PlayCamerAsCoroutine()
        {
            pluginActive = false;
            showMenu = false;
            showTextEditor = false;

            isPlaying = true;
            //Console.WriteLine("reached end: " + reachedEnd);

            do
            {
                lastplayedTimeStamp = DateTime.Now;
                reachedEnd = !ShowNextCamera();
                //Console.WriteLine("reached end " + reachedEnd);
                yield return new WaitForSeconds(float.Parse(messageDelay));
            } while (!reachedEnd);

            if (reachedEnd)
            {
                //Console.WriteLine("setting isplaying false");
                isPlaying = true;
                playButtonPressed = false;
                reachedEnd = true;
            }
        }

        private bool ShowNextCamera()
        {
            string cameraButtonNext = "C" + (int.Parse(lastPlayedButton.Substring(1)) + 1);
            return SetCameraAndMessage(cameraButtonNext);
        }

        private void AddCamera(string cameraName)
        {
            if (GUILayout.Button(cameraName, GetCorrectStyle(cameraName), new GUILayoutOption[0]))
            {
                lastButtonPressed = cameraName;

                //left click
                if (Input.GetMouseButtonUp(0))
                {
                    SetCameraAndMessage(lastButtonPressed);
                }
                // right click
                else if (Input.GetMouseButtonUp(1))
                {
                    ToggleMainMenu();
                }
            }
        }

        private GUIStyle GetCorrectStyle(string cameraName)
        {
            GUIStyle cameraButtonNormalStyle = new GUIStyle(GUI.skin.button);
            cameraButtonNormalStyle.normal.textColor = Color.white;
            cameraButtonNormalStyle.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 0.0f, 0.1f));

            GUIStyle cameraButtonSavedStyle = new GUIStyle(GUI.skin.button);
            cameraButtonSavedStyle.normal.textColor = Color.white;
            cameraButtonSavedStyle.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 1.0f, 0.2f));

            GUIStyle cameraButtonSelectedStyle = new GUIStyle(GUI.skin.button);
            cameraButtonSelectedStyle.normal.textColor = Color.white;
            cameraButtonSelectedStyle.normal.background = MakeTex(600, 1, new Color(0.0f, 1.0f, 0.0f, 0.2f));

            
            if (cameraDictionary.ContainsKey(cameraName) && lastButtonPressed == cameraName)
            {
                return cameraButtonSelectedStyle;
            }

            else if (cameraDictionary.ContainsKey(cameraName))
            {
                return cameraButtonSavedStyle;
            }

            return cameraButtonNormalStyle;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                TogglePlugin();
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.CapsLock))
            {
                ShowNextCamera();
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                ShowPreviousCamera();
            }


            if (studioneocam == null)
            {
                getCameraV2();
                getCameraBase();
                GetCameraStudioNeo();
            }
        }

        public void LateUpdate()
        {
            //if (camv2 == null && basecam == null && studioneocam == null)
            if (studioneocam == null)
            {
                getCameraV2();
                getCameraBase();
                GetCameraStudioNeo();
            }
        }

        public void GetCameraStudioNeo()
        {
            //Console.WriteLine("GetCameraStudioNeo() entered" );

            if (studioneocam != null)
            {
                // do nothing
            }
            Studio.CameraControl[] array = UnityEngine.Object.FindObjectsOfType<Studio.CameraControl>();
            int num = 0;

            if (num < array.Length)
            {
                studioneocam = array[num];
                //Console.WriteLine("camera 1 found" + studioneocam);
            }



        }

        public BaseCameraControl getCameraBase()
        {
            //Console.WriteLine("getCameraBase() entered");

            if (basecam != null)
            {
                return basecam;
            }
            BaseCameraControl[] array = UnityEngine.Object.FindObjectsOfType<BaseCameraControl>();
            int num = 0;

            if (num < array.Length)
            {
                BaseCameraControl baseCameraControl = basecam = array[num];
                //Console.WriteLine("camera 2 found" + studioneocam);
            }
            return basecam;
        }


        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }


        public CameraControl_Ver2 getCameraV2()
        {
            //Console.WriteLine("getCameraV2() entered");

            if (camv2 != null)
            {
                return camv2;
            }
            CameraControl_Ver2[] array = UnityEngine.Object.FindObjectsOfType<CameraControl_Ver2>();
            int num = 0;

            if (num < array.Length)
            {
                CameraControl_Ver2 cameraControl_Ver = camv2 = array[num];
                //Console.WriteLine("camera 3 found" + studioneocam);
            }
            return camv2;
        }

    }
}

