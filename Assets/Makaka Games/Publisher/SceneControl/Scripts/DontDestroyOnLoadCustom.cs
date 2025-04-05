/*
================================
Assets for Unity by Makaka Games
================================
 
[Online  Docs -> Updated]: https://makaka.org/unity-assets
[Offline Docs - PDF file]: find it in the package folder.

[Support]: https://makaka.org/support
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadCustom : MonoBehaviour
{
    static bool isLoaded;

    void Awake() 
    {
        DebugPrinter.Print("Back Button isLoaded=" + isLoaded);

        if (!isLoaded)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  
        }
 
        isLoaded = true;
     }
}
