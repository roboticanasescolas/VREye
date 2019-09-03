using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum FalaPredefinida {
    AUDIO_TEMPORARIO,
    AUDIO_APRESENTACAO,
    AUDIO_MENU_APRESENTACAO,
    AUDIO_PERMISSOES_OK
}


public class EstruturaFala{
    public FalaPredefinida FALA;
    public string TEXTO;
    public EstruturaFala(FalaPredefinida FALA_IN, string TEXTO_IN){
        this.FALA = FALA_IN;
        this.TEXTO = TEXTO_IN;
    }
}

public class ItemAudio{
    public EstruturaFala Fala;
    public AudioClip AudioCLP;
    public UnityWebRequest webReq;
    public ItemAudio(EstruturaFala FALA_IN){
        this.Fala = FALA_IN;
    }
    public ItemAudio(AudioClip AUD){
        this.AudioCLP = AUD;
        this.Fala = new EstruturaFala(FalaPredefinida.AUDIO_TEMPORARIO, "");
    }
    
    public IEnumerator asyncDownload() {
        string LINK = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen="+this.Fala.TEXTO.Length+"&client=tw-ob&q="+this.Fala.TEXTO+"&tl=Pt-br";
        using (WWW audioDown = new WWW(LINK)) {
            yield return audioDown;
            this.AudioCLP = audioDown.GetAudioClip(false, true, AudioType.MPEG);
        }
        
        
        /*using (webReq = UnityWebRequestMultimedia.GetAudioClip(LINK, AudioType.MPEG)) {
            yield return webReq.SendWebRequest();
            if (webReq.isNetworkError || webReq.isHttpError) {
                Debug.LogError(webReq.error);
                yield break;
            }
            this.AudioCLP = DownloadHandlerAudioClip.GetContent(webReq);
        }*/
    }



    /*IEnumerator downloadAudio() {
        string LINK = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen="+this.Fala.TEXTO.Length+"&client=tw-ob&q="+this.Fala.TEXTO+"&tl=Pt-br";
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(LINK, AudioType.MPEG)) {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError) {
                Debug.LogError(uwr.error);
                yield break;
            }
            AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
            // use audio clip
        }
    }   */

    public AudioClip GetAudio(){
        return this.AudioCLP;
        /*if(!webReq.isDone) return null;
        return DownloadHandlerAudioClip.GetContent(webReq);*/
    }
    public void Termina(){
        if(this.Fala.Equals(FalaPredefinida.AUDIO_TEMPORARIO)) this.webReq.Dispose();
    }
}

public class WebTTS : MonoBehaviour
{

    public AudioSource _audio;

    private List<ItemAudio> _FalasPredefinidas = new List<ItemAudio>();
    private List<ItemAudio> _filaPlaying = new List<ItemAudio>();
    

    private int initialAudioDownloaded=0, maxAudioDownloaded=0;

    // Start is called before the first frame update
    void Start()
    {
        _audio  = gameObject.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update(){
        if(!this._audio.isPlaying){
            if(_audio.clip != null){
                _audio.clip = null;
                Debug.Log("...Audio encerrado");
                this._filaPlaying[0].Termina();
                this._filaPlaying.RemoveAt(0);
            }
            if(this._audio.clip == null && (this._filaPlaying.Count > 0)){
                Debug.Log("Começando audio...");
                _audio.clip = this._filaPlaying[0].GetAudio();
                _audio.Play();
            }
        } //Debug.Log("Fila: "+this._filaPlaying.Count);
    }

    public void Fala(string FalaStr){
        StartCoroutine(
            asyncAddToList(new ItemAudio(new EstruturaFala(
                FalaPredefinida.AUDIO_TEMPORARIO,
                FalaStr
            )), this._filaPlaying)
        );
    }

    IEnumerator asyncAddToList(ItemAudio aud, List<ItemAudio> L) {
        yield return StartCoroutine( aud.asyncDownload() );
        Debug.Log("Baixado!");
        L.Add( aud );
        Debug.Log("Adicionado '"+aud.Fala.TEXTO+"'!");
    }
    

    AudioClip DownloadAndGetAudio(string TextIn){
        string LINK = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen="+TextIn.Length+"&client=tw-ob&q="+TextIn+"&tl=Pt-br";
        WWW www = new WWW(LINK);
        return www.GetAudioClip(false, true, AudioType.MPEG);
    }

    //IEnumerator addAudioClipByString(string TEXT, FalaPredefinida FALA_ID){


    public IEnumerator PrepareBasicAudios(){

        yield return StartCoroutine(
            asyncAddToList(new ItemAudio(new EstruturaFala(
                FalaPredefinida.AUDIO_APRESENTACAO,
                "Olá! Seja bem-vindo ou bem-vinda. Vamos começar... Precisamos de algumas permissões do seu celular."
            )), this._FalasPredefinidas)
        );

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(
            asyncAddToList(new ItemAudio(new EstruturaFala(
                FalaPredefinida.AUDIO_MENU_APRESENTACAO,
                "Para começar um comando de voz, incline a cabeça para a sua esquerda."
            )), this._FalasPredefinidas)
        );

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(
            asyncAddToList(new ItemAudio(new EstruturaFala(
                FalaPredefinida.AUDIO_PERMISSOES_OK,
                "Tudo certo. Permissões concedidas."
            )), this._FalasPredefinidas)
        );

        yield return new WaitForSeconds(0.1f);
        
    }

    public bool PlayAudioPredefinido(FalaPredefinida ESCOLHA){
        foreach (var PREDEF in this._FalasPredefinidas){
            if(ESCOLHA == PREDEF.Fala.FALA){
                this._filaPlaying.Add( new ItemAudio(PREDEF.GetAudio()) );
                //this._filaAudios.Add( new ItemFilaAudio(PREDEF.AUDIO, false) );
                return true;
            }
        }
        return false;
    }
}
