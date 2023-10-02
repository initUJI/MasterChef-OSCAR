using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExerciseManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string question;
        public string[] answers;
    }

    [System.Serializable]
    public class Exercise
    {
        public int type;
        public bool rewrite;
        public string title;
        public string[] options;
        public List<Question> Question = new List<Question>();
    }

    public Exercise exercise = new();

    public GameObject StartPanel;
    public GameObject MultipleChoicePanel;
    public GameObject MatchingPanel;
    public GameObject GridPanel;
    public GameObject EndPanel;
    public GameObject ExitPanel;
    public GameObject FinishPanel;
    public GameObject HelpPanel;


    int questionNumber;
    TextMeshProUGUI question;
    public float transitionDuration = 0.05f;
    public float colorDuration = 3f;
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    PauseManager pauseManager;
    public GameObject PlayerRig;
    AudioManager audioManager;
    Button[] buttonsArray;
    List<Button> buttons;
    Button clickedButton;
    bool started;
    int score;
    int maxScore;
    int attempts;

    bool flipped;
    Button flippedButton;
    bool timeout;
    int matched;
    int exerciseNumber;
    string last;
    string pathAudio;

    string correctAudio = "Sounds/Exercises/Correct Answer _ Royalty Free Sound Effects #shorts";
    string wrongAudio = "Sounds/Exercises/Wrong Answer_ Royalty Free Sound Effect";
    string endAudio;

    float[] xPosition = { -1.2934865951538087f, 0.5865135192871094f, 2.4365129470825197f };
    float yPosition = 0.010000228881835938f;
    float zPosition = 14.406408309936524f;

    public void InitializeExercise(string exerciseName)
    {
        exerciseNumber = (int)char.GetNumericValue(exerciseName[^1]);

        if (SetVROrNot.isOnXRDevice)
        {
            Transform TV = this.transform.parent;
            TV.gameObject.SetActive(true);
            Vector3 pos = new Vector3(xPosition[exerciseNumber - 1], TV.position.y, TV.position.z);
            TV.position = pos;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        PlayerRig.transform.position = new Vector3(xPosition[exerciseNumber - 1], yPosition, zPosition);

        buttons = new();
        question = MultipleChoicePanel.transform.Find("Question").GetComponent<TextMeshProUGUI>();
        started = false;
        questionNumber = 0;
        transitionDuration /= 60;
        pauseManager = pauseCanvas.GetComponent<PauseManager>();
        audioManager = PlayerRig.GetComponent<AudioManager>();
        pauseManager.freeze = true;
        gameCanvas.transform.Find("Panel").gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;

        this.transform.Find("Next").GetComponent<Button>().interactable = false;

        if (GameManager.exercise1 && GameManager.exercise2 && GameManager.exercise3)
        {
            this.transform.Find("Help").GetComponent<Button>().interactable = false;
            FinishPanel.SetActive(true);
        }
        else if (exerciseNumber == 1 && GameManager.exercise1)
        {
            EndPanel.SetActive(true);
            this.transform.Find("Audio").GetComponent<Button>().interactable = false;
            this.transform.Find("Help").GetComponent<Button>().interactable = false;
            SetScore(GameManager.score1, GetMaxScore());
        }
        else if (exerciseNumber == 2 && GameManager.exercise2)
        {
            EndPanel.SetActive(true);
            this.transform.Find("Audio").GetComponent<Button>().interactable = false;
            this.transform.Find("Help").GetComponent<Button>().interactable = false;
            SetScore(GameManager.score2, GetMaxScore());
        }
        else if (exerciseNumber == 3 && GameManager.exercise3)
        {
            EndPanel.SetActive(true);
            this.transform.Find("Audio").GetComponent<Button>().interactable = false;
            this.transform.Find("Help").GetComponent<Button>().interactable = false;
            SetScore(GameManager.score3, GetMaxScore());
        }
        else
        {
            string reseourcePath = "Texts/" + GameManager.levelNumber + "/Exercises/" + exerciseName;
            TextAsset fileTextAsset = Resources.Load<TextAsset>(reseourcePath);
            string fileText = fileTextAsset.text;
            exercise = JsonUtility.FromJson<Exercise>(fileText);

            pathAudio = "Sounds/Ricette/" + GameManager.levelNumber + "/exercises/" + GameManager.difficulty + exerciseNumber + "/Track";
            Play(pathAudio);

            StartPanel.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = exercise.title;
            StartPanel.SetActive(true);
        }
        MultipleChoicePanel.SetActive(false);
        MatchingPanel.SetActive(false);
        GridPanel.SetActive(false);
        ExitPanel.SetActive(false);

    }

    /* Tipi di step di ricetta:
    * 0) Quiz a risposta multipla con opzioni fisse (MultipleChoicePanel)
    * 1) Quiz a risposta multipla con opzioni variabili (MultipleChoicePanel)
    * 2) Indicare il nome dell'oggetto visualizzato (MatchingPanel)
    * 3) Acoppiare le card uguali (memory)
    * 4) Trovare le risposte corrette tra più opzioni
    */
    public void StartExercise()
    {
        started = true;
        this.transform.Find("Audio").GetComponent<Button>().interactable = false;
        Stop();
        StartPanel.SetActive(false);
        score = 0;
        attempts = 0;
        switch (exercise.type)
        {
            case 0:
                {
                    buttonsArray = MultipleChoicePanel.GetComponentsInChildren<Button>();
                    buttons = new List<Button>(buttonsArray);
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = exercise.options[i];
                    }
                    MultipleChoicePanel.SetActive(true);
                }
                break;
            case 1:
                {
                    buttonsArray = MultipleChoicePanel.GetComponentsInChildren<Button>();
                    buttons = new List<Button>(buttonsArray);
                    MultipleChoicePanel.SetActive(true);
                }
                break;
            case 2:
                {
                    buttonsArray = MatchingPanel.GetComponentsInChildren<Button>();
                    buttons = new List<Button>(buttonsArray);
                    ShuffleButtons(exercise.options);
                    MatchingPanel.SetActive(true);
                }
                break;
            case 3:
                {
                    buttonsArray = GridPanel.GetComponentsInChildren<Button>();
                    buttons = new List<Button>(buttonsArray);
                    List<string> options = new(exercise.options);
                    options.AddRange(options);
                    ShuffleCards(options);
                    foreach (Button b in buttons)
                    {
                        CardTurn(b, false);
                    }
                    flipped = false;
                    matched = 0;
                    score = exercise.options.Length;
                    GridPanel.SetActive(true);
                }
                break;
            case 4:
                {
                    buttonsArray = GridPanel.GetComponentsInChildren<Button>();
                    buttons = new List<Button>(buttonsArray);
                    ShuffleButtons(exercise.options);
                    matched = 0;
                    score = exercise.options.Length / 2;
                    GridPanel.SetActive(true);
                }
                break;
        }
        NextQuestion();
    }

    public void Opzione(GameObject obj)
    {
        string option = obj.GetComponentInChildren<TextMeshProUGUI>(true).text;
        Button button = obj.GetComponent<Button>();
        switch (exercise.type)
        {
            case 0:
            case 1:
                {
                    if (exercise.Question[questionNumber].answers[0] == option)
                    {
                        Play(correctAudio);
                        score++;
                        StartCoroutine(RightAnswer(button));
                    }
                    else
                    {
                        Play(wrongAudio);
                        SetDisabledButtonColor(button, 255, 0, 0, 255);
                        foreach (Button b in buttons)
                        {
                            if (exercise.Question[questionNumber].answers[0] == b.GetComponentInChildren<TextMeshProUGUI>(true).text)
                            {
                                StartCoroutine(RightAnswer(b));
                                break;
                            }
                        }
                        button.interactable = false;
                    }
                }
                break;
            case 2:
                {
                    if (exercise.Question[questionNumber].answers[0] == option)
                    {
                        Play(correctAudio);
                        if (attempts == 0)
                        {
                            score++;
                        }
                        StartCoroutine(RightAnswer(button));
                    }
                    else
                    {
                        Play(wrongAudio);
                        SetDisabledButtonColor(button, 255, 0, 0, 255);
                        button.interactable = false;
                        attempts++;
                    }
                }
                break;
            case 3:
                {
                    if(!timeout)
                    {
                        CardTurn(button, true);
                        if (!flipped)
                        {
                            flipped = true;
                            flippedButton = button;
                            flippedButton.interactable = false;
                        }
                        else
                        {
                            string optionFlipped = flippedButton.GetComponentInChildren<TextMeshProUGUI>().text;
                            
                            if (optionFlipped == option)
                            {
                                Play(correctAudio);
                                StartCoroutine(RightAnswer(button));
                            }
                            else
                            {
                                Play(wrongAudio);
                                attempts++;
                                if (attempts % 3 == 0)
                                {
                                    score--;
                                }
                                StartCoroutine(WrongAnswer(button));
                            }
                            flipped = false;
                        }
                    }
                }
                break;
            case 4:
                {
                    if (!timeout)
                    {
                        if (ContainsAnswer(exercise.Question[0].answers, option))
                        {
                            Play(correctAudio);
                            last = option;
                            StartCoroutine(RightAnswer(button));
                        }
                        else
                        {
                            Play(wrongAudio);
                            SetDisabledButtonColor(button, 255, 0, 0, 255);
                            button.interactable = false;
                            score--;
                        }
                    }
                }
                break;
        }
    }

    private IEnumerator RightAnswer(Button button)
    {
        clickedButton = button;
        SetDisabledButtonColor(clickedButton, 0, 255, 0, 255);
        clickedButton.interactable = false;

        switch (exercise.type)
        {
            case 0:
            case 1:
                {
                    foreach (Button b in buttons)
                    {
                        b.interactable = false;
                    }
                    if (exercise.rewrite)
                    {
                        string answer = question.text;
                        answer = answer.Replace("____", " " + clickedButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text + " ");
                        question.text = answer;
                        question.color = new Color32(0, 255, 0, 255);
                    }
                    
                    this.transform.Find("Next").GetComponent<Button>().interactable = true;
                    this.transform.Find("Audio").GetComponent<Button>().interactable = true;
                }
                break;
            case 2:
                {
                    foreach (Button b in buttons)
                    {
                        b.interactable = false;
                    }
                    this.transform.Find("Next").GetComponent<Button>().interactable = true;
                    this.transform.Find("Audio").GetComponent<Button>().interactable = true;
                }
                break;
            case 3:
                {
                    timeout = true;
                    matched++;
                    SetDisabledButtonColor(flippedButton, 0, 255, 0, 255);
                    if (matched == 1)
                    {
                        this.transform.Find("Audio").GetComponent<Button>().interactable = true;
                    }
                    if (matched == buttons.Count / 2)
                    {
                        this.transform.Find("Next").GetComponent<Button>().interactable = true;
                    }
                }
                break;
            case 4:
                {
                    timeout = true;
                    matched++;
                    SetDisabledButtonColor(button, 0, 255, 0, 255);
                    if (matched == 1)
                    {
                        this.transform.Find("Audio").GetComponent<Button>().interactable = true;
                    }
                    if (matched == buttons.Count / 2)
                    {
                        this.transform.Find("Next").GetComponent<Button>().interactable = true;
                    }
                }
                break;
        }

        yield return new WaitForSeconds(0.5f);
        Listen(false);

        if (exercise.type == 3)
        {
            yield return new WaitForSeconds(1.5f);
            clickedButton.gameObject.SetActive(false);
            flippedButton.gameObject.SetActive(false);
            timeout = false;
        }
        else if (exercise.type == 4)
        {
            yield return new WaitForSeconds(1.5f);
            clickedButton.gameObject.SetActive(false);
            timeout = false;
        }
    }

    private IEnumerator WrongAnswer(Button button)
    {
        if (exercise.type == 3)
        {
            timeout = true;
            clickedButton = button;
            SetDisabledButtonColor(clickedButton, 255, 0, 0, 255);
            SetDisabledButtonColor(flippedButton, 255, 0, 0, 255);
            clickedButton.interactable = false;
            flippedButton.interactable = false;

            yield return new WaitForSeconds(1.5f);

            SetDisabledButtonColor(clickedButton, 255, 255, 255, 255);
            SetDisabledButtonColor(flippedButton, 255, 255, 255, 255);
            clickedButton.interactable = true;
            flippedButton.interactable = true;
            CardTurn(button, false);
            CardTurn(flippedButton, false);
            timeout = false;
        }
    }

    void ShuffleButtons(string[] answers)
    {
        for (int i = 0; i < answers.Length; i++)
        {
            string temp = answers[i];
            int randomIndex = Random.Range(i, answers.Length);
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }
        foreach (Button b in buttons)
        {
            b.GetComponentInChildren<TextMeshProUGUI>(true).text = answers[buttons.IndexOf(b)];
            b.gameObject.SetActive(true);
        }
    }

    void ShuffleButtons(List<string> answers)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            string temp = answers[i];
            int randomIndex = Random.Range(i, answers.Count);
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }
        foreach (Button b in buttons)
        {
            b.GetComponentInChildren<TextMeshProUGUI>(true).text = answers[buttons.IndexOf(b)];
            b.gameObject.SetActive(true);
        }
    }

    void ShuffleCards(List<string> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            string temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
        foreach (Button b in buttons)
        {
            b.GetComponentInChildren<TextMeshProUGUI>(true).text = cards[buttons.IndexOf(b)];
            Sprite sprite = Resources.Load<Sprite>("Images/Ingredients/" + cards[buttons.IndexOf(b)]);
            Image img = b.transform.Find("Image").GetComponent<Image>();
            img.gameObject.SetActive(true);
            img.sprite = sprite;
        }
    }

    public void Listen(bool stop)
    {
        pathAudio = "Sounds/Ricette/" + GameManager.levelNumber + "/exercises/" + GameManager.difficulty + exerciseNumber + "/";
        switch (exercise.type)
        {
            case 0:
            case 1:
            case 2:
                {
                    pathAudio += (questionNumber + 1);
                }
                break;
            case 3:
                {
                    string name = flippedButton.GetComponentInChildren<TextMeshProUGUI>().text;
                    for (int i = 0; i < exercise.options.Length; i++)
                    {
                        if (exercise.options[i] == name)
                        {
                            pathAudio += (i + 1);
                            break;
                        }
                    }
                }
                break;
            case 4:
                {
                    for (int i = 0; i < exercise.Question[0].answers.Length; i++)
                    {
                        if (exercise.Question[0].answers[i] == last)
                        {
                            pathAudio += (i + 1);
                            break;
                        }
                    }
                }
                break;
        }
        Play(pathAudio, stop);
    }

    public void LastListen(bool stop)
    {
        Play(pathAudio, stop);
    }

    void Play(string path, bool stop = false)
    {
        AudioManager.AudioStart(path, stop);
    }

    void Stop()
    {
        AudioManager.AudioStop();
    }

    public void NextPage()
    {
        questionNumber++;
        attempts = 0;
        Stop();
        StopAllCoroutines();
        if (exercise.type == 2)
        {
            clickedButton.gameObject.SetActive(false);
        }
        if (exercise.Question.Count != questionNumber)
        {
            ResetPanel();
            NextQuestion();
        }
        else
        {
            switch (exerciseNumber)
            {
                case 1:
                    GameManager.exercise1 = true;
                    GameManager.score1 = score;
                    break;
                case 2:
                    GameManager.exercise2 = true;
                    GameManager.score2 = score;
                    break;
                case 3:
                    GameManager.exercise3 = true;
                    GameManager.score3 = score;
                    break;

            }
            Play("Sounds/Exercises/Crowd-applause");
            Play("Sounds/Exercises/Completed " + exerciseNumber);
            EndPanel.SetActive(true);
            SetScore(score, GetMaxScore());
            this.transform.Find("Next").GetComponent<Button>().interactable = false;
            this.transform.Find("Audio").GetComponent<Button>().interactable = false;
            this.transform.Find("Help").GetComponent<Button>().interactable = false;
            switch (exercise.type)
            {
                case 0:
                case 1:
                    MultipleChoicePanel.SetActive(false);
                    break;
                case 2:
                    MatchingPanel.SetActive(false);
                    break;
                case 3:
                case 4:
                    GridPanel.SetActive(false);
                    break;
            }
        }
    }

    public void NextQuestion()
    {
        this.transform.Find("Next").GetComponent<Button>().interactable = false;
        this.transform.Find("Audio").GetComponent<Button>().interactable = false;
        switch (exercise.type)
        {
            case 0:
                {
                    question.text = exercise.Question[questionNumber].question;
                }
                break;
            case 1:
                {
                    question.text = exercise.Question[questionNumber].question;
                    ShuffleButtons((string[])exercise.Question[questionNumber].answers.Clone());
                }
                break;
            case 2:
                {
                    GameObject obj = Resources.Load<GameObject>("Prefabs/Exercises/Matching/" + exercise.Question[questionNumber].question);
                    var o = Instantiate(obj, MatchingPanel.transform.Find("MatchingObject").transform);
                    o.name = exercise.Question[questionNumber].question;
                }
                break;
        }
    }

    public void Help()
    {
        bool set = HelpPanel.activeSelf;
        if (started)
        {
            switch (exercise.type)
            {
                case 0:
                case 1:
                    MultipleChoicePanel.SetActive(set);
                    break;
                case 2:
                    MatchingPanel.SetActive(set);
                    break;
                case 3:
                case 4:
                    GridPanel.SetActive(set);
                    break;
            }
        }
        else 
        {
            StartPanel.SetActive(set);
        }
        HelpPanel.SetActive(!set);
    }

    public void Exit()
    {
        if (EndPanel.activeSelf)
        {
            if (GameManager.exercise1 && GameManager.exercise2 && GameManager.exercise3)
            {
                EndPanel.SetActive(false);
                FinishPanel.SetActive(true);
            }
            else
            {
                Close();
            }
        }
        else if (FinishPanel.activeSelf)
        {
            Close();
        }
        else
        {
            ExitPanel.SetActive(!ExitPanel.activeSelf);
            this.transform.Find("Exit").gameObject.SetActive(!ExitPanel.activeSelf);
            this.transform.Find("Audio").gameObject.SetActive(!ExitPanel.activeSelf);
            this.transform.Find("Help").gameObject.SetActive(!ExitPanel.activeSelf);
            this.transform.Find("Next").gameObject.SetActive(!ExitPanel.activeSelf);
            if (!started)
            {
                StartPanel.SetActive(!StartPanel.activeSelf);
            }
            else switch (exercise.type)
                {
                    case 0:
                    case 1:
                        MultipleChoicePanel.SetActive(!ExitPanel.activeSelf);
                        break;
                    case 2:
                        MatchingPanel.SetActive(!ExitPanel.activeSelf);
                        break;
                    case 3:
                    case 4:
                        GridPanel.SetActive(!ExitPanel.activeSelf);
                        break;
                }
        }
    }

    public void Close()
    {
        if (exercise.type == 2 && started)
        {
            foreach (Button b in buttons)
            {
                b.gameObject.SetActive(true);
            }
        }
        ResetPanel();
        EndPanel.SetActive(false);
        FinishPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        pauseManager.freeze = false;

        if (SetVROrNot.isOnXRDevice)
        {
            this.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(false);
            gameCanvas.transform.Find("Panel").gameObject.SetActive(true);
        }

        this.transform.Find("Exit").gameObject.SetActive(true);
        this.transform.Find("Audio").gameObject.SetActive(true);
        this.transform.Find("Help").gameObject.SetActive(true);
        this.transform.Find("Next").gameObject.SetActive(true);
    }

    void ResetPanel()
    {
        foreach (Button b in buttons)
        {
            SetDisabledButtonColor(b, 255, 255, 255, 255);
            b.interactable = true;
        }
        question.color = new Color32(255, 255, 255, 255);
        if (exercise.type == 2 && started)
        {
            Destroy(MatchingPanel.transform.Find("MatchingObject").GetChild(0).gameObject);
        }
        else if (exercise.type == 3 && started)
        {
            foreach (Button b in buttons)
            {
                Image img = b.transform.Find("Image").GetComponent<Image>();
                img.gameObject.SetActive(false);
                img.sprite = null;
                b.gameObject.SetActive(true);
            }
            timeout = false;
        }
        else if (exercise.type == 4 && started)
        {
            foreach (Button b in buttons)
            {
                b.gameObject.SetActive(true);
            }
            timeout = false;
        }
        Stop();
    }

    void SetDisabledButtonColor(Button button, byte R, byte G, byte B, byte A)
    {
        ColorBlock colors = button.colors;
        colors.disabledColor = new Color32(R, G, B, A);
        button.colors = colors;
    }

    void CardTurn(Button button, bool show)
    {
        for (int i = 0; i < button.transform.childCount; i++)
        {
            button.transform.GetChild(i).gameObject.SetActive(show);
        }
    }

    public void BackToMenu()
    {
        if (!SetVROrNot.isOnXRDevice)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenuVR");
        }

    }

    bool ContainsAnswer(string[] list, string answer)
    {
        foreach (string s in list)
        {
            if (s == answer)
                return true;
        }
        return false;
    }

    int GetMaxScore()
    {
        if (exercise.Question.Count >= 2)
        {
            return exercise.Question.Count;
        }
        else
        {
            if (exercise.type == 4)
            {
                return exercise.options.Length / 2;
            }
            else
            {
                return exercise.options.Length;
            }
        }
    }

    void SetScore(int score, int maxScore)
    {
        EndPanel.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = score + "/" + maxScore;
    }
}
