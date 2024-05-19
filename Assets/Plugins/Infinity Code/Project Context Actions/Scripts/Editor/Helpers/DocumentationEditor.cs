/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.ProjectContextActions
{
    [CustomEditor(typeof(Documentation))]
    public class DocumentationEditor : Editor
    {
        private static GUIStyle _centeredLabelStyle;

        public static GUIStyle CenteredLabelStyle
        {
            get
            {
                if (_centeredLabelStyle == null)
                {
                    _centeredLabelStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                
                return _centeredLabelStyle;
            }
        }

        private static void DrawDocumentation()
        {
            if (GUILayout.Button("Local Documentation"))
            {
                Links.OpenLocalDocumentation();
            }

            if (GUILayout.Button("Online Documentation"))
            {
                Links.OpenDocumentation();
            }

            GUILayout.Space(10);
        }

        private new static void DrawHeader()
        {
            GUILayout.Label("Project Context Actions", CenteredLabelStyle);
            GUILayout.Label("by Infinity Code", CenteredLabelStyle);
            GUILayout.Label("version: " + Utils.Version, CenteredLabelStyle);
            GUILayout.Space(10);
        }

        private void DrawRateAndReview()
        {
            EditorGUILayout.HelpBox("Please don't forget to leave a review on the store page if you liked Project Context Actions, this helps us a lot!", MessageType.Warning);

            if (GUILayout.Button("Rate & Review"))
            {
                Links.OpenReviews();
            }
        }

        private void DrawSupport()
        {
            if (GUILayout.Button("Homepage"))
            {
                Links.OpenHomepage();
            }
            
            if (GUILayout.Button("Asset Store"))
            {
                Links.OpenAssetStore();
            }
            
            if (GUILayout.Button("Support"))
            {
                Links.OpenSupport();
            }
            
            if (GUILayout.Button("Discord"))
            {
                Links.OpenDiscord();
            }

            if (GUILayout.Button("Forum"))
            {
                Links.OpenForum();
            }

            GUILayout.Space(10);
        }

        public override void OnInspectorGUI()
        {
            DrawHeader();
            DrawDocumentation();
            DrawSupport();
            DrawRateAndReview();
        }
    }
}