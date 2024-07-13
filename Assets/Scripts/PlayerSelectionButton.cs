using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionButton : MonoBehaviour
{
	public GameObject characterPrefab;
	public int characterIndex;
	public CharacterSelectManager characterSelectManager;

	private void Start()
	{
		Button button = GetComponent<Button>();
		button.onClick.AddListener(OnButtonClick);

		if (characterSelectManager == null)
		{
			characterSelectManager = CharacterSelectManager.Instance;
		}
	}

	private void OnButtonClick()
	{
		if (characterSelectManager != null)
		{
			characterSelectManager.SelectCharacter(characterPrefab, characterIndex);
		}
	}
}
