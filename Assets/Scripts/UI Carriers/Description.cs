using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Carriers
{
    public class Description : Singleton<Description>
    {
        //note: the description can hide the object and cause flickering
        //note: when the mouse enters an object, it leaves the previous object
        //note: singleton has to start active!!!!!!
        
        
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private Image _background;

        private List<KeyValuePair<GameObject, string>> _descriptionsQueue;

        private void Awake()
        {
            _descriptionsQueue = new List<KeyValuePair<GameObject, string>>();
            Instance.gameObject.SetActive(false);
        }

        /**
         * removes the description of the game objects
         * if the first description is not the objects, it appears as if nothing happens
         * @return true iff gameObject was queued
         */
        public static bool RemoveDescription(GameObject obj)
        {
            return Instance.RemoveDescriptionNotStatic(obj);
        }

        public static void AddDescription(GameObject obj, string text)
        {
            Instance.AddDescriptionNotStatic(obj, text);
        }

        private void AddDescriptionNotStatic(GameObject obj, string text)
        {
            _descriptionsQueue.Add(new KeyValuePair<GameObject, string>(obj, text));
            DisplayText();
        }

        private bool RemoveDescriptionNotStatic(GameObject obj)
        {
            var originalCount = _descriptionsQueue.Count;
            _descriptionsQueue = _descriptionsQueue.Where(pair => pair.Key != obj).ToList();
            
            if (_descriptionsQueue.Count == 0) HideText();
            else DisplayText();
            
            return originalCount != _descriptionsQueue.Count;
        }

        private void DisplayText()
        {
            gameObject.SetActive(true);
            transform.position = _descriptionsQueue[0].Key.transform.position;
            _tmpText.text = _descriptionsQueue[0].Value;
        }

        private void HideText()
        {
            gameObject.SetActive(false);
        }
    }
}