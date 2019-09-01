using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUpdate : MonoBehaviour
{
    //public Renderer rend;
	

    /* WEBCAM */
    public bool frontFacing;
    public RawImage background;
    public AspectRatioFitter fit;
    private bool camAvailable;
	private WebCamTexture cameraTexture;
    private Texture2D output;
    private Color32[] data;
    
    
    void Start()
    {
        solicitaPermissoes();
        
        /*WebCamTexture webcamTexture = new WebCamTexture();
        rend.material.mainTexture = webcamTexture;
        webcamTexture.Play();*/

        // Importado parte de: https://github.com/Chamuth/unity-webcam/blob/master/MobileCam.cs

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
                    Resolution myRes = curr.availableResolutions[0]; 
                    for(int resID= 1; resID < curr.availableResolutions.Length; resID++){
                        Debug.Log("Res: "+curr.availableResolutions[resID].height+"x"+curr.availableResolutions[resID].width);
                        if(myRes.height < curr.availableResolutions[resID].height)
                            myRes = curr.availableResolutions[resID];
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


        //this.plano = GameObject.Find("PlanoRender");
        //plano.renderer.material.mainTexture = textureCamera;
    }


    // Update is called once per frame
    void Update() {

        if (!camAvailable) return;

        
        
        
		float scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
		background.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

        if (data != null) {
            cameraTexture.GetPixels32(data);
            // Processa

            // para de processar
            output.SetPixels32(data);
            output.Apply();
        }
        
        //tex2D = (cameraTexture as Texture2D);
        

        /* Post Processing */
        
        /*for (int y = 0; y < background.texture.height; y++)
        {
            for (int x = 0; x < background.texture.width; x++)
            {
                //Color color = ((x & y) != 0 ? Color.white : Color.gray);
                Color COR = (background.texture as Texture2D).GetColor(x, y);
                newTexture.SetPixel(x, y, color);
            }
        }*/

        //(background.texture as Texture2D).Apply();
        
        
        

		/*int orient = -cameraTexture.videoRotationAngle;
		background.rectTransform.localEulerAngles = new Vector3(0,0, orient);*/

        //if( NativeCamera.IsCameraBusy() ) return;
        //TakePicture( 512 );
    }

    void solicitaPermissoes(){
        string[] permissoes = {
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
            AndroidRuntimePermissions.Permission[] resultRequest = AndroidRuntimePermissions.RequestPermissions( novasRequisicoes.ToArray() );
            for(int i=0; i < resultRequest.Length; i++){
                if( resultRequest[i] == AndroidRuntimePermissions.Permission.Granted )
                    Debug.Log( "Nova permissão concedida!" );
                else
                    Debug.Log( "Estado da permissão: " + resultRequest[i] );
            }   
        }
        
    }

    public static Texture2D ToTexture2D(WebCamTexture texture)
         {
             return Texture2D.CreateExternalTexture(
                 texture.width,
                 texture.height,
                 TextureFormat.RGB24,
                 false, false,
                 texture.GetNativeTexturePtr());
         }

/* 
    private void TakePicture( int maxSize ) {
        NativeCamera.Permission permission = NativeCamera.TakePicture( ( path ) =>
        {
            //Debug.Log( "Image path: " + path );
            if( path != null )
            {
                // Create a Texture2D from the captured image
                Texture2D textureCamera = NativeCamera.LoadImageAtPath( path, maxSize );
                if( textureCamera == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }

                //rend.material.mainTexture = textureCamera;

                
                
                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                quad.transform.localScale = new Vector3( 1f, textureCamera.height / (float) textureCamera.width, 1f );
                
                Material material = quad.GetComponent<Renderer>().material;
                if( !material.shader.isSupported ) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find( "Legacy Shaders/Diffuse" );

                material.mainTexture = textureCamera;
                
                rend.material.mainTexture = textureCamera;
                    
                //Destroy( quad, 5f );
                
                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                Destroy( textureCamera, 5f );
            }
        }, maxSize );

        Debug.Log( "Permission result: " + permission );
    }
*/
}
