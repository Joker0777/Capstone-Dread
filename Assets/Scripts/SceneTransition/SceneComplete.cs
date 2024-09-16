using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SceneComplete : MonoBehaviour
{
    private ICompletionElement[] _completionElements;

    public UnityEvent OnSceneComplete;
    public UnityEvent OnSceneFailed;

    private void Start()
    {
        InitializeCompletionElements();
    }

    public void InitializeCompletionElements()
    {
        _completionElements = GetComponentsInChildren<ICompletionElement>();
    }

    public void CheckSceneStatus()
    {
        if (_completionElements == null) return;

        bool sceneComplete = true;
        bool sceneFailed = false;

        foreach (ICompletionElement element in _completionElements)
        {
            if (!element.isComplete) sceneComplete = false;
            if (element.isFailed) sceneFailed = true;
        }

        if (sceneComplete)
        {
            OnSceneComplete.Invoke();
        }
        else if (sceneFailed)
        {
            OnSceneFailed.Invoke();
        }
    }

}
