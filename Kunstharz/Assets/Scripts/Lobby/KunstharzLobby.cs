using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kunstharz
{

    public class KunstharzLobby : MonoBehaviour
    {

        [SerializeField]
        public Button startGame;
        [SerializeField]
        public Button joinGame;
        [SerializeField]
        public GameObject gamePublisher;
        [SerializeField]
        public GameObject gameFinder;

        void Awake()
        {
            gameFinder.SetActive(false);
            gamePublisher.SetActive(false);
            
            startGame.onClick.AddListener(delegate
            {
                StartGameBtnClickHandler();
            });

            joinGame.onClick.AddListener(delegate
            {
                JoinGameBtnClickHandler();
            });
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // ----------------- private methods -------------------

        private void StartGameBtnClickHandler()
        {

        }

        private void JoinGameBtnClickHandler()
        {

        }
    }

}
