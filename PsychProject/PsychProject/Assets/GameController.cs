using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public int highScore;
    public GameObject highScoreObj;

    public int patientNumber;

    public bool maleOrFemale; //Fale = male, true = female

    public List<Sprite> maleSprites;
    public List<Sprite> femaleSprites;

    public List<string> maleNames;
    public List<string> femaleNames;

    public List<string> treatments;
    public List<string> disabilities;
    public int disabilityId;
    public List<ViewableList> properTreatments;

    public GameObject TitleScreen;
    public GameObject GameUI;

    public GameObject Person;
    public GameObject PersonName;
    public GameObject Disability;

    public GameObject Treatment1;
    public GameObject Treatment2;
    public GameObject Treatment3;
    public GameObject Treatment4;

    public string correctTreatment;

    public List<string> tempTreatments;
    public List<int> addedTreatments;

    public GameObject Satisfaction;
    public GameObject PatientNumber;
    public GameObject NextPatientButton;
    public GameObject Announcement;

    public string correctAnnouncement = "Treatment Successful!";
    public string incorrectAnnouncement = "Treatment Unsuccessful!";

    public bool isRight;

    // Use this for initialization
    void Start () {
        highScore = PlayerPrefs.GetInt("highScore");
        highScoreObj.GetComponent<Text>().text = "High Score: " + highScore + " Patients";

        GenerateTreatments();
    }

    public void StartGame()
    {
        TitleScreen.SetActive(false);
        GameUI.SetActive(true);

        RandomiseValues();
        patientNumber = 0;
        PatientNumber.GetComponent<Text>().text = "Patient #: 0";
    }

    public void NextPatient()
    {
        isRight = false;

        patientNumber++;
        PatientNumber.GetComponent<Text>().text = "Patient #: " + patientNumber;

        RandomiseValues();

        NextPatientButton.SetActive(false);

        Announcement.GetComponent<Animator>().Play("DefaultInivis");
    }

    public void UseTreatment(Text text)
    {
        if(!Announcement.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("AnnouncementFadein") && !isRight)
        {
            if (text.text == correctTreatment)
            {
                Announcement.GetComponent<Text>().text = correctAnnouncement;
                isRight = true;
                NextPatientButton.SetActive(true);
            }
            else
            {
                Announcement.GetComponent<Text>().text = incorrectAnnouncement;
                Satisfaction.GetComponent<Slider>().value -= 1;
                if(Satisfaction.GetComponent<Slider>().value == 0)
                    Lose();
            }
            Announcement.GetComponent<Animator>().Play("AnnouncementFadein");
        }
    }

    void RandomiseValues()
    {
        int temp = Random.Range(0, 2);
        if (temp == 1)
            maleOrFemale = true;
        else
            maleOrFemale = false;

        if (!maleOrFemale) //If is guy
        {
            Person.GetComponent<Image>().sprite = maleSprites[Random.Range(0, maleSprites.Count)];
            PersonName.GetComponent<Text>().text = maleNames[Random.Range(0, maleNames.Count)];
        }
        else
        {
            Person.GetComponent<Image>().sprite = femaleSprites[Random.Range(0, femaleSprites.Count)];
            PersonName.GetComponent<Text>().text = femaleNames[Random.Range(0, femaleNames.Count)];
        }

        Satisfaction.GetComponent<Slider>().value = 3;

        disabilityId = Random.Range(1, disabilities.Count);
        Disability.GetComponent<Text>().text = disabilities[disabilityId];

        for(int i = 0; i < 4; i++)
        {
            tempTreatments[i] = "";
            addedTreatments[i] = 0;
        }

        int temp2 = Random.Range(0, 3);
        tempTreatments[temp2] = properTreatments[disabilityId].list[Random.Range(0, properTreatments[disabilityId].list.Count)];
        correctTreatment = tempTreatments[temp2];
        addedTreatments[temp2] = disabilityId;

        int count = 0;
        foreach (int i in addedTreatments)
        {
            if(i == 0)
            {
                int temp3 = Random.Range(0, treatments.Count);
                while (tempTreatments.Contains(treatments[temp3]) || properTreatments[disabilityId].list.Contains(treatments[temp3]))
                    temp3 = Random.Range(0, treatments.Count);

                addedTreatments[count] = temp3;
                tempTreatments[count] = treatments[temp3];
            }
            count++;
        }

        Treatment1.GetComponent<Text>().text = tempTreatments[0];
        Treatment2.GetComponent<Text>().text = tempTreatments[1];
        Treatment3.GetComponent<Text>().text = tempTreatments[2];
        Treatment4.GetComponent<Text>().text = tempTreatments[3];
    }

    void Lose()
    {
        GameUI.SetActive(false);

        if(patientNumber >  PlayerPrefs.GetInt("highScore"))
        {
            PlayerPrefs.SetInt("highScore", patientNumber);
            highScore = patientNumber;
            highScoreObj.GetComponent<Text>().text = "High Score: " + highScore + " Patients";
            Announcement.GetComponent<Text>().text = "You Lost! New High Score: " + highScore;
        }
        else
            Announcement.GetComponent<Text>().text = "You Lost! Score: " + patientNumber;

        Announcement.GetComponent<Animator>().Play("AnnouncementFadein");
        StartCoroutine(WaitForLossMessage());
    }

    IEnumerator WaitForLossMessage()
    {
        yield return new WaitForSeconds(3);
        TitleScreen.SetActive(true);
    }

    void GenerateTreatments() //Used to generate the Treatments list from the ProperTreatments list of lists 
    {
        foreach(ViewableList viewableList in properTreatments)
        {
            foreach(string str in viewableList.list)
            {
                treatments.Add(str);
            }
        }
    }

    public void QuitGame()
    {
        if (patientNumber > PlayerPrefs.GetInt("highScore"))
            PlayerPrefs.SetInt("highScore", patientNumber);
        Application.Quit();
    }
}

[System.Serializable]
public class ViewableList
{
    public List<string> list;
}
