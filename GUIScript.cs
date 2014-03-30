using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {

	public MazeGeneratorSolver mgs; // Generator reference
	float vSlider_r = 31f; // row slider value
	float vSlider_c = 31f; // column slider value

	void OnGUI() {
		GUI.Label(new Rect(Screen.width/2 - 330,Screen.height/2 - 130,40,30), "Rows");
		GUI.Label(new Rect(Screen.width/2 - 290,Screen.height/2 - 130,60,30), "Columns");

		vSlider_r = GUI.VerticalSlider(new Rect(Screen.width/2 - 320,Screen.height/2 - 100,20,130),vSlider_r,100f,5f);
		vSlider_c = GUI.VerticalSlider(new Rect(Screen.width/2 - 270,Screen.height/2 - 100,20,130),vSlider_c,100f,5f);

		GUI.Label(new Rect(Screen.width/2 - 320,Screen.height/2 + 40,40,30), ((int)vSlider_r).ToString());
		GUI.Label(new Rect(Screen.width/2 - 270,Screen.height/2 + 40,40,30), ((int)vSlider_c).ToString());

		if(GUI.Button(new Rect(Screen.width/2 - 100,Screen.height - 50,80,30),"New Maze")) {
			mgs.height = (int)vSlider_r;
			mgs.width = (int)vSlider_c;
			mgs.NewMaze();
		}

		if(GUI.Button(new Rect(Screen.width/2 + 60,Screen.height - 50,80,30),"Solve")) {
			if(!mgs.solved)
				mgs.SolveMaze();
		}
	}
}
