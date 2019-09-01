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
        Vector3 dir = Vector3.zero;

        // we assume that device is held parallel to the ground
        // and Home button is in the right hand

        // remap device acceleration axis to game coordinates:
        //  1) XY plane of the device is mapped onto XZ plane
        //  2) rotated 90 degrees around Y axis
        dir.x = -Input.acceleration.y;
        dir.z = Input.acceleration.x;

        // clamp acceleration vector to unit sphere
        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        // Make it move 10 meters per second instead of 10 meters per frame...
        dir *= Time.deltaTime;
        
        Debug.Log("X="+Input.acceleration.x*10.0f + " | Y="+Input.acceleration.y*10.0f +" | Z="+Input.acceleration.z*10.0f );
        
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
