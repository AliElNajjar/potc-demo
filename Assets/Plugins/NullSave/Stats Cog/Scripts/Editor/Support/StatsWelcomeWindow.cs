using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatsWelcomeWindow : TOCKEditorWindow
    {

        #region Variables

        private Texture2D splash;
        private static readonly int windowWidth = 500;
        private static readonly int windowHeight = 550;

        #endregion

        #region Properties

        private Texture2D Splash
        {
            get
            {
                if (splash == null)
                {
                    splash = (Texture2D)Resources.Load("Branding/stats-splash", typeof(Texture2D));
                }

                return splash;
            }
        }

        #endregion

        #region Unity Methods

        [MenuItem("Tools/NullSave/Welcome/Stats Cog™", false, 0)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(StatsWelcomeWindow), false, " Welcome", true);
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.titleContent.image = NullSaveIcon;
            editorWindow.maxSize = new Vector2(windowWidth, windowHeight);
            editorWindow.minSize = new Vector2(windowWidth, windowHeight);
            editorWindow.position = new Rect(Screen.width / 2 + windowWidth / 2, Screen.height / 2, windowWidth, windowHeight);
            editorWindow.Show();
        }

        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                this.ShowNotification(new GUIContent(" Compiling Scripts", NullSaveIcon));
            }
            else
            {
                this.RemoveNotification();
            }

            Rect welcomeImageRect = new Rect(0, 0, 500, 200);
            GUI.DrawTexture(welcomeImageRect, Splash);
            GUILayout.Space(220);

            MainContainerBegin("Welcome to Stats Cog™", string.Empty);

            SectionHeader("Getting Started");
            SimpleWrappedText("Stats Cog™ comes with an a demo to help you start getting familiar with all of the features and options. " +
                "Documentation is also regularly updated on our website." +
                "\r\n\r\nEnjoy adding inventory, crafting and more to your project!");

            GUILayout.Space(24);
            if (GUILayout.Button(new GUIContent(" Online Documentation", EditorGUIUtility.IconContent("_Help").image), GUILayout.MaxWidth(185f)))
            {
                Application.OpenURL("http://www.nullsave.com/docs/inventorycog.pdf");
            }

            MainContainerEnd();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        #endregion

    }
}