using UnityEngine;

public class UserCommand
{
	//public int ServerFrame;

	// Movement axis
	public float MoveForward, MoveRight;

	// Look direction
	public float Yaw, Pitch;

	// Buttons down and buttons pressed this frame
	public UserKeys Down, Pressed, Last;

	/*public float age {
		get {
			return Mathf.Clamp((float)(BoltNetwork.serverFrame - ServerFrame) * BoltNetwork.frameDeltaTime, 0f, 0.5f);
		}
	}*/

	public UserCommand()
	{
		Down = new UserKeys();
		Pressed = new UserKeys();
		Last = new UserKeys();
	}

	public void Clear()
	{
		ClearMovement();
		ClearDown();
		ClearPressed();
	}

	public void ClearMovement()
	{
		MoveForward = MoveRight = 0f;
	}

	public void ClearDown()
	{
		Down.Button1 =
		Down.Button2 =
		Down.Jump =
		Down.Use =
		Down.Dash = false;
	}

	public void ClearPressed()
	{
		Pressed.Button1 =
		Pressed.Button2 = 
		Pressed.Jump = 
		Pressed.Use = 
		Pressed.Dash = false;
	}

	public void RefreshPressedStates()
	{
		Pressed.Button1 = !Last.Button1 && Down.Button1;
		Pressed.Button2 = !Last.Button2 && Down.Button2;
		Pressed.Jump = !Last.Jump && Down.Jump;
		Pressed.Dash = !Last.Dash && Down.Dash;
		Pressed.Use = !Last.Use && Down.Use;

		Last.Button1 = Down.Button1;
		Last.Button2 = Down.Button2;
		Last.Jump = Down.Jump;
		Last.Dash = Down.Dash;
		Last.Use = Down.Use;
	}
}

public class UserKeys
{
	public bool
	Button1,
	Button2,
	Jump,
	Use,
	Dash;
}
