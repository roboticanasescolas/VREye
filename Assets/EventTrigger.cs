//#define ACCEL_DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        #if ACCEL_DEBUG
        Debug.Log("X="+Input.acceleration.x*10.0f + " | Y="+Input.acceleration.y*10.0f +" | Z="+Input.acceleration.z*10.0f );
        #endif

        // Move object
        if( Mathf.Abs(Input.acceleration.x*10.0f) > 4.5){
            TaskListenFrases();
        }
    }

    void TaskListenFrases() {   
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.plugin.speech.pluginlibrary.TestPlugin");
        //Debug.Log("Call 1 Started");

        // Pass the name of the game object which has the onActivityResult(string recognizedText) attached to it.
        // The speech recognizer intent will return the string result to onActivityResult method of "Main Camera"
        pluginClass.CallStatic("setReturnObject", "Player");
        //Debug.Log("Return Object Set");


        // Setting language is optional. If you don't run this line, it will try to figure out language based on device settings
        //pluginClass.CallStatic("setLanguage", "en_US");
        //Debug.Log("Language Set");


        // The following line sets the maximum results you want for recognition
        pluginClass.CallStatic("setMaxResults", 3);
        //Debug.Log("Max Results Set");

        // The following line sets the question which appears on intent over the microphone icon
        pluginClass.CallStatic("changeQuestion", "");
        //Debug.Log("Question Set");


        //Debug.Log("Call 2 Started");

        // Calls the function from the jar file
        pluginClass.CallStatic("promptSpeechInput");

        //Debug.Log("Call End");
    }
}
