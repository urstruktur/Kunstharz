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
            gameFinder.SetActive(false);
            gameFinder.GetComponent<GameFinder>().enabled = false;
            gamePublisher.SetActive(true);
            gamePublisher.GetComponent<GamePublisher>().enabled = true;
        }

        private void JoinGameBtnClickHandler()
        {
            gamePublisher.SetActive(false);
            gamePublisher.GetComponent<GamePublisher>().enabled = false;
            gameFinder.SetActive(true);
            gameFinder.GetComponent<GameFinder>().enabled = true;
        }
    }

}
