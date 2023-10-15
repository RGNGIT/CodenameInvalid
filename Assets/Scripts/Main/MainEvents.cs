using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainEvents : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene((int)Constants.EAdditiveScene.Controls, LoadSceneMode.Additive);
    }
}
