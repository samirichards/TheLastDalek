using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject LevelTransitionUIPanel;
    [SerializeField] private float fadeDuration = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(Vector3 newPlayerPosition, string SceneName)
    {
        StartCoroutine(Activate(SceneName, newPlayerPosition));
    }

    public void ReloadScene(float delay)
    {
        StartCoroutine(_reloadScene(delay));
    }

    IEnumerator _reloadScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        Player._PropController.StopSound();
        LevelTransitionUIPanel.SetActive(true);
        LevelTransitionUIPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        op.allowSceneActivation = false;

        float t = 0;

        while (op.progress < 0.9f || t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            t = Mathf.Clamp01(t);
            LevelTransitionUIPanel.GetComponent<Image>().color = new Color(0, 0, 0, t);
            //controller.fade = t;
            yield return null;
        }
        Destroy(CameraObject.CameraObjectReference);
        Destroy(CameraObject.Instance);
        Destroy(Player.GetPlayerReference());
        Destroy(Player.Instance);
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        op.allowSceneActivation = true;
        Player._movement.IsElevating = false;

        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / fadeDuration;
            t = Mathf.Clamp01(t);
            LevelTransitionUIPanel.GetComponent<Image>().color = new Color(0, 0, 0, t);
            yield return null;
        }

        LevelTransitionUIPanel.SetActive(false);

        LevelTransitionUIPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        LevelTransitionUIPanel.SetActive(false);
    }

    IEnumerator Activate(string SceneName, Vector3 newLocation)
    {
        Player._movement.MovementEnabled = false;
        Player._PropController.StopSound();
        LevelTransitionUIPanel.SetActive(true);
        LevelTransitionUIPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName);
        op.allowSceneActivation = false;

        float t = 0;

        while (op.progress < 0.9f || t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            t = Mathf.Clamp01(t);
            LevelTransitionUIPanel.GetComponent<Image>().color = new Color(0, 0, 0, t);
            //controller.fade = t;
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        op.allowSceneActivation = true;
        Player.GetPlayerReference().transform.position = newLocation;
        Player._movement.IsElevating = false;

        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / fadeDuration;
            t = Mathf.Clamp01(t);
            LevelTransitionUIPanel.GetComponent<Image>().color = new Color(0, 0, 0, t);
            yield return null;
        }

        LevelTransitionUIPanel.SetActive(false);

        LevelTransitionUIPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        LevelTransitionUIPanel.SetActive(false);
        Player._movement.MovementEnabled = true;
    }
}
