using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load_Farm : MonoBehaviour
{
    [SerializeField] private int levelEnclo1 = 0;
    [SerializeField] private int levelEnclo2 = 0;
    [SerializeField] private int levelEnclo3 = 0;
    public GameObject EncloCasseVer;
    public GameObject EncloCasseRenard;
    public GameObject EncloRenard;
    public GameObject EncloVer;
    private PlayerController PC;
    void Start()
    {
        PC = FindObjectOfType<PlayerController>();
    }
    

    // Update is called once per frame
    void Update()
    {
        Synchro();
    }
    
    public void Synchro()
    {
       if(PC.InFarm == false)
        {
            LoadEncloslevel();
            LoadEnclos();
            PC.InFarm = true;
        }

    }
    public void LoadEncloslevel()
    {


        Player_Data data = SaveSystem.LoadEnclosLevel();
        levelEnclo1 = data.LevelEnclo1;
        levelEnclo2 = data.LevelEnclo2;
        levelEnclo3 = data.LevelEnclo3;


    }
    void LoadEnclos()
    {
        if(levelEnclo1 == 0)
        {
            EncloCasseVer.SetActive(true);
            EncloVer.SetActive(false);
        }
        if(levelEnclo1 >= 1)
        {
            EncloCasseVer.SetActive(false);
            EncloVer.SetActive(true);
        }
        if (levelEnclo2 == 0)
        {
            EncloCasseRenard.SetActive(true);
            EncloRenard.SetActive(false);
        }
        if (levelEnclo2 >= 1)
        {
            EncloCasseRenard.SetActive(false);
            EncloRenard.SetActive(true);
        }
    }
}
