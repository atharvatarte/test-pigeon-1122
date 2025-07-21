using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;
        
        // Draw the default inspector
        DrawDefaultInspector();
        
        // Add validation info
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Validation", EditorStyles.boldLabel);
        
        bool widthIsOdd = gameManager.GetGridWidth() % 2 != 0;
        bool heightIsOdd = gameManager.GetGridHeight() % 2 != 0;
        
        if (widthIsOdd && heightIsOdd)
        {
            EditorGUILayout.HelpBox(
                "⚠️ Both grid dimensions are odd numbers!\n" +
                "At least one dimension must be even for proper card pairing.\n" +
                "The system will automatically increment the smaller dimension by 1.",
                MessageType.Warning
            );
        }
        else if (widthIsOdd || heightIsOdd)
        {
            EditorGUILayout.HelpBox(
                "✅ Grid validation passed!\n" +
                "At least one dimension is even, which is required for card pairing.",
                MessageType.Info
            );
        }
        else
        {
            EditorGUILayout.HelpBox(
                "✅ Both dimensions are even!\n" +
                "Perfect for card matching games.",
                MessageType.Info
            );
        }
        
        // Show constraint count info
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Layout Info", EditorStyles.boldLabel);
        
        int constraintCount = Mathf.Max(gameManager.GetGridWidth(), gameManager.GetGridHeight());
        EditorGUILayout.HelpBox(
            $"Grid Layout Constraint Count: {constraintCount}\n" +
            $"This is set to the higher of width ({gameManager.GetGridWidth()}) or height ({gameManager.GetGridHeight()})",
            MessageType.None
        );
        
        // Add helpful tips
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tips", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "💡 Tips for optimal grid setup:\n" +
            "• At least one dimension should be even for proper card pairing\n" +
            "• Common grid sizes: 4x4, 6x4, 5x6, 6x6\n" +
            "• The constraint count automatically adjusts to the larger dimension\n" +
            "• Odd dimensions will be automatically corrected during game initialization",
            MessageType.None
        );
    }
} 