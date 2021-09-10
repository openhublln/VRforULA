========================================================
Facilitating the Selection of a Myoelectric Prosthesis 
Using Virtual Reality for UpperLimb Amputee Patients
A Proof of Concept
June 2020
========================================================

CONTENT:

Only the most important files are detailed here. 
For additional questions of use, please contact the authors.
Jeanne Evrard and Gregoire van Oldeneel

•UnityProject > Assets: the Unity project used for development
	–Images: The images used for the scenesMainMenuandThresholdAdjust-ment.
	–Scenes: The four scenes: MainMenu,ProsthesisSelection,TestRoomandThresholdAdjustment
	–Prostheses
		∗ForeArm: The model of the Forearm.
		∗Greifer: All the files used for the Greifer prosthesis.
			·GreiferSlide: Slides displayed for explanation and tasks.
			·PrefabGreifer: All files for the prosthesis GO and control.
		∗Bebionic: Content hierarchy similar to the Greifer file.
		∗ILimb: Content hierarchy similar to the Greifer file.
		∗Audio: Contains all files for playing sounds (used for auditory feedback)
	–Scripts: All the scripts, except the ones for prosthesis control
	–SteamVR, SteamVR_Input, StreamingAssets: imported SteamVR assets. Required for VR.
	–_VRHands: Imported package for the model of the left hand.

•UnityApp > Master_Thesis_VRforULA_june2020.exe: The executable file that contains the project

•ArduinoInterface: Script to load on the Arduino micro-controller for inter-facing the electrodes.

•Videos: The presentation videos of the different scenes and tasks.

========================================================

REMARK: Be sure that the VR material and Arduino setup are well connected before trying to launch the game.
	Trackers should be attached to the right game object.
	By default, ForeArm is attached to device 3; LeftHand is attached to device 4. 
	This can be changed inside the Unity project:
	scene "TestRoom", Hierarchy:
		> LeftHand GameObject > Steam VR_Tracked Object (Script) component > Index
		> ForeArm GameObject > Steam VR_Tracked Object (Script) component > Index 