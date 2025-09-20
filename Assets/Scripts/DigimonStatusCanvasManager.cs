using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DigimonStatusCanvasManager : MonoBehaviour
{
    public digimonStatsManager digimonStatsManager;
    public DigimonMoodManager digimonMoodManager;

    public Slider happinessSlider, Discipline, Virus;

    public Image Hp_Image, Mp_Image, Off_Image, Def_Image, Speed_image, Brain_Image;
    public TextMeshProUGUI hp_Text, MP_text, Off_text, Def_text, Speed_text, Brain_text, digimonName, maxHpText, maxMPText, weight_Text;

    private int maxHP, maxMp, maxOff, maxDef, maxSpeed, maxBrain;

    private void Start()
    {
        InitialStats();
    }

    public void InitialStats()
    {
        maxHP = digimonStatsManager.maxHP;
        maxMp = digimonStatsManager.maxMp;

        happinessSlider.value = digimonMoodManager.Happiness;
        Discipline.value = digimonMoodManager.Discipline;
        hp_Text.text = digimonStatsManager.Hp.ToString();
        MP_text.text = digimonStatsManager.Mp.ToString();
        Off_text.text = digimonStatsManager.Off.ToString();
        Def_text.text = digimonStatsManager.Def.ToString();
        Speed_text.text = digimonStatsManager.Speed.ToString();
        Brain_text.text = digimonStatsManager.Brain.ToString();
        maxHpText.text = digimonStatsManager.maxHP.ToString();
        maxMPText.text = digimonStatsManager.maxMp.ToString();
        //maxHP = 4000;
        //maxMp = 2000;
        //maxOff = 1000;
        //maxDef = 1000;
        //maxSpeed = 1000;
        //maxBrain = 1000;

        Hp_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Hp / maxHP, 0f, 1f);
        Mp_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Mp / maxMp, 0f, 1f);
        Off_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Off / maxOff, 0f, 1f);
        Def_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Def / maxDef, 0f, 1f);
        Speed_image.fillAmount = Mathf.Clamp(digimonStatsManager.Speed / maxSpeed, 0f, 1f);
        Brain_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Brain / maxBrain, 0f, 1f);
    }
    public void updateStats()
    {
        happinessSlider.value = digimonMoodManager.Happiness;
        Discipline.value = digimonMoodManager.Discipline;
        hp_Text.text = digimonStatsManager.Hp.ToString();
        MP_text.text = digimonStatsManager.Mp.ToString();
        maxHpText.text = digimonStatsManager.maxHP.ToString();
        maxMPText.text = digimonStatsManager.maxMp.ToString();
        Off_text.text = digimonStatsManager.Off.ToString();
        Def_text.text = digimonStatsManager.Def.ToString();
        Speed_text.text = digimonStatsManager.Speed.ToString();
        Brain_text.text = digimonStatsManager.Brain.ToString();
        weight_Text.text= digimonStatsManager.Weight.ToString();
        Hp_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Hp / maxHP, 0f, 1f);
        Mp_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Mp / maxMp, 0f, 1f);
        Off_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Off / maxOff, 0f, 1f);
        Def_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Def / maxDef, 0f, 1f);
        Speed_image.fillAmount = Mathf.Clamp(digimonStatsManager.Speed / maxSpeed, 0f, 1f);
        Brain_Image.fillAmount = Mathf.Clamp(digimonStatsManager.Brain / maxBrain, 0f, 1f);

    }
    public void updateName(string name)
    {
        digimonName.text = name;
    }

}
    


