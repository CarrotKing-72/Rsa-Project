using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class GoToLevel : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Level";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Leaving Hub... Entering the Wild!");
        SceneManager.LoadScene(sceneToLoad);
    }
}