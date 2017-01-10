using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Controls the entire game UI.

public class GameHUD : MonoBehaviour
{
	private PlayerController playerController;

	// These are the 'sections' of the UI. They get turned on and off depending on which screen we're on.
	public GameObject UI_Level;
	public GameObject UI_EndLevel;
	public GameObject UI_MainMenu;
	public GameObject UI_Tutorial;

	// Current tutorial.
	public Text textTutorial;
	private Tutorial tutorial;

	// The game HUD.
	public Text textTime;
	public Text textCombo;

	// End level scoring stuff.
	public Text endTextTime;
	public Text endTextPar;
	public Text endTextCollectables;
	public Text endTextCombo;
	public Text endTextRank;

	// Level name displayed at start.
	public Text textLevelName;
	public GameObject panelLevelName;

	// Instructions on the main menu.
	private int currentInstructions = 0;
	[Multiline]
	public string[] instructionsHeadings;
	[Multiline]
	public string[] instructionsBodies;
	public GameObject instructionsSection;
	public Text instructionsHeadingText;
	public Text instructionsBodyText;

	// Level selection menu.
	public GameObject levelsSection;

	public AudioClip levelCompletedSound;

	// Used for various screen effects (fade out, flash, etc.)
	private float deathStart;
	private float deathEnd;
	public Image panelDeathBlackout;
	public Image panelLightningPickup;
	public Image panelRespawnFlash;

	// Only keep one copy of this entity at any time. Just despawn any additional that are created.
	private static bool firstSpawn = false;
	void Start()
	{
		if (firstSpawn)
			DestroyImmediate(gameObject);
		else
			DontDestroyOnLoad(gameObject);

		firstSpawn = true;
	}

	void Awake()
	{
		RefreshUIState();
	}

	public void StartTutorial(Tutorial tut)
	{
		tutorial = tut;
	}

	public void EndTutorial(Tutorial tut)
	{
		if (tutorial == tut)
			EndTutorial();
	}

	public void EndTutorial()
	{
		tutorial = null;
	}
	
	void Update()
	{
		//Screen.lockCursor = !(UI_MainMenu && UI_MainMenu.activeSelf || UI_EndLevel && UI_EndLevel.activeSelf);

		if (UI_Level && UI_Level.activeSelf)
			UpdateLevelUI();

		if (UI_EndLevel && UI_EndLevel.activeSelf)
			UpdateEndLevelUI();

		if (UI_MainMenu && UI_MainMenu.activeSelf)
			UpdateMainMenuUI();

		if (UI_Tutorial)
			UpdateTutorialUI();

		// Toggle menu states when pressing menu button.
		if (Input.GetButtonDown("Menu") && Application.loadedLevelName != "main_menu")
		{
			if (UI_Level && !(UI_EndLevel && UI_EndLevel.activeSelf))
			{
				if (UI_Level.activeSelf)
				{
					UI_Level.SetActive(false);
					UI_MainMenu.SetActive(true);
				}
				else
				{
					UI_Level.SetActive(true);
					UI_MainMenu.SetActive(false);
				}

				RefreshGameState();
			}
		}
	}

	private void UpdateMainMenuUI()
	{
		//...
	}

	private void UpdateTutorialUI()
	{
		if (!playerController)
			return;

		// Show or hide the tutorial box, set the tutorial text depending on the currently active tutorial.
		if (UI_Level.activeSelf)
		{
			if (tutorial)
			{
				textTutorial.text = tutorial.Text;
				UI_Tutorial.SetActive(true);
			}
			else
				UI_Tutorial.SetActive(false);
		}
		else
			UI_Tutorial.SetActive(false);
	}

	public void OnTouchCollectable(GameObject obj)
	{
		// Pulse the combo counter.
		textCombo.rectTransform.localScale = new Vector2(2.0f, 2.0f);

		// Play a screen effect.
		Color col = panelLightningPickup.color;
		col.a = 0.2f;
		panelLightningPickup.color = col;
	}

	public void LevelFinished(float level_completion_time, int collected_collectables, int total_collectables, int droppedCombo)
	{
		if (UI_Level)
			UI_Level.SetActive(false);
		if (UI_MainMenu)
			UI_MainMenu.SetActive(false);

		if (!UI_EndLevel)
			return;

		UI_EndLevel.SetActive(true);

		RefreshGameState();

		if (levelCompletedSound)
			GetComponent<AudioSource>().PlayOneShot(levelCompletedSound);

		// Calculates and displays various ranking information.

		float time_score = Mathf.Clamp(LevelController.currentLevel.ParTime / level_completion_time, 0.0f, 1.0f);
		float collect_score = Mathf.Clamp(collected_collectables / total_collectables, 0.0f, 1.0f);
		float combo_score = Mathf.Max(1.0f - droppedCombo * 0.25f, 0.0f);
		float final_score = time_score + collect_score + combo_score;

		print("Scores: " + time_score + " " + collect_score + " " + combo_score + " FINAL: " + final_score);

		try
		{
			if (collected_collectables >= total_collectables)
			{
				endTextCollectables.text = "GOT ALL COLLECTABLES!";
				endTextCollectables.color = Color.green;
			}
			else
			{
				endTextCollectables.text = "COLLECTABLES MISSED: " + (total_collectables - collected_collectables);
				endTextCollectables.color = Color.red;
			}

			if (droppedCombo == 0)
			{
				endTextCombo.text = "PERFECT COMBO!";
				endTextCombo.color = Color.green;
			}
			else
			{
				endTextCombo.text = "DROPPED COMBO: " + droppedCombo + " TIMES";
				endTextCombo.color = Color.red;
			}

			if (LevelController.currentLevel)
			{
				endTextPar.text = "PAR TIME: " + UtilTime.toMinutesSecondsMilliseconds(LevelController.currentLevel.ParTime);
				endTextPar.color = level_completion_time <= LevelController.currentLevel.ParTime ? Color.green : Color.red;
			}
			else
				endTextPar.text = "";

			endTextTime.text = "TIME: " + UtilTime.toMinutesSecondsMilliseconds(level_completion_time);

			string text = "RANK ";
			if (final_score >= 3.0f)
				text += "S";
			else if (final_score >= 2.5f)
				text += "A";
			else if (final_score >= 2.0f)
				text += "B";
			else if (final_score >= 1.5f)
				text += "C";
			else
				text += "D";

			endTextRank.text = text;
		}
		catch {}
	}

	private void UpdateLevelUI()
	{
		if (playerController == null)
			return;

		// Over time, shrink the level name popup that appears when the game starts.
		/*if (panelLevelName.activeSelf)
		{
			float lnscale = Mathf.Min(2.0f - Mathf.Pow(Time.time - playerController.levelStartTime, 2.0f) * 0.05f, 1.0f);

			if (lnscale <= 0.0f)
				panelLevelName.SetActive(false);
			else
				panelLevelName.transform.localScale = new Vector2(1.0f, lnscale);
		}*/

		// Shrink the combo counter back to normal.
		Vector2 scale = textCombo.rectTransform.localScale;
		scale.x = scale.y = Mathf.Lerp(scale.x, 1.0f, Time.deltaTime * 5.0f);
		textCombo.rectTransform.localScale = scale;

		// Dim the lightning pickup screen effect back to invisible.
		if (panelLightningPickup)
		{
			Color col = panelLightningPickup.color;
			col.a = Mathf.Lerp(col.a, 0.0f, Time.deltaTime * 6.0f);
			panelLightningPickup.color = col;
		}

		// Black and red the screen out when we die.
		if (deathEnd > 0)
		{
			if (Time.time >= deathEnd)
			{
				deathEnd = 0;
				deathStart = 0;
				panelDeathBlackout.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			}
			else
			{
				float deathlength = deathEnd - deathStart;
				float deathdelta = 1.0f - (deathEnd - Time.time) / deathlength;

				panelDeathBlackout.color = new Color(1 - deathdelta, 0, 0, deathdelta);
			}
		}

		panelRespawnFlash.color = new Color(1f, 1f, 1f, Mathf.Lerp(panelRespawnFlash.color.a, 0f, Time.deltaTime));

		// Flash the combo counter when we're about to lose our combo.
		float delta = playerController.levelStartTime > 0.0f ? Time.time - playerController.levelStartTime : 0.0f;
		int combo = playerController.getCombo();
		float combodroppercent = playerController.getComboDropPercent();
		try
		{
			if (Application.loadedLevelName == "nexus")
				textTime.text = "";
			else
				textTime.text = UtilTime.toMinutesSecondsMilliseconds(delta);

			textCombo.text = combo.ToString();

			if (combo == 0)
				textCombo.gameObject.SetActive(false);
			else if (combodroppercent <= 0.33f && Time.time * 10 % 1 < 0.5f)
				textCombo.gameObject.SetActive(false);
			else
				textCombo.gameObject.SetActive(true);
		}
		catch { }
	}

	void OnLevelWasLoaded(int levelid)
	{
		RefreshUIState();

		Time.timeScale = 1.0f;
	}

	public void LevelStarted(Level level)
	{
		//print("level started " + level.Name);
		/*try
		{
			textLevelName.text = "\"" + level.Name + "\"";
		}
		catch {}*/

		//panelLevelName.SetActive(true);

		RefreshUIState();
	}

	private void RefreshUIState()
	{
		string level = Application.loadedLevelName;

		//print("Scene loaded: " + level);

		// Force open / closed some different sections depending on the scene we load.

		if (UI_Level)
			UI_Level.SetActive(level != "main_menu" && level != "splash");
		if (UI_MainMenu)
			UI_MainMenu.SetActive(level == "main_menu");
		if (UI_EndLevel)
			UI_EndLevel.SetActive(false);

		GameObject pl = GameObject.FindGameObjectWithTag("Player");
		if (pl)
			playerController = pl.GetComponent<PlayerController>();

		RefreshGameState();
	}

	private void RefreshGameState()
	{
		// Only show the cursor if we're in the main menu or end level screen.
		bool vis = UI_MainMenu && UI_MainMenu.activeSelf || UI_EndLevel && UI_EndLevel.activeSelf;
		Cursor.visible = vis;
		Cursor.lockState = vis ? CursorLockMode.None : CursorLockMode.Locked;

		// Pause the game if the cursor is visible and we're not on the main menu scene.
		Time.timeScale = vis && Application.loadedLevelName != "main_menu" && Application.loadedLevelName != "splash" ? 0.0f : 1.0f;
	}

	private void UpdateEndLevelUI()
	{
		// Flashy colors and stuff for end level screen.
		endTextRank.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Sin(Time.unscaledTime * 5.0f) * 5.0f);
		endTextRank.color = Color.Lerp(Color.blue, Color.cyan, Mathf.Abs(Mathf.Cos(Time.unscaledTime * 2.0f)));
	}

	public void DoPlayerDeath(DeathType deathtype)
	{
		// Used for tilting the camera and blacking out the screen.
		deathStart = playerController.getStateStart();
		deathEnd = playerController.getStateEnd();
	}

	public void DoRespawn()
	{
		panelRespawnFlash.color = new Color(1f, 1f, 1f, 0.9f);
	}

	void ButtonPressedRetry()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	void ButtonPressedExit()
	{
		Application.LoadLevel("nexus");
	}

	void MMButtonPressedPlay()
	{
		if (Application.loadedLevelName == "main_menu")
		{
			//if (levelsSection)
				//levelsSection.SetActive(true);
			if (instructionsSection)
				instructionsSection.SetActive(false);

			Application.LoadLevel("nexus");
		}
		else
		{
			if (UI_MainMenu)
				UI_MainMenu.SetActive(false);
			if (UI_Level)
				UI_Level.SetActive(true);

			if (levelsSection)
				levelsSection.SetActive(false);
			if (instructionsSection)
				instructionsSection.SetActive(false);

			RefreshGameState();
		}
	}

	void LoadLevel1()
	{
		Application.LoadLevel("level_001");
	}

	void LoadLevel2()
	{
		Application.LoadLevel("level_002");
	}

	void LoadLevel3()
	{
		Application.LoadLevel("level_003");
	}

	void LoadLevel4()
	{
		Application.LoadLevel("level_004");
	}

	void LoadLevel5()
	{
		Application.LoadLevel("level_005");
	}

	void MMButtonPressedInstructions()
	{
		RefreshInstructions();

		if (instructionsSection)
			instructionsSection.SetActive(true);
		if (levelsSection)
			levelsSection.SetActive(false);
	}

	void MMButtonPressedTutorial()
	{
		Application.LoadLevel("tutorial");
	}

	void MMButtonPressedQuit()
	{
		if (Application.loadedLevelName == "main_menu")
		{
			if (!Application.isWebPlayer)
			{
				gameObject.SetActive(false);
				Destroy(gameObject);
				Application.LoadLevel("quit");
			}
		}
		else
			Application.LoadLevel("main_menu");
	}

	void MMButtonPressedInstructionsPrev()
	{
		currentInstructions--;
		if (currentInstructions < 0)
			currentInstructions = instructionsHeadings.Length - 1;

		RefreshInstructions();
	}

	void MMButtonPressedInstructionsNext()
	{
		currentInstructions++;
		if (currentInstructions >= instructionsHeadings.Length)
			currentInstructions = 0;

		RefreshInstructions();
	}

	private void RefreshInstructions()
	{
		if (instructionsHeadingText)
			instructionsHeadingText.text = currentInstructions < instructionsHeadings.Length ? instructionsHeadings[currentInstructions] : "";
		if (instructionsBodyText)
			instructionsBodyText.text = currentInstructions < instructionsBodies.Length ? instructionsBodies[currentInstructions] : "";
	}

	void MMButtonPressedInstructionsClose()
	{
		if (instructionsSection)
			instructionsSection.SetActive(false);
	}

	void MMButtonPressedLevelsClose()
	{
		if (levelsSection)
			levelsSection.SetActive(false);
	}
}
