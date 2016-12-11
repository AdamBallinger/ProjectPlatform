README
---------

REQUIREMENTS:
	- Unity v5.5.0f3 - Minimum as game uses new LineRenderer system in 5.5 update.
		- This is also the version the project is developed in.

Running the game
------------------
- Open the provided executable, and press the level editor button to enter the level editor.

- Once in the level editor, you can either create your own level using the tools on left of the screen,
or load one of the provided levels using the Load Level button.

- To play the level you have created/loaded, click on the play button on the left of the screen.

- To return to the editor you can press the return to editor button in the top left corner of the screen.


Using the editor
------------------
- When creating/modifying a level, press the ">" buttons next to the buttons that have them to open any
additional settings for that object. 

- To remove bounce pads and coins simple make sure you press the respective objects button on the left
and click on a position with a pad/coin to add/remove it.

- Any levels you create, or any of the provided levels you change can be loaded by pressing the load level
button on the left of the screen, and choosing the Saved Levels button on the right side of the prompt that
appears. 

- To delete a saved level press the X button next to the level you want to delete. (Only saved levels can
be deleted and not provided levels)

- To create a test path for an AI inside of the editor, press the pathfinding button and then Create Test Path.
	- This will create an AI in the editor scene if one didn't already exist.
	- Select the start position for the AI (must be a node at this position)
	- Then select the end position for the AI (must also be a node at this position).
	- The AI will be moved to the start and will begin following the path if a valid path could be found.
	- To create a new path you don't need to select the create test path button again, just select a start and end.
	- The Clear Path button will clear the AI's path and delete the AI object from the scene.


Finding the code
-------------------
- All physics code is found in "Assets/Scripts/Physics"
	- The physics step is found in "/PhysicsWorld.cs"
	- Collision Detection/Resolution is found in "/Colliders/CollisionManifold.cs"
	- Collision callback system is found in "/CollisionListener.cs". This contains the logic for various collision callbacks etc.
	- PhysicsWorld step function is called from Unity's FixedUpdate for "WorldController.cs" which is found in the "General" folder.
	- Code for integrating Unity with the physics code is found in "Assets/Scripts/General/UnityLayer/Physics Components"
	
- All AI code is found in "Assets/Scripts/AI/Pathfinding"
	- "/NavGraph.cs" contains the code for constructing the nodes and creating the various links between them.
	- "/Pathfinder.cs" contains the A* code for finding a path from a given start and end position. This code is threaded.
	- The code for making an AI follow a created path is found in "Assets/Scripts/General/UnityLayer/AI/PathfinderAgent.cs"