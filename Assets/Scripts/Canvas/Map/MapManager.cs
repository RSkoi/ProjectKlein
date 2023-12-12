using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : ControllerWithWindow
{
    [Tooltip("The indicator for the current node.")]
    public GameObject indicator;
    [Tooltip("The node container inside of the map scrollview.")]
    public MapNodeDictionary nodes;

    [Tooltip("The list of nodes the player has already visited this playthrough.")]
    private MapNodeVisitedDictionary visitedNodes = new();

    public void Init()
    {
        SetCurrentNode();
        LockUnvisitedNodes();
    }

    private void SetCurrentNode()
    {
        string curSceneName = SceneManager.GetActiveScene().name;
        foreach ((string sceneName, GameObject mapNode) in nodes)
        {
            if (sceneName.Equals(curSceneName))
            {
                indicator.transform.SetParent(mapNode.transform, false);
                LockNode(mapNode);
            }
        }
        SetVisistedNode(curSceneName, true);
    }

    public MapNodeVisitedDictionary PrepareVisitedNodeEntriesForSave()
    {
        return visitedNodes;
    }

    public void LockUnvisitedNodes()
    {
        foreach ((string sceneName, GameObject mapNode) in nodes)
            if (!GetNodeVisisted(sceneName))
                LockNode(mapNode);
    }

    private void LockNode(GameObject mapNode)
    {
        // this could create problems if the layout/prefab of a map node changes
        mapNode.transform.GetChild(0).GetComponent<Button>().interactable = false;
        //mapNode.transform.GetComponentInChildren<TMP_Text>().alpha = 0.6f;
    }

    /// <summary>
    /// this method should be used to mark the current node as visisted on encountering it
    /// <summary>
    public void SetVisistedNode(string sceneName, bool visisted)
    {
        visitedNodes[sceneName] = visisted;
    }

    /// <summary>
    /// this method should be used to load all visisted node data as CrossSceneData from the NodeManager
    /// <summary>
    public void SetVisistedNodes(MapNodeVisitedDictionary dictionary)
    {
        visitedNodes = dictionary;
    }

    public bool GetNodeVisisted(string sceneName)
    {
        if (visitedNodes.Count == 0 || !visitedNodes.ContainsKey(sceneName))
            return false;

        return visitedNodes[sceneName];
    }

    public void ToggleMap()
    {
        if (window.activeSelf)
            window.SetActive(false);
        else
            window.SetActive(true);
    }

    public override void ToggleWindow()
    {
        ToggleMap();
    }
}
