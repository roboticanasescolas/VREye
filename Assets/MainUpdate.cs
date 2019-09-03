using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FILTROS_POSTPROCESS {NENHUM, ALTA_SATURACAO, ALTO_CONTRASTE_1};

public class MainUpdate : MonoBehaviour
{
    
	

    /* WEBCAM */
    
    public bool frontFacing;
    public RawImage background;
    public FILTROS_POSTPROCESS Filtro;
    public int Contraste;
    public AspectRatioFitter fit;
    private bool camAvailable;
	private WebCamTexture cameraTexture;
    private Texture2D output;
    private Color32[] data;

    private WebTTS _webtts;

    private int maxResolution = 900;
    
    IEnumerator vozInicializacao(){
        _webtts.Fala("Aguarde, preparando voz.");

        yield return StartCoroutine(_webtts.PrepareBasicAudios());
        solicitaPermissoes();
        


        _webtts.PlayAudioPredefinido(FalaPredefinida.AUDIO_MENU_APRESENTACAO);

        cameraConfig();
    }

    void cameraConfig(){
        WebCamDevice[] devices = WebCamTexture.devices;

        Debug.Log("CAMs count: " + devices.Length );

        int rendWidth = Screen.width, rendHeight = Screen.height;

        if (devices.Length == 0) return;
        for (int devID = 0; devID < devices.Length; devID++) {
            
			var curr = devices[devID];
            Debug.Log("CAM: "+curr.name);
                        
			if (curr.isFrontFacing == frontFacing || (devices.Length == 1)) {

                if(curr.availableResolutions != null)
                    if( curr.availableResolutions.Length > 0){
                        Resolution myRes = new Resolution(); 
                        foreach (var res in curr.availableResolutions) {
                            //Debug.Log("Res: "+curr.availableResolutions[resID].height+"x"+curr.availableResolutions[resID].width);
                            if(res.height < this.maxResolution && res.width < this.maxResolution) {
                                myRes = res;
                                break;
                            }
                        }
                        rendWidth = myRes.width;
                        rendHeight = myRes.height;
                    }
				cameraTexture = new WebCamTexture(curr.name, rendWidth, rendHeight);
				break;
			}
        }

        if (cameraTexture == null) return;

        float ratio = (float)rendWidth / (float)rendHeight;
        fit.aspectRatio = ratio; // Set the aspect ratio


        cameraTexture.Play(); // Start the camera
        camAvailable = true;
        output = new Texture2D(cameraTexture.width, cameraTexture.height);
        background.texture = output;
        data = new Color32[cameraTexture.width * cameraTexture.height];

        
    }
    
    void Start(){
        /* Instancia Text-to-speech */
        _webtts = GameObject.FindObjectOfType(typeof(WebTTS)) as WebTTS;

        
        StartCoroutine(vozInicializacao());
        

        

        //
        
        /*WebCamTexture webcamTexture = new WebCamTexture();
        rend.material.mainTexture = webcamTexture;
        webcamTexture.Play();*/

        // Importado parte de: https://github.com/Chamuth/unity-webcam/blob/master/MobileCam.cs

		

    }


    // Update is called once per frame
    void Update() {

        if (!camAvailable) return;

        
        
        
		float scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
		background.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

        if (data != null) {
            cameraTexture.GetPixels32(data);

            if(this.Filtro != FILTROS_POSTPROCESS.NENHUM){
                int pixel=0;
                
                if(this.Filtro == FILTROS_POSTPROCESS.ALTA_SATURACAO){
                    float H, S, V;
                    while(pixel < data.Length){
                        Color.RGBToHSV( data[pixel] , out H, out S, out V);
                        data[pixel] = Color.HSVToRGB(H, 1, V);
                        pixel++;
                    }
                } else if (this.Filtro == FILTROS_POSTPROCESS.ALTO_CONTRASTE_1){
                    /*
                    Ref.: https://www.dfstudios.co.uk/articles/programming/image-programming-algorithms/image-processing-algorithms-part-5-contrast-adjustment/
                    factor = (259 * (contrast + 255)) / (255 * (259 - contrast))
                    colour = GetPixelColour(x, y)
                    newRed   = Truncate(factor * (Red(colour)   - 128) + 128)
                    newGreen = Truncate(factor * (Green(colour) - 128) + 128)
                    newBlue  = Truncate(factor * (Blue(colour)  - 128) + 128)
                    PutPixelColour(x, y) = RGB(newRed, newGreen, newBlue)
                    */

                    float fator = ((float)(259f * (this.Contraste + 255f))) / ((float)(255f * (259f - this.Contraste)));
                    while(pixel < data.Length){
                        data[pixel] = new Color32(
                            (byte) Mathf.Round(fator * (data[pixel].r   - 128f) + 128f),
                            (byte) Mathf.Round(fator * (data[pixel].g   - 128f) + 128f),
                            (byte) Mathf.Round(fator * (data[pixel].b   - 128f) + 128f),
                            (byte) data[pixel].a
                        );
                        pixel++;
                    }
                }

            }

            output.SetPixels32(data);
            output.Apply();
        }
        

    }

    private void solicitaPermissoes(){
        string[] permissoes = {
            "android.permission.INTERNET",
            "android.permission.ACCESS_NETWORK_STATE",
            "android.permission.WRITE_EXTERNAL_STORAGE",
            "android.permission.CAMERA"
            };
        AndroidRuntimePermissions.Permission[] permChecadas = AndroidRuntimePermissions.CheckPermissions( permissoes );
        List<string> novasRequisicoes = new List<string>();

        for(int i=0; i < permChecadas.Length; i++){
            if( ! (permChecadas[i] == AndroidRuntimePermissions.Permission.Granted) )
			    novasRequisicoes.Add(permissoes[i]);
        }

        if(novasRequisicoes.Count > 0){
            _webtts.PlayAudioPredefinido(FalaPredefinida.AUDIO_APRESENTACAO);
            int Permitido = 0;
            AndroidRuntimePermissions.Permission[] resultRequest = AndroidRuntimePermissions.RequestPermissions( novasRequisicoes.ToArray() );
            for(int i=0; i < resultRequest.Length; i++){
                if( resultRequest[i] == AndroidRuntimePermissions.Permission.Granted ){
                    Permitido++;
                    Debug.Log( "Nova permissão concedida!" );
                }
                else
                    Debug.Log( "Estado da permissão: " + resultRequest[i] );
            }
            if(Permitido == resultRequest.Length){
                _webtts.PlayAudioPredefinido(FalaPredefinida.AUDIO_PERMISSOES_OK);
            }  
        }
        
    }

    


}
