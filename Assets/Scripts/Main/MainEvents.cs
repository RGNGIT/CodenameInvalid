using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainEvents : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene((int)Constants.EAdditiveScene.Controls, LoadSceneMode.Additive);
    }
}
