using UnityEngine;
using System.Collections;

public class LoadSceneWithoutFade : MonoBehaviour {

    public void loadScene(string scene)
    {
        Application.LoadLevel(scene);
    }
}
