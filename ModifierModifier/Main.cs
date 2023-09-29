using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx;
using HarmonyLib;
using CobwebAPI.API;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;


namespace ModifierModifier
{

    [BepInPlugin(ModName, ModGUID, ModVersion)]
    public class Main : BaseUnityPlugin
    {


        private Texture2D LoadTextureFromAssembly(string textName)
        {
            Stream inTex = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ModifierModifier.Resources.{textName}.png"); // Load Embedded PNG
            byte[] inBytes = inTex.ReadAllBytes(); // Convert Stream to Bytes
            Texture2D tempTex = new Texture2D(14, 14); // Empty Texture
            ImageConversion.LoadImage(tempTex, inBytes);
            return tempTex;
        }

        
        public static GUIStyle backgroundStyle = new GUIStyle();
        public static GUIStyle GUIStyle        = new GUIStyle();
        public static GUIStyle btnStyle        = new GUIStyle();

        public bool menuEnabled;
        public bool gotModifiers;
        public bool overrideMaxLevel;
        public int addValue = 1;
        public bool devConsoleEnabled;
        private InputAction _MenuAction = new InputAction(binding: "<Keyboard>/Insert");

        public const string ModName = "ModifierModifier";
        public const string ModAuthor = "V1AC";
        public const string ModGUID = "V1AC.modifiermodifier";
        public const string ModVersion = "1.2.0";

        // position;// = new Vector2(0, 38.5f);
        //Rect ModifierRect;// = new Rect(5, 5, 362, 462);

        Texture2D toggleActive = new Texture2D(14, 14);
        Texture2D toggleHover = new Texture2D(14, 14);
        Texture2D toggleOn = new Texture2D(14, 14);
        Texture2D toggleOnActive = new Texture2D(14, 14);
        Texture2D toggleOnHover = new Texture2D(14, 14);
        Texture2D toggle = new Texture2D(14, 14);
        Texture2D uimessy = new Texture2D(512, 512);
        Texture2D buttonOn = new Texture2D(14, 14);
        Texture2D buttonHover = new Texture2D(14, 14);
        Texture2D buttonActive = new Texture2D(14, 14);
        Texture2D buttonNormal = new Texture2D(14, 14);
        Texture2D buttonOnHover = new Texture2D(14, 14);
        object[] resources = Resources.LoadAll("");

        private List<Modifier> modifiers;
        internal Harmony? Harmony;

        internal void Awake()
        {
            
            if (Assembly.GetExecutingAssembly().GetManifestResourceStream("ModifierModifier.Resources.toggleactive.png") != null)
            {
                Logger.LogMessage("loaded Resources//toggleactive.png");
            }
            else
            {
                Logger.LogMessage("Could not load Resources//toggleactive.png");
            }
            Harmony = new Harmony(ModGUID);

            Harmony.PatchAll();
            Logger.LogMessage($"{ModName} successfully loaded! Made by {ModAuthor}");

            

            toggleActive =      LoadTextureFromAssembly("toggleactive");
            toggleOnActive =    LoadTextureFromAssembly("toggleonactive");
            toggleHover =       LoadTextureFromAssembly("togglehover");
            toggleOnHover =     LoadTextureFromAssembly("toggleonhover");
            toggleOn =          LoadTextureFromAssembly("toggleon");
            toggle =            LoadTextureFromAssembly("toggle");
            buttonOn =          LoadTextureFromAssembly("buttonon");
            buttonHover =       LoadTextureFromAssembly("buttonhover");
            buttonNormal =      LoadTextureFromAssembly("button");
            buttonActive =      LoadTextureFromAssembly("buttonactive");
            buttonOnHover =     LoadTextureFromAssembly("buttononhover");
            uimessy =           LoadTextureFromAssembly("uimessy");

            Main.GUIStyle.alignment = TextAnchor.MiddleCenter;
            Main.GUIStyle.fontSize = 16;
            Main.GUIStyle.fontStyle = FontStyle.Bold;
            Main.GUIStyle.normal.background = Texture2D.grayTexture;


            backgroundStyle.alignment = TextAnchor.MiddleCenter;
            backgroundStyle.normal.textColor = Color.black;
            backgroundStyle.normal.background = uimessy;
            

            menuEnabled = false;
            _MenuAction.Enable();
            btnStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                fixedHeight = 146,
                fixedWidth = 77,
                stretchHeight = false,
                stretchWidth = false,
                onActive =
                {
                    background = buttonOn
                },
                onHover =
                {
                    background = buttonOnHover
                },
                onNormal = 
                {
                    background = buttonOn
                },
                hover =
                {
                    background = buttonHover
                },
                normal =
                {
                    background = buttonNormal
                },
                active =
                {
                    background = buttonActive
                }
            };
        }

        private void Update()
        {
            if (_MenuAction.triggered)
                menuEnabled = !menuEnabled;

        }

        private void OnGUI()
        {
            
            List<GameObject> btns = new List<GameObject>();
            if (menuEnabled)
            {
                if (!gotModifiers)
                {
                    modifiers = GetAllModifiers();
                    gotModifiers = true;
                }
                CreateGUI();
            }
        }
        Rect ModifierRect = new Rect(5, 5, 232f, 466);

        Vector2 scrollPosition;
        // UI
        private void CreateGUI()
        {
            // Title Text && Background
            GUI.Label(new Rect(63, 5, 55, 55), "ModifierModifier", new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = new Color(255,165,165, 255)
                }
            });
            //GUI.Box(new Rect(5, 5, 232.5f, 24), "", );

            GUILayout.BeginArea(ModifierRect, backgroundStyle);
            {
                overrideMaxLevel = GUI.Toggle(new Rect(5f, 30f, 16.4f, 16.4f), overrideMaxLevel, "", new GUIStyle
                {
                    alignment = TextAnchor.MiddleLeft,
                    normal = new GUIStyleState
                    {
                        textColor = Color.gray,
                        background = toggle
                    },
                    hover =
                    {
                        background = toggleHover
                    },
                    onHover =
                    {
                        background = toggleOnHover
                    },
                    active =
                    {
                        background = toggleActive
                    },
                    onActive =
                    {
                        background = toggleOnActive
                    },
                    onNormal =
                    {
                        background = toggleOn
                    }
                });

                GUI.Label(new Rect(0f, 30f, 142.6f, 16.4f), "Bypass max level", new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    contentOffset = new Vector2(22f, 0),
                    normal =
                    {
                        textColor = Color.white
                    }
                });
                GUI.Box(new Rect(5f, 55f, 222.5f, 2.5f), "", GUIStyle);
                GUILayout.BeginArea(new Rect(5, 60, 232f, 402), GUIStyle.none);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition,false,false, GUIStyle.none, GUIStyle.none, GUILayout.Width(362), GUILayout.Height(402)) ;
                if (!gotModifiers) return;
                foreach (Modifier mod in this.modifiers)
                {
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button($"{mod.data.name} : {mod.levelInSurvival}", GUIStyle, GUILayout.Width(222.5f), GUILayout.Height(77f)))
                    {
                        HandleButtonClick(mod);
                    }
                }
                GUILayout.EndArea();
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }

        private void HandleButtonClick(Modifier modifier)
        {
            switch(Event.current.button)
            {
                case 0: // ADD
                    if (overrideMaxLevel)
                    {
                        modifier.levelInSurvival++;
                    }
                    if (!overrideMaxLevel)
                    {
                        if (modifier.levelInSurvival > modifier.data.maxLevel) return;
                        modifier.levelInSurvival++;
                    }
                    break;
                case 1: // REMOVE
                    if(modifier.levelInSurvival > 0)
                    {
                        modifier.levelInSurvival--;
                    }
                    break;
            }
            //if (overrideMaxLevel)
            //{
            //    modifier.levelInSurvival++;
            //}
            //if (!overrideMaxLevel)
            //{
            //    if (modifier.levelInSurvival > modifier.data.maxLevel) return;
            //    modifier.levelInSurvival++;
            //}
        }

        public List<Modifier> GetAllModifiers()
        {
            var a = Traverse.Create(ModifierManager.instance).Field<List<Modifier>>("_modifiers").Value;
            return a;
        }

        [HarmonyPatch(typeof(VersionNumberTextMesh), "Start")]
        public class VersionNumberTextMeshPatch
        {
            public static void Postfix(VersionNumberTextMesh __instance)
            {
                __instance.textMesh.text +=
                    $"\n<color=red>{Main.ModName} {Main.ModVersion} by {Main.ModAuthor} \n use the <color=white>INS</color> or <color=white>INSERT</color> key to toggle the menu!</color>";
            }
        }
    }

}