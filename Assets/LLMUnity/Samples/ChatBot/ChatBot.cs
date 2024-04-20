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
            
            /* PLOT STAGE DETECTION */
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
            // debug log the plot stage
            Debug.Log("Plot Stage: " + plotStage);

            /* GENRE DETECTION */
            // check if the original message contains the word "mystery" (case-insensitive) and if it does, assign genre to "mystery".
            if (message.ToLower().Contains("mystery"))
            {
                genre = "mystery";
            }
            // check if the original message contains the word "fantasy" (case-insensitive) and if it does, assign genre to "fantasy".
            else if (message.ToLower().Contains("fantasy"))
            {
                genre = "fantasy";
            }
            // check if the original message contains the word "science fiction" or "sci fi" or "sci-fi" or "scifi" (case-insensitive) and if it does, assign genre to "scifi".
            else if (message.ToLower().Contains("science fiction") || message.ToLower().Contains("sci fi") || message.ToLower().Contains("sci-fi") || message.ToLower().Contains("scifi"))
            {
                genre = "sci_fi";
            }
            // check if the original message contains the word "romance" (case-insensitive) and if it does, assign genre to "romance".
            else if (message.ToLower().Contains("romance"))
            {
                genre = "romance";
            }
            // check if the original message contains the word "horror" (case-insensitive) and if it does, assign genre to "horror".
            else if (message.ToLower().Contains("horror"))
            {
                genre = "horror";
            }
            // check if the original message contains the word "comedy" (case-insensitive) and if it does, assign genre to "comedy".
            else if (message.ToLower().Contains("comedy"))
            {
                genre = "comedy";
            }
            // check if the original message contains the word "drama" (case-insensitive) and if it does, assign genre to "drama".
            else if (message.ToLower().Contains("drama"))
            {
                genre = "drama";
            }
            // check if the original message contains the word "fairy tale" or "fairytale" or "fairy-tale" or "folk tale" or "folktale" or "folk-tale" (case-insensitive) and if it does, assign genre to "tale".
            else if (message.ToLower().Contains("fairy tale") || message.ToLower().Contains("fairytale") || message.ToLower().Contains("fairy-tale") || message.ToLower().Contains("folk tale") || message.ToLower().Contains("folktale") || message.ToLower().Contains("folk-tale"))
            {
                genre = "tale";
            }
            // debug log the genre
            Debug.Log("Genre: " + genre);
            
            // append a string to the player's message telling the chatbot to prepend their message with "Response: "
            string augmentedMessage = message + "\n Please start every response with: Gotcha!";
            // print the augmented message to the console
            Debug.Log("Augmented_Message: " + augmentedMessage);

            Task chatTask = llm.Chat(augmentedMessage, aiBubble.SetText, AllowInput);
            inputBubble.SetText("");
        }

        // a function to choose a random .txt file from the "Assets/Literature/(genre)" folder given genre as a string input
        public string ChooseRandomText(string genre)
        {
            // get all the .txt files in the "Assets/Literature/(genre)" folder
            string[] files = System.IO.Directory.GetFiles("Assets/Literature/" + genre, "*.txt");
            // choose a random .txt file from the files array
            string randomFile = files[Random.Range(0, files.Length)];
            // return the path of the random .txt file
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
