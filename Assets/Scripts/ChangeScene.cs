using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {
	
    public void LoadScene (string scene) {
        Application.LoadLevel(scene);
	}
}
