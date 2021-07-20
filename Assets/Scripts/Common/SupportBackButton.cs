using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBackButton : MonoBehaviour
{
    protected void OnEnable()
    {
        UIStackManager.PushUiStack(transform);
    }

    protected void OnDisable()
    {
        UIStackManager.PopUiStack(gameObject.GetInstanceID());
    }
}
