using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour {

	public Animator animator;

	private int levelToLoad;
	
	// Update is called once per frame
	//void Update () {
	//	if (Input.GetMouseButtonDown(0))
	//	{
	//		FadeToNextLevel();
	//	}
	//}

	//public void FadeToNextLevel ()
	//{
	//	FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
	//}
	public void FadeOut() {
		animator.SetTrigger("FadeOut");
	}
	public void FadeToLevel (int levelIndex)
	{
		levelToLoad = levelIndex;
		animator.SetTrigger("FadeOut");
	}

	public void OnFadeComplete ()
	{
		SceneManager.LoadScene(levelToLoad);
	}

	public void CloseGame()
    {
		Debug.Log("Add Nodal window to confirm choice"); //TODO Add Nodal window to confirm choice
		Application.Quit();

    }
}
