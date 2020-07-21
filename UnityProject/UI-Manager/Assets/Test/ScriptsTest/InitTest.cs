using PartySystems.UIParty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var hud = UIManager.Instance.GetHUD<HUDTest>(UIManager.EViewPriority.HighRenderPriority);
        hud.Reference.ShowView();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
