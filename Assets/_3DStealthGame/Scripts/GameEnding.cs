using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 0.6f;
    public UIDocument uiDocument;
    public AudioSource exitAudio;
    public AudioSource caughtAudio;
    private bool m_HasAudioPlayed = false;

    private bool m_HasEnded;

    private VisualElement m_EndScreen;
    private VisualElement m_CaughtScreen;

    private bool m_PlayerAtExit = false;

    private float m_Demo_GameTimer = 0f;
    private bool m_Demo_GameTimerIsTicking = false;
    private Label m_Demo_GameTimerLabel;

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        m_EndScreen = root.Q<VisualElement>("EndScreen");
        m_CaughtScreen = root.Q<VisualElement>("CaughtScreen");

        if (m_EndScreen != null)
        {
            m_EndScreen.style.display = DisplayStyle.None;
            m_EndScreen.style.opacity = 0;
        }

        if (m_CaughtScreen != null)
        {
            m_CaughtScreen.style.display = DisplayStyle.None;
            m_CaughtScreen.style.opacity = 0;
        }

        Debug.Log($"[GameEnding] EndScreen: {m_EndScreen != null}, CaughtScreen: {m_CaughtScreen != null}");

        m_Demo_GameTimerLabel = root.Q<Label>("TimerLabel");
        m_Demo_GameTimer = 0f;
        m_Demo_GameTimerIsTicking = true;
        Demo_UpdateTimerLabel();
    }

    void Update()
    {
      
        if (m_Demo_GameTimerIsTicking)
        {
            m_Demo_GameTimer += Time.deltaTime;
            Demo_UpdateTimerLabel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("[GameEnding] Player llegó a la salida → Activar EndScreen");
        m_PlayerAtExit = true;

        m_Demo_GameTimerIsTicking = false;

        if (!m_HasEnded)
        {
            m_HasEnded = true;
            StartCoroutine(ShowScreen(m_EndScreen, false, exitAudio));
        }
    }

    public void CaughtPlayer()
    {
        if (m_HasEnded) return;

        Debug.Log("[GameEnding] ¡Jugador atrapado!");
        m_HasEnded = true;

        m_Demo_GameTimerIsTicking = false;

        StartCoroutine(ShowScreen(m_CaughtScreen, true, caughtAudio));
    }

    IEnumerator ShowScreen(VisualElement screen, bool restart, AudioSource audioSource)
    {
        if (screen == null)
        {
            Debug.LogError("[GameEnding] La pantalla es NULL!");
            yield break;
        }

        if (!m_HasAudioPlayed && audioSource != null)
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }

        screen.style.display = DisplayStyle.Flex;

        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            screen.style.opacity = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        screen.style.opacity = 1;

        yield return new WaitForSeconds(displayImageDuration);

        if (restart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
    void Demo_UpdateTimerLabel()
    {
        if (m_Demo_GameTimerLabel != null)
        {
            m_Demo_GameTimerLabel.text = m_Demo_GameTimer.ToString("0.00");
        }
    }
}
