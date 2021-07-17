using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISample
{
    public class UISampleTest: MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChatUISample.Instance.Show();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChatUISample.Instance.Close();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                QuestUISample.Instance.Show();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                QuestUISample.Instance.Close();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                HistoryUI.ShowPreviousMenu();
            }


            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                MonobehaviorSingtonSample.Instance.MyPoint++;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                MonobehaviorSingtonSample.Instance.SomeMethod();
            }


            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                CSharpSingtonSample.Instance.MyPoint++;
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                CSharpSingtonSample.Instance.SomeMethod();
            }
        }
    }
}