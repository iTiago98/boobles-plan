using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;

public class FlagChecker : MonoBehaviour
{
    [SerializeField] private GameObject _ppb;
    [SerializeField] private GameObject _dennis;
    [SerializeField] private GameObject _jose;
    [SerializeField] private GameObject _rosalinda;
    [SerializeField] private GameObject _arcadio;
    [SerializeField] private GameObject _queca;
    [SerializeField] private GameObject _elevator;

    private void Awake()
    {
        if (!FlagManager.Instance.GetFlag(Flag.Reference.PPBVictoriaAlternativa))
        {
            _ppb.SetActive(false);
            _dennis.SetActive(false);
        }

        if (!FlagManager.Instance.GetFlag(Flag.Reference.CitrianoVictoriaAlternativa))
        {
            _jose.SetActive(false);
        }

        if (!FlagManager.Instance.GetFlag(Flag.Reference.SecretaryVictoriaAlternativa))
        {
            _rosalinda.SetActive(false);
        }

        if (!FlagManager.Instance.GetFlag(Flag.Reference.GranFinalObtenida))
        {
            _arcadio.SetActive(false);
        }

        if (!FlagManager.Instance.GetFlag(Flag.Reference.FinalBueno))
        {
            _queca.SetActive(false);
        }
        else
        {
            _elevator.SetActive(false);
        }
    }
}
