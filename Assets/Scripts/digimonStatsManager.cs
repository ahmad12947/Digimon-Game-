using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Invector.vCamera;
using Invector.vCharacterController;

public class digimonStatsManager : MonoBehaviour
{
    public float Hp, Mp, Off, Def, Speed, Brain, Weight, Age;
    private int maxOff, maxDef, maxSpeed, maxBrain;
    public int maxHP, maxMp;

    [Header("UI")]
    public TextMeshProUGUI hp_Text, MP_text, Off_text, Def_text, Speed_text, Brain_text, weight_Text, Age_Text, maxHpText, maxMpText;
    public Image Hp_Image, Mp_Image, Off_Image, Def_Image, Speed_Image, Brain_Image;
    public GameObject StatsCanvas;

    [Header("Refs")]
    public DigimonMoodManager moodManager;
    public vThirdPersonCamera cam;
    public GameObject hungerIcon, poopyIcon, sleepIcon, injuredIcon;
    public CanvasesManager canvasManager;
    public DigimonStatusCanvasManager statusCanvasManager;
    public vShooterMeleeInput input;
    public GameObject babyDigimon;
   

   

    private void Start()
    {
        Invoke("Initialize", 2);
        moodManager = FindObjectOfType<DigimonMoodManager>();
      
    }

    private void Initialize()
    {
        hp_Text.text = Hp.ToString();
        MP_text.text = Mp.ToString();
        Off_text.text = Off.ToString();
        Def_text.text = Def.ToString();
        Speed_text.text = Speed.ToString();
        Brain_text.text = Brain.ToString();
        maxHpText.text = maxHP.ToString();
        maxMpText.text = maxMp.ToString();
        weight_Text.text=Weight.ToString();
        Age_Text.text = Age.ToString();
        maxOff = 1000;
        maxDef = 1000;
        maxSpeed = 500;
        maxBrain = 500;

        Hp_Image.fillAmount = Mathf.Clamp(Hp / maxHP, 0f, 1f);
        Mp_Image.fillAmount = Mathf.Clamp(Mp / maxMp, 0f, 1f);
        Off_Image.fillAmount = Mathf.Clamp(Off / maxOff, 0f, 1f);
        Def_Image.fillAmount = Mathf.Clamp(Def / maxDef, 0f, 1f);
        Speed_Image.fillAmount = Mathf.Clamp(Speed / maxSpeed, 0f, 1f);
        Brain_Image.fillAmount = Mathf.Clamp(Brain / maxBrain, 0f, 1f);
    }
    private void ClampStats()
    {
       
        Weight = Mathf.Clamp(Weight, 1, 100);
    }
    private void Update()
    {
       
        ClampStats();
        CheckLifeCycle();
    }

    public void HandleAging()
    {     
            Age += 1;
            Age_Text.text = Age.ToString();
   
    }

    private void CheckLifeCycle()
    {
        int maxLifeSpan =360;
        if (Age >= maxLifeSpan)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Debug.Log("Your Digimon has passed away due to old age.");
        Age = 0;
        //input.enabled = false;
        //input.GetComponent<Animator>().enabled = false;

        Animator anim = moodManager.gameObject.GetComponent<Animator>();
        if (anim != null) anim.Play("Death");

        moodManager.gameObject.GetComponent<EvolutionManager>().onDeathEvolved(babyDigimon);

        StatsCanvas.SetActive(false);
    }

    public void clearAllAndDisable()
    {
        moodManager = FindObjectOfType<DigimonMoodManager>();
        DeactivateStatBar(Hp_Image);
        DeactivateStatBar(Mp_Image);
        DeactivateStatBar(Off_Image);
        DeactivateStatBar(Def_Image);
        DeactivateStatBar(Speed_Image);
        DeactivateStatBar(Brain_Image);

        cam.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StatsCanvas.SetActive(false);
    }

    private void DeactivateStatBar(Image img)
    {
        GameObject parent = img.transform.parent.transform.parent.GetChild(0).gameObject;
        parent.SetActive(false);
        parent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
    }

    private int ModifyTrainingGain(int amount)
    {
        int tiredness = moodManager.Tiredness;
        if (tiredness >= 100) amount = 1;
        if (tiredness >= 80)
        {
            moodManager.ChangeHappiness(-4);
            moodManager.ChangeDiscipline(-3);
        }
        return amount;
    }

    // Stat Adders ----------------------
    public void addHp(int amount)
    {
        moodManager.changeTiredness(10);
        amount = ModifyTrainingGain(amount);
        maxHP += amount;
        Hp += amount;
        hp_Text.text = Hp.ToString();
        maxHpText.text = maxHP.ToString();
        hp_Text.text= Hp.ToString();
        UpdateStatVisual(Hp_Image, amount);

    }

    public void addMp(int amount)
    {
        moodManager.changeTiredness(7);
        amount = ModifyTrainingGain(amount);
        maxMp += amount;
        Mp += amount;
        maxMpText.text = maxMp.ToString();
        MP_text.text=Mp.ToString();
        UpdateStatVisual(Mp_Image, amount);
        
    }

    public void addOff(int amount)
    {
        moodManager.changeTiredness(9);
        amount = ModifyTrainingGain(amount);
        Off += amount;
        Off_text.text = Off.ToString();
        UpdateStatVisual(Off_Image, amount);
    }

    public void addDef(int amount)
    {
        moodManager.changeTiredness(9);
        amount = ModifyTrainingGain(amount);
        Def += amount;
        Def_text.text = Def.ToString();
        UpdateStatVisual(Def_Image, amount);
    }

    public void addSpeed(int amount)
    {
        moodManager.changeTiredness(8);
        amount = ModifyTrainingGain(amount);
        Speed += amount;
        Speed_text.text = Speed.ToString();
        UpdateStatVisual(Speed_Image, amount);
    }

    public void addBrain(int amount)
    {
        moodManager.changeTiredness(6);
        amount = ModifyTrainingGain(amount);
        Brain += amount;
        Brain_text.text = Brain.ToString();
        UpdateStatVisual(Brain_Image, amount);
    }

    private void UpdateStatVisual(Image img, int amount)
    {
        float currentValue = 0f;
        float maxValue = 1f;

        if (img == Hp_Image) currentValue = Hp / maxHP;
        else if (img == Mp_Image) currentValue = Mp / maxMp;
        else if (img == Off_Image) currentValue = Off / maxOff;
        else if (img == Def_Image) currentValue = Def / maxDef;
        else if (img == Speed_Image) currentValue = Speed / maxSpeed;
        else if (img == Brain_Image) currentValue = Brain / maxBrain;

        img.fillAmount = Mathf.Clamp(currentValue, 0f, 1f);
        GameObject parent = img.transform.parent.transform.parent.GetChild(0).gameObject;
        parent.SetActive(true);
        parent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = amount.ToString();
        statusCanvasManager.updateStats();
    }

    public void changeWeight(int amount)
    {
        Weight += amount;
        //weight_Text.text = Weight.ToString();
        Invoke("updateStatsCanvas", 2);
        
    }
    public void updateStatsCanvas()
    {
        statusCanvasManager.updateStats();
    }
    public void changeAge(int amount)
    {
        Age += amount;
        Age_Text.text = Age.ToString();
        statusCanvasManager.updateStats();
    }

    public void SetStats(int hp, int mp, int off, int def, int speed, int brain, int weight, int m_hp, int m_mp)
    {
        Hp = hp;
        Mp = mp;
        Off = off;
        Def = def;
        Speed = speed;
        Brain = brain;
        Weight = weight;
        maxHP = m_hp;
        maxMp = m_mp;
    }
    public void AddStats(int hpGain, int mpGain, int offGain, int defGain, int speedGain, int brainGain, int weightGain)
    {
        Hp += hpGain;
        Mp += mpGain;
        Off += offGain;
        Def += defGain;
        Speed += speedGain;
        Brain += brainGain;
        Weight += weightGain;

        // Clamp to max values
        Hp = Mathf.Clamp(Hp, 0, maxHP);
        Mp = Mathf.Clamp(Mp, 0, maxMp);
        Off = Mathf.Clamp(Off, 0, maxOff);
        Def = Mathf.Clamp(Def, 0, maxDef);
        Speed = Mathf.Clamp(Speed, 0, maxSpeed);
        Brain = Mathf.Clamp(Brain, 0, maxBrain);
    }
    public void ReduceAllStatsByPercent(int percent)
    {
        float multiplier = 1f - (percent / 100f);

        Hp = Mathf.FloorToInt(Hp * multiplier);
        Mp = Mathf.FloorToInt(Mp * multiplier);
        Off = Mathf.FloorToInt(Off * multiplier);
        Def = Mathf.FloorToInt(Def * multiplier);
        Speed = Mathf.FloorToInt(Speed * multiplier);
        Brain = Mathf.FloorToInt(Brain * multiplier);

        Debug.Log($"Stats reduced by {percent}% due to butterfly effect.");
    }
    public void OnStatsOkButtonClicked()
    {
        pausePlayerMovement.instance.unPausePlayer();

        clearAllAndDisable();
        canvasManager.enabled = true;
        cam.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
