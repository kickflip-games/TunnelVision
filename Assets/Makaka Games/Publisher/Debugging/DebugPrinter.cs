/*
================================
Assets for Unity by Makaka Games
================================
 
[Online  Docs -> Updated]: https://makaka.org/unity-assets
[Offline Docs - PDF file]: find it in the package folder.

[Support]: https://makaka.org/support
*/

using UnityEngine;

public static class DebugPrinter
{
    public static void Print(object message)
    {

#if DEVELOPMENT_BUILD || UNITY_EDITOR

        Debug.Log(message);

#endif

    }

    public static void PrintWarning(object message)
    {

#if DEVELOPMENT_BUILD || UNITY_EDITOR

        Debug.LogWarning(message);

#endif

    }

    public static void PrintError(object message)
    {

#if DEVELOPMENT_BUILD || UNITY_EDITOR

        Debug.LogError(message);

#endif

    }

    public static void PrintArray(object[] objects)
    {

#if DEVELOPMENT_BUILD || UNITY_EDITOR

        for (int i = 0; i < objects.Length; i++)
        {
            DebugPrinter.Print(objects[i]);
        }

#endif

    }

}
