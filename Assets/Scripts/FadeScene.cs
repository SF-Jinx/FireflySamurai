using UnityEngine;
using System.Collections;

public class FadeScene : MonoBehaviour
{
    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;
    private int drawDepth = -1000;  // render on top of everything
    private float alpha = 1.0f;
    private int fadeDir = -1;
    private bool fade = true;

    private void OnGUI()
    {
        if (fade)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = drawDepth;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
        }
    }

    public void LoadScene(string scene)
    {
        fade = true;
        StartCoroutine(FadeToScene(scene));
    }

    public void LoadSceneWithoutFade(string scene)
    {
        fade = false;
        Application.LoadLevel(scene);
    }
    
    IEnumerator FadeToScene(string scene)
    {
        float fadeTime = BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        Application.LoadLevel(scene);
    }

    private float BeginFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);
    }

    private void OnLevelWasLoaded()
    {
        BeginFade(-1);
    }
}
