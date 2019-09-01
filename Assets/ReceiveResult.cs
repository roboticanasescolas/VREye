using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiveResult : MonoBehaviour {

    public GameObject mainPlayerObject;  // Objeto com script MainUpdate
     
    private MainUpdate _mainUpdateScript;
 

	// Use this for initialization
	void Start () {
        _mainUpdateScript = GameObject.FindObjectOfType(typeof(MainUpdate)) as MainUpdate;
	}
	
    void onActivityResult(string recognizedText){
        if( _mainUpdateScript == null){
            Debug.Log("Script era nulo. Acessando por parâmetro...");
            _mainUpdateScript = mainPlayerObject.GetComponent<MainUpdate>();
        }

        char[] delimiterChars = {'~'};
        string[] result = recognizedText.Split(delimiterChars);

        //You can get the number of results with result.Length
        //And access a particular result with result[i] where i is an int
        //I have just assigned the best result to UI text
        GameObject.Find("TextoTempView").GetComponent<Text>().text = result[0];

        processaMsg(result[0]);

    }

	// Update is called once per frame
	void Update () {
		
	}

    private void processaMsg(string txt){
        if(txt.ToLower().IndexOf("modo de câmera") > -1){
            if(txt.ToLower().IndexOf("contraste") > -1){
                Debug.Log("Filtro: Alto-contraste");
                
                if(txt.ToLower().IndexOf("alto") > -1){
                    _mainUpdateScript.Contraste = 85;
                    Debug.Log("Nivel: Alto");
                }
                else if(txt.ToLower().IndexOf("médio") > -1){
                    _mainUpdateScript.Contraste = 60;
                    Debug.Log("Nivel: Médio");
                }
                else if(txt.ToLower().IndexOf("baixo") > -1){
                    _mainUpdateScript.Contraste = 25;
                    Debug.Log("Nivel: Baixo");
                } else _mainUpdateScript.Contraste = 15;
                
                _mainUpdateScript.Filtro = FILTROS_POSTPROCESS.ALTO_CONTRASTE_1;
            } else if(txt.ToLower().IndexOf("normal") > -1){
                Debug.Log("Filtro: Off");
                _mainUpdateScript.Filtro = FILTROS_POSTPROCESS.NENHUM;
            } else if(txt.ToLower().IndexOf("saturação") > -1){
                Debug.Log("Filtro: Saturação");
                _mainUpdateScript.Filtro = FILTROS_POSTPROCESS.ALTA_SATURACAO;
            }
        }
    }

}
