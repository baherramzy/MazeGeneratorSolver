using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGeneratorSolver : MonoBehaviour {
	// Maze variables
	public int width = 5;
	public int height = 5;

	private int[,] maze;
	private int[] directions = {1,2,3,4}; // North, East, South, West
	private System.Random _random = new System.Random();

	// Game variables
	public GameObject Maze_parent;
	public GameObject Path_parent;
	public GameObject Wall_obj;
	public GameObject Start_obj;
	public GameObject End_obj;
	public GameObject Path_obj;
	public bool solved = false;

	private int start_r, end_r;
	private int start_c, end_c;
	private Stack<Vector3> solutionPath = new Stack<Vector3>();
	private bool [,] visited;
	
	void Start () {
		// Run maze generator
		GenerateMaze(1,1);

		// Show visual representation of the maze
		for(int i = 0; i < height; ++i) {
			for(int j = 0; j < width; ++j) {
				if(maze[i,j] == 1) {
					// instantiate wall object at [i,j] position
					GameObject wall = Instantiate(Wall_obj, new Vector3(i, 0, j), Quaternion.identity) as GameObject;

					// assign wall as a child of the Maze parent GameObject
					wall.transform.parent = Maze_parent.transform;
				}
			}
		}
	}

	void GenerateMaze(int ir, int ic) {
		// Create maze
		maze = new int[height, width];

		// starting point co-ordinates
		start_r = ir;
		start_c = ic;

		// initialize end point (to be determined by DFS)
		end_r = end_c = 1;

		// Clear solution path
		solutionPath.Clear();

		// Initialize visited array for solution finding
		visited = new bool[height,width];

		// Initialize all cells to walls (value = 1)
		for(int i = 0; i < height; ++i)
			for(int j = 0; j < width; ++j)
				maze[i,j] = 1;

		// Center camera
		Camera.main.transform.position = new Vector3(height/2,0,width/2) + Vector3.up;
		Camera.main.orthographicSize = Mathf.Max(height,width)/1.5f;

		// "Destroy" wall at starting point (value = 0)
		maze[start_r,start_c] = 0;

		// Create maze using a Depth First Search (DFS) algorithm
		// under the condition that the end point is not a wall
		DFS(start_r,start_c); 
		while(maze[end_r,end_c] == 1) {
			NewMaze(); // Create a new maze
		}

		//Instantiate start object at starting position
		Instantiate(Start_obj, new Vector3(start_r,0,start_c),Quaternion.identity);

		//Instantiate end object at ending position (top right corner of maze, determined by DFS)
		Instantiate(End_obj, new Vector3(end_r,0,end_c),Quaternion.identity);
	}

	void DFS(int r, int c) {
		/*Directions:
		 * 1 - North
		 * 2 - East
		 * 3 - South
		 * 4 - West
		 */

		Shuffle(directions); // shuffle directions array in every recursive call for maze pattern diversity

		for(int i = 0; i < directions.Length; ++i) {
			switch(directions[i])
			{
			case 1: //North
				if(r - 2 <= 0) // if direction out of maze, proceed to next direction
					continue;
				if(maze[r - 2,c] != 0)  // if wall present two cells away, "destroy" path to it
				{
					maze[r - 2, c] = 0;
					maze[r - 1, c] = 0;
					end_r = Mathf.Max(end_r, r - 2);
					end_c = Mathf.Max(end_c, c);
					DFS (r - 2, c); // recurse with new path 
				}
				break;

			case 2: //East
				if(c + 2 >= this.width - 1) // if direction out of maze, proceed to next direction
					continue;
				if(maze[r, c + 2] != 0) // if wall present two cells away, "destroy" path to it
				{ 
					maze[r, c + 2] = 0;
					maze[r, c + 1] = 0;
					end_r = Mathf.Max(end_r, r);
					end_c = Mathf.Max(end_c, c + 2);
					DFS (r, c + 2); // recurse with new path 
				}
				break;

			case 3: //South
				if(r + 2 >= this.height - 1) // if direction out of maze, proceed to next direction
					continue;
				if(maze[r + 2,c] != 0) // if wall present two cells away, "destroy" path to it
				{ 
					maze[r + 2, c] = 0;
					maze[r + 1, c] = 0;
					end_r = Mathf.Max(end_r, r+2);
					end_c = Mathf.Max(end_c, c);
					DFS (r + 2, c); // recurse with new path 
				}
				break;

			case 4: //West
				if(c - 2 <= 0) // if direction out of maze, proceed to next direction
					continue;
				if(maze[r, c - 2] != 0) // if wall present two cells away, "destroy" path to it
				{
					maze[r, c - 2] = 0;
					maze[r, c - 1] = 0;
					end_r = Mathf.Max(end_r, r);
					end_c = Mathf.Max(end_c, c - 2);
					DFS (r, c - 2); // recurse with new path 
				}
				break;
			}
		}
	}

	void Shuffle(int[] array) {
		for(int i = array.Length; i > 1; --i) {
			// retrieve a random number between array length and 1
			int j = _random.Next(i); 

			// swap
			int temp = array[j];
			array[j] = array[i - 1];
			array[i - 1] = temp;
		}
	}

	public void NewMaze() {
		// Retrieve all instantiated game objects and destroy them
		GameObject[] children = GameObject.FindGameObjectsWithTag("Child");
		foreach(GameObject child in children) {
			Destroy(child);
		}

		// Set solved to false
		solved = false;

		// Re-start the script
		Start ();
	}

	public void SolveMaze() {
		solved = true;

		FindPath(start_r, start_c);
		
		Vector3[] arr = solutionPath.ToArray();
		
		for(int i = 0; i < arr.Length; ++i) {
			GameObject obj = Instantiate(Path_obj,arr[i],Quaternion.identity) as GameObject;
			obj.transform.parent = Path_parent.transform;
		}
	}

	bool FindPath(int r, int c) { // call initially with starting r,c

		// base cases
		if(r < 0 || r >= height || 
		   c < 0 || c >= width ||
		   visited[r,c] == true ||
		   maze[r,c] == 1) 
			return false;

		// target condition
		if(r == end_r && c == end_c)
			return true;

		// mark cell as visited and add it to the solution path
		visited[r,c] = true;
		solutionPath.Push(new Vector3(r,0,c));

		if(FindPath(r, c + 1)) return true;
		if(FindPath(r + 1, c)) return true;
		if(FindPath(r, c - 1)) return true;
		if(FindPath(r - 1, c)) return true;

		// if none of the four directions have returned successfully,
		// remove current cell from solution path and return false
		solutionPath.Pop();
		return false;
	}
}