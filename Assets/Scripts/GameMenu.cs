using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour {

    public Canvas GameCanvas;
    public Camera GameCamera;

    public void createGameMenu()
    {
        if (!GameCanvas.gameObject.activeInHierarchy)
        {
/*            Instantiate(GameCanvas);
            GameCanvas.worldCamera = GameCamera;
*/
            GameCanvas.gameObject.SetActive(true);
        }
    }
}
