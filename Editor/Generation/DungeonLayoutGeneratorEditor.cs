#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace EZRoomGen.Generation.Editor
{
    /// <summary>
    /// Editor utility class used to display Dungeon Layout Generator settings.
    /// </summary>
    public class DungeonLayoutGeneratorEditor : BaseLayoutGeneratorEditor<DungeonLayoutGeneratorSettings>
    {
        public override bool DrawInspector(DungeonLayoutGeneratorSettings settings)
        {
            EditorGUI.BeginChangeCheck();

            DrawBaseFields(settings);

            EditorGUILayout.Space();

            settings.density = EditorGUILayout.Slider(
                new GUIContent("Density", "Initial wall density (0-1)"),
                settings.density, 0f, 1f);

            settings.iterations = EditorGUILayout.IntSlider(
                new GUIContent("Iterations", "Cellular automata iterations"),
                settings.iterations, 1, 10);

            settings.pathWidth = EditorGUILayout.IntSlider(
                new GUIContent("Path Width", "Width of corridors"),
                settings.pathWidth, 1, 5);

            settings.smoothEdges = EditorGUILayout.Toggle(
                new GUIContent("Smooth Edges", "Round off sharp corners"),
                settings.smoothEdges);

            bool changed = EditorGUI.EndChangeCheck();

            return changed;
        }
    }
}

#endif