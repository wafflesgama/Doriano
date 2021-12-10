using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWriter : MonoBehaviour
{
    public System.Action OnFinishedWriting;
    public int writingDelay = 5;
    
    Task writingTask;
    TMP_Text textComponent;

    string[] messagesToWrite;
    int currentMsgIndex;

    void Awake()
    {
        textComponent = gameObject.GetComponent<TMP_Text>();
    }


    void Start()
    {
        currentMsgIndex = 0;
    }

 

    public void RegisterMessages(string[] messages)
    {
        writingTask = null;
        currentMsgIndex = 0;
        messagesToWrite= messages;
    }

    public bool WriteNextMessage()
    {
        if (currentMsgIndex >= messagesToWrite.Length) return false;
        //if(writingTask != null)
        //{
        //    writingTask.Ca
        //}
        WriteMessage(messagesToWrite[currentMsgIndex]);
        currentMsgIndex++;
        return true;
    }

   
    /// <summary>
    /// Method revealing the text one character at a time.
    /// </summary>
    /// <returns></returns>
    public async void WriteMessage(string message)
    {
      
        textComponent.maxVisibleCharacters = 0;
        textComponent.text = message;
        TMP_TextInfo textInfo = textComponent.textInfo;
        textComponent.ForceMeshUpdate();

        await Task.Delay(writingDelay/2);

        int totalVisibleCharacters = textInfo.characterCount; // Get # of Visible Character in text object

        for (int visibleCount = 0; visibleCount < totalVisibleCharacters+1; visibleCount++)
        {
            textComponent.ForceMeshUpdate();
            PlayerSoundManager.currentManager.PlayDialogueTypeSound();
            textComponent.maxVisibleCharacters = visibleCount;
            await Task.Delay(writingDelay);
        }

        OnFinishedWriting?.Invoke();
    }

}

