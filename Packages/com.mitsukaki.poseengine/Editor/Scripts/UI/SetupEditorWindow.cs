using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using com.mitsukaki.poseengine.editor.ui.Views;

namespace com.mitsukaki.poseengine.editor.ui
{
    public class SetupEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Pose Engine")]
        public static void ShowExample()
        {
            SetupEditorWindow wnd = GetWindow<SetupEditorWindow>();
            wnd.titleContent = new GUIContent("Pose Engine");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // Add the stylesheet from the resources folder
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.mitsukaki.poseengine/Editor/Resources/PEStyleBase.uss");
            root.styleSheets.Add(styleSheet);

            // Create the view
            root.Add(CreateBannerButton());
            root.Add(new EditPanelView());
        }

        public Button CreateBannerButton()
        {
            Button bannerButton = new Button(() =>
            {
                // open pose engine docs webpage
                Application.OpenURL("https://mitsukaki.github.io/pe-docs/");
            });

            bannerButton.AddToClassList("pe_banner");
            bannerButton.AddToClassList("pe_banner-primary");

            return bannerButton;
        }
    }
}
