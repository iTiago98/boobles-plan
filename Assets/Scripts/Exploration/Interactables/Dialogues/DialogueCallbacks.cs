using Booble.CardGame;
using Booble.CardGame.Level;
using Booble.Managers;
using UnityEngine;

public class DialogueCallbacks : MonoBehaviour
{
    public void OnQuecaInterview()
    {
        SceneLoader.Instance.LoadNelaOffice0();
    }

    public void PinPon()
    {
        DeckManager.Instance.SetOpponent(Opponent_Name.PPBros);
        SceneLoader.Instance.LoadInterviewScene();
    }

    public void OnBajando()
    {
        SceneLoader.Instance.LoadNelaOffice1();
    }

    public void LoadLounge1()
    {
        SceneLoader.Instance.LoadLoungeScene1();
    }

    public void LoadLounge2()
    {
        SceneLoader.Instance.LoadLoungeScene2();
    }

    public void LoadLounge3()
    {
        SceneLoader.Instance.LoadLoungeScene3();
    }
    
    public void LoadLowerHall1()
    {
        SceneLoader.Instance.LoadLowerHall1();
    }
    
    public void LoadLowerHall2()
    {
        SceneLoader.Instance.LoadLowerHall2();
    }

    public void LoadLowerHall3()
    {
        SceneLoader.Instance.LoadLowerHall3();
    }

    public void LoadBossHall3()
    {
        SceneLoader.Instance.LoadBossHall3();
    }
    
    public void LoadNelaOffice1()
    {
        SceneLoader.Instance.LoadNelaOffice1();
    }

    public void LoadNelaOffice2()
    {
        SceneLoader.Instance.LoadNelaOffice2();
    }

    public void LoadNelaOffice3()
    {
        SceneLoader.Instance.LoadNelaOffice3();
    }

    public void LoadUpperHall1()
    {
        SceneLoader.Instance.LoadUpperHall1();
    }
    
    public void LoadUpperHall2()
    {
        SceneLoader.Instance.LoadUpperHall2();
    }

    public void LoadUpperHall3()
    {
        SceneLoader.Instance.LoadUpperHall3();
    }
    
    public void LoadComedor2()
    {
        SceneLoader.Instance.LoadCanteenScene2();
    }

    public void LoadPPBOffice()
    {
        SceneLoader.Instance.LoadPPBOffice();
    }

    public void LoadHome0()
    {
        SceneLoader.Instance.LoadHome0();
    }

    public void AddGranFinal()
    {
        DeckManager.Instance.AddGranFinal();
    }
    
    public void AddEscorbuto()
    {
        DeckManager.Instance.AddNuevaCepaDelEscorbuto();    
    }

    public void AddExprimir()
    {
        DeckManager.Instance.AddExprimirLaVerdad();
    }

    public void AddMaquinaZumos()
    {
        DeckManager.Instance.AddMaquinaDeZumo();
    }

    public void AddHipervitaminado()
    {
        DeckManager.Instance.AddHipervitaminado();
    }

    public void AddPared()
    {
        DeckManager.Instance.AddPared();
    }

    public void AddPalaDeNocobich()
    {
        DeckManager.Instance.AddPalaDeNocobich();
    }
    
    public void AddGomuGomuNo()
    {
        DeckManager.Instance.AddGomuGomuNo();
    }

    public void AddPelotaBomba()
    {
        DeckManager.Instance.AddPelotaBomba();
    }
    
    public void AddVictoriaPorDesgaste()
    {
        DeckManager.Instance.AddVictoriaPorDesgaste();
    }

    public void AddAnexosCorrespondientes()
    {
        DeckManager.Instance.AddTraigoLosAnexosCorrespondientes();
    }

    public void AddAfidavit()
    {
        DeckManager.Instance.AddAfidavit();
    }

    public void AddResaltarContradicción()
    {
        DeckManager.Instance.AddResaltarUnaContradiccion();
    }

    public void AddPerderPapeles()
    {
        DeckManager.Instance.AddHaPerdidoUsteLosPapele();
    }
}
