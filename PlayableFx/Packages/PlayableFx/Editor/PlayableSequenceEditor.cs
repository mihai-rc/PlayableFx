#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace PlayableFx
{
    [CustomEditor(typeof(PlayableSequence), true)]
    public class PlayableSequenceEditor : Editor
    {
        private ISequencePlayer Player => (target as PlayableSequence)?.Player;

        private const float k_ButtonsHeight = 25;
        private const float k_ButtonsWidth = 25;

        public override void OnInspectorGUI()
        {
            if (Player is null)
            {
                base.OnInspectorGUI();
                return;
            }
            
            var revertButtonIcon = EditorGUIUtility.IconContent("Animation.FirstKey");
            var completeButtonIcon = EditorGUIUtility.IconContent("Animation.LastKey");
            
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            var backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b);
            GUI.backgroundColor = Player.IsPlaying || Player.IsPaused ? HexToColor("#96C3FB") : backgroundColor;
            
            var playButtonIcon = EditorGUIUtility.IconContent(Player.IsPlaying ? "PauseButton" : "PlayButton");
            var playButton = GUILayout.Button(playButtonIcon, GUILayout.Height(k_ButtonsHeight));
            if (playButton && !Player.IsPlaying && !Player.IsPaused)
            {
                Player.PlayAsync().Forget();
            }
            else if (playButton && Player.IsPlaying && !Player.IsPaused)
            {
                Player.Pause();
            } 
            else if (playButton && Player.IsPaused)
            {
                Player.Resume();
            }
            
            GUI.backgroundColor = backgroundColor;
            
            if (GUILayout.Button(revertButtonIcon, GUILayout.Height(k_ButtonsHeight), GUILayout.Width(k_ButtonsWidth)))
            {
                Player.Revert();
            }
            
            if (GUILayout.Button(completeButtonIcon, GUILayout.Height(k_ButtonsHeight), GUILayout.Width(k_ButtonsWidth)))
            {
                Player.Complete();
            }
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            Player.Progress = EditorGUILayout.Slider("Progress", Player.Progress, 0f, 1f);
            GUILayout.Space(10);
            
            base.OnInspectorGUI();
        }
        
        private static Color HexToColor(string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.white;
        }
    }
}

#endif