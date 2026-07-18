using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeMinigameBehavior : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject PlayerImage;
    [SerializeField] private Image CustomerImage;
    [SerializeField] private SpriteRenderer CustomerSpriteRenderer;
    [SerializeField] private GameObject OrderingUI;
    [SerializeField] private TextMeshProUGUI DialogueTextBox;
    [SerializeField] private CharacterAnimations anims;
    private Dictionary<string, RuntimeAnimatorController> _animations;
    [Header("Camera")]
    [SerializeField] private GameObject Cam;

    private DialogueSequence _currentDialogueSequence;
    private int _currentDialogueSequenceIndex;


    private void Awake()
    {
        _animations = anims.ConvertListToDictionary();
    }
    private void OnEnable()
    {
        OrderingUI.SetActive(false);
        _currentDialogueSequence = CustomerManager.Instance.CurrentCustomer.GetDialogueSequence();
        _currentDialogueSequenceIndex = 0;
        DialogueTextBox.text = _currentDialogueSequence.GetDialgoueLine(_currentDialogueSequenceIndex);
        PlayerImage.GetComponent<Animator>().runtimeAnimatorController = _animations[PlayerPrefs.GetString("PlayerType")];
        PlayerImage.GetComponent<Animator>().SetInteger("Direction", 0);
    }

    private void OnDisable()
    {
        OrderingUI.SetActive(true);
    }

    private void Update()
    {
        CustomerImage.sprite = CustomerSpriteRenderer.sprite;
        PlayerImage.GetComponent<Image>().sprite = PlayerImage.GetComponent<SpriteRenderer>().sprite;
    }

    /// <summary>
    /// Display the next entry within the dialgoue sequence if we are in bounds (++index < number of lines). Return to gameplay if not
    /// </summary>
    public void ProceedToNextDialogueEntry()
    {
        if(_currentDialogueSequenceIndex < _currentDialogueSequence.GetDialgoueSequenceLength() - 1)
        {
            _currentDialogueSequenceIndex++;
            DialogueTextBox.text = _currentDialogueSequence.GetDialgoueLine(_currentDialogueSequenceIndex);
        }

        else
        {
            CustomerManager.Instance.CurrentCustomer.ProceedToNextDay();
            LeaveNarrative();
            Debug.Log("Reached The end of the dialgoue sequence for the current customer");
        }
    }

    public void LeaveNarrative()
    {
        if (!SoundManager.TutorialIsPlaying())
        {
            UIManager.Instance.SendBackToCatCamera(Cam);
            CustomerManager.Instance.CurrentCustomer.CompleteTakingTheCustomerOrder();
        }
    }
}
