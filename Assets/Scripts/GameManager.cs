using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // data
    [SerializeField] private int scoreCounter;
    [SerializeField] private List<CharacterData> characterData;
    [SerializeField] private List<String> preludeDialogue;
    [SerializeField] private List<Sprite> spriteOneData;
    [SerializeField] private List<Sprite> spriteTwoData;
    [SerializeField] private List<Sprite> spriteThreeData;
    [SerializeField] private List<AudioClip> soundData;
    [SerializeField] private bool firstCase;

    // dialogue
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI pageZeroText;
    [SerializeField] private TextMeshProUGUI pageOneTextOne; // summary
    [SerializeField] private TextMeshProUGUI pageOneTextTwo; // traits
    [SerializeField] private TextMeshProUGUI pageTwoTextOne; // indictment
    [SerializeField] private TextMeshProUGUI pageTwoTextTwo; // proof
    [SerializeField] private GameObject dialogueAnchor;

    // dialogue data and fields
    private Coroutine typingText;
    [SerializeField] private bool currentlyTyping;
    [SerializeField] private float textRevealSpeed;
    [SerializeField] private int chosenCharacter = 0;
    [SerializeField] private int dialogueStageNumber = 0;
    [SerializeField] private int dialogueLineNumber = 0;
    [SerializeField] private bool typedOnce;

    // buttons
    [SerializeField] private TextMeshProUGUI buttonOneText;
    [SerializeField] private TextMeshProUGUI buttonTwoText;
    [SerializeField] private GameObject questionButtonOne;
    [SerializeField] private GameObject questionButtonTwo;
    [SerializeField] private bool buttonOnePressed;
    [SerializeField] private bool buttonTwoPressed;

    // files (left side)
    [SerializeField] private GameObject pageZeroAnchor;
    [SerializeField] private GameObject pageOneAnchor;
    [SerializeField] private GameObject pageTwoAnchor;

    // character
    [SerializeField] private GameObject character;

    // clock
    [SerializeField] private GameObject clockHand;

    //timers
    [SerializeField] private float mainTimer;
    [SerializeField] private bool mainTimerStarted;
    [SerializeField] private float mainTimerLimit;
    [SerializeField] private float movementTimer;
    [SerializeField] private bool movementTimerStarted;

    void Awake()
    {
        // data
        scoreCounter = 0;
        dialogueStageNumber = 0;
        preludeDialogue.Add("Ehh... kolejny dzie� w tej pracy...");
        preludeDialogue.Add("I jeszcze tyle spraw dzisiaj!");
        preludeDialogue.Add("Jutro pi�tek, wi�c b�d� musia�a si� po�pieszy� z tymi wyrokami.");
        preludeDialogue.Add("Dobra, co my tutaj mamy...");
        firstCase = true;

        // dialogue
        dialogueText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        pageZeroText = GameObject.Find("PageZeroText").GetComponent<TextMeshProUGUI>();
        pageOneTextOne = GameObject.Find("PageOneTextOne").GetComponent<TextMeshProUGUI>();
        pageOneTextTwo = GameObject.Find("PageOneTextTwo").GetComponent<TextMeshProUGUI>();
        pageTwoTextOne = GameObject.Find("PageTwoTextOne").GetComponent<TextMeshProUGUI>();
        pageTwoTextTwo = GameObject.Find("PageTwoTextTwo").GetComponent<TextMeshProUGUI>();
        dialogueAnchor = GameObject.Find("DialogueBox");
        textRevealSpeed = .01f;
        typedOnce = false;

        // buttons
        buttonOneText = GameObject.Find("ButtonOneText").GetComponent<TextMeshProUGUI>();
        buttonTwoText = GameObject.Find("ButtonTwoText").GetComponent<TextMeshProUGUI>();
        questionButtonOne = GameObject.Find("QuestionButtonOne");
        questionButtonTwo = GameObject.Find("QuestionButtonTwo");

        // files
        pageZeroAnchor = GameObject.Find("PageZero");
        pageOneAnchor = GameObject.Find("PageOne");
        pageTwoAnchor = GameObject.Find("PageTwo");

        //character
        character = GameObject.Find("CharacterSprite");

        // clock
        clockHand = GameObject.Find("ClockHand");

        //timers
        mainTimer = 0f;
        mainTimerLimit = 120f;
        movementTimer = 0;
                
    }

    void Start()
    {
        //chosenCharacter = UnityEngine.Random.Range(0, characterData.Count);
        Debug.Log(chosenCharacter);
        chosenCharacter = 0; // RANDOMIZE IT LATER (0;charCount)
        SetTextField(pageZeroText, characterData[chosenCharacter].characterName);
        SetTextField(pageOneTextOne, characterData[chosenCharacter].characterFilesSummary);
        SetTextField(pageOneTextTwo, characterData[chosenCharacter].characterFilesTraits);
        SetTextField(pageTwoTextOne, characterData[chosenCharacter].characterFilesIndictment);
        SetTextField(pageTwoTextTwo, characterData[chosenCharacter].characterFilesProof);
    }

    void Update()
    {
        if(mainTimerStarted)
        {
            clockHand.transform.rotation = Quaternion.Euler(0, 0, (360-mainTimer)*(360/mainTimerLimit));
        }

        movementTimer += Time.deltaTime;
        mainTimer += Time.deltaTime;
        switch (dialogueStageNumber)
        {
            case 0:
                questionButtonOne.SetActive(false);
                questionButtonTwo.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, preludeDialogue[dialogueLineNumber++], textRevealSpeed));
                    }
                }
                if ((dialogueLineNumber > preludeDialogue.Count - 1) && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 1;
                }
                break;
            case 1:
                if(!firstCase && !movementTimerStarted)
                {
                    movementTimerStarted = true;
                    movementTimer = 0;
                    StartCoroutine(MoveTo(pageZeroAnchor.transform, new Vector3(0, 0, 0), .5f));
                    StartCoroutine(MoveTo(pageOneAnchor.transform, new Vector3(0, 0, 0), .5f));
                    StartCoroutine(MoveTo(pageTwoAnchor.transform, new Vector3(0, 0, 0), .5f));
                    MoveCharacterAwayFromScreen();
                    // load sprite
                    SetTextField(pageZeroText, characterData[chosenCharacter].characterName);
                    SetTextField(pageOneTextOne, characterData[chosenCharacter].characterFilesSummary);
                    SetTextField(pageOneTextTwo, characterData[chosenCharacter].characterFilesTraits);
                    SetTextField(pageTwoTextOne, characterData[chosenCharacter].characterFilesIndictment);
                    SetTextField(pageTwoTextTwo, characterData[chosenCharacter].characterFilesProof);
                }
                if((movementTimerStarted && movementTimer > .75f) || !movementTimerStarted)
                {
                    firstCase = false;
                    StartCoroutine(MoveTo(pageZeroAnchor.transform, new Vector3(630, 0, 0), .5f));
                    StartCoroutine(MoveTo(pageOneAnchor.transform, new Vector3(630, 0, 0), .5f));
                    StartCoroutine(MoveTo(pageTwoAnchor.transform, new Vector3(630, 0, 0), .5f));
                    StartCoroutine(MoveTo(dialogueAnchor.transform, new Vector3(242, 0, 0), .5f));
                    MoveCharacterOnScreen();
                    dialogueStageNumber = 2;
                    movementTimerStarted = false;
                }
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.Space) || !typedOnce)
                {
                    typedOnce = true;
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, characterData[chosenCharacter].introDialogue[dialogueLineNumber++], textRevealSpeed));
                    }
                }
                if ((dialogueLineNumber > characterData[chosenCharacter].introDialogue.Count - 1) && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 3;
                    mainTimer = 0;
                    mainTimerStarted = true;
                }
                break;
            case 3:
                if(mainTimerStarted && mainTimer > mainTimerLimit)
                {
                    dialogueStageNumber = 9;
                    typedOnce = false;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, characterData[chosenCharacter].entryDialogue[dialogueLineNumber++], textRevealSpeed));
                    }
                }
                if ((dialogueLineNumber > characterData[chosenCharacter].entryDialogue.Count - 1) && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 4;
                    typedOnce = false;
                }
                break;
            case 4:
                if (mainTimerStarted && mainTimer > mainTimerLimit)
                {
                    dialogueStageNumber = 9;
                    typedOnce = false;
                }
                if (Input.GetKeyDown(KeyCode.Space) && !typedOnce)
                {
                    typedOnce = true;
                    dialogueText.gameObject.SetActive(false);
                    questionButtonOne.SetActive(true);
                    questionButtonTwo.SetActive(true);
                    StartCoroutine(UpdateTextField(buttonOneText, characterData[chosenCharacter].questionOne, textRevealSpeed * 0.01f));
                    StartCoroutine(UpdateTextField(buttonTwoText, characterData[chosenCharacter].questionTwo, textRevealSpeed * 0.01f));
                }
                if (buttonOnePressed)
                {
                    dialogueStageNumber = 5;
                    typedOnce = false;
                }
                if (buttonTwoPressed)
                {
                    dialogueStageNumber = 6;
                    typedOnce = false;
                }
                break;
            case 5:
                if (mainTimerStarted && mainTimer > mainTimerLimit)
                {
                    dialogueStageNumber = 9;
                    typedOnce = false;
                }
                dialogueText.gameObject.SetActive(true);
                questionButtonOne.SetActive(false);
                questionButtonTwo.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Space) || !typedOnce)
                {
                    typedOnce = true;
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, characterData[chosenCharacter].fullQuestionOne, textRevealSpeed));
                    }
                }
                if (typedOnce && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 7;
                }
                break;
            case 6:
                if (mainTimerStarted && mainTimer > mainTimerLimit)
                {
                    dialogueStageNumber = 9;
                    typedOnce = false;
                }
                dialogueText.gameObject.SetActive(true);
                questionButtonOne.SetActive(false);
                questionButtonTwo.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Space) || !typedOnce)
                {
                    typedOnce = true;
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, characterData[chosenCharacter].fullQuestionTwo, textRevealSpeed));
                    }
                }
                if (typedOnce && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 7;
                }
                break;
            case 7:
                if (mainTimerStarted && mainTimer > mainTimerLimit)
                {
                    dialogueStageNumber = 9;
                    typedOnce = false;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, characterData[chosenCharacter].responseOneDialogue[dialogueLineNumber++], textRevealSpeed));
                    }
                }
                if ((dialogueLineNumber > characterData[chosenCharacter].responseOneDialogue.Count - 1) && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 9;
                    typedOnce = false;
                }
                break;
            case 8:
                if (mainTimerStarted && mainTimer > mainTimerLimit)
                {
                    dialogueStageNumber = 9;
                    typedOnce = false;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, characterData[chosenCharacter].responseTwoDialogue[dialogueLineNumber++], textRevealSpeed));
                    }
                }
                if ((dialogueLineNumber > characterData[chosenCharacter].responseTwoDialogue.Count - 1) && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 9;
                    typedOnce = false;
                }
                break;
            case 9:
                if ((Input.GetKeyDown(KeyCode.Space) || (mainTimerStarted && mainTimer > mainTimerLimit)) && !typedOnce)
                {
                    typedOnce = true;
                    dialogueText.gameObject.SetActive(false);
                    questionButtonOne.SetActive(true);
                    questionButtonTwo.SetActive(true);
                    StartCoroutine(UpdateTextField(buttonOneText, "W I N N Y", textRevealSpeed * 0.01f));
                    StartCoroutine(UpdateTextField(buttonTwoText, "� A S K A", textRevealSpeed * 0.01f));
                }
                if (buttonOnePressed)
                {
                    dialogueStageNumber = 10;
                    typedOnce = false;
                }
                if (buttonTwoPressed)
                {
                    dialogueStageNumber = 11;
                    typedOnce = false;
                }
                break;
            case 10:
                dialogueText.gameObject.SetActive(true);
                questionButtonOne.SetActive(false);
                questionButtonTwo.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Space) || !typedOnce)
                {
                    typedOnce = true;
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, "S�d po zapoznaniu si� z obron� oskar�onego," + characterData[chosenCharacter].characterName + " , dochodzi do wniosku, i� oskar�ony ponosi odpowiedzialno�� za zaistnia�y incydent.", textRevealSpeed));
                    }
                }
                if (typedOnce && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 12;
                    typedOnce = false;
                }
                break;
            case 11:
                dialogueText.gameObject.SetActive(true);
                questionButtonOne.SetActive(false);
                questionButtonTwo.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Space) || !typedOnce)
                {
                    typedOnce = true;
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, "S�d po zapoznaniu si� z obron� oskar�onego," + characterData[chosenCharacter].characterName + " , dochodzi do wniosku, i� oskar�ony nie ponosi odpowiedzialno�ci za zaistnia�y incydent.", textRevealSpeed));
                    }
                }
                if (typedOnce && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 13;
                    typedOnce = false;
                }
                break;
            case 12:
                dialogueText.gameObject.SetActive(true);
                questionButtonOne.SetActive(false);
                questionButtonTwo.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Space) && !typedOnce)
                {
                    typedOnce = true;
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, "Oskar�ony uznany jest za winnego zarzucanego czynu.", textRevealSpeed));
                    }
                }
                if (typedOnce && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 100;
                }
                break;
            case 13:
                dialogueText.gameObject.SetActive(true);
                questionButtonOne.SetActive(false);
                questionButtonTwo.SetActive(false);
                if (Input.GetKeyDown(KeyCode.Space) && !typedOnce)
                {
                    typedOnce = true;
                    if (typingText != null)
                    {
                        StopCoroutine(typingText);
                    }
                    if (currentlyTyping)
                    {
                        dialogueText.maxVisibleCharacters = Int32.MaxValue;
                        currentlyTyping = false;
                    }
                    else
                    {
                        currentlyTyping = true;
                        typingText = StartCoroutine(UpdateTextField(dialogueText, "Oskar�ony zostaje uniewinniony z zarzutu.", textRevealSpeed));
                    }
                }
                if (typedOnce && currentlyTyping == false)
                {
                    dialogueLineNumber = 0;
                    dialogueStageNumber = 100;
                }
                break;
            case 100:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    chosenCharacter++;
                    dialogueStageNumber = 1;
                    typedOnce = false;
                }
                break;
            default:

                break;
        }
        CleanUp();
    }
    
    void CleanUp()
    {
        buttonOnePressed = false;
        buttonTwoPressed = false;
    }

    public void ButtonOnePressed()
    {
        buttonOnePressed = true;
    }

    public void ButtonTwoPressed()
    {
        buttonTwoPressed = true;
    }

    public void movePageZeroOnScreen()
    {
        StartCoroutine(MoveTo(pageZeroAnchor.transform, new Vector3(630, 0, 0), .5f));
    }

    public void movePageZeroAwayFromScreen()
    {
        StartCoroutine(MoveTo(pageZeroAnchor.transform, new Vector3(0, 0, 0), .5f));
    }
    public void movePageOneOnScreen()
    {
        StartCoroutine(MoveTo(pageOneAnchor.transform, new Vector3(630, 0, 0), .5f));
    }

    public void movePageOneAwayFromScreen()
    {
        StartCoroutine(MoveTo(pageOneAnchor.transform, new Vector3(0, 0, 0), .5f));
    }

    public void MoveCharacterOnScreen()
    {
        StartCoroutine(MoveTo(character.transform, new Vector3(0, 0, 0), .5f));
    }    

    public void MoveCharacterAwayFromScreen()
    {
        StartCoroutine(MoveTo(character.transform, new Vector3(0, -1000, 0), .5f));
    }

    public void Test()
    {
        Debug.Log("test");
    }

    IEnumerator MoveTo(Transform fromPosition, Vector3 toPosition, float duration)
    {
        float counter = 0;

        Vector3 startPos = fromPosition.localPosition;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            fromPosition.localPosition = Vector3.Lerp(startPos, toPosition, counter / duration);
            yield return null;
        }
    }

    void SetCharacterData(List<CharacterData> setCharacterData)
    {
        characterData = setCharacterData;
        Debug.Log("Loaded!" + characterData.Count);
    }

    void SetTextField(TextMeshProUGUI textField, string updatedText)
    {
        textField.text = updatedText;
        dialogueText.maxVisibleCharacters = Int32.MaxValue;
    }

    IEnumerator UpdateTextField(TextMeshProUGUI textField, string updatedText, float letterDelay)
    {
        textField.text = updatedText;
        textField.maxVisibleCharacters = 0;
        for (int i = 0; i < updatedText.Length; i++)
        {
            textField.maxVisibleCharacters++;
            yield return new WaitForSeconds(letterDelay);
        }
        currentlyTyping = false;
        yield return null;
    }
}