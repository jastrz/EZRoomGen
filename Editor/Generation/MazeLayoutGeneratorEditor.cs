#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace EZRoomGen.Generation.Editor
{
    /// <summary>
    /// Editor utility class used to display Maze Layout Generator settings.
    /// </summary>
    public class MazeLayoutGeneratorEditor : BaseLayoutGeneratorEditor<MazeLayoutLayoutGeneratorSettings>
    {
        public override bool DrawInspector(MazeLayoutLayoutGeneratorSettings settings)
        {
            EditorGUI.BeginChangeCheck();

            DrawBaseFields(settings);

            EditorGUILayout.Space();

            settings.loopCount = EditorGUILayout.IntSlider(
                new GUIContent("Loop Count", "Number of extra connections"),
                settings.loopCount, 0, 20);

            settings.deadEndKeepChance = EditorGUILayout.Slider(
                new GUIContent("Keep Chance", "Percentage of dead ends to keep"),
                settings.deadEndKeepChance, 0f, 1f);

            settings.smoothEdges = EditorGUILayout.Toggle(
                new GUIContent("Smooth Edges", "Round off sharp corners"),
                settings.smoothEdges);

            bool changed = EditorGUI.EndChangeCheck();

            return changed;
        }
    }
}

#endif