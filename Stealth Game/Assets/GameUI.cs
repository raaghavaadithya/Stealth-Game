using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
    public GameObject gameWinUI;
    public GameObject gameLoseUI;
    bool gameOver = false;

    void Start() {
        Gaurd.OnGuardHasSpottedPlayer += showGameLoseUI;
        Player.OnReachedFinishPoint += showGameWinUI;
    }

    // Update is called once per frame
    void Update() {
        if(gameOver) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(0);
            }
        }
    }

    void showGameWinUI() {
        showGameOverUI(gameWinUI);
    }

    void showGameLoseUI() {
        showGameOverUI(gameLoseUI);
    }

    void showGameOverUI(GameObject GameUI) {
        GameUI.SetActive(true);
        gameOver = true;
        Gaurd.OnGuardHasSpottedPlayer -= showGameLoseUI;
        Player.OnReachedFinishPoint -= showGameWinUI;
    }
}
