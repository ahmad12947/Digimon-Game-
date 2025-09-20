using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class DialogueManager : MonoBehaviour
{
    [Header("UI Panels")]
    public Text dialogueText;
    public GameObject textPanel;
    public GameObject choicePanel;
    public GameObject nameInputPanel;

    [Header("Choices")]
    public Button[] choiceButtons;

    [Header("Name Input")]
    public Text nameDisplayText; // shows the typed name
    public Button backspaceButton;
    public Button okButton;
    public Transform alphabetGrid; // parent object containing alphabet buttons
    private Button[] alphabetButtons;

    private string playerName = "";
    private int currentStepIndex = 0;
    public DialogueStep[] steps;

    private bool awaitingChoice = false;
    private bool choiceTextShown = false;
    private bool nameTextShown = false;   // flag for two-phase name input
    private bool skipSubmitThisFrame = false;
    void Start()
    {
        SetupNameInputPanel();
        ShowStep(0);
    }
    void Update()
    {
        if (skipSubmitThisFrame)
        {
            skipSubmitThisFrame = false;
            return;
        }

        //  If the name input panel is open, ignore global Submit handling
        if (nameInputPanel.activeSelf)
            return;

        if (Input.GetButtonDown("Submit"))
        {
            DialogueStep step = steps[currentStepIndex];

            if (step.type == DialogueType.Text)
            {
                NextStep();
            }
            else if (step.type == DialogueType.Choice)
            {
                if (!choiceTextShown)
                {
                    ShowChoiceText(step);
                    choiceTextShown = true;
                }
                else if (!awaitingChoice)
                {
                    ShowChoices(step);
                    awaitingChoice = true;
                }
            }
            else if (step.type == DialogueType.NameInput)
            {
                if (!nameTextShown)
                {
                    ShowNameQuestion(step);
                    nameTextShown = true;
                }
                else
                {
                    ShowNamePanel();
                }
            }
        }
    }

    void ShowStep(int index)
    {
        currentStepIndex = index;
        DialogueStep step = steps[index];

        // Reset all panels
        textPanel.SetActive(false);
        choicePanel.SetActive(false);
        nameInputPanel.SetActive(false);

        awaitingChoice = false;
        choiceTextShown = false;
        nameTextShown = false;

        if (step.type == DialogueType.Text)
        {
            textPanel.SetActive(true);
            dialogueText.text = step.speaker + ": " + ReplaceName(step.text);
        }
        else if (step.type == DialogueType.Choice)
        {
            ShowChoiceText(step);
            choiceTextShown = true;
        }
        else if (step.type == DialogueType.NameInput)
        {
            // Start with just showing the dialogue text
            ShowNameQuestion(step);
            nameTextShown = true;
        }
    }

    void ShowChoiceText(DialogueStep step)
    {
        textPanel.SetActive(true);
        choicePanel.SetActive(false);
        dialogueText.text = step.speaker + ": " + ReplaceName(step.text);
    }

    void ShowChoices(DialogueStep step)
    {
        textPanel.SetActive(false);
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < step.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<Text>().text = step.choices[i];

                int choiceIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void ShowNameQuestion(DialogueStep step)
    {
        textPanel.SetActive(true);
        dialogueText.text = step.speaker + ": " + ReplaceName(step.text);
    }

    void ShowNamePanel()
    {
        textPanel.SetActive(false);
        nameInputPanel.SetActive(true);
        playerName = "";
        UpdateNameDisplay();
    }

    void NextStep()
    {
        currentStepIndex++;
        if (currentStepIndex < steps.Length)
        {
            ShowStep(currentStepIndex);
        }
        else
        {
            Invoke("loadScene", 3);
            Debug.Log("Dialogue finished!");
        }
    }

    void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log("Player chose: " + steps[currentStepIndex].choices[choiceIndex]);
        awaitingChoice = false;
        choiceTextShown = false;

        skipSubmitThisFrame = true; //  prevent double NextStep
        NextStep();
    }
    // -------------------------
    // NAME INPUT PANEL LOGIC
    // -------------------------
    void SetupNameInputPanel()
    {
        alphabetButtons = alphabetGrid.GetComponentsInChildren<Button>();

        foreach (Button btn in alphabetButtons)
        {
            string letter = btn.GetComponentInChildren<Text>().text;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnLetterPressed(letter));
        }

        backspaceButton.onClick.RemoveAllListeners();
        backspaceButton.onClick.AddListener(OnBackspace);

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(OnConfirmName);
    }

    void OnLetterPressed(string letter)
    {
        if (playerName.Length < 12)
        {
            playerName += letter;
            UpdateNameDisplay();
        }
    }

    void OnBackspace()
    {
        if (playerName.Length > 0)
        {
            playerName = playerName.Substring(0, playerName.Length - 1);
            UpdateNameDisplay();
        }
    }


    void OnConfirmName()
{
    Debug.Log("Player entered name: " + playerName);

    // Close name panel, reopen dialogue text
    nameInputPanel.SetActive(false);
    textPanel.SetActive(true);

    skipSubmitThisFrame = true; //  prevent double NextStep
    NextStep();
}
    void UpdateNameDisplay()
    {
        nameDisplayText.text = playerName;
    }


    public void loadScene()
    {
        SceneManager.LoadScene(1);
    }
    string ReplaceName(string text)
    {
        if (string.IsNullOrEmpty(playerName))
            return text;
        return text.Replace("__", playerName);
    }
}
