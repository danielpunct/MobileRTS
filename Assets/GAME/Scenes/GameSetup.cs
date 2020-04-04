using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR && !SERVER
        SceneManager.LoadScene("Client", LoadSceneMode.Additive);
#endif

#if !UNITY_EDITOR && SERVER
        SceneManager.LoadScene("Server", LoadSceneMode.Additive);
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }
}