using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using LLMUnity;
using System.ComponentModel;
using System.Diagnostics;

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
        public int expositionStart = 0;
        public int expositionEnd = 10;
        public int conflictStart = 11;
        public int conflictEnd = 25;
        public int risingActionStart = 26;
        public int risingActionEnd = 50;
        public int climaxStart = 51;
        public int climaxEnd = 75;
        public int fallingActionStart = 76;
        public int fallingActionEnd = 89;
        public int resolutionStart = 90;
        public int resolutionEnd = 100;
        public int maxExtractCharLength = 1000;
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
            UnityEngine.Debug.Log("Original Message: " + message);
            
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
            UnityEngine.Debug.Log("Plot Stage: " + plotStage);

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
            UnityEngine.Debug.Log("Genre: " + genre);

            // store the output of ChooseRandomText in an array.
            // the first item is the text's file path, the second is the text's title, and the third is the text's author
            string[] randomTextOutput = ChooseRandomText(genre);

            // decompose the string array into three strings
            string randomTextPath = randomTextOutput[0];
            string textTitle = randomTextOutput[1];
            string textAuthor = randomTextOutput[2];
            // debug all three
            UnityEngine.Debug.Log("Random Text Path: " + randomTextPath);
            UnityEngine.Debug.Log("Text Title: " + textTitle);
            UnityEngine.Debug.Log("Text Author: " + textAuthor);

            // extract a portion of the text corresponding to the plot stage
            string extractedText = ExtractText(randomTextPath);
            
            // TODO: APPEND AUTHOR AND TEXT NAME TO THE EXTRACTED TEXT

            // TODO: APPEND EXTRACTED TEXT TO THE PLAYER'S MESSAGE WITH INSTRUCTIONS

            // append a string to the player's message telling the chatbot to prepend their message with "Response: "
            string augmentedMessage = message + "\n Please start every response with: Gotcha!";
            // print the augmented message to the console
            UnityEngine.Debug.Log("Augmented_Message: " + augmentedMessage);

            // Define a callback that updates the AI bubble's text with the response
            Callback<string> callback = (response) =>
            {
                // THIS CODE EXECUTES AS EVERY WORD THE AI GENERATES IS RECEIVED
                aiBubble.SetText(response);
            };

            // Define a completion callback that can be used for any post-processing
            EmptyCallback completionCallback = () =>
            {
                // THIS CODE EXECUTES AFTER ALL WORDS OF THE AI'S RESPONSE ARE GENERATED
                string result = aiBubble.GetText();
                // USE THIS RESULT - AND PERHAPS ANOTHER - FOR PLATFORM GENERATION
                UnityEngine.Debug.Log("AI Response: " + result);
                // Any additional logic that should run after the full response is received
            };

            // Call the Chat function with the callbacks
            _ = llm.Chat(augmentedMessage, callback, completionCallback, true);

            inputBubble.SetText("");
        }

        // a function to choose a random .txt file from the "Assets/Literature/(genre)" folder given genre as a string input
        public string[] ChooseRandomText(string genre)
        {
            // the current relative path is: Assets\LLMUnity\Samples\ChatBot\ChatBot.cs
            // the relative path to the literature is: Assets\Literature

            // define the path to the literature folder for the given genre
            string literaturePath = Application.dataPath + "/Literature/" + genre;

            // include error handling if folder does not exist for that genre
            if (!System.IO.Directory.Exists(literaturePath))
            {
                UnityEngine.Debug.LogError("The folder for the genre " + genre + " does not exist.");
                return new string[] {"ERROR: Folder does not exist.", "ERROR: Folder does not exist.", "ERROR: Folder does not exist."};
            }

            // within the "Assets/Literature/(genre)" folder, there are folders named after authors; within them, a random file is selected.
            // get all the folders in the "Assets/Literature/(genre)" folder
            string[] folders = System.IO.Directory.GetDirectories(literaturePath);

            // choose a random folder from the folders array
            string randomFolder = folders[Random.Range(0, folders.Length)];

            // save the folder name
            string folderName = System.IO.Path.GetFileName(randomFolder);
            // debug log the folder name
            UnityEngine.Debug.Log("Folder Name: " + folderName);

            // get all the .txt files in the random folder
            string[] files = System.IO.Directory.GetFiles(randomFolder, "*.txt");

            // include error handling if there are no .txt files in the folder
            if (files.Length == 0)
            {
                UnityEngine.Debug.LogError("There are no .txt files in the folder " + folderName);
                return new string[] {"ERROR: No .txt files in the folder.", "ERROR: No .txt files in the folder.", "ERROR: No .txt files in the folder."};
            }

            // choose a random .txt file from the files array
            string randomFile = files[Random.Range(0, files.Length)];
            // debug log the file path
            UnityEngine.Debug.Log("File Path: " + randomFile);
            
            // save the file name
            string fileName = System.IO.Path.GetFileName(randomFile);
            // debug log the file name
            UnityEngine.Debug.Log("File Name: " + fileName);

            // return the path of the random .txt file, the file name (the title of the text), and the folder name (the author of the text)
            return new string[] {randomFile, fileName, folderName};
        }

        // a function to which, given a plot stage (integer) and file path (string), extracts a portion of text from the file
        public string ExtractText(string fileName)
        {
            // include error handling if the file does not exist at that path
            if (!System.IO.File.Exists(fileName))
            {
                UnityEngine.Debug.LogError("The file " + fileName + " does not exist.");
                return "ERROR: File does not exist.";
            }

            // read the contents of the file
            string text = System.IO.File.ReadAllText(fileName);

            // calculate the total length of the text
            int totalLength = text.Length;

            // depending on the plot stage, calculate the start and end positions of the text to be extracted
            // if plotStage is 1, extract the text from expositionStart to expositionEnd
            // store the extracted text in a string variable

            string plotSegment = "";
            int start = -1;
            int end = -1;
            if (plotStage == 1)
            {
                start = (int)(totalLength * expositionStart / 100.0);
                end = (int)(totalLength * expositionEnd / 100.0);
            }
            // if plotStage is 2, extract the text from conflictStart to conflictEnd
            else if (plotStage == 2)
            {
                start = (int)(totalLength * conflictStart / 100.0);
                end = (int)(totalLength * conflictEnd / 100.0);
            }
            // if plotStage is 3, extract the text from risingActionStart to risingActionEnd
            else if (plotStage == 3)
            {
                start = (int)(totalLength * risingActionStart / 100.0);
                end = (int)(totalLength * risingActionEnd / 100.0);
            }
            // if plotStage is 4, extract the text from climaxStart to climaxEnd
            else if (plotStage == 4)
            {
                start = (int)(totalLength * climaxStart / 100.0);
                end = (int)(totalLength * climaxEnd / 100.0);
            }
            // if plotStage is 5, extract the text from fallingActionStart to fallingActionEnd
            else if (plotStage == 5)
            {
                start = (int)(totalLength * fallingActionStart / 100.0);
                end = (int)(totalLength * fallingActionEnd / 100.0);
            }
            // if plotStage is 6, extract the text from resolutionStart to resolutionEnd
            else if (plotStage == 6)
            {
                start = (int)(totalLength * resolutionStart / 100.0);
                end = (int)(totalLength * resolutionEnd / 100.0);
            }
            // debug log the start and end positions
            UnityEngine.Debug.Log("Start: " + start + ", End: " + end);

            plotSegment = text.Substring(start, end - start);

            /*
                This segment, depending on the length of the file, could be very long.
                It is necessary to limit it to about 150 words, or 1000 characters.

                REFERENCE: https://capitalizemytitle.com/character-count/1000-characters/

                "According to research by Miller, Newman, and Friedman,
                the average length of a function word was 3.13 letters;
                for a content word the average was 6.47 letters. To get...estimates...,
                we [assume] that words in sentences range from 4-7 letters. We also [assume]
                that sentences have 15-20 words and paragraphs typically have 100-200 words and 5-6 sentences.
            */

            // if the extracted text is longer than the maximum character length, limit it to the maximum character length
            if (plotSegment.Length > maxExtractCharLength)
            {
                // calculate the maximum starting position for the chunk
                int maxStartPosition = plotSegment.Length - maxExtractCharLength;

                // select a random starting point within the valid range
                int randomStartPosition = Random.Range(0, maxStartPosition);

                // extract the chunk of text starting from the random point
                plotSegment = plotSegment.Substring(randomStartPosition, maxExtractCharLength);
            }
            // if the extracted text is shorter than the maximum character length, nothing is necessary to be done

            // debug log the extracted text (PRE-FORMATTING)
            UnityEngine.Debug.Log("PRE-FORMAT EXTRACTED TEXT: " + plotSegment);
            /*
                Additional formatting of the text could be done here, such as ensuring it ends at the end of a sentence (denoted by a period, exclamation point, or question mark).
                This would be accomplished by finding the last instances of '.', '!', or '?' and truncating the text to the one with the index at the furthest point in the text.
                Similar action could be done for the beginning of the text, ensuring it starts at the beginning of a sentence.
            */

            // ensure the text ends at the end of a sentence
            int lastPeriod = plotSegment.LastIndexOf('.');
            int lastExclamation = plotSegment.LastIndexOf('!');
            int lastQuestion = plotSegment.LastIndexOf('?');
            int lastSentenceEnd = Mathf.Max(lastPeriod, lastExclamation, lastQuestion);
            if (lastSentenceEnd != -1)
            {
                plotSegment = plotSegment.Substring(0, lastSentenceEnd + 1);
            }

            // ensure the text starts at the beginning of a sentence
            int firstPeriod = plotSegment.IndexOf('.');
            int firstExclamation = plotSegment.IndexOf('!');
            int firstQuestion = plotSegment.IndexOf('?');
            int firstSentenceStart = Mathf.Min(firstPeriod, firstExclamation, firstQuestion);
            if (firstSentenceStart != -1)
            {
                plotSegment = plotSegment.Substring(firstSentenceStart + 1);
            }

            // remove trailing whitespace
            plotSegment = plotSegment.Trim();
            
            // debug log the extracted text (POST-FORMATTING)
            UnityEngine.Debug.Log("POST-FORMAT EXTRACTED TEXT: " + plotSegment);

            return plotSegment;
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
            UnityEngine.Debug.Log("Exit button clicked");
            Application.Quit();
        }
    }
}
