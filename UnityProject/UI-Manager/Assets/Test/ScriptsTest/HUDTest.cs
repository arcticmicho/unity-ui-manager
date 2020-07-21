using PartySystems.UIParty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDTest : UIView
{
    public void OpenView1()
    {
        var view1 = UIManager.Instance.RequestView<ViewTest1>(UIManager.EViewPriority.MediumRenderPriority);
        view1.Reference.ShowView();
    }

    public void OpenView2()
    {
        var view2 = UIManager.Instance.RequestView<ViewTest2>(UIManager.EViewPriority.HighRenderPriority);
        view2.Reference.ShowView();
    }
}
