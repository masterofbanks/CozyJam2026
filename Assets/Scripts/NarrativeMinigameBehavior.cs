using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeMinigameBehavior : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image PlayerImage;
    [SerializeField] private Image CustomerImage;
    [SerializeField] private SpriteRenderer CustomerSpriteRenderer;
    [SerializeField] private GameObject OrderingUI;
    [SerializeField] private TextMeshProUGUI DialogueTextBox;

    [Header("Camera")]
    [SerializeField] private GameObject Cam;

    private DialogueSequence _currentDialogueSequence;
    private int _currentDialogueSequenceIndex;
    private void OnEnable()
    {
        OrderingUI.SetActive(false);
        _currentDialogueSequence = CustomerManager.Instance.CurrentCustomer.GetDialogueSequence();
        _currentDialogueSequenceIndex = 0;
        DialogueTextBox.text = _currentDialogueSequence.GetDialgoueLine(_currentDialogueSequenceIndex);
    }

    private void OnDisable()
    {
        OrderingUI.SetActive(true);
    }

    private void Update()
    {
        CustomerImage.sprite = CustomerSpriteRenderer.sprite;
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
