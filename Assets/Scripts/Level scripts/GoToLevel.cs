using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLevel : MonoBehaviour
{
    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector2.Distance(transform.position, player.transform.position) < 0.1f)
        {
            Debug.Log("Leaving Hub... Entering the Wild!");
            SceneManager.LoadScene("Level");
        }
    }
}