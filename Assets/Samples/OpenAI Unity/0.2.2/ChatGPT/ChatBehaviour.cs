// Licensed under the MIT License. See LICENSE in the project root for license information.

// using OpenAI.ChatGPT;
using OpenAI.Chat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Async;
using Utilities.Audio;
using Utilities.Encoding.Wav;
using Utilities.Extensions;
using Utilities.WebRequestRest;

namespace OpenAI.Samples.Chat
{
public class ChatBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool enableDebug;

    [SerializeField]
    private Button submitButton;
    
    [SerializeField]
    public TMP_InputField inputField;

    [SerializeField]
    private RectTransform contentArea;

    [SerializeField]
    private ScrollRect scrollView;

    public Button resetButton;

    public Button modelButton;

    public TMP_Text modelButtonText;

    public TMP_FontAsset myCustomFont;

    public HPManager hpManager;
    
    [SerializeField]
    [TextArea(3, 10)]
    private string systemPrompt;

    private OpenAIClient openAI;
    private Conversation conversation = new();

    //For Non-OpenAI models
    private OpenAIApi openaiOther = new OpenAIApi(""); //Put openrouter API key here
    private List<ChatMessage> messages = new List<ChatMessage>();

    private enum TextModel { GPT4o = 0, Claude37Sonnet = 1, Gemini2Flash = 2, Grok3 = 3 }

    private Dictionary<TextModel,string> Labels = new()
    {
        { TextModel.GPT4o, "GPT-4o" },
        { TextModel.Claude37Sonnet,"Claude3.7S" },
        { TextModel.Gemini2Flash,"G2Flash" },
        { TextModel.Grok3,"Grok 3" }
    };

    private Dictionary<TextModel,string> ModelIds = new()
    {
        { TextModel.GPT4o,      "openai/chatgpt-4o-latest" },
        { TextModel.Claude37Sonnet,"anthropic/claude-3.7-sonnet" },
        { TextModel.Gemini2Flash,"google/gemini-2.0-flash-001" },
        { TextModel.Grok3,"x-ai/grok-3-beta" }
    };

    private int modelIndex = 0;

    private void OnValidate()
    {
        inputField.Validate();
        contentArea.Validate();
        submitButton.Validate();
    }

    private void Awake()
    {
        OnValidate();
        openAI = new OpenAIClient("") //Don't put any API here
        {
            EnableDebug = enableDebug
        };


        conversation.AppendMessage(new Message(Role.System, systemPrompt));
        resetButton.onClick.AddListener(ResetChat);

        var disclaimerMessageContent = AddNewTextMessageContent(Role.Assistant);
        disclaimerMessageContent.text = "Rabbit: I've studied so much that I've partially lost my hearing :( ! therefore I might misunderstand how some words are spelled. If I make a mistake, apologies and please correct me if I heard you wrong.";
    }
    
    private static bool isChatPending;

    public async void SubmitChat()
    {
        if (isChatPending || string.IsNullOrWhiteSpace(inputField.text)) { return; }

        if(hpManager.GetHP() <= 0) {

            hpManager.ResetHP(); //For demo
        }

        // else //Commented out for demo
        // {    
        isChatPending = true;

        inputField.ReleaseSelection();
        inputField.interactable = false;
        submitButton.interactable = false;
        modelButton.interactable = false;


        //For openAI
        conversation.AppendMessage(new Message(Role.User, inputField.text));

        //For openRouter
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = inputField.text
        };
        if (messages.Count == 0) newMessage.Content = systemPrompt + "\n" + inputField.text; 
        messages.Add(newMessage);

        var userMessageContent = AddNewTextMessageContent(Role.User);
        userMessageContent.text = $"You: {inputField.text}";
        inputField.text = string.Empty;
        var assistantMessageContent = AddNewTextMessageContent(Role.Assistant);
        assistantMessageContent.text = "Rabbit: ";

        try
        {
            // var request = new ChatRequest(  messages: conversation.Messages,
            //                                 model: "gpt-4o",
            //                                 temperature: 0.2,
            //                                 maxTokens: 2048,
            //                                 topP: 1,
            //                                 frequencyPenalty: 0,
            //                                 presencePenalty: 0
            //                                 );
            // var response = await openAI.ChatEndpoint.StreamCompletionAsync(request, resultHandler: deltaResponse =>
            // {
            //     if (deltaResponse?.FirstChoice?.Delta == null) { return; }
            //     assistantMessageContent.text += deltaResponse.FirstChoice.Delta.ToString();
            //     scrollView.verticalNormalizedPosition = 0f;
            // }, CancellationToken.None);

            var completionResponse = await openaiOther.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                // Model = ModelIds[(TextModel)modelIndex],
                Model = "google/gemini-2.5-flash",
                Messages = messages,
                Temperature = 0.2f,
                MaxTokens = 2048
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                assistantMessageContent.text += message.Content;
                messages.Add(message);
            }
            else
            {
                assistantMessageContent.text += "No text was generated from this prompt.";
            }
            scrollView.verticalNormalizedPosition = 0f;

            assistantMessageContent.text = RenderCodeBlocks(assistantMessageContent.text);
            // conversation.AppendMessage(response.FirstChoice.Message);
            // hpManager.DecrementHP();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            inputField.interactable = true;
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            submitButton.interactable = true;
            isChatPending = false;
        }
        // }
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollView.verticalNormalizedPosition = 0f;
    }

    public void ResetChat()
    {
        conversation = new Conversation();
        conversation.AppendMessage(new Message(Role.System, systemPrompt));

        messages.Clear();
        modelButton.interactable = true;

        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }
        scrollView.verticalNormalizedPosition = 1f;

        var assistantMessageContent = AddNewTextMessageContent(Role.Assistant);
        assistantMessageContent.text = "Rabbit: You have reset the chat. Please wait for 5 secs.";

        StartCoroutine(ContinueMessageAfterDelay(assistantMessageContent));
    }

    private IEnumerator ContinueMessageAfterDelay(TextMeshProUGUI assistantMessageContent)
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds

        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        scrollView.verticalNormalizedPosition = 1f;
        assistantMessageContent.text = "Rabbit: You can speak now or feel free to choose another model.";
    }
    
    private TextMeshProUGUI AddNewTextMessageContent(Role role)
    {
        var textObject = new GameObject($"{contentArea.childCount + 1}_{role}");
        textObject.transform.SetParent(contentArea, false);
        var textMesh = textObject.AddComponent<TextMeshProUGUI>();
        textMesh.font = myCustomFont;
        textMesh.fontSize = 24;
        #if UNITY_2023_1_OR_NEWER
        textMesh.textWrappingMode = TextWrappingModes.Normal;
        #else
        textMesh.enableWordWrapping = true;
        #endif

        // string formattedMessageText = RenderCodeBlocks(messageText);
        // textMesh.text = formattedMessageText;

        return textMesh;
    }

    private string RenderCodeBlocks(string text)
    {
        // Regular expression pattern to match code blocks
        string pattern = @"```(.*?)\n(.*?)```";

        // Find all code blocks in the text
        MatchCollection matches = Regex.Matches(text, pattern, RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            string codeBlock = match.Value;
            string language = match.Groups[1].Value;
            string code = match.Groups[2].Value;

            // Replace the code block with formatted text
            string formattedCodeBlock = $"<color=#00ff00>{language}</color>\n<color=#ffffff>{code}</color>";
            text = text.Replace(codeBlock, formattedCodeBlock);
        }

        return text;
    }

    public void changeModel()
    {
        modelIndex = (modelIndex >= 4) ? 0 : modelIndex + 1;

        modelButtonText.text = Labels[(TextModel)modelIndex];
    }
}
}