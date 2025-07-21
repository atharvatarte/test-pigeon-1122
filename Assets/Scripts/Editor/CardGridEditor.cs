using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardGrid))]
public class CardGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CardGrid cardGrid = (CardGrid)target;
        
        // Draw the default inspector
        DrawDefaultInspector();
        
        // Add responsive scaling info
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Responsive Scaling Info", EditorStyles.boldLabel);
        
        if (cardGrid.autoScaleCards)
        {
            EditorGUILayout.HelpBox(
                "✅ Auto-scaling is enabled!\n" +
                "Cards will automatically resize to fit the available display area.",
                MessageType.Info
            );
            
            // Show current card size if available
            Vector2 currentSize = cardGrid.GetCurrentCardSize();
            EditorGUILayout.HelpBox(
                $"Current Card Size: {currentSize.x:F1} x {currentSize.y:F1}\n" +
                $"Aspect Ratio: {currentSize.x / currentSize.y:F2}",
                MessageType.None
            );
        }
        else
        {
            EditorGUILayout.HelpBox(
                "⚠️ Auto-scaling is disabled!\n" +
                "Cards will use the fixed size specified in Card Size.",
                MessageType.Warning
            );
        }
        
        // Show scaling constraints
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scaling Constraints", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            $"Minimum Size: {cardGrid.minCardSize.x:F0} x {cardGrid.minCardSize.y:F0}\n" +
            $"Maximum Size: {cardGrid.maxCardSize.x:F0} x {cardGrid.maxCardSize.y:F0}\n" +
            $"Grid Padding: {cardGrid.gridPadding:F0}px\n" +
            $"Card Spacing: {cardGrid.spacing:F0}px",
            MessageType.None
        );
        
        // Add helpful tips
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tips", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "💡 Responsive Scaling Tips:\n" +
            "• Cards maintain a 5:7 aspect ratio for optimal appearance\n" +
            "• Minimum size ensures cards remain readable on small screens\n" +
            "• Maximum size prevents oversized cards on large displays\n" +
            "• Grid padding keeps cards away from container edges\n" +
            "• Auto-scaling works with any grid size and screen resolution\n" +
            "• Cards automatically adjust when screen size changes",
            MessageType.None
        );
        
        // Add manual update button
        EditorGUILayout.Space();
        if (GUILayout.Button("Update Card Scaling Now"))
        {
            cardGrid.UpdateCardScaling();
            EditorUtility.SetDirty(cardGrid);
        }
    }
} 