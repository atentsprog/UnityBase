using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISample
{
    public class QueryUISample : MonoBehaviour
    {
        public static QueryUISample Instace;
        GameObject buttonBase;
        private void Awake()
        {
            Instace = this;
            buttonBase = transform.Find("ButtonGroup/Button").gameObject;
            buttonBase.SetActive(false);
        }

        List<GameObject> buttons = new List<GameObject>();
        Action<string> fn;
        internal void Show(string content, Action<string> _fn
            , params string[] buttonTexts)
        {
            fn = _fn;
            transform.Find("ContentText").GetComponent<Text>().text = content;

            foreach (var buttonText in buttonTexts)
            {
                GameObject newButton = Instantiate(buttonBase, buttonBase.transform.parent);
                newButton.SetActive(true);
                buttons.Add(newButton);
                Button button = newButton.GetComponent<Button>();
                button.onClick.AddListener(() => { OnClick(button); });
                newButton.GetComponentInChildren<Text>().text = buttonText;
            }
        }
        void OnClick(Button button)
        {
            var text = button.GetComponentInChildren<Text>().text;
            fn(text);
            gameObject.SetActive(false);
            buttons.ForEach(x => Destroy(x));
            buttons.Clear();
        }
    }
}
