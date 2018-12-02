﻿using System;
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

        public static string[] cameraLabelCache = new string[90];
        public static float[] cameraXCoordCache = new float[90];

        private GUIContent[] bunchOfButtons;
        private Vector2 listScroller;

        public static bool showMenu = false;
        public static bool pluginActive = false;
        public static bool showTextEditor = false;
        public static bool saveDialogVisible = false;
        public static bool loadDialogVisible = false;
        public static bool playButtonPressed = false;
        public static bool reachedEnd = false;
        public static DateTime lastplayedTimeStamp;
        public static bool isPlaying = false;
        public bool showDisplayBox { get; private set; }
        public static bool stylesAreInitialized = false;

        public static string messageDelay = "8";
        public static string saveFileName = "Enter Filename here";
        public static string lastButtonPressed = "C1";
        public static string lastPlayedButton = "C0";
        public string messageTextToEdit = "";
        public string loadedMessageText = "";
        public string displayedCurrentMessageText = "";

        public static Studio.CameraControl studioneocam = null;
        public static BaseCameraControl basecam = null;
        public static CameraControl_Ver2 camv2 = null;
        public static Dictionary<string, CameraPositionAndMessage> cameraDictionary = new Dictionary<string, CameraPositionAndMessage>();

        public static GUIStyle cameraButtonNormalStyle;
        public static GUIStyle cameraButtonSavedStyle;
        public static GUIStyle cameraButtonSelectedStyle;
        public static GUIStyle largeTextStyle;

        public void InitializeCaches()
        {
            // label cache
            for (int i = 0; i < 90; i++)
            {
                cameraLabelCache[i] = "C" + i;
            }

            // x coord cache
            int counter = 0;
            for (int i = 0; i < 15; i++)
            {
                cameraXCoordCache[i] = 640 + counter * 40;
                counter = counter + 1;
            }

            counter = 0;
            for (int i = 15; i < 30; i++)
            {
                cameraXCoordCache[i] = 640 + counter * 40;
                counter = counter + 1;
            }

            counter = 0;
            for (int i = 30; i < 45; i++)
            {
                cameraXCoordCache[i] = 640 + counter * 40;
                counter = counter + 1;
            }

            counter = 0;
            for (int i = 45; i < 60; i++)
            {
                cameraXCoordCache[i] = 640 + counter * 40;
                counter = counter + 1;
            }

            counter = 0;
            for (int i = 60; i < 75; i++)
            {
                cameraXCoordCache[i] = 640 + counter * 40;
                counter = counter + 1;
            }

            counter = 0;
            for (int i = 75; i < 90; i++)
            {
                cameraXCoordCache[i] = 640 + counter * 40;
                counter = counter + 1;
            }


            bunchOfButtons = new GUIContent[90];
            for (int i = 0; i < bunchOfButtons.Length; i++)
            {
                bunchOfButtons[i] = new GUIContent(cameraLabelCache[i]);
            }

        }

        private void InitializeStyles()
        {
            // button styles
            cameraButtonNormalStyle = new GUIStyle(GUI.skin.button);
            cameraButtonNormalStyle.normal.textColor = Color.white;
            cameraButtonNormalStyle.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 0.0f, 0.1f));

            cameraButtonSavedStyle = new GUIStyle(GUI.skin.button);
            cameraButtonSavedStyle.normal.textColor = Color.white;
            cameraButtonSavedStyle.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 1.0f, 0.2f));

            cameraButtonSelectedStyle = new GUIStyle(GUI.skin.button);
            cameraButtonSelectedStyle.normal.textColor = Color.white;
            cameraButtonSelectedStyle.normal.background = MakeTex(600, 1, new Color(0.0f, 1.0f, 0.0f, 0.2f));

            // big text style
            largeTextStyle = new GUIStyle();
            largeTextStyle.fontSize = 30;
            largeTextStyle.normal.textColor = Color.white;
            largeTextStyle.wordWrap = true;
            largeTextStyle.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 0.0f, 0.1f));

            GUI.skin.textArea.wordWrap = true;

            stylesAreInitialized = true;
        }


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
                loadedMessageText = cameraDictionary[cameraButtonName].Text;
                displayedCurrentMessageText = string.Empty;

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

                StartCoroutine("DisplayMessageCoRoutine");

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
            if (!stylesAreInitialized)
            {
                InitializeStyles();
            }

            // display box
            GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.3), TEXTBOXWIDTH + 100, TEXTBOXHEIGHT + 100));

            // this is the display box of the messages
            if (showDisplayBox)
            {
                GUILayout.TextArea(displayedCurrentMessageText, largeTextStyle, GUILayout.Height(TEXTBOXHEIGHT), GUILayout.Width(TEXTBOXWIDTH));
            }

            GUILayout.EndArea();

            if (showDisplayBox)
            {

                GUILayout.BeginArea(new Rect((float)(Screen.width / 3) + TEXTBOXWIDTH, (float)(Screen.height / 1.3) + TEXTBOXHEIGHT, 40, 60));

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
                ShowTextEditor();
            }

            // Menu shown to do stuff
            if (showMenu)
            {
                ShowMenu();
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
                ShowMainAndCameraButtons();
            }

            if (saveDialogVisible)
            {
                ShowSaveDialog();
            }
            if (loadDialogVisible)
            {
                ShowLoadDialog();
            }
        }

        private static void ShowLoadDialog()
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

        private static void ShowSaveDialog()
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

        private void ShowMainAndCameraButtons()
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
                loadedMessageText = "";
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


            // todo: make a slider
            GUILayout.Label("  Delay between Messages: ");

            messageDelay = GUILayout.TextField(messageDelay, new GUILayoutOption[0]);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();


            // better performance with scrollarea
            /*
            GUI.BeginGroup(new Rect(640, 620, 50, 300), "", "box");
            DoScrollArea(new Rect(0, 0, 50, 300), bunchOfButtons, 32);
            GUI.EndGroup();
            */

            DrawCameraButtons();

        }

        private void ShowMenu()
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

        private void ShowTextEditor()
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
                }

            }


            if (GUILayout.Button("Cancel"))
            {
                ToggleTextEditor();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DoScrollArea(Rect position, GUIContent[] buttons, int buttonHeight)
        {

            float height = 0;
            int index = 0;

            if (buttons.Length > 0)
            {
                height = ((buttons.Length - 1) * buttonHeight);
            }

            listScroller = GUI.BeginScrollView(position, listScroller, new Rect(0, 0, position.width - 20, height + buttonHeight));

            for (index = 0; index < buttons.Length; index++)
            {

                if (((index + 1) * buttonHeight) > listScroller.y)
                {
                    break;
                }
            }

            for (; index < buttons.Length; index++)
            {
                AddCameraButtonGUI(new Rect(0, index * buttonHeight, position.width - 16, buttonHeight), cameraLabelCache[index]);
            }

            GUI.EndScrollView();
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

            do
            {
                lastplayedTimeStamp = DateTime.Now;
                reachedEnd = !ShowNextCamera();
                yield return new WaitForSeconds(float.Parse(messageDelay));
            } while (!reachedEnd);

            if (reachedEnd)
            {
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

        private void AddCameraButtonGUI(Rect position, string cameraName)
        {
            if (GUI.Button(position, cameraName, GetCorrectStyle(cameraName)))
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


        private void AddCameraButtonGUILayout(string cameraName)
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
            if (camv2 != null)
            {
                return camv2;
            }
            CameraControl_Ver2[] array = UnityEngine.Object.FindObjectsOfType<CameraControl_Ver2>();
            int num = 0;

            if (num < array.Length)
            {
                CameraControl_Ver2 cameraControl_Ver = camv2 = array[num];
            }
            return camv2;
        }

        IEnumerator DisplayMessageCoRoutine()
        {
            int length = loadedMessageText.Length;

            for (int i = 0; i < length; i++)
            {
                displayedCurrentMessageText = displayedCurrentMessageText + loadedMessageText[i];
                yield return new WaitForSeconds(0.06f);
            }
        }

        private void DrawCameraButtons()
        {
            GUILayout.BeginArea(new Rect((Screen.width / 3), (float)(Screen.height / 1.75), (Screen.width / 3), (Screen.height / 2) + 50));

            // Camera Buttons below
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            AddCameraButtonGUILayout("C1");
            AddCameraButtonGUILayout("C2");
            AddCameraButtonGUILayout("C3");
            AddCameraButtonGUILayout("C4");
            AddCameraButtonGUILayout("C5");
            AddCameraButtonGUILayout("C6");
            AddCameraButtonGUILayout("C7");
            AddCameraButtonGUILayout("C8");
            AddCameraButtonGUILayout("C9");
            AddCameraButtonGUILayout("C10");
            AddCameraButtonGUILayout("C11");
            AddCameraButtonGUILayout("C12");
            AddCameraButtonGUILayout("C13");
            AddCameraButtonGUILayout("C14");
            AddCameraButtonGUILayout("C15");

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 20, (float)(Screen.width / 3), (float)(Screen.height / 2)));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            AddCameraButtonGUILayout("C16");
            AddCameraButtonGUILayout("C17");
            AddCameraButtonGUILayout("C18");
            AddCameraButtonGUILayout("C19");
            AddCameraButtonGUILayout("C20");
            AddCameraButtonGUILayout("C21");
            AddCameraButtonGUILayout("C22");
            AddCameraButtonGUILayout("C23");
            AddCameraButtonGUILayout("C24");
            AddCameraButtonGUILayout("C25");
            AddCameraButtonGUILayout("C26");
            AddCameraButtonGUILayout("C27");
            AddCameraButtonGUILayout("C28");
            AddCameraButtonGUILayout("C29");
            AddCameraButtonGUILayout("C30");

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 40, (float)(Screen.width / 3), (float)(Screen.height / 2)));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            AddCameraButtonGUILayout("C31");
            AddCameraButtonGUILayout("C32");
            AddCameraButtonGUILayout("C33");
            AddCameraButtonGUILayout("C34");
            AddCameraButtonGUILayout("C35");
            AddCameraButtonGUILayout("C36");
            AddCameraButtonGUILayout("C37");
            AddCameraButtonGUILayout("C38");
            AddCameraButtonGUILayout("C39");
            AddCameraButtonGUILayout("C40");
            AddCameraButtonGUILayout("C41");
            AddCameraButtonGUILayout("C42");
            AddCameraButtonGUILayout("C43");
            AddCameraButtonGUILayout("C44");
            AddCameraButtonGUILayout("C45");

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 60, (float)(Screen.width / 3), (float)(Screen.height / 2)));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            AddCameraButtonGUILayout("C46");
            AddCameraButtonGUILayout("C47");
            AddCameraButtonGUILayout("C48");
            AddCameraButtonGUILayout("C49");
            AddCameraButtonGUILayout("C50");
            AddCameraButtonGUILayout("C51");
            AddCameraButtonGUILayout("C52");
            AddCameraButtonGUILayout("C53");
            AddCameraButtonGUILayout("C54");
            AddCameraButtonGUILayout("C55");
            AddCameraButtonGUILayout("C56");
            AddCameraButtonGUILayout("C57");
            AddCameraButtonGUILayout("C58");
            AddCameraButtonGUILayout("C59");
            AddCameraButtonGUILayout("C60");

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 80, (float)(Screen.width / 3), (float)(Screen.height / 2)));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            AddCameraButtonGUILayout("C61");
            AddCameraButtonGUILayout("C62");
            AddCameraButtonGUILayout("C63");
            AddCameraButtonGUILayout("C64");
            AddCameraButtonGUILayout("C65");
            AddCameraButtonGUILayout("C66");
            AddCameraButtonGUILayout("C67");
            AddCameraButtonGUILayout("C68");
            AddCameraButtonGUILayout("C69");
            AddCameraButtonGUILayout("C70");
            AddCameraButtonGUILayout("C71");
            AddCameraButtonGUILayout("C72");
            AddCameraButtonGUILayout("C73");
            AddCameraButtonGUILayout("C74");
            AddCameraButtonGUILayout("C75");

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((float)(Screen.width / 3), (float)(Screen.height / 1.75) + 100, (float)(Screen.width / 3), (float)(Screen.height / 2)));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            AddCameraButtonGUILayout("C76");
            AddCameraButtonGUILayout("C77");
            AddCameraButtonGUILayout("C78");
            AddCameraButtonGUILayout("C79");
            AddCameraButtonGUILayout("C80");
            AddCameraButtonGUILayout("C81");
            AddCameraButtonGUILayout("C82");
            AddCameraButtonGUILayout("C83");
            AddCameraButtonGUILayout("C84");
            AddCameraButtonGUILayout("C85");
            AddCameraButtonGUILayout("C86");
            AddCameraButtonGUILayout("C87");
            AddCameraButtonGUILayout("C88");
            AddCameraButtonGUILayout("C89");
            AddCameraButtonGUILayout("C90");

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }
    }
}

