using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {
	public Animator loadingAnimation;
	public InstantGuiButton start_game_button;
    private bool loadScene = false;
    private float time_passed = 0f;

    [SerializeField]
    private int scene;
    [SerializeField]
    private Text loadingText;
    private int loadProgress;


    // Updates once per frame
    void Update() {
    	if(start_game_button.activated && loadScene == false){
    		// ...set the loadScene boolean to true to prevent loading a new scene more than once...
            loadScene = true;
    		StartCoroutine(LoadNewScene());
    	}

        // If the new scene has started loading...
        if (loadScene == true) {
        	time_passed += Time.deltaTime;
        	int text_choice = (int)((time_passed * 10 / 3) % 3);
            // ...let the player know that the computer is still working.
            if(text_choice == 0){
	            loadingText.text = "L o a d i n g  .";
	        } else if (text_choice == 1){
	            loadingText.text = "L o a d i n g  .  .";
	        } else {
	        	loadingText.text = "L o a d i n g  .  .  .";
	        }

        }

    }


    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadNewScene() {
    	loadingAnimation.SetBool("loading_started", true);
        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(3);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = Application.LoadLevelAsync(scene);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone) {
        	loadProgress = (int)(async.progress * 100);
            yield return null;
        }

    }

}
