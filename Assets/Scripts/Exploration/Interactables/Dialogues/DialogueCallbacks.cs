using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCallbacks : MonoBehaviour
{
    public void OnQuecaInterview()
    {
        SceneLoader.Instance.LoadNelaOffice0();
    }

    public void OnBajando()
    {
        SceneLoader.Instance.LoadNelaOffice1();
    }

    public void LoadLounge1()
    {
        SceneLoader.Instance.LoadLoungeScene1();
    }

    public void LoadLowerHall1()
    {
        SceneLoader.Instance.LoadLowerHall1();
    }

    public void LoadNelaOffice1()
    {
        SceneLoader.Instance.LoadNelaOffice1();
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
}
