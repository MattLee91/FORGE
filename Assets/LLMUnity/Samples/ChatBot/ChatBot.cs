using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using LLMUnity;

namespace LLMUnitySamples
{
    public class ChatBot : MonoBehaviour
    {
        public Transform chatContainer;
        public Color playerColor = new Color32(81, 164, 81, 255);
        public Color aiColor = new Color32(29, 29, 73, 255);
        public Color fontColor = Color.white;
        public Font font;
        public int fontSize = 16;
        public int bubbleWidth = 600;
        public LLMClient llm;
        public float textPadding = 10f;
        public float bubbleSpacing = 10f;
        public Sprite sprite;
        public int plotStage = -1;
        public string genre;
        private InputBubble inputBubble;
        private List<Bubble> chatBubbles = new List<Bubble>();
        private bool blockInput = true;
        private BubbleUI playerUI, aiUI;
        private bool warmUpDone = false;
        private int lastBubbleOutsideFOV = -1;

        void Start()
        {
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            playerUI = new BubbleUI
            {
                sprite = sprite,
                font = font,
                fontSize = fontSize,
                fontColor = fontColor,
                bubbleColor = playerColor,
                bottomPosition = 0,
                leftPosition = 0,
                textPadding = textPadding,
                bubbleOffset = bubbleSpacing,
                bubbleWidth = bubbleWidth,
                bubbleHeight = -1
            };
            aiUI = playerUI;
            aiUI.bubbleColor = aiColor;
            aiUI.leftPosition = 1;

            inputBubble = new InputBubble(chatContainer, playerUI, "InputBubble", "Loading...", 4);
            inputBubble.AddSubmitListener(onInputFieldSubmit);
            inputBubble.AddValueChangedListener(onValueChanged);
            inputBubble.setInteractable(false);
            _ = llm.Warmup(WarmUpCallback);
        }

        void onInputFieldSubmit(string newText)
        {
            inputBubble.ActivateInputField();
            if (blockInput || newText.Trim() == "" || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                StartCoroutine(BlockInteraction());
                return;
            }
            blockInput = true;
            // replace vertical_tab
            string message = inputBubble.GetText().Replace("\v", "\n");

            Bubble playerBubble = new Bubble(chatContainer, playerUI, "PlayerBubble", message);
            Bubble aiBubble = new Bubble(chatContainer, aiUI, "AIBubble", "...");
            chatBubbles.Add(playerBubble);
            chatBubbles.Add(aiBubble);
            playerBubble.OnResize(UpdateBubblePositions);
            aiBubble.OnResize(UpdateBubblePositions);

            // print the message to the console
            Debug.Log("Original Message: " + message);
            
            // check if the original message contains the word "exposition" (case-insensitive) and if it does, assign plotStage to 1.
            if (message.ToLower().Contains("exposition"))
            {
                plotStage = 1;
            }
            // check if the original message contains the word "conflict" (case-insensitive) and if it does, assign plotStage to 2.
            else if (message.ToLower().Contains("conflict"))
            {
                plotStage = 2;
            }
            // check if the original message contains the word "rising action" (case-insensitive) and if it does, assign plotStage to 3.
            else if (message.ToLower().Contains("rising action"))
            {
                plotStage = 3;
            }
            // check if the original message contains the word "climax" (case-insensitive) and if it does, assign plotStage to 4.
            else if (message.ToLower().Contains("climax"))
            {
                plotStage = 4;
            }
            // check if the original message contains the word "falling action" (case-insensitive) and if it does, assign plotStage to 5.
            else if (message.ToLower().Contains("falling action"))
            {
                plotStage = 5;
            }
            // check if the original message contains the word "resolution" (case-insensitive) and if it does, assign plotStage to 6.
            else if (message.ToLower().Contains("resolution"))
            {
                plotStage = 6;
            }

            // 
            
            // append a string to the player's message telling the chatbot to prepend their message with "Response: "
            string augmentedMessage = message + "\n Please start every response with: Gotcha!";
            // print the augmented message to the console
            Debug.Log("Augmented_Message: " + augmentedMessage);

            Task chatTask = llm.Chat(augmentedMessage, aiBubble.SetText, AllowInput);
            inputBubble.SetText("");
        }

        // create a function to choose a random .txt file from a folder given a filepath
        public string ChooseRandomFile(string folderPath)
        {
            // get all the files in the folder
            string[] files = System.IO.Directory.GetFiles(folderPath);
            // choose a random file
            string randomFile = files[Random.Range(0, files.Length)];
            // return the random file
            return randomFile;
        }

        public void WarmUpCallback()
        {
            warmUpDone = true;
            inputBubble.SetPlaceHolderText("Message me");
            AllowInput();
        }

        public void AllowInput()
        {
            blockInput = false;
            inputBubble.ReActivateInputField();
        }

        public void CancelRequests()
        {
            llm.CancelRequests();
            AllowInput();
        }

        IEnumerator<string> BlockInteraction()
        {
            // prevent from change until next frame
            inputBubble.setInteractable(false);
            yield return null;
            inputBubble.setInteractable(true);
            // change the caret position to the end of the text
            inputBubble.MoveTextEnd();
        }

        void onValueChanged(string newText)
        {
            // Get rid of newline character added when we press enter
            if (Input.GetKey(KeyCode.Return))
            {
                if (inputBubble.GetText().Trim() == "")
                    inputBubble.SetText("");
            }
        }

        public void UpdateBubblePositions()
        {
            float y = inputBubble.GetSize().y + inputBubble.GetRectTransform().offsetMin.y + bubbleSpacing;
            float containerHeight = chatContainer.GetComponent<RectTransform>().rect.height;
            for (int i = chatBubbles.Count - 1; i >= 0; i--)
            {
                Bubble bubble = chatBubbles[i];
                RectTransform childRect = bubble.GetRectTransform();
                childRect.anchoredPosition = new Vector2(childRect.anchoredPosition.x, y);

                // last bubble outside the container
                if (y > containerHeight && lastBubbleOutsideFOV == -1)
                {
                    lastBubbleOutsideFOV = i;
                }
                y += bubble.GetSize().y + bubbleSpacing;
            }
        }

        void Update()
        {
            if (!inputBubble.inputFocused() && warmUpDone)
            {
                inputBubble.ActivateInputField();
                StartCoroutine(BlockInteraction());
            }
            if (lastBubbleOutsideFOV != -1)
            {
                // destroy bubbles outside the container
                for (int i = 0; i <= lastBubbleOutsideFOV; i++)
                {
                    chatBubbles[i].Destroy();
                }
                chatBubbles.RemoveRange(0, lastBubbleOutsideFOV + 1);
                lastBubbleOutsideFOV = -1;
            }
        }

        public void ExitGame()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
        }
    }
}
