using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 0.4f;
    public UIDocument uiDocument;

    private bool m_HasEnded;

    private VisualElement m_CaughtScreen;

    void Start()
    {
        m_CaughtScreen = uiDocument.rootVisualElement.Q<VisualElement>("CaughtScreen");

        if (m_CaughtScreen != null)
        {
            m_CaughtScreen.style.display = DisplayStyle.None;
            m_CaughtScreen.style.opacity = 0;
        }
    }

    public void CaughtPlayer()
    {
        if (m_HasEnded) return;

        Debug.Log("[GameEnding] MOSTRANDO pantalla CaughtScreen");
        m_HasEnded = true;

        StartCoroutine(ShowCaughtScreen());
    }

    IEnumerator ShowCaughtScreen()
    {
        if (m_CaughtScreen == null)
        {
            Debug.LogError("CaughtScreen ES NULL en UI Toolkit!");
            yield break;
        }

        // Mostrar pantalla
        m_CaughtScreen.style.display = DisplayStyle.Flex;

        float timer = 0f;

        // Fade In
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            m_CaughtScreen.style.opacity = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        m_CaughtScreen.style.opacity = 1;

        // Esperarr
        yield return new WaitForSeconds(displayImageDuration);

        // Reiniciar escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
