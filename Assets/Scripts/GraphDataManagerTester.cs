using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(GraphDataManager))]
public class GraphDataManagerTester : MonoBehaviour
{
    [SerializeField] private string testDate = "2015-01-01";
    [SerializeField] private int testWindowSize = 60;

    private GraphDataManager _graphDataManager;

    private void Awake()
    {
        _graphDataManager = GetComponent<GraphDataManager>();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GraphDataManagerTester))]
    public class GraphDataManagerTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GraphDataManagerTester tester = (GraphDataManagerTester)target;

            if (GUILayout.Button("Test RequestCorrMatrix"))
            {
                if (tester._graphDataManager != null && tester._graphDataManager.IsInitialized)
                {
                    tester._graphDataManager.RequestCorrMatrix(tester.testDate, tester.testWindowSize);
                }
                else
                {
                    Debug.LogWarning("GraphDataManager is not initialized. Please wait for setup to complete.");
                }
            }
        }
    }
#endif
}